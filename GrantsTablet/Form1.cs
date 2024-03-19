// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Windows.Forms;

namespace GrantsTablet
{
    public partial class Form1 : Form
    {
        public class StudentSession
        {
            public string StudentName { get; set; }
            public string GroupNumber { get; set; }
            public List<bool> test { get; set; }
            public List<int> exam { get; set; }
            public bool PublicWork { get; set; }
        }
        public Form1()
        {
            InitializeComponent();
        }
        bool start = false; //Для блокировки dataGridView1_CellValueChanged
        string group; //Текущая группа
        SortedSet<string> groups = new SortedSet<string>(); //Все группы
        SortedList<string, List<StudentSession>> students = new SortedList<string, List<StudentSession>>(); //Вся информация
        System.Drawing.Font common_font;  //Шрифт
        StudentSession buffer = new StudentSession()
        {
            StudentName = "",
            GroupNumber = "",
            exam = new List<int>(5) { 0, 0, 0, 0, 0 },
            test = new List<bool>(5) { false, false, false, false, false },
            PublicWork = false
        }; //Переменная-буффер
        private void Form1_Load(object sender, EventArgs e)
        {
            common_font = new System.Drawing.Font(file_menu.Font.Name, 14);
            {
                DataGridViewColumn col1 = new DataGridViewColumn()
                {
                    HeaderText = "№",
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    FillWeight = 35,
                    Frozen = false,
                    CellTemplate = new DataGridViewTextBoxCell()
                };

                DataGridViewColumn col2 = new DataGridViewColumn()
                {
                    HeaderText = "ФИО",
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    FillWeight = 160,
                    Frozen = false,
                    CellTemplate = new DataGridViewTextBoxCell()
                };

                DataGridViewColumn col3 = new DataGridViewColumn()
                {
                    HeaderText = "ГРУППА",
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    ReadOnly = true,
                    Frozen = false,
                    CellTemplate = new DataGridViewTextBoxCell()
                };

                DataGridViewColumn col4 = new DataGridViewColumn()
                {
                    HeaderText = "СРЕДНЯЯ ОЦЕНКА",
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    ReadOnly = true,
                    Frozen = false,
                    CellTemplate = new DataGridViewTextBoxCell()
                };

                DataGridViewColumn col5 = new DataGridViewColumn()
                {
                    HeaderText = "КОЛ-ВО СДАННЫХ ЗАЧЕТОВ",
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    ReadOnly = true,
                    Frozen = false,
                    CellTemplate = new DataGridViewTextBoxCell()
                };

                DataGridViewColumn col6 = new DataGridViewColumn()
                {
                    HeaderText = "ОБЩ. РАБОТА",
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    Frozen = false,
                    CellTemplate = new DataGridViewCheckBoxCell()
                };

                DataGridViewColumn col7 = new DataGridViewColumn()
                {
                    HeaderText = "СТИПЕНДИЯ",
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    ReadOnly = true,
                    Frozen = false,
                    CellTemplate = new DataGridViewTextBoxCell()
                };

                dataGridView1.Columns.Add(col1);
                dataGridView1.Columns.Add(col2);
                dataGridView1.Columns.Add(col3);
                dataGridView1.Columns.Add(col4);
                dataGridView1.Columns.Add(col5);
                dataGridView1.Columns.Add(col6);
                dataGridView1.Columns.Add(col7);
            }
            {
                ToolStripMenuItem copy = new ToolStripMenuItem("Копировать строку");
                ToolStripMenuItem paste = new ToolStripMenuItem("Вставить строку");
                ToolStripMenuItem delete = new ToolStripMenuItem("Удалить строку");
                delete.Click += delete_Click;
                copy.Click += copy_Click;
                paste.Click += paste_Click;
                ContextMenuStrip con_menu = new ContextMenuStrip();
                con_menu.Items.AddRange(new[] { copy, paste, delete });
                dataGridView1.ContextMenuStrip = con_menu;

                save_menu.Click += save_Click;
                load_menu.Click += load_Click;
                export_menu.Click += export_Click;
                exit_menu.Click += exit_Click;

                delGroup_menu.Click += deletegroup_Click;
                delAll_menu.Click += deleteAll_Click;

                group_menu.Click += AddGroup_Click;
                student_menu.Click += AddRow_Click;

                filter_menu.Click += FilterPrint;

                menuStrip1.BackColor = System.Drawing.Color.White;
                menuStrip1.Font = common_font;
            }

            dataGridView1.DoubleBuffered(true);

            //ExtensionMethods.DoubleBuffered(dataGridView1, true);

            LoadFile();

            tabControl1.Font = common_font;
            tabControl1.SelectedIndex = 0;

            Font = common_font;
        }
        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        { //При изменении вкладки, перепечатать таблицу
            if (tabControl1.TabPages.Count != 0)
            {
                group = tabControl1.SelectedTab.Text;
                Print();
            }
        }
        private void deleteAll_Click(object sender, EventArgs e)
        {//Удалить все
            tabControl1.TabPages.Clear();
            dataGridView1.Rows.Clear();
            students.Clear();
            groups.Clear();
            number = 1;
        }
        private void deletegroup_Click(object sender, EventArgs e)
        {//Удалить текущую группу
            students.Remove(group);
            groups.Remove(group);
            dataGridView1.Rows.Clear();
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }
        private void delete_Click(object sender, EventArgs e)
        {//Удалить выделенную строку
            int a = dataGridView1.CurrentRow.Index;
            dataGridView1.Rows.RemoveAt(a);
            students[group].RemoveAt(a);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1[0, i].Value = i + 1;
                dataGridView1.Rows[i].DefaultCellStyle.BackColor =
                (i % 2 == 0) ? System.Drawing.Color.LightBlue
                : System.Drawing.Color.White;
            }
        }
        private void copy_Click(object sender, EventArgs e)
        {//Скопировать строку в buffer
            int a = dataGridView1.CurrentRow.Index;
            StudentSession st = new StudentSession()
            {
                StudentName = dataGridView1[1, a].Value.ToString(),
                GroupNumber = dataGridView1[2, a].Value.ToString(),
                exam = students[group][a].exam,
                test = students[group][a].test,
                PublicWork = Convert.ToBoolean(dataGridView1[5, a].Value)
            };
            buffer = st;
        }
        private void paste_Click(object sender, EventArgs e)
        {//Вставить данные из buffer в выделенную строку
            int a = dataGridView1.CurrentRow.Index;
            StudentSession st = buffer;
            dataGridView1[1, a].Value = buffer.StudentName;
            dataGridView1[2, a].Value = buffer.GroupNumber;
            students[group][a].exam = buffer.exam;
            students[group][a].test = buffer.test;
            dataGridView1[3, a].Value = AvarageScore(buffer.exam);
            dataGridView1[4, a].Value = TestCount(buffer.test);
            dataGridView1[5, a].Value = buffer.PublicWork;
        }
        private void AddRow_Click(object sender, EventArgs e)
        {//Добавить строку
            if (tabControl1.TabCount > 0)
            {
                DataGridViewRow row = new DataGridViewRow() { Frozen = false };
                row.DefaultCellStyle.BackColor =
                    (dataGridView1.Rows.Count % 2 == 0) ? System.Drawing.Color.LightBlue
                    : System.Drawing.Color.White;
                dataGridView1.Rows.Add(row);

                StudentSession st = new StudentSession()
                {
                    StudentName = "",
                    GroupNumber = group,
                    exam = new List<int>(5) { 0, 0, 0, 0, 0 },
                    test = new List<bool>(5) { false, false, false, false, false },
                    PublicWork = false
                };

                students[group].Add(st);
                dataGridView1[0, dataGridView1.Rows.Count - 1].Value = dataGridView1.Rows.Count;
                dataGridView1[1, dataGridView1.Rows.Count - 1].Value = st.StudentName;
                dataGridView1[2, dataGridView1.Rows.Count - 1].Value = st.GroupNumber;
                dataGridView1[3, dataGridView1.Rows.Count - 1].Value = 0;
                dataGridView1[4, dataGridView1.Rows.Count - 1].Value = 0;
                dataGridView1[5, dataGridView1.Rows.Count - 1].Value = false; 
            }
        }
        int number = 1; //Номер новой группы
        private void AddGroup_Click(object sender, EventArgs e)
        {//Создать группу
            string group_name = "AB-" + number.ToString();
            if (InputBox(ref group_name) == DialogResult.OK)
            {
                students.Add(group_name, new List<StudentSession>());
                groups.Add(group_name);
                tabControl1.TabPages.Add(group_name);
                tabControl1_TabIndexChanged(new object(), new EventArgs());
                number++;
            }

        }
        private void save_Click(object sender, EventArgs e)
        {//Сохранить данные в файл(сериализация)
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\student_list.txt";
            if (!(File.Exists(path)))
                using (StreamWriter sw = File.CreateText(path))
                    sw.WriteLine("Saving data to file");
            else
                File.WriteAllText(path, string.Empty);
            string stu; 
            StreamWriter file = new StreamWriter(path, true);
            foreach (string x in groups)
                foreach (StudentSession student in students[x])
                {
                    stu = JsonSerializer.Serialize(student);
                    file.WriteLine(stu);
                }
            file.Close();
            MessageBox.Show("Файл сохранён!", "Сохранение файла", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void load_Click(object sender, EventArgs e)
        { //Обновить лист данных и перепечатать таблицу
            LoadFile();
            Print();
        }
        private void export_Click(object sender, EventArgs e)
        { //Экспортировать данные из таблицы в эксель файл
            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            ExcelApp.SheetsInNewWorkbook = groups.Count;
            Microsoft.Office.Interop.Excel.Workbook workBook = ExcelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet sheet;
            for (int k = 0; k < tabControl1.TabCount; k++)
            {
                tabControl1.SelectedIndex = k;
                sheet = ExcelApp.Worksheets.get_Item(k + 1);
                sheet.Name = tabControl1.SelectedTab.Text;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        ExcelApp.Sheets[k + 1].Cells[i + 1, j + 1] = dataGridView1.Rows[i].Cells[j].Value;
                    }
                }
            }
            ExcelApp.Visible = false;
            ExcelApp.UserControl = true;

            SaveFileDialog save = new SaveFileDialog();
            if (save.ShowDialog() == DialogResult.Cancel)
                return;
            string fullfilename2007 = System.IO.Path.Combine(save.ToString(), save.FileName);
            if (System.IO.File.Exists(fullfilename2007)) System.IO.File.Delete(fullfilename2007);
            workBook.SaveAs(fullfilename2007, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);

            workBook.Close(false);
            ExcelApp.Quit();
            MessageBox.Show("Файл экспортирован!", "Сохранение файла", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void exit_Click(object sender, EventArgs e)
        { //Закрыть приложение
            if (MessageBox.Show("Сохранить перед выходом?", "Сообщение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                save_Click(new object(), new EventArgs());
            this.Close();
        }
        private void LoadFile()
        { //Загрузить данные из файла в лист(десериализация)
            tabControl1.TabPages.Clear();
            groups.Clear();
            students.Clear();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\student_list.txt";
            if (!(File.Exists(path)))
                using (StreamWriter fw = File.CreateText(path))
                    fw.WriteLine("Loading data from file");
            else
            {
                StreamReader sr = new StreamReader(path);
                StudentSession text;
                while (!sr.EndOfStream)
                {
                    text = JsonSerializer.Deserialize<StudentSession>(sr.ReadLine());
                    groups.Add(text.GroupNumber);
                    if (!students.ContainsKey(text.GroupNumber))
                        students.Add(text.GroupNumber, new List<StudentSession>());
                    students[text.GroupNumber].Add(text);
                }
                foreach (string x in groups)
                    tabControl1.TabPages.Add(x);
                tabControl1_TabIndexChanged(new object(), new EventArgs());
                sr.Close();
            }
        }
        private void Print()
        {//Перепечатать таблицу
            dataGridView1.Rows.Clear();
            int ix = 0;
            start = false;
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (StudentSession text in students[group])
            {
                DataGridViewRow row = new DataGridViewRow() { Frozen = false };
                row.CreateCells(dataGridView1);

                row.DefaultCellStyle.BackColor =
                (rows.Count % 2 == 0) ? System.Drawing.Color.LightBlue
                : System.Drawing.Color.White;
                row.Cells[0].Value = ix + 1;
                row.Cells[1].Value = text.StudentName;
                row.Cells[2].Value = text.GroupNumber;
                row.Cells[3].Value = AvarageScore(text.exam);
                row.Cells[4].Value = TestCount(text.test);
                row.Cells[5].Value = text.PublicWork.ToString();
                if (AvarageScore(text.exam) == 5 && text.PublicWork)
                    row.Cells[6].Value = 6750;
                else if (AvarageScore(text.exam) == 5)
                    row.Cells[6].Value = 5625;
                else if (AvarageScore(text.exam) >= 4)
                    row.Cells[6].Value = 4500;
                else if (AvarageScore(text.exam) >= 3.8f && text.PublicWork)
                    row.Cells[6].Value = 4500;
                else row.Cells[6].Value = 0;
                rows.Add(row);
                ix++;
            }
            dataGridView1.Rows.AddRange(rows.ToArray());
            start = true;
        }
        string[] filters = new string[5] { "", "", "", "", "" }; //Фильтры
        private void FilterPrint(object sender, EventArgs e)
        {//Перепечатать таблицу с учетом фильтров
            DialogResult dr = FilterBox(ref filters);
            if (dr == DialogResult.OK)
            {
                tabControl1.Visible = false;
                dataGridView1.Rows.Clear();
                int ix = 0;
                start = false;
                List<DataGridViewRow> rows = new List<DataGridViewRow>();
                foreach (string group in groups)
                    foreach (StudentSession text in students[group])
                    {
                        DataGridViewRow row = new DataGridViewRow() { Frozen = false };
                        row.CreateCells(dataGridView1);

                        row.DefaultCellStyle.BackColor =
                        (rows.Count % 2 == 0) ? System.Drawing.Color.LightBlue
                        : System.Drawing.Color.White;
                        row.Cells[0].Value = ix + 1;
                        row.Cells[1].Value = text.StudentName;
                        row.Cells[2].Value = text.GroupNumber;
                        row.Cells[3].Value = AvarageScore(text.exam);
                        row.Cells[4].Value = TestCount(text.test);
                        row.Cells[5].Value = text.PublicWork.ToString();
                        if (AvarageScore(text.exam) == 5 && text.PublicWork)
                            row.Cells[6].Value = 6750;
                        else if (AvarageScore(text.exam) == 5)
                            row.Cells[6].Value = 5625;
                        else if (AvarageScore(text.exam) >= 4)
                            row.Cells[6].Value = 4500;
                        else if (AvarageScore(text.exam) >= 3.8f && text.PublicWork)
                            row.Cells[6].Value = 4500;
                        else row.Cells[6].Value = 0;
                        if ((row.Cells[1].Value.ToString().StartsWith(filters[0]) || filters[0] == "") &&
                        (row.Cells[2].Value.ToString() == filters[1] || filters[1] == "") &&
                        (row.Cells[3].Value.ToString() == filters[2] || filters[2] == "") &&
                        (row.Cells[4].Value.ToString() == filters[3] || filters[3] == "") &&
                        (row.Cells[6].Value.ToString() == filters[4] || filters[4] == ""))
                        {
                            rows.Add(row);
                            ix++;
                        }
                    }
                dataGridView1.Rows.AddRange(rows.ToArray());
                start = true;
                add_menu.Enabled = false;
                del_menu.Enabled = false;
            }
            else
                if (dr == DialogResult.Cancel)
            {
                tabControl1.Visible = true;
                add_menu.Enabled = true;
                del_menu.Enabled = true;
                tabControl1_TabIndexChanged(new object(), new EventArgs());
                filters = new string[5] { "", "", "", "", "" };
            }
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//При изменении данных подсчет стипендии и внесение измененных данных в лист
            if (start)
            {
                if (e.ColumnIndex == 1)
                    students[group][e.RowIndex].StudentName = dataGridView1[1, e.RowIndex].Value.ToString();
                if (e.ColumnIndex == 5)
                    students[group][e.RowIndex].PublicWork = Convert.ToBoolean(dataGridView1[5, e.RowIndex].Value);
                if (e.ColumnIndex == 3 || e.ColumnIndex == 5)
                {
                    double x = Convert.ToDouble(dataGridView1[3, e.RowIndex].Value);
                    bool y = Convert.ToBoolean(dataGridView1[5, e.RowIndex].Value);
                    double data_epsilon = 0.000000111;
                    if ((Math.Abs(x - 5) < data_epsilon) && y)
                        dataGridView1[6, e.RowIndex].Value = 6750;
                    else if (Math.Abs(x - 5) < data_epsilon)
                        dataGridView1[6, e.RowIndex].Value = 5625;
                    else if (x >= 4)
                        dataGridView1[6, e.RowIndex].Value = 4500;
                    else if (x >= 3.8f && y)
                        dataGridView1[6, e.RowIndex].Value = 4500;
                    else dataGridView1[6, e.RowIndex].Value = 0;
                }
            }
        }
        int row;
        DataGridView dgv2;
        private void dgv2_test_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                students[group][row].test[i] = Convert.ToBoolean(dgv2[i, e.RowIndex].Value);
            dataGridView1[4, row].Value = TestCount(students[group][row].test);
        }
        private void dgv2_exam_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                students[group][row].exam[i] = Convert.ToInt32(dgv2[i, e.RowIndex].Value);
            dataGridView1[3, row].Value = AvarageScore(students[group][row].exam);
        }
        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {//Изменение курсора при наведении на определенные столбцы
            if ((e.ColumnIndex == 3 || e.ColumnIndex == 4) && e.RowIndex >= 0
                && e.Location.Y < dataGridView1.Location.Y)
                Cursor = Cursors.Hand;
            else Cursor = Cursors.Default;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {//Создание формы с оценками при клике на определенные столбцы
            if ((e.ColumnIndex == 3 || e.ColumnIndex == 4) && e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                row = e.RowIndex;

                Form frm2 = new Form()
                {
                    Width = 780,
                    Height = 100,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    StartPosition = FormStartPosition.Manual,
                    Location = new System.Drawing.Point(dataGridView1.Width / 2 - 390,
                    dataGridView1.Height / 2 - 50),
                    Font = common_font
                };
                dgv2 = new DataGridView()
                {
                    AllowUserToResizeColumns = false,
                    AllowUserToResizeRows = false,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                    Location = new System.Drawing.Point(0, 0),
                    ColumnHeadersHeight = dataGridView1.ColumnHeadersHeight,
                    RowHeadersVisible = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    Dock = DockStyle.Fill
                };

                dgv2.EditingControlShowing += table_EditingControlShowing;

                if (e.ColumnIndex == 3)
                {
                    string[] subjects = new string[5] { "Лин. алгебра", "Мат. анализ", "Программ.",
                        "Физика", "Теор. вероятностей" };
                    frm2.Text = "Результаты экзаменов: " + dataGridView1[1, e.RowIndex].Value.ToString();
                    for (int i = 0; i < 5; i++)
                    {
                        DataGridViewColumn col = new DataGridViewColumn()
                        {
                            HeaderText = subjects[i],
                            Frozen = false,
                            CellTemplate = new DataGridViewTextBoxCell()
                        };
                        dgv2.Columns.Add(col);
                    }
                    DataGridViewRow row = new DataGridViewRow() { Frozen = false };
                    dgv2.Rows.Add(row);
                    for (int i = 0; i < 5; i++)
                        dgv2[i, 0].Value = students[group][e.RowIndex].exam[i];
                    dgv2.CellValueChanged += new DataGridViewCellEventHandler(dgv2_exam_CellValueChanged);
                }
                if (e.ColumnIndex == 4)
                {
                    string[] subjects = new string[5] { "Физкультура", "История", "Философия",
                        "ОЛКК", "Англ. яз" };
                    frm2.Text = "Результаты зачетов " + dataGridView1[1, e.RowIndex].Value.ToString();
                    for (int i = 0; i < 5; i++)
                    {
                        DataGridViewColumn col = new DataGridViewColumn()
                        {
                            HeaderText = subjects[i],
                            Frozen = false,
                            CellTemplate = new DataGridViewCheckBoxCell()
                        };
                        dgv2.Columns.Add(col);
                    }
                    DataGridViewRow row = new DataGridViewRow() { Frozen = false };
                    dgv2.Rows.Add(row);
                    for (int i = 0; i < 5; i++)
                        dgv2[i, 0].Value = students[group][e.RowIndex].test[i];
                    dgv2.CellValueChanged += new DataGridViewCellEventHandler(dgv2_test_CellValueChanged);
                }
                frm2.Controls.Add(dgv2);
                frm2.ShowDialog();
            }
        }
        private void dataGridView1_Sorted(object sender, EventArgs e)
        {//При сортировке пересчет первого столбца и перекрашивание строк
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1[0, i].Value = i + 1;
                if (i % 2 == 0)
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                else
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }
        private void OnlyIntDigits(object sender, KeyPressEventArgs e)
        {//Ввод только целых чисел 
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }
        private void OnlyFloatDigits(object sender, KeyPressEventArgs e)
        {//Ввод только чисел 
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != 44)
                e.Handled = true;
        }
        private void table_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = (TextBox)e.Control;
            tb.KeyPress += OnlyIntDigits;
        }
        private float AvarageScore(List<int> scores)
        {//Среднее арифметическое
            float summ = 0;
            foreach (int x in scores)
                summ += (float)x;
            return summ / scores.Count;
        }
        private int TestCount(List<bool> tests)
        {//Количество true
            int summ = 0;
            foreach (bool x in tests)
                if (x == true)
                    summ++;
            return summ;
        }
        public DialogResult InputBox(ref string value)
        {//Создание формы для создания группы
            Form form = new Form()
            {
                Text = "Новая группа",
                ClientSize = new System.Drawing.Size(396, 107),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };
            Label label = new Label()
            {
                Text = "Введите название новой группы",
                AutoSize = true
            };
            TextBox textBox = new TextBox()
            {
                Text = value
            };
            Button buttonOk = new Button()
            {
                Text = "Oк",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK
            };
            Button buttonCancel = new Button()
            {
                Text = "Отмена",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel
            };

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;

            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
        public DialogResult FilterBox(ref string[] filters)
        {//Создание формы для внесения фильтров
            Form form = new Form()
            {
                Text = "Фильтр",
                ClientSize = new System.Drawing.Size(396, 247),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false,
                ControlBox = false
            };
            Button buttonOk = new Button()
            {
                Text = "Добавить фильтры",
                Anchor = AnchorStyles.Bottom,
                DialogResult = DialogResult.OK
            };
            Button buttonCancel = new Button()
            {
                Text = "Отменить фильтры",
                Anchor = AnchorStyles.Bottom,
                DialogResult = DialogResult.Cancel
            };

            Label label1 = new Label()
            {
                Text = "Имя студента",
                AutoSize = true
            };
            TextBox textBox1 = new TextBox()
            {
                Text = filters[0],
                Anchor = AnchorStyles.Right | AnchorStyles.Left
            };

            Label label2 = new Label()
            {
                Text = "Номер группы",
                AutoSize = true
            };
            TextBox textBox2 = new TextBox()
            {
                Text = filters[1],
                Anchor = AnchorStyles.Right | AnchorStyles.Left
            };

            Label label3 = new Label()
            {
                Text = "Средняя оценка",
                AutoSize = true
            };
            TextBox textBox3 = new TextBox()
            {
                Text = filters[2],
                Anchor = AnchorStyles.Right | AnchorStyles.Left
            };
            textBox3.KeyPress += OnlyFloatDigits;

            Label label4 = new Label()
            {
                Text = "Количество зачетов",
                AutoSize = true
            };
            TextBox textBox4 = new TextBox()
            {
                Text = filters[3],
                Anchor = AnchorStyles.Right | AnchorStyles.Left
            };
            textBox4.KeyPress += OnlyIntDigits;

            Label label5 = new Label()
            {
                Text = "Стипендия",
                AutoSize = true
            };
            TextBox textBox5 = new TextBox()
            {
                Text = filters[4],
                Anchor = AnchorStyles.Right | AnchorStyles.Left
            };
            textBox5.KeyPress += OnlyIntDigits;

            label1.SetBounds(9, 10, 372, 13);
            textBox1.SetBounds(12, 26, 372, 20);

            label2.SetBounds(9, 51, 372, 13);
            textBox2.SetBounds(12, 67, 372, 20);

            label3.SetBounds(9, 92, 372, 13);
            textBox3.SetBounds(12, 108, 372, 20);

            label4.SetBounds(9, 133, 372, 13);
            textBox4.SetBounds(12, 149, 372, 20);

            label5.SetBounds(9, 174, 372, 13);
            textBox5.SetBounds(12, 190, 372, 20);

            //form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            buttonCancel.SetBounds(form.Width - 160, form.Height - 70, 130, 25);
            buttonOk.SetBounds(form.Width - 295, form.Height - 70, 130, 25);

            form.Controls.AddRange(new Control[] { buttonOk, buttonCancel, label1, textBox1,
            label2, textBox2, label3, textBox3, label4, textBox4, label5, textBox5,});
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();

            filters[0] = textBox1.Text;
            filters[1] = textBox2.Text;
            filters[2] = textBox3.Text;
            filters[3] = textBox4.Text;
            filters[4] = textBox5.Text;
            return dialogResult;
        }

        public void Dispose_Close()
        {
            common_font.Dispose();
        }
    }
    public static class ExtensionMethods
    {//Двойная буфферизация
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            System.Reflection.PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

}
