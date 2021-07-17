using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pfim;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using 图片尺寸修改.classs;
using System.Collections;

namespace 图片尺寸修改
{

    public partial class Form1 : Form
    {


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);


        mousehover mousehover = new mousehover();
        PickBox imagecontroll = new PickBox();

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private Image image = null;
        private Bitmap bmp = new Bitmap(1280, 800);
        private Color backcolor = Color.FromArgb(224, 224, 224);
        private string sanbaipath = "";
        private string filenamename;
        public float X;
        public float Y;
        const int WM_NCHITTEST = 0x0084;
        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 0x10;
        const int HTBOTTOMRIGHT = 17;

        public Form1()
        {
            LoadResourceDll.RegistDLL();
            InitializeComponent();
            //数据初始化
            this.Icon = global::图片尺寸修改.Properties.Resources.未标题_3;
            listBox1.SelectedItem = "平铺";
            pictureBox2.Parent = pictureBox1;
            if (File.Exists("config.txt"))
            {
                if (File.Exists(File.ReadAllText("config.txt") + @"\300.exe"))
                {
                    sanbaipath = File.ReadAllText("config.txt");
                    设置三百路径ToolStripMenuItem.Text = "设置三百路径(已设置)";
                }
            }
            
            this.MouseWheel += FrmMain_MouseWheel;
            this.Resize += new EventHandler(form1_Resize);//窗体调整大小时引发事件
            
            X = panel1.Width;//获取窗体的宽度

            Y = panel1.Height;//获取窗体的高度
            /*控件缩放初始化
            this.Resize += new EventHandler(modular_calEchoPhaseFromSignal1_Resize);//窗体调整大小时引发事件

            X = this.Width;//获取窗体的宽度

            Y = this.Height;//获取窗体的高度

            setTag(this);调用方法*/
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        else m.Result = (IntPtr)HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        else m.Result = (IntPtr)HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)HTBOTTOM;
                    break;
            }
        }
        //控件缩放
        private void setTag(Control cons)
        {
            //遍历窗体中的控件
            foreach (Control con in cons.Controls)

            {

                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size + ":" + con.Name;

                if (con.Controls.Count > 0)

                    setTag(con);
            }

        }
        private void setControls(float newx, float newy, Control cons)

        {
            foreach (Control con in cons.Controls)

            {
                con.Visible = false;

            }
            //遍历窗体中的控件，重新设置控件的值

            foreach (Control con in cons.Controls)

            {

                if (con.Name == panel1.Name)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//获取控件的Tag属性值，并分割后存储字符串数组

                    float a = Convert.ToSingle(mytag[0]) * newx;//根据窗体缩放比例确定控件的值，宽度

                    con.Width = (int)a;//宽度

                    a = Convert.ToSingle(mytag[1]) * newy;//高度

                    con.Height = (int)(a);

                    a = Convert.ToSingle(mytag[2]) * newx;//左边距离

                    con.Left = (int)(a);

                    a = Convert.ToSingle(mytag[3]) * newy;//上边缘距离

                    con.Top = (int)(a);

                    Single currentSize = Convert.ToSingle(mytag[4]) * newx;//字体大小

                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);

                    if (con.Controls.Count > 0)

                    {

                        setControls(newx, newy, con);

                    }
                }

            }
            foreach (Control con in cons.Controls)

            {
                con.Visible = true;

            }
        }
        void modular_calEchoPhaseFromSignal1_Resize(object sender, EventArgs e)

        {

            float newx = (this.Width) / X; //窗体宽度缩放比例

            float newy = this.Height / Y;//窗体高度缩放比例

            setControls(newx, newy, this);//随窗体改变控件大小

            // this.Text = this.Width.ToString() + " " + this.Height.ToString();//窗体标题栏文本

        }
        //控件缩放
        void form1_Resize(object sender, EventArgs e)
        {
            //panel尺寸
            panel1.Height = this.Height - 43;
            panel1.Width = this.Width - 136;

            float newx = (panel1.Width) / X; //窗体宽度缩放比例

            float newy = panel1.Height / Y;//窗体高度缩放比例

            if (newx > newy)
            {
                float a = Convert.ToSingle(640) * newy;//根据窗体缩放比例确定控件的值，宽度

                pictureBox1.Width = (int)a;//宽度

                a = Convert.ToSingle(400) * newy;//高度

                pictureBox1.Height = (int)(a);

                pictureBox1.Left = panel1.Width / 2 - pictureBox1.Width/2;
                pictureBox1.Top = panel1.Height / 2 - pictureBox1.Height / 2;


            }
            else
            {
                float a = Convert.ToSingle(640) * newx;//根据窗体缩放比例确定控件的值，宽度

                pictureBox1.Width = (int)a;//宽度

                a = Convert.ToSingle(400) * newx;//高度

                pictureBox1.Height = (int)(a);

                pictureBox1.Left = panel1.Width / 2 - pictureBox1.Width / 2;
                pictureBox1.Top = panel1.Height / 2 - pictureBox1.Height / 2;


            }
            if (image != null)
            {
                imagecontroll.SelectControl(pictureBox2, new EventArgs());
            }
        }

        //打开图片
        //拖拽添加
        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))      //判断该文件是否可以转换到文件放置格式

            {
                e.Effect = DragDropEffects.Link;       //放置效果为链接放置

            }

            else

            {
                e.Effect = DragDropEffects.None;      //不接受该数据,无法放置，后续事件也无法触发

            }
        }
        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            dakaitupian(path);
        }
        //打开添加
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择一个图片";
            dialog.Filter = "图片文件(*.png,*.jpg,*.bmp,*.jpeg,*.dds,*.tga)|*.png;*.jpg;*.bmp;*.jpeg;*.dds;*.tga";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file1 = dialog.FileName;
                dakaitupian(file1);
            }
        }
        //打开图片时
        private void dakaitupian(string path)
        {
            if (Path.GetExtension(path) == ".dds" || Path.GetExtension(path) == ".tga")
            {

                using (var image1 = Pfim.Pfim.FromFile(path))
                {
                    PixelFormat format;

                    // Convert from Pfim's backend agnostic image format into GDI+'s image format
                    switch (image1.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        default:
                            // see the sample for more details
                            throw new NotImplementedException();
                    }

                    // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                    // in this snippet but useful technique if the data was going to be used in
                    // control like a picture box
                    var handle = GCHandle.Alloc(image1.Data, GCHandleType.Pinned);
                    try
                    {
                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(image1.Data, 0);
                        var bmp = new Bitmap(image1.Width, image1.Height, image1.Stride, format, data);
                        image = bmp;
                        pictureBox2.Image = image;
                    }
                    finally
                    {
                        handle.Free();
                    }
                }

                creatimage();
                imagecontroll.WireControl(pictureBox2);
                imagecontroll.SelectControl(pictureBox2, new EventArgs());
            }//dds和tga处理
            else
            {
                image = Image.FromFile(path);
                pictureBox2.Image = image;
                creatimage();
                imagecontroll.WireControl(pictureBox2);
                imagecontroll.SelectControl(pictureBox2, new EventArgs());
            }//非这个情况处理
            pictureBox2.Visible = true;
            pictureBox2.BackColor = backcolor;
            filenamename = Path.GetFileName(path);
            pictureBox1.Image = bmp;
            backcolor = Color.FromArgb(224, 224, 224);
        }
        //打开图片

        //按设定sizemode刷新图片
        private void creatimage()
        {
            if (listBox1.SelectedItem == "平铺")
            {
                float scale = float.Parse(image.Width.ToString()) / float.Parse(image.Height.ToString());//获得图片比例宽:长
                if (scale < 1.6)//是短图片
                {
                    pictureBox2.Location = new Point((pictureBox1.Width - image.Width * pictureBox1.Height / image.Height) / 2, 0);
                    pictureBox2.Size = new Size(image.Width * pictureBox1.Height/image.Height, pictureBox1.Height);
                }
                if (scale >= 1.6)
                {
                    pictureBox2.Location = new Point(0, (pictureBox1.Height - image.Height*pictureBox1.Width/image.Width) / 2);
                    pictureBox2.Size = new Size(pictureBox1.Width, image.Height * pictureBox1.Width / image.Width);
                }
            }
            if (listBox1.SelectedItem == "拉伸")
            {
                pictureBox2.Location = new Point(0, 0);
                pictureBox2.Size = new Size(pictureBox1.Width, pictureBox1.Height);
            }
            if (listBox1.SelectedItem == "填充")
            {
                float scale = float.Parse(image.Width.ToString()) / float.Parse(image.Height.ToString());
                if (scale > 1.6)
                {
                    pictureBox2.Location = new Point((pictureBox1.Width - image.Width * pictureBox1.Height / image.Height) / 2, 0);
                    pictureBox2.Size = new Size(image.Width * pictureBox1.Height / image.Height, pictureBox1.Height);
                }
                if (scale <= 1.6)
                {
                    pictureBox2.Location = new Point(0, (pictureBox1.Height - image.Height * pictureBox1.Width / image.Width) / 2);
                    pictureBox2.Size = new Size(pictureBox1.Width, image.Height * pictureBox1.Width / image.Width);
                }
            }
        }
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                creatimage();
                imagecontroll.SelectControl(pictureBox2, new EventArgs());
            }
        }//重置贴图
        //按设定sizemode刷新图片

        private void FrmMain_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0&&(pictureBox1.Width+pictureBox1.Left<panel1.Width-16&&pictureBox1.Top+pictureBox1.Height<panel1.Height-10&&pictureBox1.Top>16&&pictureBox1.Left>10))
            {
                
                pictureBox1.Width = (int)(pictureBox1.Width + 8);//宽度
                pictureBox1.Height = (int)(pictureBox1.Height + 5);
                pictureBox1.Left = panel1.Width / 2 - pictureBox1.Width / 2;
                pictureBox1.Top = panel1.Height / 2 - pictureBox1.Height / 2;
            }
            else
            {
                
                pictureBox1.Width = (int)(pictureBox1.Width - 8);//宽度
                pictureBox1.Height = (int)(pictureBox1.Height -5);
                pictureBox1.Left = panel1.Width / 2 - pictureBox1.Width / 2;
                pictureBox1.Top = panel1.Height / 2 - pictureBox1.Height / 2;
            }
            if (image != null)
            {
                imagecontroll.SelectControl(pictureBox2, new EventArgs());
            }
        }//缩放


        //平铺啥的选择框
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (image != null)
            {
                creatimage();
                imagecontroll.SelectControl(pictureBox2, new EventArgs());
            }
        }

        //下拉栏里选
        private void 平铺ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            平铺ToolStripMenuItem.Text = "平铺✔";
            填充ToolStripMenuItem.Text = "填充";
            拉伸ToolStripMenuItem.Text = "拉伸";
            listBox1.SelectedItem = "平铺";
            if (image != null)
            {
                creatimage();
            }
        }

        private void 填充ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            平铺ToolStripMenuItem.Text = "平铺";
            填充ToolStripMenuItem.Text = "填充✔";
            拉伸ToolStripMenuItem.Text = "拉伸";
            listBox1.SelectedItem = "填充";
            if (image != null)
            {
                creatimage();
            }
        }

        private void 拉伸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            平铺ToolStripMenuItem.Text = "平铺";
            填充ToolStripMenuItem.Text = "填充";
            拉伸ToolStripMenuItem.Text = "拉伸✔";
            listBox1.SelectedItem = "拉伸";
            if (image != null)
            {
                creatimage();
            }
        }
        //平铺啥的选择框

        //磁吸切换
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (imagecontroll.cixi)
            {
                imagecontroll.cixi = false;
                pictureBox5.Image = global::图片尺寸修改.Properties.Resources.button_32px1;
            }
            else
            {
                imagecontroll.cixi = true;
                pictureBox5.Image = global::图片尺寸修改.Properties.Resources.button_32px2;
            }
        }
        //磁吸切换


        private void pictureBox6_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                backcolor = dialog.Color;
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(backcolor);
                g.Save();
                pictureBox1.Image = bmp;
                pictureBox2.BackColor = backcolor;
            }
        }//背景色

        
        //杂项
        private void 设置三百路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选300路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && File.Exists(dialog.SelectedPath + @"\300.exe"))
            {
                sanbaipath = dialog.SelectedPath;
                设置三百路径ToolStripMenuItem.Text = "设置三百路径(已设置)";
                File.WriteAllText("config.txt", sanbaipath);
            }
            else
            {
                MessageBox.Show("路径不对","要选三百根目录");
            }
        }//三百路径

        private void 打开300壁纸文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(sanbaipath != "")
            {
                System.Diagnostics.Process.Start(sanbaipath + @"\LinmengConfig\BackGround\");
            }
            else
            {
                MessageBox.Show("未设置300路径");
            }
        }//打开壁纸文件夹

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (sanbaipath != "" && pictureBox2.Image != null)
            {
                imagecontroll.Remove();
                pictureBox2.Image = null;

                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(image, pictureBox2.Left * 1280 / pictureBox1.Width, pictureBox2.Location.Y * 800 / pictureBox1.Height, pictureBox2.Size.Width * 1280 / pictureBox1.Width, pictureBox2.Size.Height * 800 / pictureBox1.Height);
                GC.Collect();
                bmp.Save(sanbaipath + @"\LinmengConfig\BackGround\" + filenamename);
                
                GC.Collect();

                backcolor = Color.FromArgb(224, 224, 224);
                bmp = new Bitmap(1280, 800);
                Bitmap b1 = new Bitmap(1280, 800);
                Graphics g1 = Graphics.FromImage(b1);
                g1.Clear(backcolor);
                g1.Save();
                pictureBox1.Image = b1;
                pictureBox2.Visible = false;
                MessageBox.Show("应该可以了");

            }
            else
            {
                MessageBox.Show("未设置300路径");
            }
        }//导出

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imagecontroll.Remove();
            pictureBox2.Image = null;
            filenamename = "";
            Bitmap b = new Bitmap(1280, 800);
            Graphics g = Graphics.FromImage(b);
            g.Clear(backcolor);
            g.Save();
            pictureBox1.Image = b;
            backcolor = Color.FromArgb(224, 224, 224);
            pictureBox2.BackColor = backcolor;
            pictureBox2.Visible = false;
        }//关闭

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "另存为";
            saveFileDialog.Filter = "png(*.png)|*.png|jpg(*.jpg)|*.jpg|bmp(*.bmp)|*.bmp";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                imagecontroll.Remove();
                pictureBox2.Image = null;
                
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(image, pictureBox2.Left*1280/pictureBox1.Width, pictureBox2.Location.Y * 800 / pictureBox1.Height, pictureBox2.Size.Width * 1280 / pictureBox1.Width, pictureBox2.Size.Height * 800 / pictureBox1.Height);
                GC.Collect();
                bmp.Save(saveFileDialog.FileName.ToString());
                
                GC.Collect();

                backcolor = Color.FromArgb(224, 224, 224);
                bmp = new Bitmap(1280, 800);
                Bitmap b1 = new Bitmap(1280, 800);
                Graphics g1 = Graphics.FromImage(b1);
                g1.Clear(backcolor);
                g1.Save();
                pictureBox1.Image = b1;
                pictureBox2.Visible = false;
                MessageBox.Show("应该可以了");
            }
        }//另存为

        private void 透明背景ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            bmp = new Bitmap(1280, 800);
            creatimage();
            pictureBox1.Image = bmp;
            backcolor = Color.FromArgb(224, 224, 224);
            pictureBox2.BackColor = backcolor;

        }

        private void 帮助与作者信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("作者：林蒙\n功能介绍在我b站首页\nb站林蒙のゲーム\nq群：631487094");
        }


        //ui切换图片
        private void MouseEnter(object sender, EventArgs e)
        {
            switch ((sender as PictureBox).Name)
            {
                case "pictureBox9":
                    mousehover.mouseinhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.resize_vertical_26pxred, 2);
                    break;
                case "pictureBox6":
                    mousehover.mouseinhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.button_32px_red, 0);
                    break;

                case "pictureBox7":
                    mousehover.mouseinhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.重置2, 0);
                    break;

                case "pictureBox4":
                    mousehover.mouseinhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.导出红, 0);
                    break;

                case "pictureBox3":
                    mousehover.mouseinhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.compress_26px_red, 2);
                    break;

                case "关闭按钮":
                    mousehover.mouseinhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.delete_26px_red, 2);
                    break;
            }

        }
        private void Mouseleave(object sender, EventArgs e)
        {
            switch ((sender as PictureBox).Name)
            {
                case "pictureBox9":
                    mousehover.mouseunhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.resize_vertical_26px, 2);
                    break;

                case "pictureBox6":
                    mousehover.mouseunhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.button_32px, 0);
                    break;

                case "pictureBox7":
                    mousehover.mouseunhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.重置1, 0);
                    break;

                case "pictureBox4":
                    mousehover.mouseunhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.导出灰, 0);
                    break;

                case "pictureBox3":
                    mousehover.mouseunhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.compress_26px, 2);
                    break;
                case "关闭按钮":
                    mousehover.mouseunhover(sender as PictureBox, global::图片尺寸修改.Properties.Resources.delete_26px, 2);
                    break;
            }

        }
        //ui切换图片

        //关闭和最小化
        private void 关闭按钮_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            
            _=(this.WindowState == FormWindowState.Maximized) ? this.WindowState = FormWindowState.Normal : this.WindowState=FormWindowState.Maximized;

        }

        private void menuStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
            }
            catch

            { }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
            }
            catch

            { }
        }


        //关闭和最小化


    }

}
