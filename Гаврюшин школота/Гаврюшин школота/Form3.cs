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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            dgv1Rows();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dgv1Rows()
        {
            this.f3dataGridView1.Rows.Add
                ("Длина тела(см)", "", "", "", "", "", "", "", "", "-", "-", "-");
            this.f3dataGridView1.Rows.Add
                ("Масса тела(см)", "", "", "", "", "", "", "", "", "", "", "");            
            f3dataGridView1[9, 0].ReadOnly = true;
            f3dataGridView1[10, 0].ReadOnly = true;
            f3dataGridView1[11, 0].ReadOnly = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (regionSelect.SelectedItem == null)
            {
                MessageBox.Show("Чтобы сохранить значения, их следует сначала построить",
                    "Внимание",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }            
            Form2 f2 = new Form2();
            f2.normsSaver(regionSelect.SelectedItem.ToString());
        }
    }
}
