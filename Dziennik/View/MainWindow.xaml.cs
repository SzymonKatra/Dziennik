using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dziennik.Controls;
using Dziennik.ViewModel;
using System.Threading;

namespace Dziennik.View
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
                m_oneInstanceMutex = new Mutex(false, "Dziennik_Katra", out isCreated);
                if (!isCreated)
                {
                    MessageBox.Show("Nie można uruchomić dwóch instancji Dziennika na raz", "Dziennik", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    this.Close();
                }
            }

            InitializeComponent();

            MainViewModel viewModel = new MainViewModel();

            GlobalConfig.Main = viewModel;
            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);

            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((MainViewModel)this.DataContext).WindowWidth = (int)e.NewSize.Width;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)this.DataContext).Init();
            ((MainViewModel)this.DataContext).WindowWidth = (int)this.ActualWidth;
        }
    }
}
