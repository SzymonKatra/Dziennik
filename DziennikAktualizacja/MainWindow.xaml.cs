using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DziennikAktualizacja
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Mutex m_oneInstanceMutex = null;

        public MainWindow()
        {
            if (m_oneInstanceMutex == null)
            {
                bool isCreated;
                m_oneInstanceMutex = new Mutex(false, "DziennikAktualizacja_Katra_cd1af19d-9cab-4990-8091-cba29e44c5d9", out isCreated);
                if (!isCreated)
                {
                    MessageBox.Show("Nie można uruchomić dwóch instancji Aktualizacji Dziennika na raz", "Dziennik", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    this.Close();
                }
            }
            InitializeComponent();

            MainViewModel viewModel = new MainViewModel();
            viewModel.InvokeWindow = (param) =>
            {
                this.Dispatcher.Invoke(param, null);
            };
            this.DataContext = viewModel;
            viewModel.Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !((MainViewModel)this.DataContext).Completed;
        }
    }
}
