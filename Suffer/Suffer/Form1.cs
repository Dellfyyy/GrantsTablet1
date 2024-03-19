using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Suffer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int newSizeX, newSizeY, pixelSize = 20;
        bool back_is_exist = false;
        Graphics G;
        Bitmap bm;
        private List<Point> polygonPoints = new List<Point>(),
            polygonPoints1 = new List<Point>();
        Point p1 = new Point();
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!back_is_exist)
            {
                var pic = (PictureBox)sender;
                G = pic.CreateGraphics();
                bm = new Bitmap(pic.Width, pic.Height, G);
                Graphics g = Graphics.FromImage(bm);
                g.Clear(Color.White);
                DrawBackground();
                pictureBox1.Image = bm;
                back_is_exist = true;
               
            }
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            DrawBackground();
            switch (e.Button)
            {
                case MouseButtons.Left:
                    polygonPoints.Add(new Point(e.X - e.X % 20 + 10, e.Y - e.Y % 20 + 10));
                    DrawPixel(e.X / pixelSize, e.Y / pixelSize, Color.Black);
                    break;

                case MouseButtons.Right:
                    if (polygonPoints.Count > 1)
                    {
                        polygonPoints.Add(polygonPoints[0]);
                        for (int i = 1; i < polygonPoints.Count; i++)
                        {
                            DrawLine(polygonPoints[i], polygonPoints[i - 1]);
                        }
                        if(polygonPoints1.Count == 0)
                        {
                            polygonPoints1.AddRange(polygonPoints);
                            polygonPoints.Clear();
                        }
                    }
                    break;
                case MouseButtons.Middle:
                    DrawPixel(e.X / pixelSize, e.Y / pixelSize, Color.Blue);
                    p1.X = e.X - e.X % 20 + 30;
                    p1.Y = e.Y - e.Y % 20 + 10;
                    break;
            }
            pictureBox1.Image = bm;
        }
        private void Wait(double seconds)
        {
            int ticks = System.Environment.TickCount + (int)Math.Round(seconds * 1000.0);
            while (System.Environment.TickCount < ticks)
            {
                Application.DoEvents();
            }
        }
        private void button_clear_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pictureBox1.Refresh();
            DrawBackground();
            polygonPoints.Clear();
            polygonPoints1.Clear();
            pictureBox1.Image = bm;
        }
        private Point Center()
        {
            int x_max = 0, x_min = 650,
                y_max = 0, y_min = 650,
                x, y;
            foreach(Point point in polygonPoints1)
            {
                if (point.X > x_max) x_max = point.X; 
                if (point.X < x_min) x_min = point.X; 
                if (point.Y > y_max) y_max = point.Y; 
                if (point.Y < y_min) y_min = point.Y; 
            }
            x = (x_max + x_min) / 2;
            y = (y_max + y_min) / 2;
            Point center_p = new Point(x - x % pixelSize + pixelSize / 2, y - y % pixelSize + pixelSize / 2);
            return center_p;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Fill1(ref polygonPoints1);
            Fill1(ref polygonPoints);
        }
        void Fill1(ref List<Point> points)
        {
            float b, k;
            int x2;
            Color color = Color.Red;
            if (points.Count > 2)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    if ((float)points[i].Y == (float)points[i + 1].Y)
                        continue;
                    k = ((float)points[i].Y - (float)points[i + 1].Y) /
                        ((float)points[i].X - (float)points[i + 1].X);
                    b = (float)points[i + 1].Y - k * (float)points[i + 1].X;
                    if (points[i].Y < points[i + 1].Y)
                        for (int j = points[i].Y; j < points[i + 1].Y + pixelSize; j += pixelSize)
                        {
                            if ((float)points[i].X != (float)points[i + 1].X)
                            {
                                float c_x = (j - b) / k;
                                x2 = Convert.ToInt32(c_x);
                            }
                            else x2 = points[i].X;
                            for (int l = x2 + pixelSize; l < pictureBox1.Width - 10; l += pixelSize)
                            {
                                if (l % pixelSize == 0) l += 1;
                                if (bm.GetPixel(l, j).ToArgb() == Color.Black.ToArgb())
                                    continue;
                                if (bm.GetPixel(l, j).ToArgb() == Color.White.ToArgb())
                                    DrawPixel(l / pixelSize, j / pixelSize, color);
                                else if (bm.GetPixel(l, j).ToArgb() == color.ToArgb())
                                    DrawPixel(l / pixelSize, j / pixelSize, Color.White);
                            }
                            Wait(0.05f);
                            pictureBox1.Image = bm;
                        }
                    else
                        for (int j = points[i].Y; j > points[i + 1].Y - pixelSize; j -= pixelSize)
                        {
                            if ((float)points[i].X != (float)points[i + 1].X)
                            {
                                float c_x = (j - b) / k;
                                x2 = Convert.ToInt32(c_x);
                            }
                            else x2 = points[i].X;
                            for (int l = x2 + pixelSize; l < pictureBox1.Width - 10; l += pixelSize)
                            {
                                if (l % pixelSize == 0) l += 1;
                                if (bm.GetPixel(l, j).ToArgb() == Color.Black.ToArgb())
                                    continue;
                                if (bm.GetPixel(l, j).ToArgb() == Color.White.ToArgb())
                                    DrawPixel(l / pixelSize, j / pixelSize, color);
                                else if (bm.GetPixel(l, j).ToArgb() == color.ToArgb())
                                    DrawPixel(l / pixelSize, j / pixelSize, Color.White);
                            }
                            Wait(0.05f);
                            pictureBox1.Image = bm;
                        }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Fill2(p1.X, p1.Y);
        }
        void Fill2(int x, int y)
        {
            if (bm.GetPixel(x, y).ToArgb() == Color.White.ToArgb())
            {
                DrawPixel(x / pixelSize, y / pixelSize, Color.Blue);
                Wait(0.1f);
                pictureBox1.Image = bm;
                Fill2(x + pixelSize, y);
                Fill2(x, y + pixelSize);
                Fill2(x - pixelSize, y);
                Fill2(x, y - pixelSize);
            }
        }
        private void DrawLine(Point p1, Point p2)
        {
            int x1 = p1.X, y1 = p1.Y,
                x2 = p2.X, y2 = p2.Y;
            int deltaX = Math.Abs(x2 - x1),
                deltaY = Math.Abs(y2 - y1);
            int signX = x1 < x2 ? 1 : -1,
                signY = y1 < y2 ? 1 : -1;
            int error = deltaX - deltaY;
            DrawPixel(x2 / pixelSize, y2 / pixelSize, Color.Black);
            while (x1 != x2 || y1 != y2)
            {
                DrawPixel(x1 / pixelSize, y1 / pixelSize, Color.Black);
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

