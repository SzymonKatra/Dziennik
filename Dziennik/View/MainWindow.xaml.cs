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
using System.Globalization;
using System.IO;

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

                CultureInfo culture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                culture.DateTimeFormat.ShortDatePattern = GlobalConfig.DateFormat;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            InitializeComponent();

            MainViewModel viewModel = new MainViewModel();

            GlobalConfig.Main = viewModel;
            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);

            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalConfig.ErrorLogFileName, true))
                {
                    writer.WriteLine("====================");
                    writer.WriteLine(DateTime.Now.ToString(GlobalConfig.DateTimeFormat));
                    writer.WriteLine("====================");
                    writer.WriteLine();
                    writer.WriteLine("Is terminating: " + e.IsTerminating);
                    writer.WriteLine();
                    writer.WriteLine(e.ExceptionObject.ToString());
                }
            }
            catch { }
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

        private void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((MainViewModel)this.DataContext).SearchAndSelectClass(((ComboBox)sender).ItemsSource);
        }
    }
}
