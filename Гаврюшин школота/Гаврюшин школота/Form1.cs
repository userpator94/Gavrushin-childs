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
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Гаврюшин_школота
{
    public partial class Form1 : Form
    {
        public static System.Timers.Timer aTimer;
        public int copies = 0;
        Form greet = new greeting();

        public Form1()
        {
            greetingForm();
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            var w = Screen.PrimaryScreen.Bounds.Width;
            var h = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new System.Drawing.Point((w - this.Width) / 2, (h - this.Height) / 2);
            RegisterWriterNChecker();
            RegionSelector(@"\базы\");
        }

        public void greetingForm()
        {
            greet.StartPosition = FormStartPosition.Manual;
            greet.Show();
            //_pause(5000);
            var t = Task.Run(async delegate { await Task.Delay(TimeSpan.FromSeconds(3)); return 42; });
            t.Wait();
            //System.Threading.Thread.Sleep(5000);
            greet.Close();
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
                string Gdate = "15.06.2018";
                DateTime.TryParse(Gdate, out date2);
                TimeSpan ts = date1 - date2;
                helloKey.Close();
                if (ts.Days > 0)
                {
                    MessageBox.Show("Неоплаченный период истёк", "Внимание",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);                    
                    Timer timer = new Timer() { Interval = 5000, Enabled = true };
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

        //private void _pause(int value)
        //{
        //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    //string s = null;
        //    while (sw.ElapsedMilliseconds < value)
        //        greet.Close();
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пол");
                return;
            }
            if (regionSelect.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите регион");
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
            double[,] dniTable = new double[(strokiParts.GetLength(0)), 3];
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
                dniTable[i, 0] = datesBetween.Days;
            }

            //добавляем рост и массу из первого массива
            for (int i = 0; i < stroki.Length; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    excelTable[i, j] = Math.Round(double.Parse(strokiParts[i, j + 1]), 1);
                    dniTable[i, j] = Math.Round(double.Parse(strokiParts[i, j + 1]), 1);
                }                    
            }

            ///////////////////////////
            //here must be antiduplicate method
            //duplicatesInTextBox(dniTable);

            //получаем регион
            string name = regionSelect.SelectedItem.ToString();

            CountOfAgesMessage(age);
            //writeToExcel(excelTable, excelTable.GetLength(0), excelTable.GetLength(1), age, name);   
            writeToExcel(duplicatesInTextBox(dniTable), excelTable.GetLength(0), excelTable.GetLength(1), age, name);                      
        }

        private void writeToExcel(double[,] arrayExc, int k, int m, int[] age, string name)
        {            
            Microsoft.Office.Interop.Excel.Application ObjWorkExcel = 
                new Microsoft.Office.Interop.Excel.Application(); //открыть эксель
            ObjWorkExcel.Visible = false;            

            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            exeDir += @"\базы" + name;
            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = 
                ObjWorkExcel.Workbooks.Open(exeDir, 
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
                
                //try
                //{
                    var lastCell = ObjWorkSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing);//1 ячейку              
                //}
                //catch (System.Runtime.InteropServices.COMException)
                //{
                //    lastCell = ObjWorkSheet.Cells.Find(
                //"*",
                //System.Reflection.Missing.Value,
                //XlFindLookIn.xlValues,
                //XlLookAt.xlWhole,
                //XlSearchOrder.xlByRows,
                //XlSearchDirection.xlPrevious,
                //false,
                //System.Reflection.Missing.Value,
                //System.Reflection.Missing.Value).Row;
                //}
                

                //записываем из textBox1 в Excel
                if (lastCell.Row == 1) R = 1;
                else R = lastCell.Row + 1;

                double[] outputExc = new double[2];
                for (int i = 1; i < 3; i++)
                    outputExc[i-1] = arrayExc[q, i];

                R = lastCell.Row;
                Range c1 = (Range)ObjWorkSheet.Cells[R, 1];
                Range c2 = (Range)ObjWorkSheet.Cells[R + 1, 2]; //2 поменять на 3, если не так отображается
                Range r = ObjWorkSheet.get_Range(c1, c2);
                r.Value2 = outputExc;

                //проверка на дубликаты
                //r = ObjWorkSheet.UsedRange;
                //var primaArr = (object[,])r.Value;
                //values = (System.Array)xlRange.Cells.Value;
                //duplicatesElimination(values, outputExc);
                //MessageBox.Show(Compare(values, outputExc) + "");
            }

            ObjWorkBook.Close(true, Type.Missing, Type.Missing); //закрыть с сохранением
            ObjWorkExcel.Quit(); // выйти из экселя
            GC.Collect(); // убрать за собой
            //GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(ObjWorkExcel);
        }

        private double[,] duplicatesInTextBox(double[,] primal)
        {
            //for (int i = 0; i < 3; i++) element[i] = primal[0, i];
            var array = new int[primal.GetLength(0),3];
            for (int j = 0; j < primal.GetLength(0); j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    array[j, i] = Convert.ToInt16(primal[j, i] * 10);
                }
            }

            array = array.ToEnumerableOfEnumerable()
                     .Distinct(new ListEqualityComparer<int>())
                     .ToList()
                     .ToTwoDimensionalArray();

            double[,] final = new double[array.GetLength(0), 3];
            textBox1.Text = null;
            for (int j = 0; j < array.GetLength(0); j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    final[j, i] = Convert.ToDouble(array[j, i]/10.0);
                    if (i>0)
                        textBox1.Text += final[j, i] + "	";
                }
                if (j!= array.GetLength(0)-1)
                    textBox1.Text += Environment.NewLine;
            }

            copies = primal.GetLength(0) - array.GetLength(0);
            return final;
        }

        private void ShowNormativsButton_Click(object sender, EventArgs e)
        {
            if (regionSelect.Items.Count == 0)
                MessageBox.Show("Нет ни одной базы, по которой могли бы быть построены нормативы");

            if (checkBox1.Checked)
            {
                Form normativs = new Form2();
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
            if (checkBox2.Checked)
            {
                Form normativsStoch = new Form3();
                normativsStoch.StartPosition = FormStartPosition.CenterParent;
                normativsStoch.Location = this.Location;
                normativsStoch.Show(this);
                if (normativsStoch.StartPosition == FormStartPosition.CenterParent)
                {
                    var x = Location.X + (Width - normativsStoch.Width) / 2;
                    var y = Location.Y + (Height - normativsStoch.Height) / 2;
                    normativsStoch.Location = new System.Drawing.Point(Math.Max(x, 0), Math.Max(y, 0));
                }
            }

            
        }

        private void CountOfAgesMessage(int[] age)
        {
            string s = "Детей всего "+ age.Length + Environment.NewLine +"дубликатов: " + copies + Environment.NewLine
                + "\r\nпо возрасту:\r\n";
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

        public void RegionSelector(string s)
        {
            regionSelect.Items.Clear();
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = exeDir + s;
            string[] file_list = System.IO.Directory.GetFiles(path, "*.xls");
            string regions = null;
            regionSelect.Items.Add("такого региона нет");
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

        private void regionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (regionSelect.SelectedIndex == 0)
            {
                label6.Visible = true;
                textBox2.Visible = true;
                addRegion.Visible = true;
            }
            else
            {
                label6.Visible = false;
                textBox2.Visible = false;
                addRegion.Visible = false;
            }
        }

        private void addRegion_Click(object sender, EventArgs e)
        {
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\базы\";
            //System.IO.File.CreateText(exeDir + textBox2.Text + ".xls");
            if (textBox2.Text == null)
            {
                MessageBox.Show("Введите название региона");
                return;
            }        
            System.IO.File.Copy(exeDir + "z_base_example.xls", exeDir + textBox2.Text + ".xls");
            MessageBox.Show("Регион добавлен");
            label6.Visible = false;
            textBox2.Visible = false;
            addRegion.Visible = false;
            RegionSelector(@"\базы\");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) checkBox2.Checked = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) checkBox1.Checked = false;
        }        

    }

    public static class MyExtensions
    {
        public static IEnumerable<List<T>> ToEnumerableOfEnumerable<T>(this T[,] array)
        {
            int rowCount = array.GetLength(0);
            int columnCount = array.GetLength(1);

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var row = new List<T>();
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    row.Add(array[rowIndex, columnIndex]);
                }
                yield return row;
            }
        }
        public static T[,] ToTwoDimensionalArray<T>(this List<List<T>> tuples)
        {
            var list = tuples.ToList();
            T[,] array = null;
            for (int rowIndex = 0; rowIndex < list.Count; rowIndex++)
            {
                var row = list[rowIndex];
                if (array == null)
                {
                    array = new T[list.Count, row.Count];
                }
                for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
                {
                    array[rowIndex, columnIndex] = row[columnIndex];
                }
            }
            return array;
        }
    }

    public class ListEqualityComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T> x, List<T> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<T> obj)
        {
            int hash = 19;
            foreach (var o in obj)
            {
                hash = hash * 31 + o.GetHashCode();
            }
            return hash;
        }
    }
}
