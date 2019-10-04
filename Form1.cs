using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crop_Tool
{
    public partial class Form1 : Form
    {
        // Mouse handling variables
        private int selectX;
        private int selectY;
        private int selectWidth;
        private int selectHeight;
        public Pen selectPen;
        private bool start = false;

        public Form1()
        {
            InitializeComponent();
            createTool();
            Console.WriteLine("Crop Tool initialized!");
        }

        private void createTool()
        {
            this.Hide();

            // Take screenshot from the active screen.
            Bitmap printscreen = new Bitmap(Screen.FromControl(this).Bounds.Width,
                                     Screen.FromControl(this).Bounds.Height);

            Graphics graphics = Graphics.FromImage(printscreen as Image);

            // Point the screenshot to the active window.
            Point panelLocation = PointToScreen(new Point(0, 0));

            graphics.CopyFromScreen(panelLocation.X, panelLocation.Y, 0, 0, printscreen.Size);

            using (MemoryStream s = new MemoryStream())
            {
                //save graphic variable into memory
                printscreen.Save(s, ImageFormat.Bmp);
                pictureBox1.Size = new System.Drawing.Size(this.Width, this.Height);
                //set the picture box with temporary stream
                pictureBox1.Image = Image.FromStream(s);
            }

            this.Show();

            Cursor = Cursors.Cross;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Image cropping started!");
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            
            if (this.start)
            {
                pictureBox1.Refresh();

                selectWidth = e.X - selectX;
                selectHeight = e.Y - selectY;

                pictureBox1.CreateGraphics().DrawRectangle(selectPen,
                          selectX, selectY, selectWidth, selectHeight);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!start)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    selectX = e.X;
                    selectY = e.Y;
                    selectPen = new Pen(Color.Red, 1);
                    selectPen.DashStyle = DashStyle.DashDotDot;
                }
                
                pictureBox1.Refresh();
                
                start = true;
            }
            else
            {
                if (pictureBox1.Image == null)
                    return;

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    pictureBox1.Refresh();
                    selectWidth = e.X - selectX;
                    selectHeight = e.Y - selectY;
                    pictureBox1.CreateGraphics().DrawRectangle(selectPen, selectX,
                             selectY, selectWidth, selectHeight);

                }
                start = false;

                Console.WriteLine("Image cropping ended! Saving image..");
                SaveToClipboard();
            }
        }

        private void SaveToClipboard()
        {
            if (selectWidth > 0)
            {

                Rectangle rect = new Rectangle(selectX, selectY, selectWidth, selectHeight);
                Bitmap OriginalImage = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
                Bitmap _img = new Bitmap(selectWidth, selectHeight);
                Graphics g = Graphics.FromImage(_img);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(OriginalImage, 0, 0, rect, GraphicsUnit.Pixel);
                //insert image stream into clipboard
                Clipboard.SetImage(_img);
            }

            Console.WriteLine("Image saved");
            Application.Exit();
        }
    }
}
