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
using System.Security;
using System.Diagnostics;

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
                m_oneInstanceMutex = new Mutex(false, "Dziennik_Katra_e5d3b59b-f5d4-4cfd-b714-ecea7bd05f96", out isCreated);
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

            if (GlobalConfig.Notifier.GetWasCrashed())
            {
                GlobalConfig.Notifier.ResetWasCrashed();

                List<MessageBoxSuperButton> mboxButtons = new List<MessageBoxSuperButton>();
                mboxButtons.Add(MessageBoxSuperButton.Ignore);
                mboxButtons.Add(MessageBoxSuperButton.Retry);
                Dictionary<MessageBoxSuperButton, string> mboxStrings = new Dictionary<MessageBoxSuperButton,string>();
                mboxStrings.Add(MessageBoxSuperButton.Ignore, GlobalConfig.GetStringResource("lang_IgnoreError"));
                mboxStrings.Add(MessageBoxSuperButton.Retry, GlobalConfig.GetStringResource("lang_ForceUpdate"));

                MessageBoxSuper mboxDialog = new MessageBoxSuper(GlobalConfig.GetStringResource("lang_WasCrashedMessage"), GlobalConfig.GetStringResource("lang_AppName"), mboxButtons, mboxStrings);
                mboxDialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                mboxDialog.ShowDialog();

                if (mboxDialog.Result == MessageBoxSuperButton.Retry)
                {
                    if (!File.Exists(GlobalConfig.AutoUpdaterPath))
                    {
                        MessageBox.Show(GlobalConfig.GetStringResource("lang_AutoupdaterNotFound"));
                    }
                    else
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = GlobalConfig.AutoUpdaterPath;
                        p.StartInfo.Arguments = GlobalConfig.CurrentVersion.ToString() + " " + GlobalConfig.UpdateInfoLink;
                        p.Start();
                    }

                    Application.Current.Shutdown();
                    return;
                }
            }

            InitializeComponent();

            MainViewModel viewModel = new MainViewModel();

            viewModel.InvokeWindow = (param) =>
            {
                this.Dispatcher.BeginInvoke(param, null);
            };

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
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("====================");
                sb.AppendLine(DateTime.Now.ToString(GlobalConfig.DateTimeFormat));
                sb.AppendLine("====================");
                sb.AppendLine();
                sb.AppendLine("Is terminating: " + e.IsTerminating);
                sb.AppendLine();
                sb.AppendLine(e.ExceptionObject.ToString());
                string error = sb.ToString();

                GlobalConfig.Notifier.SetWasCrashed();
                using (StreamWriter writer = new StreamWriter(GlobalConfig.ErrorLogFileName, true))
                {
                    writer.Write(error);
                }

                sb.Clear();
                sb.AppendLine("Wystąpił nieznany błąd");
                if (e.IsTerminating) sb.AppendLine("Dziennik zostanie zamknięty");
                sb.AppendLine();
                sb.AppendLine("Czy chcesz rozwinąć szczegóły techniczne? (zostały zapisane w " + GlobalConfig.ErrorLogFileName + ")");
                if (MessageBox.Show(sb.ToString(), "Dziennik ZSE", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Szczegóły techniczne: " + Environment.NewLine + error, "Dziennik ZSE", MessageBoxButton.OK, MessageBoxImage.Error);
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
