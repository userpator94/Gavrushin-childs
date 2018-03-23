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
using Microsoft.Win32;

namespace Гаврюшин_школота
{
    public partial class Form1 : Form
    {
        public static System.Timers.Timer aTimer;

        public Form1()
        {
            InitializeComponent();
            RegisterWriterNChecker();
        }

        public static void RegisterWriterNChecker()
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            DateTime date1 = DateTime.Today;
            DateTime date2;
            RegistryKey helloKey = currentUserKey.OpenSubKey("GavrushinControl", true);
            if (helloKey == null)
            {
                helloKey = currentUserKey.CreateSubKey("GavrushinControl");
                //helloKey.SetValue("was_create", date1.ToString("d"));
                helloKey.SetValue("was_create", "7.03.2018");
                helloKey.Close();
            }
            else if (helloKey != null)
            {
                //string Gdate = helloKey.GetValue("was_create").ToString();
                string Gdate = "23.03.2018";
                DateTime.TryParse(Gdate, out date2);
                TimeSpan ts = date1 - date2;
                helloKey.Close();
                if (ts.Days > 0)
                {
                    MessageBox.Show("Неоплаченный период истёк", "Внимание",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                    Timer timer = new Timer() { Interval = 8000, Enabled = true };
                    timer.Tick += new EventHandler(timer_Tick);
                }
            }
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            (sender as Timer).Enabled = false;
            System.Windows.Forms.Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пол");
                return;
            }

            //Получаем данные из textBox1
            textBox1.Text = textBox1.Text.Replace("/", ".");
            //textBox1.Text = textBox1.Text.Replace(",", "."); // а нужна ли эта строчка
            if (textBox1.Text == "")
            {
                MessageBox.Show("Поле ввода данных пустое", "Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            //разделяем на строчки, а, затем, и на элементы
            String[] stroki = textBox1.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            String[,] strokiParts = new String[stroki.Length, 4];            
            for (int i=0; i < stroki.Length; i++)
            {
                String[] cells = stroki[i].Split(new char[] { '\u0009' });  //'\u000d'
                for (int j = 0; j < 4; j++)
                    strokiParts[i, j] = cells[j];               
            }

            //считаем возраст в днях и вносим в новый массив
            TimeSpan datesBetween;
            int[] age = new int[stroki.Length];
            double[,] excelTable = new double[(strokiParts.GetLength(0)), 3];
            for (int i = 0; i < stroki.Length; i++)
            {
                datesBetween = Convert.ToDateTime(strokiParts[i, 1]) - Convert.ToDateTime(strokiParts[i, 0]);
                //малышня
                if (datesBetween.Days >= 1004 && datesBetween.Days < 1187) age[i] = 21;
                if (datesBetween.Days >= 1187 && datesBetween.Days < 1369) age[i] = 22;
                if (datesBetween.Days >= 1369 && datesBetween.Days < 1551) age[i] = 23;
                if (datesBetween.Days >= 1551 && datesBetween.Days < 1734) age[i] = 24;
                if (datesBetween.Days >= 1734 && datesBetween.Days < 1916) age[i] = 25;
                if (datesBetween.Days >= 1916 && datesBetween.Days < 2099) age[i] = 26;
                if (datesBetween.Days >= 2099 && datesBetween.Days < 2281) age[i] = 27;
                if (datesBetween.Days >= 2281 && datesBetween.Days < 2464) age[i] = 28;
                //от 7 и старше
                if (datesBetween.Days >= 2464 && datesBetween.Days < 2738) age[i] = 7;
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
                if (datesBetween.Days >= 6392) age[i] = 18;

                excelTable[i, 0] = age[i];
            }

            //добавляем рост и массу из первого массива
            for (int i = 0; i < stroki.Length; i++)
            {
                for (int j = 1; j < 3; j++)
                    excelTable[i, j] = double.Parse(strokiParts[i, j + 1]);
            }

            CountOfAgesMessage(age);
            writeToExcel(excelTable, excelTable.GetLength(0), excelTable.GetLength(1), age);                        
        }

        private void writeToExcel(double[,] arrayExc, int k, int m, int[] age)
        {            
            Microsoft.Office.Interop.Excel.Application ObjWorkExcel = 
                new Microsoft.Office.Interop.Excel.Application(); //открыть эксель
            ObjWorkExcel.Visible = false;            

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
                if (comboBox1.SelectedItem.ToString() == "мужской") age[q] = age[q] + 20;
                if (age[q] > 40 || age[q] > 20 && comboBox1.SelectedItem.ToString() == "женский")
                {
                    age[q] = age[q] - 20;                    
                    ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[age[q]];
                }
                else                  
                    ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[age[q] + 2]; //получить нужный лист              
                var lastCell = ObjWorkSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell);//1 ячейку              

                //записываем из textBox1 в Excel
                if (lastCell.Row == 1) R = 1;
                else R = lastCell.Row + 1;

                double[] outputExc = new double[3] ;
                for (int i = 0; i < 3; i++)
                    outputExc[i] = arrayExc[q, i];

                R = lastCell.Row;
                Range c1 = (Range)ObjWorkSheet.Cells[R, 1];
                Range c2 = (Range)ObjWorkSheet.Cells[R + 1, 3];
                Range r = ObjWorkSheet.get_Range(c1, c2);
                r.Value2 = outputExc;
            }

            ObjWorkBook.Close(true, Type.Missing, Type.Missing); //закрыть с сохранением
            ObjWorkExcel.Quit(); // выйти из экселя
            GC.Collect(); // убрать за собой
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

        private void CountOfAgesMessage(int[] age)
        {
            string s = "Детей всего "+ age.Length + Environment.NewLine +
                "\r\nпо возрасту:\r\n";
            double baby_age = 0;
            var h = new Dictionary<int, int>();
            foreach (var i in age)
            {
                int res;
                if (h.TryGetValue(i, out res))
                    h[i] += 1;
                else
                    h.Add(i, 1);
            }
            foreach (var kv in h)
            {
                if (kv.Key > 20)
                {
                    switch (kv.Key)
                    {
                        case 21:
                            baby_age = 3;
                            break;
                        case 22:
                            baby_age = 3.5;
                            break;
                        case 23:
                            baby_age = 4;
                            break;
                        case 24:
                            baby_age = 4.5;
                            break;
                        case 25:
                            baby_age = 5;
                            break;
                        case 26:
                            baby_age = 5.5;
                            break;
                        case 27:
                            baby_age = 6;
                            break;
                        case 28:
                            baby_age = 6.5;
                            break;
                    }
                    s += baby_age + " лет — " + kv.Value + Environment.NewLine;
                }
                else
                    s += kv.Key + " лет — " + kv.Value + Environment.NewLine;
            }                
            //MessageBox.Show(s);
            MessageBox.Show(s, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
