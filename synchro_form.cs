using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace Norms_physicalDev
{
    public partial class synchro_form : Form
    {
        public synchro_form()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.ru/");
            //this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo prc_info =
                new System.Diagnostics.ProcessStartInfo("childs_stats_sync.exe");
            //prc_info.CreateNoWindow = true;
            prc_info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process.Start(prc_info);

            this.Close();
        }

        private void zip_button_Click(object sender, EventArgs e)
        {
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dir = exeDir + @"\to send";
            string tmp = exeDir + @"\tmp";
            System.IO.Directory.CreateDirectory(dir);
            System.IO.Directory.CreateDirectory(tmp);
            string[] base_list = System.IO.Directory.GetFiles(exeDir+ @"\базы\", "*.xls");
            string[] norm_list = System.IO.Directory.GetFiles(exeDir + @"\нормативы\", "*.xls");
            DateTime date = DateTime.Now;
           
            string zipPath = exeDir + @"\result "+date.ToString().Replace('.','_').Replace(":","")+".zip";
            foreach (var xls in base_list)
            {
                if (System.IO.Path.GetFileName(xls).Equals("z_base_example.xls") ||
                    System.IO.Path.GetFileName(xls).Equals("z_norm_example.xls")) continue;
                string fileName = System.IO.Path.GetFileName(xls);
                string destFile = System.IO.Path.Combine(tmp, fileName);
                System.IO.File.Copy(xls, destFile, true);
            }
            foreach (var xls in norm_list)
            {
                if (System.IO.Path.GetFileName(xls).Equals("z_base_example.xls") ||
                    System.IO.Path.GetFileName(xls).Equals("z_norm_example.xls")) continue;
                string fileName = System.IO.Path.GetFileName(xls);
                string destFile = System.IO.Path.Combine(tmp, fileName);
                System.IO.File.Copy(xls, destFile, true);
            }

            string[] total = System.IO.Directory.GetFiles(tmp, "*.xls");
            if (total.Length == 0)
            {
                MessageBox.Show("Внимание", "Нет файлов для отправки");
                return;
            }
            ZipFile.CreateFromDirectory(tmp, zipPath );

            System.IO.Directory.Delete(tmp,true);
                      
            //foreach (var xls in total)
            //{
            //    System.IO.File.Delete(xls);
            //}
            System.IO.File.Move(zipPath, dir+@"\"+System.IO.Path.GetFileName(zipPath));

            System.Diagnostics.Process.Start("explorer", dir);           
        }
    }
}
