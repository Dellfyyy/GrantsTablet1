using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hope
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int pixelSize = 20;
        Graphics g;
        Bitmap bm;
        List<Point> points = new List<Point>();
        Color color1 = Color.Green;
        private void Form1_Load(object sender, EventArgs e)
        {
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bm);
            trackBar1.Value = 20;
            DrawBackground();
            pictureBox1.Image = bm;
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X - e.X % pixelSize + pixelSize / 2;
            int y = e.Y - e.Y % pixelSize + pixelSize / 2;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    points.Add(new Point(x, y));
                    DrawPixel(x, y, color1);
                    break;
                case MouseButtons.Right:
                    if (points.Count > 1)
                    {
                        points.Add(points[0]);
                        for (int i = 1; i < points.Count; i++)
                        {
                            DrawLine(points[i], points[i - 1]);
                        }
                        Fill(x, y);
                        points.Clear();
                    }
                    break;
                case MouseButtons.Middle:
                    break;
            }
            pictureBox1.Image = bm;
        }
        void Fill(int x, int y)
        {
            if (bm.GetPixel(x, y).ToArgb() == Color.White.ToArgb())
            {
                DrawPixel(x, y, color1);
                pictureBox1.Image = bm;
                Fill(x + pixelSize, y);
                Fill(x, y + pixelSize);
                Fill(x - pixelSize, y);
                Fill(x, y - pixelSize);
            }
            if ((color1.ToArgb() == Color.Green.ToArgb() && bm.GetPixel(x, y).ToArgb() == Color.Red.ToArgb()) ||
                (color1.ToArgb() == Color.Red.ToArgb() && bm.GetPixel(x, y).ToArgb() == Color.Green.ToArgb()))
            {
                DrawPixel(x, y, Color.Yellow);
                pictureBox1.Image = bm;
                Fill(x + pixelSize, y);
                Fill(x, y + pixelSize);
                Fill(x - pixelSize, y);
                Fill(x, y - pixelSize);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (color1 == Color.Green) color1 = Color.Red;
            else color1 = Color.Green;
        }
        private void button_clear_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            DrawBackground();
            points.Clear();
            pictureBox1.Image = bm;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            pixelSize = trackBar1.Value;
            textBox1.Text = "Pixel size: " + trackBar1.Value.ToString();
            button_clear_Click(sender, e);
        }
        private void DrawLine(Point p1, Point p2)
        {
            int x1 = p1.X, y1 = p1.Y,
                x2 = p2.X, y2 = p2.Y;
            int deltaX = Math.Abs(x2 - x1), 
                deltaY = Math.Abs(y2 - y1),
                signX = x1 < x2 ? pixelSize : -pixelSize,
                signY = y1 < y2 ? pixelSize : -pixelSize;
            int error = deltaX - deltaY;
            if ((color1.ToArgb() == Color.Green.ToArgb() && bm.GetPixel(x2, y2).ToArgb() == Color.Red.ToArgb()) ||
                    (color1.ToArgb() == Color.Red.ToArgb() && bm.GetPixel(x2, y2).ToArgb() == Color.Green.ToArgb()))
                DrawPixel(x2, y2, Color.Yellow);
            else
                if(!(bm.GetPixel(x2, y2).ToArgb() == Color.Yellow.ToArgb()))
                DrawPixel(x2, y2, color1);
            while (x1 != x2 || y1 != y2)
            {
                if ((color1.ToArgb() == Color.Green.ToArgb() && bm.GetPixel(x1, y1).ToArgb() == Color.Red.ToArgb()) ||
                    (color1.ToArgb() == Color.Red.ToArgb() && bm.GetPixel(x1, y1).ToArgb() == Color.Green.ToArgb()))
                    DrawPixel(x1, y1, Color.Yellow);
                else
                if (!(bm.GetPixel(x1, y1).ToArgb() == Color.Yellow.ToArgb()))
                    DrawPixel(x1, y1, color1);
                int error2 = error * 2;
                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }
        }
        private void DrawPixel(int xS, int yS, Color color)
        {
            xS -= xS % pixelSize;
            yS -= yS % pixelSize;

            {
                for (int x = xS + 1; x < xS + pixelSize && x < pictureBox1.Width; x++)
                {
                    for (int y = yS + 1; y < yS + pixelSize && y < pictureBox1.Height; y++)
                    {
                        bm.SetPixel(x, y, color);
                    }
                }
            }
        }
        private void DrawBackground()
        {
            g.Clear(Color.White);
            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    if (x % pixelSize == 0f || y % pixelSize == 0f)
                    {
                        bm.SetPixel(x, y, Color.Gray);
                    }
                }
            }
        }
    }
}
