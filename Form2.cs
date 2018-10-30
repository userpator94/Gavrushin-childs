using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Norms_physicalDev
{
    public partial class Form2 : Form
    {
        public double stDevH = 0, stDevM = 0;
        public double averageH = 0, averageM =0 , Rxy = 0, sigmaR = 0;
        private int copies = 0;
               
        public Form2()
        {
            InitializeComponent();
            RegionSelector(@"\базы\");              
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //asInvoker
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
            int page = comboBox2.SelectedIndex + 1;
            if (comboBox1.SelectedIndex == 0) page += 20; //мужской пол с 21 страницы
            //if (comboBox2.SelectedIndex < 8) page += comboBox2.SelectedIndex;
            //if (comboBox2.SelectedIndex >= 8) page += comboBox2.SelectedIndex+1;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            Microsoft.Office.Interop.Excel.Application excApp =
                new Microsoft.Office.Interop.Excel.Application();
            excApp.Visible = false;
            Workbook wb;
            Worksheet wsh;
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            exeDir += @"\базы\" + regionSelect.Text;            
            wb = excApp.Workbooks.Open(exeDir, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            wsh = (Worksheet)wb.Sheets[page];
            var lastCell = wsh.Cells.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing);            
            Range tRange = wsh.get_Range("B1", "B65000");
            tRange.SpecialCells(XlCellType.xlCellTypeLastCell, XlSpecialCellsValue.xlTextValues);
            int R = tRange.get_End(XlDirection.xlDown).get_Offset(1, 0).Row - 1;          

            //проверка на дубликаты
            if (R > 100)
            {
                int row = lastCell.Row, column = lastCell.Column;
                row = R;
                string dupsEliminate_message = "В файле \""+ regionSelect.Text + "\" сейчас будет проведена проверка на дубликаты. Дублированные элементы согласно умному алгоритму будут удалены.\n\n" +
                    "\"ДА\" - будут удалены дублирующие значения*\n"+
                    "\"НЕТ\" - этот файл будет скопирован без изменений, в оригинале будут удалены дубликаты*\n"+
                    "\"ОТМЕНА\" - отменить удаление дубликатов и открыть файл\n"+
                    "*Если копий менее 10% от количества значений, они не будут удалены\n" +
                    "Закрытие этого окна прервёт операцию построения нормативов";
                DialogResult dialogResult = MessageBox.Show(dupsEliminate_message, "Внимание", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);                

                if (dialogResult == DialogResult.Yes)
                {                   
                    try {
                        eliminateDuplicates(wsh, row, column, R);
                    }
                    catch(Exception exception) {
                        excApp.Quit();
                        MessageBox.Show("ошибка: " + exception, "Произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (dialogResult == DialogResult.No)
                {                  
                    string finalName = exeDir;
                    finalName = finalName.Substring(1, finalName.Length-5);
                    DateTime date = DateTime.Today;
                    string dateFormat = "dd_MM";
                    finalName = "C"+ finalName+ " дубликаты " + date.ToString(dateFormat) + ".xls";
                    System.IO.File.Copy(exeDir, finalName);
                    try {
                        eliminateDuplicates(wsh, row, column, R);
                    }
                    catch (Exception exception) {
                        excApp.Quit();
                        MessageBox.Show("ошибка: " + exception, "Произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (dialogResult == DialogResult.Cancel)
                {
                    excApp.Visible = true;
                    return;
                }
            }

            //R = R - copies;
            tRange.SpecialCells(XlCellType.xlCellTypeLastCell, XlSpecialCellsValue.xlTextValues);
            int Nx = tRange.get_End(XlDirection.xlDown).get_Offset(1, 0).Row - 1;

            if (Nx < 3){
                MessageBox.Show("Данных в это категории недостаточно, чтобы построить нормативы");
                return;
            }
            string[,] list = new string[Nx, 2]; // массив значений с листа равен по размеру листу
            for (int i = 0; i < Nx; i++) //по всем колонкам
                for (int j = 0; j < 2; j++) // по всем строкам кроме последней
                    list[i, j] = wsh.Cells[i+ 1, j + 1].Text.ToString();//считываем текст в строку

            //int Nx = list.GetLength(0);
            //tRange.SpecialCells(XlCellType.xlCellTypeLastCell, XlSpecialCellsValue.xlTextValues);
            //int Nx = tRange.get_End(XlDirection.xlDown).get_Offset(1, 0).Row - 1; 
            //проверка задваивания
            if (list[list.GetLength(0)-1, 0] == list[list.GetLength(0) - 2, 0] &&
                list[list.GetLength(0)-1, 1] == list[list.GetLength(0) - 2, 1])
                Nx = list.GetLength(0) - 1;

            if (Nx < 100 && Nx > 2) MessageBox.Show(
                "Записей о детях данной группы меньше 100",
                "Обратите внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            if (Nx <= 2)
            {
                MessageBox.Show(
                "Записей о детях данной группы меньше 2",
                "Построение стандартов прервано",
                MessageBoxButtons.OK,
                MessageBoxIcon.Hand,
                MessageBoxDefaultButton.Button1);
                return;
            }
       
            //это всё ради r
            var wshFunc = excApp.WorksheetFunction;
            double[] forCorrelation = new double[Nx];
            double[] forPercent = new double[Nx];
            for (int j = 0; j < Nx; j++)
            {
                forCorrelation[j] = double.Parse(list[j, 1]);
                forPercent[j] = double.Parse(list[j, 0]);
            }
            double r = Math.Round(wshFunc.Correl(forPercent, forCorrelation), 15);           

            //вывод в первую таблицу
            dataGridDisplay1(0, list, Nx, r);            
            dataGridDisplay1(1, list, Nx, r);

            //вывод во вторую таблицу
            dataGridDisplay2(Nx, list, Rxy);

            //DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[-1].Clone();
            //row.Cells[0].Value = "Рост";
            //row.Cells[1].Value = list.GetLength(0);
            //row.Cells[2].Value = sumH/Nx;
            //dataGridView1.Rows.Add(row);
            
            wb.Close(true, Type.Missing, Type.Missing);
            excApp.Quit();
            GC.Collect();

            saveButtom.Enabled = true;
        }
        private void saveButtom_Click(object sender, EventArgs e)
        {
            if (regionSelect.SelectedIndex == -1 || regionSelect.SelectedItem == null)
            {
                MessageBox.Show("Чтобы сохранить значения, их следует сначала построить",
                    "Внимание",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            normsSaver(regionSelect.SelectedItem.ToString());
            saveButtom.Enabled = false;
        }

        public void dataGridDisplay1(int P, string[,] list, int N, double r)
        {
            double sum = 0;            
            double sumOfDifference = 0;
            for (int j = 0; j < N; j++)
                sum += double.Parse(list[j, P]);

            double[] forPercent = new double[N];
            for (int j = 0; j < N; j++)
            {
                //делаем одномерный массив
                forPercent[j] = double.Parse(list[j, P]);
                //сумма квадратов разниц
                sumOfDifference += Math.Pow(((double.Parse(list[j, P])) - sum / N), 2);
            }
            double stDeviation = Math.Sqrt(sumOfDifference / (N-1)); //N-1
            if (P == 0)
                stDevH = stDeviation;
            if (P == 1)
            {
                stDevM = stDeviation;
                Rxy = r * stDevM / stDevH;                
                sigmaR = stDevM * Math.Sqrt(1 - r * r);                
            }

            double average = sum / N;
            if (P == 0) averageH = average;
            if (P == 1) averageM = average;            
            double mediana = stDeviation / Math.Sqrt(N);            
            double percent25 = Math.Round(Percentile(forPercent, 0.25), 1); //25%
            double percent50 = Math.Round(Percentile(forPercent, 0.5), 1); //50%
            double percent75 = Math.Round(Percentile(forPercent, 0.75), 1); //75%
            double V = stDeviation / average * 100;

            stDeviation = Math.Round(stDeviation, 1);
            average = Math.Round((average), 1);
            mediana = Math.Round((mediana), 1);
            V = Math.Round(V, 1);
            if (P == 0)
                this.dataGridView1.Rows.Add
                ("Длина тела(см)", N, average, mediana, stDeviation, percent25, percent50,
                percent75, V, "-", "-", "-");
            if (P == 1)
                this.dataGridView1.Rows.Add
                ("Масса тела(кг)", N, average, mediana, stDeviation, percent25, percent50,
                percent75, V, Math.Round(r, 1), Math.Round(Rxy, 1), Math.Round(sigmaR, 1));
        }

        public void dataGridDisplay2(int N, string[,] list, double Rxy)
        {
            double[] forHeight = new double[N];
            for (int i = 0; i < N; i++)
                forHeight[i] = Math.Round(double.Parse(list[i, 0]));

            double MaxHdouble = forHeight.Max();
            int MaxH = Convert.ToInt32(MaxHdouble);
            double MinHdouble = forHeight.Min();
            int MinH = Convert.ToInt32(MinHdouble);
            double[] forOutput = new double[MaxH - MinH + 1];
            for (int i = 0; i < forOutput.Length; i++)
                forOutput[i] = i + MinH;

            averageH = Math.Round(averageH);
            stDevH = Math.Round(stDevH);

            string s = null;
            double MsigmaR = 0;
            int low = 0, high = 0;
            for (int i = 0; i < forOutput.Length; i++)
            {
                if (s == "высокий") return;

                if (forOutput[i] < Math.Ceiling(averageH - 2 * stDevH))
                {
                    if (low > 0) continue;
                    else
                    {
                        s = "низкий";
                        low++;
                    }
                }
     
                if (forOutput[i] < Math.Ceiling(averageH + stDevH) &&
                    forOutput[i] >= Math.Ceiling(averageH - 2 * stDevH))
                {
                    if (s == null) goto AddLowValue;
                    else s = "ниже среднего"; 
                }
                if (forOutput[i] < Math.Ceiling(averageH + stDevH) &&
                    forOutput[i] >= Math.Ceiling(averageH - stDevH))
                    s = "средний";
                if (forOutput[i] > Math.Ceiling(averageH + stDevH) &&
                    forOutput[i] <= Math.Ceiling(averageH + 2 * stDevH))
                    s = "выше среднего";

                if (forOutput[i] > Math.Ceiling(averageH + 2 * stDevH))
                {
                    if (high > 0) continue;
                    else
                    {
                        s = "высокий";
                        high++;                        
                    }
                }
                //if (i == forOutput.Length && s != "высокий") i--;

                MsigmaR = averageM + Rxy * (forOutput[i] - averageH);

                if (s != "высокий")
                {
                    this.dataGridView2.Rows.Add
                    (s, forOutput[i], Math.Round(MsigmaR - sigmaR, 1), Math.Round(MsigmaR, 1),
                    Math.Round(MsigmaR + sigmaR, 1), Math.Round(MsigmaR + 1.5 * sigmaR, 1)); //2 на 1,5
                }
                else this.dataGridView2.Rows.Add(s, forOutput[i], "", "", "", "");
                continue;

                AddLowValue:
                {
                    s = "низкий";
                    MsigmaR = averageM + Rxy * (Math.Floor(averageH - 2.1 * stDevH) - averageH);
                    this.dataGridView2.Rows.Add(s, forOutput[i] - 1, "", "", "", "");
                    i--;
                }

            }

        }

        public double Percentile(double[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }

        public void RegionSelector(string s)
        {
            regionSelect.Items.Clear();
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = exeDir + s;
            string[] file_list = System.IO.Directory.GetFiles(path, "*.xls");
            string regions = null;           
            for (int i = 0; i < file_list.Length; i++)
            {               
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

        public void normsSaver(string name)
        {
            this.Enabled = false;
            Microsoft.Office.Interop.Excel.Application ExcelApp =
                new Microsoft.Office.Interop.Excel.Application();
            ExcelApp.Visible = true;

            name = name.Remove(name.Length - 4);
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\нормативы";
            name = exeDir + name + "_norm.xls";

            if (System.IO.File.Exists(name) == false)
                System.IO.File.Copy(exeDir + @"\z_norm_example.xls", name);

            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook =
                ExcelApp.Workbooks.Open(name,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing); //открыть файл

            Worksheet ObjWorkSheet;
            int page = comboBox2.SelectedIndex + 1;
            if (comboBox1.SelectedIndex == 0) page += 20;
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[page];
            ObjWorkSheet.Cells.ClearContents();
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

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    ObjWorkSheet.Cells[j + 2, i + 1] = (dataGridView1[i, j].Value);
                }
            }

            ObjWorkSheet.Cells[5, 1] = "Оценка роста";
            ObjWorkSheet.Cells[5, 2] = "Рост, см";
            ObjWorkSheet.Cells[5, 3] = "М-δR";
            ObjWorkSheet.Cells[5, 4] = "Mcp";
            ObjWorkSheet.Cells[5, 5] = "М+δR";
            ObjWorkSheet.Cells[5, 6] = "М+1.5δR";

            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView2.RowCount; j++)
                {
                    ObjWorkSheet.Cells[j + 6, i + 1] = (dataGridView2[i, j].Value);
                }
            }

            ObjWorkBook.Close(true, Type.Missing, Type.Missing);
            Workbook wb =
                ExcelApp.Workbooks.Open(name,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Worksheet ws = (Worksheet)wb.Sheets[page];
            ws.Select(Type.Missing);
            this.Enabled = true;
            //ExcelApp.Quit();            
            //GC.Collect();
        }

        private void baseFolder_Click(object sender, EventArgs e)
        {
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\базы";
            System.Diagnostics.Process.Start("explorer", exeDir);
        }

        private void normFolder_Click(object sender, EventArgs e)
        {
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\нормативы";
            System.Diagnostics.Process.Start("explorer", exeDir);
        }

        public void eliminateDuplicates(Worksheet wsh, int row, int column, int R)
        {
            Range range = wsh.UsedRange;
            double[,] primaArr = new double[row, column];
            for (int i = 0; i < R; i++)
                for (int j = 0; j < column; j++)
                    primaArr[i, j] = double.Parse(wsh.Cells[i + 1, j + 1].Text.ToString());
            var repairedArr = duplicatesInExcelBase(primaArr);
            copies = primaArr.GetLength(0) - repairedArr.GetLength(0);
            if (copies > primaArr.GetLength(0) * 0.1)
            {
                //range.Delete();
                range.Value2 = null;
                Range c1 = (Range)wsh.Cells[1, 1];
                Range c2 = (Range)wsh.Cells[repairedArr.GetLength(0), 2];
                range = wsh.get_Range(c1, c2);
                range.Value2 = repairedArr;
                R = repairedArr.GetLength(0);
                MessageBox.Show("Было удалено " + copies + " дублированных значений");
            }            
        }

        public double[,] duplicatesInExcelBase(double[,] primal)
        {
            var array = new int[primal.GetLength(0), 2];
            for (int j = 0; j < primal.GetLength(0); j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    array[j, i] = Convert.ToInt16(primal[j, i] * 100);
                }
            }

            array = array.ToEnumerableOfEnumerable()
                     .Distinct(new ListEqualityComparer<int>())
                     .ToList()
                     .ToTwoDimensionalArray();

            double[,] final = new double[array.GetLength(0), 2];
            for (int j = 0; j < array.GetLength(0); j++)
                for (int i = 0; i < 2; i++)
                    final[j, i] = Convert.ToDouble(array[j, i] / 100.0);

            return final;
        }
    }
}
