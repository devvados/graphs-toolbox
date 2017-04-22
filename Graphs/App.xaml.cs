using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Graphs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex InstanceCheckMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!InstanceCheck())
            {
                MessageBox.Show("Программа уже работает!", "Ошибка!");
                this.Shutdown();
            }
        }

        private bool InstanceCheck()
        {
            bool isNew;
            InstanceCheckMutex = new Mutex(true, "MainWindow", out isNew);
            return isNew;
        }
    }
}
