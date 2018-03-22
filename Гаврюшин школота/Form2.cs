﻿using System;
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
        public double stDevH = 0, stDevM = 0;
        public double averageH = 0, averageM =0 , Rxy = 0, sigmaR = 0;        

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
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            int page = 0;
            if (comboBox1.SelectedIndex == 0) page += 11; //мужской пол с 12 страницы
            page += Convert.ToInt32(comboBox2.SelectedItem.ToString()) - 6;

            Microsoft.Office.Interop.Excel.Application excApp =
                new Microsoft.Office.Interop.Excel.Application();
            excApp.Visible = false;
            Workbook wb;
            Worksheet wsh;
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            exeDir = System.IO.Path.Combine(exeDir, "base.xls");
            wb = excApp.Workbooks.Open(exeDir, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            wsh = (Worksheet)wb.Sheets[page];
            var lastCell = wsh.Cells.SpecialCells(XlCellType.xlCellTypeLastCell);
            if (lastCell.Row < 3)
            {
                MessageBox.Show("Данных в это категории недостаточно, чтобы построить нормативы");
                return;
            }
            string[,] list = new string[lastCell.Row, 3]; // массив значений с листа равен по размеру листу
            for (int i = 0; i < lastCell.Row; i++) //по всем колонкам
                for (int j = 0; j < 3; j++) // по всем строкам кроме последней
                    list[i, j] = wsh.Cells[i+ 1, j + 1].Text.ToString();//считываем текст в строку

            int Nx = list.GetLength(0);
            if (list[list.GetLength(0)-1, 1] == list[list.GetLength(0) - 2, 1] &&
                list[list.GetLength(0)-1, 2] == list[list.GetLength(0) - 2, 2])
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
                forCorrelation[j] = double.Parse(list[j, 2]);
                forPercent[j] = double.Parse(list[j, 1]);
            }                
            double r = wshFunc.Correl(forPercent, forCorrelation);
            
            //вывод в первую таблицу
            dataGridDisplay1(1, list, Nx, r);            
            dataGridDisplay1(2, list, Nx, r);

            //вывод во вторую таблицу
            dataGridDisplay2(Nx, list, Rxy);

            //this.dataGridView1.Rows.Insert(0, "one", "two", "three", "four");
            //DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[-1].Clone();
            //row.Cells[0].Value = "Рост";
            //row.Cells[1].Value = list.GetLength(0);
            //row.Cells[2].Value = sumH/Nx;
            //dataGridView1.Rows.Add(row);

            wb.Close(false, Type.Missing, Type.Missing);
            excApp.Quit();
            GC.Collect();

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
            double stDeviation = Math.Sqrt(sumOfDifference / N); //N-1
            if (P == 1)
                stDevH = stDeviation;
            if (P == 2)
            {
                stDevM = stDeviation;
                Rxy = r * stDevM / stDevH;                
                sigmaR = stDevM * Math.Sqrt(1 - r * r);                
            }

            double average = sum / N;
            if (P == 1) averageH = average;
            if (P == 2) averageM = average;            
            double mediana = stDeviation / Math.Sqrt(N);            
            double percent25 = Math.Round(Percentile(forPercent, 0.25), 1); //25%
            double percent50 = Math.Round(Percentile(forPercent, 0.5), 1); //50%
            double percent75 = Math.Round(Percentile(forPercent, 0.75), 1); //75%
            double V = stDeviation / average * 100;

            stDeviation = Math.Round(stDeviation, 1);
            average = Math.Round((average), 1);
            mediana = Math.Round((mediana), 1);
            V = Math.Round(V, 1);
            if (P == 1)
                this.dataGridView1.Rows.Add
                ("Рост", list.GetLength(0), average, mediana, stDeviation, percent25, percent50,
                percent75, V, "-", "-", "-");
            if (P == 2)
                this.dataGridView1.Rows.Add
                ("Масса", list.GetLength(0), average, mediana, stDeviation, percent25, percent50,
                percent75, V, Math.Round(r, 1), Math.Round(Rxy, 1), Math.Round(sigmaR, 1));
        }

        public void dataGridDisplay2(int N, string[,] list, double Rxy)
        {
            //int Max = list
            double[] forHeight = new double[N];
            for (int i = 0; i < N; i++)
                forHeight[i] = Math.Round(double.Parse(list[i, 1]));

            double MaxHdouble = forHeight.Max();
            int MaxH = Convert.ToInt32(MaxHdouble);
            double MinHdouble = forHeight.Min();
            int MinH = Convert.ToInt32(MinHdouble);
            double[] forOutput = new double[MaxH - MinH+1];
            for (int i = 0; i <= MaxH - MinH; i++)
                forOutput[i] = i + MinH;

            averageH = Math.Round(averageH);
            averageM = Math.Round(averageM);
            stDevH = Math.Round(stDevH);

            string s = null;
            double MsigmaR = 0;
            int low = 0, high = 0;   
            for (int i = 0; i < forOutput.Length; i++)
            {                
                if (forOutput[i] < Math.Ceiling(averageH - 2 * stDevH))
                {
                    if (low > 0) continue;
                    else
                    {
                        s = "низкий  М-2,1δ и меньше";
                        low++;
                    }                                        
                }                    
                if (forOutput[i] < Math.Ceiling(averageH + stDevH) && 
                    forOutput[i] >= Math.Ceiling(averageH - 2 * stDevH))
                {
                    if (s == null) goto AddLowValue;
                    else s = "ниже среднего от М-1,1δ до М-2δ";
                }                    
                if (forOutput[i] < Math.Ceiling(averageH + stDevH) && 
                    forOutput[i] >= Math.Ceiling(averageH - stDevH))
                    s = "средний М±1δ";
                if (forOutput[i] > Math.Ceiling(averageH + stDevH) && 
                    forOutput[i] <= Math.Ceiling(averageH + 2 * stDevH))
                    s = "выше среднего от М+1,1δ до М+2δ";
                if (forOutput[i] > Math.Ceiling(averageH + 2 * stDevH))
                {
                    if (high > 0) continue;
                    else
                    {
                        s = "высокий от М+2,1δ и больше";
                        high++;
                    }
                }                    

                MsigmaR = averageM + Rxy * (forOutput[i] - averageH);
                
                this.dataGridView2.Rows.Add
                    (s, forOutput[i], Math.Round(MsigmaR - sigmaR, 1), Math.Round(MsigmaR, 1),
                    Math.Round(MsigmaR + sigmaR, 1), Math.Round(MsigmaR + 2 * sigmaR,1));
                continue;

                AddLowValue:
                {
                    s = "низкий  М-2,1δ и меньше";
                    MsigmaR = averageM + Rxy * (Math.Floor(averageH - 2.1 * stDevH) - averageH);
                    this.dataGridView2.Rows.Add
                    (s, forOutput[i]-1, Math.Round(MsigmaR - sigmaR, 1), Math.Round(MsigmaR, 1),
                    Math.Round(MsigmaR + sigmaR, 1), Math.Round(MsigmaR + 2 * sigmaR, 1));
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
    }
}
