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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("choose your destiny");
                return;
            }
            comboBox1.SelectedIndex = 1;

            //Получаем данные из textBox1
            textBox1.Text = textBox1.Text.Replace("/", ".");
            //char[] tab = new char[] { '\u0009' }; //'\u000d'
            String[] stroki = textBox1.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //MessageBox.Show(stroki.Length.ToString());
            String[,] strokiParts = new String[stroki.Length, 4];            

            for (int i=0; i < stroki.Length; i++)
            {
                String[] cells = stroki[i].Split(new char[] { '\u0009' });
                for (int j = 0; j < 4; j++)
                    strokiParts[i, j] = cells[j];               
            }


            //считаем возраст в днях и вносим в новый массив
            TimeSpan datesBetween;
            string qwe = "";
            int[] age = new int[stroki.Length];
            double[,] excelTable = new double[(strokiParts.GetLength(0)), 3];
            for (int i = 0; i < stroki.Length; i++)
            {
                datesBetween = Convert.ToDateTime(strokiParts[i, 1]) - Convert.ToDateTime(strokiParts[i, 0]);
                if (datesBetween.Days >= 2373 && datesBetween.Days < 2738) age[i] = 7;
                if (datesBetween.Days >= 2738 && datesBetween.Days < 3104) age[i] = 8;
                if (datesBetween.Days >= 3104 && datesBetween.Days < 3469) age[i] = 9;
                if (datesBetween.Days >= 3469 && datesBetween.Days < 3835) age[i] = 10;
                if (datesBetween.Days >= 3835 && datesBetween.Days < 4202) age[i] = 11;
                if (datesBetween.Days >= 4202 && datesBetween.Days < 4569) age[i] = 12;
                if (datesBetween.Days >= 4569 && datesBetween.Days < 4930) age[i] = 13;
                if (datesBetween.Days >= 4930 && datesBetween.Days < 5296) age[i] = 14;
                if (datesBetween.Days >= 5296 && datesBetween.Days < 5661) age[i] = 15;
                if (datesBetween.Days >= 5661 && datesBetween.Days < 6026) age[i] = 16;
                if (datesBetween.Days >= 6026 && datesBetween.Days < 6392) age[i] = 17;

                excelTable[i, 0] = age[i];
                qwe += age[i] + " ";
            }

            //добавляем рост и массу и первого массива
            for (int i = 0; i < stroki.Length; i++)
            {
                for (int j = 1; j < 3; j++)
                    excelTable[i, j] = double.Parse(strokiParts[i, j + 1]);
            }

            writeToExcel(excelTable, excelTable.GetLength(0), excelTable.GetLength(1), age);

            MessageBox.Show(qwe + "");
        }

        private void writeToExcel(double[,] arrayExc, int k, int m, int[] age)
        {            
            Microsoft.Office.Interop.Excel.Application ObjWorkExcel = 
                new Microsoft.Office.Interop.Excel.Application(); //открыть эксель
            ObjWorkExcel.Visible = true;            

            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = 
                ObjWorkExcel.Workbooks.Open(System.IO.Path.Combine(exeDir, "base.xls"), 
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                Type.Missing, Type.Missing, Type.Missing, Type.Missing); //открыть файл

            Worksheet ObjWorkSheet;
            int R = 0;

            for (int q = 0; q < age.Length; q++)
            {
                if (comboBox1.SelectedItem.ToString() == "мужской") age[q] = age[q] + 11;
                ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[age[q] - 6]; //получить нужный лист

                var lastCell = ObjWorkSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell);//1 ячейку
                                                                                              
                //string[,] list = new string[lastCell.Row, lastCell.Column]; // массив значений с листа равен по размеру листу
                //for (int i = 0; i < lastCell.Column; i++) //по всем колонкам
                //    for (int j = 0; j < lastCell.Row; j++) // по всем строкам
                //        list[j, i] = ObjWorkSheet.Cells[i + 1, j + 1].Text.ToString();//считываем текст в строку

                //записываем из textBox1 в Excel
                if (lastCell.Row == 1) R = 1;
                else R = lastCell.Row + 1;

                double[] outputExc = new double[3] ;
                for (int i = 0; i < 3; i++)
                    outputExc[i] = arrayExc[q, i];

                R = lastCell.Row;
                //Range c1 = (Range)ObjWorkSheet.Cells[R, 1];
                ////Range c2 = (Range)ObjWorkSheet.Cells[R + k, m];
                //Range c2 = (Range)ObjWorkSheet.Cells[R + 1, 1];
                //Range range = ObjWorkSheet.get_Range(c1, c2);
                //range.set_Value(XlRangeValueDataType.xlRangeValueDefault, outputExc);

                Range c1 = (Range)ObjWorkSheet.Cells[R, 1];
                Range c2 = (Range)ObjWorkSheet.Cells[R + 1, 3];
                Range r = ObjWorkSheet.get_Range(c1, c2);
                r.Value2 = outputExc;
            }


            //ObjWorkBook.Close(true, Type.Missing, Type.Missing); //закрыть с сохранением
            //ObjWorkExcel.Quit(); // выйти из экселя
            //GC.Collect(); // убрать за собой
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
                normativs.Location = new System.Drawing.Point(Math.Max(x, 0), Math.Max(y, 0));
            }
        }
    }
}
