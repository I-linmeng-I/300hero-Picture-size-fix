using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 图片尺寸修改.classs
{
    class mousehover
    {
        public void mouseinhover(PictureBox control,Image image,int newsize)
        {
            control.Width = control.Width + newsize;
            control.Height = control.Height + newsize;
            control.Location = new Point(control.Location.X - (newsize/2), control.Location.Y - (newsize / 2));
            control.Image = image;
        }
        public void mouseunhover(PictureBox control,Image image,int newsize)
        {
            control.Width = control.Width - newsize;
            control.Height = control.Height - newsize;
            control.Location = new Point(control.Location.X + (newsize / 2), control.Location.Y + (newsize / 2));
            control.Image = image;
        }
    }
}
