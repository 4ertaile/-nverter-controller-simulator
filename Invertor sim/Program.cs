using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invertor_sim
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            MainForm main = new MainForm();
            Simulator sim = new Simulator(main);

            main.AddSimulatorForm(sim);
            Application.Run(main);
        }
    }
}
