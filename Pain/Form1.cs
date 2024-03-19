using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int newSizeX, newSizeY, pixelSize = 20, square_size = 7;
        bool back_is_exist = false;
        Graphics g;
        Bitmap bm;
        List<Point> points = new List<Point>();
        Color fg_color = Color.Aqua,
            bg_color = Color.White;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!back_is_exist)
            {
                bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics g = Graphics.FromImage(bm);
                g.Clear(bg_color);
                DrawBackground();
                textBox1.Text = square_size.ToString();
                pictureBox1.Image = bm;
                back_is_exist = true;
            }
            button1.BackColor = fg_color;
            button2.BackColor = bg_color;
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int i = e.X - e.X % pixelSize + pixelSize / 2;
            int j = e.Y - e.Y % pixelSize + pixelSize / 2;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    for (int x = i; x < i + square_size * pixelSize && x + pixelSize < pictureBox1.Width && x > 0; x += pixelSize)
                        for (int y = j; y < j + square_size * pixelSize && y + pixelSize < pictureBox1.Height && y > 0; y += pixelSize)
                            DrawPixel(x / pixelSize, y / pixelSize, fg_color);
                    break;
                case MouseButtons.Right:
                    points.Add(new Point(i, j));
                    if (points.Count > 1)
                    {
                        DrawLine(points[0], points[1]);
                        points.Clear();
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
            DrawBackground();
            pictureBox1.Image = bm;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog crd = new ColorDialog();
            if (crd.ShowDialog() == DialogResult.OK)
                fg_color = crd.Color;
            button_clear_Click(sender, e);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog crd = new ColorDialog();
            if (crd.ShowDialog() == DialogResult.OK)
                bg_color = crd.Color;
            button_clear_Click(sender, e);
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                square_size = Convert.ToInt32(textBox1.Text);
            }
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
            for (int x = x1; x <= x2; x += pixelSize)
            {
                if (steep)
                {
                    if (bm.GetPixel(y, x).ToArgb() == bg_color.ToArgb())
                        DrawPixel(y / pixelSize, x / pixelSize, Color.Green);
                    else
                        if (bm.GetPixel(y, x).ToArgb() == fg_color.ToArgb())
                        DrawPixel(y / pixelSize, x / pixelSize, Color.Red);
                }
                else
                {
                    if (bm.GetPixel(x, y).ToArgb() == bg_color.ToArgb())
                        DrawPixel(x / pixelSize, y / pixelSize, Color.Green);
                    else
                        if (bm.GetPixel(x, y).ToArgb() == fg_color.ToArgb())
                        DrawPixel(x / pixelSize, y / pixelSize, Color.Red);
                }

                error2 += derror2;

                if (error2 > dx)
                {
                    y += (y2 > y1 ? pixelSize : -pixelSize);
                    error2 -= dx * 2;
                }
            }
        }
        private void DrawBackground()
        {
            int deltaX = pictureBox1.Width % pixelSize != 0 ? 1 : 0;
            newSizeX = (pictureBox1.Width / pixelSize) * pixelSize + deltaX;
            int deltaY = pictureBox1.Height % pixelSize != 0 ? 1 : 0;
            newSizeY = (pictureBox1.Height / pixelSize) * pixelSize + deltaY;

            for (int x = 0; x < newSizeX; x++)
            {
                for (int y = 0; y < newSizeY; y++)
                {
                    if (x % pixelSize == 0f || y % pixelSize == 0f)
                    {
                        if (x == 0 || y == 0)
                            bm.SetPixel(x, y, Color.Red);
                        else
                            bm.SetPixel(x, y, Color.Gray);
                    }
                }
            }
        }
        private void DrawPixel(int xS, int yS, Color color)
        {
            {
                for (int x = xS * pixelSize + 1; x < (xS + 1) * pixelSize; x++)
                {
                    for (int y = yS * pixelSize + 1; y < (yS + 1) * pixelSize; y++)
                    {
                        bm.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
}
