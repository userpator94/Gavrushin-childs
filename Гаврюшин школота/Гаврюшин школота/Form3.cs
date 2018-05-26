using Microsoft.Office.Interop.Excel;
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
            RegionSelector(@"\базы\");
            dgv1Rows();
        }

        private void normMaker_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 ||
                regionSelect.SelectedIndex == -1)
            {
                MessageBox.Show("Проверьте выбраны ли ПОЛ, ВОЗРАСТ и РЕГИОН",
                    "Внимание!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            f3dataGridView1.Rows.Clear();
            dgvOUT.Rows.Clear();

            //int N = Convert.ToInt32(f3dataGridView1[1, 0].Value.ToString());
            double averageH = Convert.ToDouble(f3dataGridView1[2, 0].Value.ToString().Replace(".", ","));
            //averageH = Math.Round(averageH);
            double averageM = Convert.ToDouble(f3dataGridView1[2, 1].Value.ToString().Replace(".", ","));
            //double medianaH = Convert.ToDouble(f3dataGridView1[3, 0].Value.ToString());
            //double medianaM = Convert.ToDouble(f3dataGridView1[3, 1].Value.ToString());
            double stDeviationH = Convert.ToDouble(f3dataGridView1[4, 0].Value.ToString().Replace(".", ","));
            stDeviationH = Math.Round(stDeviationH);
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
            double Rxy = 0;
            Rxy = Convert.ToDouble(f3dataGridView1[10, 1].Value.ToString().Replace(".", ","));
            double sigmaR = 0;
            sigmaR = Convert.ToDouble(f3dataGridView1[11, 1].Value.ToString().Replace(".", ","));

            //вывод роста
            int min = Convert.ToInt16(Math.Truncate(Math.Round(averageH) - 2.1 * stDeviationH)); //Math.Round(averageH)
            int max = Convert.ToInt16(Math.Round(Math.Round(averageH) + 2.1 * stDeviationH)); //Math.Round(averageH)
            double MsigmaR = 0;
            string s = null;
            int count = 0;
            for (int i = min; i <= max; i++)
            {
                MsigmaR = averageM + Rxy * (i - Math.Round(averageH));
                if (i == min && count < 1)
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

            normsSaver.Enabled = true;
        }

        private void normsSaver_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 ||
                regionSelect.SelectedIndex == -1)
            {
                MessageBox.Show("Проверьте выбраны ли ПОЛ, ВОЗРАСТ и РЕГИОН",
                    "Внимание!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            Microsoft.Office.Interop.Excel.Application ExcelApp =
                new Microsoft.Office.Interop.Excel.Application(); //открыть эксель
            ExcelApp.Visible = true;

            string name = regionSelect.SelectedItem.ToString();
            name = name.Remove(name.Length - 4);
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\нормативы";
            //exeDir += @"\нормативы" + name + "_norm.xls";
            name = exeDir + name + "_norm.xls";
            //name = System.IO.Path.Combine(exeDir, name);

            if (System.IO.File.Exists(name) == false)
                System.IO.File.Copy(exeDir + @"\z_norm_example.xls", name);

            Workbook ObjWorkBook =
                ExcelApp.Workbooks.Open(name,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing); //открыть файл

            Worksheet ObjWorkSheet;
            int page = comboBox2.SelectedIndex + 1;
            if (comboBox1.SelectedIndex == 0) page += 20;
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[page];
            ObjWorkSheet.Columns.ColumnWidth = 7;
            ObjWorkSheet.Columns[1].ColumnWidth = 14;
            ObjWorkSheet.Cells[1, 1] = "Параметры";
            ObjWorkSheet.Cells[1, 2] = "N";
            ObjWorkSheet.Cells[1, 3] = "M";
            ObjWorkSheet.Cells[1, 4] = "m";
            ObjWorkSheet.Cells[1, 5] = "σ";
            ObjWorkSheet.Cells[1, 6] = "P25";
            ObjWorkSheet.Cells[1, 7] = "P50";
            ObjWorkSheet.Cells[1, 8] = "P75";
            ObjWorkSheet.Cells[1, 9] = "V";
            ObjWorkSheet.Cells[1, 10] = "r";
            ObjWorkSheet.Cells[1, 11] = "Rx/y";
            ObjWorkSheet.Cells[1, 12] = "δR";

            for (int i = 0; i < f3dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < f3dataGridView1.RowCount; j++)
                {
                    ObjWorkSheet.Cells[j + 2, i + 1] = (f3dataGridView1[i, j].Value);
                }
            }

            ObjWorkSheet.Cells[5, 1] = "Оценка роста";
            ObjWorkSheet.Cells[5, 2] = "Рост, см";
            ObjWorkSheet.Cells[5, 3] = "М-δR";
            ObjWorkSheet.Cells[5, 4] = "Mcp";
            ObjWorkSheet.Cells[5, 5] = "М+δR";
            ObjWorkSheet.Cells[5, 6] = "М+1.5δR";

            for (int i = 0; i < dgvOUT.ColumnCount; i++)
            {
                for (int j = 0; j < dgvOUT.RowCount; j++)
                {
                    ObjWorkSheet.Cells[j + 6, i + 1] = (dgvOUT[i, j].Value);
                }
            }

            //ObjWorkBook.Save();
            ObjWorkBook.Close(true, Type.Missing, Type.Missing);
            ExcelApp.Quit();
            ExcelApp.Quit();
            GC.Collect();
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
            //f3dataGridView1[1, 1].ReadOnly = true;
        }

        public void RegionSelector(string s)
        {
            regionSelect.Items.Clear();
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = exeDir + s;
            string[] file_list = System.IO.Directory.GetFiles(path, "*.xls");
            string regions = null;
            //regionSelect.Items.Add("такого региона нет");
            for (int i = 0; i < file_list.Length; i++)
            {
                //regions = Regex.Replace(file_list[i], exeDir, String.Empty);
                regions = file_list[i].Substring(file_list[i].LastIndexOf("\\"));
                regionSelect.Items.Add(regions);
                if (regions == @"\z_base_example.xls" || regions == @"\z_norm_example.xls")
                    regionSelect.Items.Remove(regions);
            }

            int maxWidth = 0, temp = 0;
            foreach (var obj in regionSelect.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), regionSelect.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            if (maxWidth == 0) maxWidth = 45;
            regionSelect.DropDownWidth = maxWidth;
        }
    }
}
