using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Net;
using Ionic.Zip;
using System.Diagnostics;

namespace DziennikAktualizacja
{
    public sealed class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_runAppCommand = new RelayCommand(RunApp, CanRunApp);
            m_closeCommand = new RelayCommand(Close, CanClose);
        }

        private string m_updateInfoLink;
        private Version m_currentVersion;
        private WebClient m_client;
        private string m_updateFilePath;
        private long m_totalExtractedBytes = 0;
        private long m_totalToExtract = 0;
        private long m_oldBytesWritten = 0;
        private ZipEntry m_oldZipEntry = null;

        public Action<Action> InvokeWindow { get; set; }

        private RelayCommand m_runAppCommand;
        public ICommand RunAppCommand
        {
            get { return m_runAppCommand; }
        }

        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        private string m_currentText = GetStringResource("lang_Init");
        public string CurrentText
        {
            get { return m_currentText; }
            set { m_currentText = value; RaisePropertyChanged("CurrentText"); }
        }

        private int m_currentProgress = 0;
        public int CurrentProgress
        {
            get { return m_currentProgress; }
            set { m_currentProgress = value; RaisePropertyChanged("CurrentProgress"); }
        }

        private bool m_completed = false;
        public bool Completed
        {
            get { return m_completed; }
            private set { m_completed = value; RaisePropertyChanged("Completed"); m_runAppCommand.RaiseCanExecuteChanged(); m_closeCommand.RaiseCanExecuteChanged(); }
        }

        public void Init()
        {
            string[] args = Environment.GetCommandLineArgs();
            try
            {
                m_currentVersion = Version.Parse(args[1]);
                m_updateInfoLink = args[2];
            }
            catch
            {
                CurrentProgress = 100;
                CurrentText = GetStringResource("lang_InitError");
                Completed = true;
                Application.Current.MainWindow.Close();
                return;
            }

            DownloadUpdateInfo();
        }

        private void DownloadUpdateInfo()
        {
            CurrentText = GetStringResource("lang_DownloadingUpdateInfo");

            VersionChecker.CheckVersionAsync(m_updateInfoLink, (x) =>
            {
                bool error = false;
                error = x == null;
                InvokeWindow.Invoke(() =>
                {
                    if(error)
                    {
                        CurrentProgress = 100;
                        CurrentText = GetStringResource("lang_DownloadingUpdateInfoError");
                        Completed = true;
                    }
                    else if (x.NewestVersion > m_currentVersion)
                    {
                        CurrentText = GetStringResource("lang_UpdateDetected");

                        m_updateFilePath = System.IO.Path.GetTempPath() + @"\DziennikAktualizacja" + Guid.NewGuid().ToString().Replace('-', '_') + ".zip";

                        m_client = new WebClient();
                        m_client.DownloadProgressChanged += m_client_DownloadProgressChanged;
                        m_client.DownloadFileCompleted += m_client_DownloadFileCompleted;
                        m_client.DownloadFileAsync(new Uri(x.DownloadLink), m_updateFilePath);
                    }
                    else
                    {
                        CurrentProgress = 100;
                        CurrentText = GetStringResource("lang_YouHaveNewestVersion");
                        Completed = true;
                    }
                });
            });
        }

        private void m_client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            InvokeWindow.Invoke(() =>
            {
                CurrentProgress = e.ProgressPercentage / 2;
                CurrentText = string.Format(GetStringResource("lang_DownloadingFormat"), e.BytesReceived / 1024, e.TotalBytesToReceive / 1024);
            });
        }
        private void m_client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            InvokeWindow.Invoke(() =>
            {
                if (e.Cancelled || e.Error != null)
                {
                    CurrentText = GetStringResource("lang_DownloadingError");
                    CurrentProgress = 100;
                    Completed = true;
                }
                else
                {
                    Unpack();
                }
            });
        }
        private void Unpack()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (ZipFile zip = new ZipFile(m_updateFilePath))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            m_totalToExtract += entry.UncompressedSize;
                        }

                        zip.ExtractProgress += zip_ExtractProgress;
                        zip.ExtractAll(Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }

                    InvokeWindow.Invoke(() =>
                    {
                        CurrentProgress = 100;
                        CurrentText = GetStringResource("lang_UpdateSuccessful");
                        Completed = true;
                    });
                }
                catch
                {
                    InvokeWindow.Invoke(() =>
                    {
                        CurrentProgress = 100;
                        CurrentText = GetStringResource("lang_UnpackingError");
                        Completed = true;
                    });
                }
            });
        }

        private void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            InvokeWindow.Invoke(() =>
            {
                if (m_oldZipEntry != e.CurrentEntry)
                {
                    m_oldZipEntry = e.CurrentEntry;
                    m_oldBytesWritten = 0;
                }

                m_totalExtractedBytes += e.BytesTransferred - m_oldBytesWritten;

                CurrentText = string.Format(GetStringResource("lang_UnpackingFormat"), m_totalExtractedBytes / 1024, m_totalToExtract / 1024);
                CurrentProgress = 50 + (int)(((double)m_totalExtractedBytes / (double)m_totalToExtract) * 50);
            });
        }

        private void RunApp(object param)
        {
            Process.Start("Dziennik.exe");
            Application.Current.MainWindow.Close();
        }
        private bool CanRunApp(object param)
        {
            return m_completed;
        }
        private void Close(object param)
        {
            Application.Current.MainWindow.Close();
        }
        private bool CanClose(object param)
        {
            return m_completed;
        }

        public static string GetStringResource(object key)
        {
            object result = Application.Current.TryFindResource(key);
            return (result == null ? key + " NOT FOUND - RESOURCE ERROR" : (string)result);
        }
    }
}
