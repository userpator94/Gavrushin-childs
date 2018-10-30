using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Norms_physicalDev
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            //bool AdminRight = principal.IsInRole(WindowsBuiltInRole.Administrator);

            //if (AdminRight == false)
            //{
            //    ProcessStartInfo processInfo = new ProcessStartInfo(); //создаем новый процесс
            //    processInfo.Verb = "runas"; //в данном случае указываем, что процесс должен быть запущен с правами администратора
            //    processInfo.FileName = Application.ExecutablePath; //указываем исполняемый файл (программу) для запуска
            //    try
            //    {
            //        Process.Start(processInfo); //пытаемся запустить процесс
            //    }
            //    catch (Win32Exception)
            //    {
            //        //Ничего не делаем, потому что пользователь, возможно, нажал кнопку "Нет" в ответ на вопрос о запуске программы в окне предупреждения UAC (для Windows 7)
            //    }
            //    Application.Exit(); //закрываем текущую копию программы (в любом случае, даже если пользователь отменил запуск с правами администратора в окне UAC)
            //}
            //else
            //{
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            //}

        }
    }
}
