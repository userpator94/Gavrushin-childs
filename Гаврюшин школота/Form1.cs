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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //char[] tab = new char[] { '\u0009' }; //'\u000d'
            String[] stroki = textBox1.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            MessageBox.Show(stroki.Length.ToString());
            String[,] strokiParts = new String[stroki.Length, 4];            

            for (int i=0; i < stroki.Length; i++)
            {
                String[] cells = stroki[i].Split(new char[] { '\u0009' });
                for (int j = 0; j < 4; j++)
                {
                    strokiParts[i, j] = cells[j];
                }
            }

            
        }

        private void ShowNormativsButton_Click(object sender, EventArgs e)
        {
            Form normativs = new Form2();
            //Form f1 = new Form1();
            //normativs.StartPosition = f1.StartPosition;
            normativs.StartPosition = FormStartPosition.CenterParent;
            normativs.Location = this.Location;
            normativs.Show(this);
            if (normativs.StartPosition == FormStartPosition.CenterParent)
            {
                var x = Location.X + (Width - normativs.Width) / 2;
                var y = Location.Y + (Height - normativs.Height) / 2;
                normativs.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
            }
        }
    }
}
