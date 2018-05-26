using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Гаврюшин_школота
{
    public partial class greeting : Form
    {
        public greeting()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            var w = Screen.PrimaryScreen.Bounds.Width;
            var h = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new System.Drawing.Point((w - this.Width) / 2, (h - this.Height) / 2);
        }
    }
}
