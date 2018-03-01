using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Гаврюшин_школота
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Проверьте выбраны ли ПОЛ и ВОЗРАСТ",
                    "Внимание!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,                    
                    MessageBoxOptions.DefaultDesktopOnly);
                return;
            }                

            int page = 0;
            if (comboBox1.SelectedIndex == 0) page += 11; //мужской пол с 12 страницы
            page += Convert.ToInt32(comboBox2.SelectedItem.ToString()) - 6;

            Microsoft.Office.Interop.Excel.Application excApp = 
                new Microsoft.Office.Interop.Excel.Application();
            excApp.Visible = true;
            Workbook wb;
            Worksheet wsh;
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            exeDir = System.IO.Path.Combine(exeDir, "base.xls");
            wb = excApp.Workbooks.Open(exeDir, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            wsh = (Worksheet)wb.Sheets[page];
            var lastCell = wsh.Cells.SpecialCells(XlCellType.xlCellTypeLastCell);
            string[,] list = new string[lastCell.Row - 1, 3]; // массив значений с листа равен по размеру листу
            for (int i = 0; i < lastCell.Row - 1; i++) //по всем колонкам
                for (int j = 0; j < 3; j++) // по всем строкам кроме последней
                    list[i, j] = wsh.Cells[i+ 1, j + 1].Text.ToString();//считываем текст в строку

            int Nx = list.GetLength(0);
            if (Nx < 100) MessageBox.Show(
                "Записей о детях данной группы меньше 100",
                "Обратите внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1 );
            if (Nx < 2)
            {
                MessageBox.Show(
                "Записей о детях данной группы меньше 2",
                "Построение стандартов прервано",
                MessageBoxButtons.OK,
                MessageBoxIcon.Hand,
                MessageBoxDefaultButton.Button1);
                return;
            }

            //рост
            int sumH = 0;
            for (int j=0; j < Nx; j++)
            {
                //sumH += Convert.ToInt32(list[j, 1]);
                sumH += Convert.ToInt32(Math.Round(double.Parse(list[j, 1])));
            }
            //MessageBox.Show("" + dataGridView1.Rows.Count);
            this.dataGridView1.Rows.Add
                ("Рост", list.GetLength(0), sumH / Nx, "eight");


            //this.dataGridView1.Rows.Insert(0, "one", "two", "three", "four");
            //DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[-1].Clone();
            //row.Cells[0].Value = "Рост";
            //row.Cells[1].Value = list.GetLength(0);
            //row.Cells[2].Value = sumH/Nx;
            //dataGridView1.Rows.Add(row);
        }
    }
}
