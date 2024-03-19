using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnotherSuffer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics g;
        Bitmap bm;
        SolidBrush brush;
        Color fg_color = Color.Aqua,
            bg_color = Color.White;
        List<Point> points = new List<Point>(),
            points1 = new List<Point>();

        private void Form1_Load(object sender, EventArgs e)
        {
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bm);
            g.Clear(bg_color);
            pictureBox1.Image = bm;
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            brush = new SolidBrush(fg_color);
            button2.BackColor = fg_color;
            button3.BackColor = bg_color;
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int size = 5;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    points1.Add(new Point(e.X, e.Y));
                    g.FillEllipse(new SolidBrush(Color.Black), new Rectangle(e.X, e.Y, size, size));
                    break;
                case MouseButtons.Right:
                    points.Add(new Point(e.X, e.Y));
                    if (points.Count > 1)
                    {
                        DrawLine(points[0], points[1]);
                        points.Clear();
                    }
                    break;
                case MouseButtons.Middle:
                    if (points1.Count > 2)
                    {
                        foreach (Point x in points1)
                            g.FillEllipse(new SolidBrush(bg_color), new Rectangle(x.X, x.Y, size, size));
                        g.FillPolygon(brush, points1.ToArray());
                        points1.Clear();
                    }
                    break;
            }
            pictureBox1.Image = bm;
        }
        private void button_clear_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(bm);
            g.Clear(bg_color);
            pictureBox1.Refresh();
            pictureBox1.Image = bm;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog crd = new ColorDialog();
            if (crd.ShowDialog() == DialogResult.OK)
                fg_color = crd.Color;
            button_clear_Click(sender, e);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog crd = new ColorDialog();
            if (crd.ShowDialog() == DialogResult.OK)
                bg_color = crd.Color;
            button_clear_Click(sender, e);
        }
        private void DrawLine(Point p1, Point p2)
        {
            int x1 = p1.X, y1 = p1.Y,
                x2 = p2.X, y2 = p2.Y;
            bool steep = false;
            int c;
            if (Math.Abs(x1 - x2) < Math.Abs(y1 - y2))
            {
                c = x1;
                x1 = y1;
                y1 = c;
                c = x2;
                x2 = y2;
                y2 = c;
                steep = true;
            }
            if (x1 > x2)
            {
                c = x1;
                x1 = x2;
                x2 = c;
                c = y1;
                y1 = y2;
                y2 = c;
            }
            int dx = x2 - x1;
            int dy = y2 - y1;
            int derror2 = Math.Abs(dy) * 2;
            int error2 = 0;
            int y = y1;
            for (int x = x1; x <= x2; x++)
            {
                if (steep)
                {
                    if (bm.GetPixel(y, x).ToArgb() == bg_color.ToArgb())
                        bm.SetPixel(y, x, Color.Green);
                    else
                        if (bm.GetPixel(y, x).ToArgb() == fg_color.ToArgb())
                        bm.SetPixel(y, x, Color.Red);
                }
                else
                {
                    if (bm.GetPixel(x, y).ToArgb() == bg_color.ToArgb())
                        bm.SetPixel(x, y, Color.Green);
                    else
                        if (bm.GetPixel(x, y).ToArgb() == fg_color.ToArgb())
                        bm.SetPixel(x, y, Color.Red);
                }

                error2 += derror2;

                if (error2 > dx)
                {
                    y += (y2 > y1 ? 1 : -1);
                    error2 -= dx * 2;
                }
            }
        }
    }
}
