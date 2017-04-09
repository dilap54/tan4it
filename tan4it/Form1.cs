using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tan4it
{
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        Graphics g;
        tan4itGame game;
        long LastRenderTime;
        public Form1()
        {
            InitializeComponent();
            game = new tan4itGame();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            LastRenderTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            g.Clear(Color.White);
            game.Print(g);
            pictureBox1.Image = bitmap;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            game.Update((int)(now-LastRenderTime));
            LastRenderTime = now;
            Invalidate();
        }
    }
}
