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

        private void normMaker_Click(object sender, EventArgs e)
        {
            //if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 ||
            //    regionSelect.SelectedIndex == -1)
            //{
            //    MessageBox.Show("Проверьте выбраны ли ПОЛ, ВОЗРАСТ и РЕГИОН",
            //        "Внимание!",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Exclamation,
            //        MessageBoxDefaultButton.Button1,
            //        MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}

            //("Длина тела(см)", N, average, mediana, stDeviation, percent25, percent50,
            //    percent75, V, "-", "-", "-");

            //int N = Convert.ToInt32(f3dataGridView1[1, 0].Value.ToString());
            double averageH = Convert.ToDouble(f3dataGridView1[2, 0].Value.ToString().Replace(".", ","));
            double averageM = Convert.ToDouble(f3dataGridView1[2, 1].Value.ToString().Replace(".", ","));
            //double medianaH = Convert.ToDouble(f3dataGridView1[3, 0].Value.ToString());
            //double medianaM = Convert.ToDouble(f3dataGridView1[3, 1].Value.ToString());
            double stDeviationH = Convert.ToDouble(f3dataGridView1[4, 0].Value.ToString().Replace(".", ","));
            //double stDeviationM = Convert.ToDouble(f3dataGridView1[4, 1].Value);
            //double Hpercent25 = Convert.ToDouble(f3dataGridView1[5, 0].Value);
            //double Mpercent25 = Convert.ToDouble(f3dataGridView1[5, 1].Value);
            //double Hpercent50 = Convert.ToDouble(f3dataGridView1[6, 0].Value);
            //double Mpercent50 = Convert.ToDouble(f3dataGridView1[6, 1].Value);
            //double Hpercent75 = Convert.ToDouble(f3dataGridView1[7, 0].Value);
            //double Mpercent75 = Convert.ToDouble(f3dataGridView1[7, 1].Value);
            //double VH = Convert.ToDouble(f3dataGridView1[8, 0].Value);
            //double VM = Convert.ToDouble(f3dataGridView1[8, 1].Value);
            //double r = Convert.ToDouble(f3dataGridView1[9, 1].Value);
            double Rxy = Convert.ToDouble(f3dataGridView1[10, 1].Value.ToString().Replace(".", ","));
            double sigmaR = Convert.ToDouble(f3dataGridView1[11, 1].Value);

            //вывод роста
            int min = Convert.ToInt16(Math.Truncate(averageH - 2.1 * stDeviationH));
            int max = Convert.ToInt16(Math.Round(averageH + 2.1 * stDeviationH));
            double MsigmaR = 0;
            string s = null;
            int count = 0;
            for (int i = min-1; i <= max; i++)
            {
                MsigmaR = averageM + Rxy * (i - averageH);
                if (i == min - 1 && count < 1)
                {
                    s = "низкий";
                    count++;
                    this.dgvOUT.Rows.Add
                        (s, i, "", "", "", "");
                    continue;
                }
                if (i < Math.Ceiling(averageH - stDeviationH) && count > 0)
                    s = "ниже среднего";
                if (i < Math.Ceiling(averageH + stDeviationH) &&
                    i >= Math.Ceiling(averageH - stDeviationH))
                    s = "средний";
                if (i > Math.Ceiling(averageH + stDeviationH) &&
                    i <= Math.Ceiling(averageH + 2 * stDeviationH))
                    s = "выше среднего";
                if (i == max)
                {
                    s = "выскоий";
                    this.dgvOUT.Rows.Add
                        (s, i, "", "", "", "");
                    continue;
                }                    
                this.dgvOUT.Rows.Add
                        (s, i, Math.Round(MsigmaR - sigmaR, 1), Math.Round(MsigmaR, 1),
                        Math.Round(MsigmaR + sigmaR, 1), Math.Round(MsigmaR + 1.5 * sigmaR, 1));
            }


        }

        private void normsSaver_Click(object sender, EventArgs e)
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

        private void dgv1Rows()
        {
            this.f3dataGridView1.Rows.Add
                ("Длина тела(см)", "", "", "", "", "", "", "", "", "-", "-", "-");
            this.f3dataGridView1.Rows.Add
                ("Масса тела(см)", "", "", "", "", "", "", "", "", "", "", "");            
            f3dataGridView1[9, 0].ReadOnly = true;
            f3dataGridView1[10, 0].ReadOnly = true;
            f3dataGridView1[11, 0].ReadOnly = true;
            f3dataGridView1[0, 0].ReadOnly = true;
            f3dataGridView1[0, 1].ReadOnly = true;
            f3dataGridView1[1, 1].ReadOnly = true;
        }
        
    }
}
