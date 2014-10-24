using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Dziennik.CommandUtils;
using System.Windows.Input;
using System.IO;
using Ionic.Zip;

namespace Dziennik.View
{
    public sealed class ArchivesListViewModel : ObservableObject
    {
        public enum ArchivesListResult
        {
            Close,
            Preview,
            Restore,
        }

        public class ArchiveInfo : ObservableObject
        {
            private string m_description;
            public string Description
            {
                get { return m_description; }
                set { m_description = value; RaisePropertyChanged("Description"); }
            }

            private DateTime m_date;
            public DateTime Date
            {
                get { return m_date; }
                set { m_date = value; RaisePropertyChanged("Date"); }
            }

            private string m_path;
            public string Path
            {
                get { return m_path; }
                set { m_path = value; RaisePropertyChanged("Path"); }
            }
        }

        public ArchivesListViewModel(ObservableCollection<ArchiveInfo> archives)
        {
            m_previewArchiveCommand = new RelayCommand(PreviewArchive, CanPreviewArchive);
            m_restoreArchiveCommand = new RelayCommand(RestoreArchive, CanRestoreArchive);
            m_closeCommand = new RelayCommand(Close);

            m_archives = archives;
        }

        private ArchivesListResult m_result = ArchivesListResult.Close;
        public ArchivesListResult Result
        {
            get { return m_result; }
        }

        private RelayCommand m_previewArchiveCommand;
        public ICommand PreviewArchiveCommand
        {
            get { return m_previewArchiveCommand; }
        }

        private RelayCommand m_restoreArchiveCommand;
        public ICommand RestoreArchiveCommand
        {
            get { return m_restoreArchiveCommand; }
        }

        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        private ObservableCollection<ArchiveInfo> m_archives;
        public ObservableCollection<ArchiveInfo> Archives
        {
            get { return m_archives; }
            set { m_archives = value; RaisePropertyChanged("Archives"); }
        }

        private ArchiveInfo m_selectedArchive;
        public ArchiveInfo SelectedArchive
        {
            get { return m_selectedArchive; }
            set
            {
                m_selectedArchive = value;
                RaisePropertyChanged("SelectedArchive");
                m_previewArchiveCommand.RaiseCanExecuteChanged();
                m_restoreArchiveCommand.RaiseCanExecuteChanged();
            }
        }

        private void PreviewArchive(object e)
        {
            m_result = ArchivesListResult.Preview;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanPreviewArchive(object e)
        {
            return m_selectedArchive != null;
        }
        private void RestoreArchive(object e)
        {
            if (GlobalConfig.MessageBox(this, string.Format("{1}{0}{2}{0}{3}",
                                             Environment.NewLine,
                                             GlobalConfig.GetStringResource("lang_RestoreArchiveAsk"),
                                             GlobalConfig.GetStringResource("lang_CurrentDatabaseWillBeReplaced"),
                                             GlobalConfig.GetStringResource("lang_DoYouWantToContinue")), Controls.MessageBoxSuperPredefinedButtons.YesNo) != Controls.MessageBoxSuperButton.Yes) return;

            m_result = ArchivesListResult.Restore;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRestoreArchive(object e)
        {
            return m_selectedArchive != null;
        }
        private void Close(object e)
        {
            m_result = ArchivesListResult.Close;
            GlobalConfig.Dialogs.Close(this);
        }

        public static ObservableCollection<ArchiveInfo> LoadArchives(object progressDialogOwner)
        {
            List<string> errors = new List<string>();
            ObservableCollection<ArchiveInfo> result = new ObservableCollection<ArchiveInfo>();

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                IEnumerable<string> files = Directory.EnumerateFiles(GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.ArchiveDatabasesSubdirectory);
                IEnumerable<string> archives = from f in files where f.EndsWith(GlobalConfig.DatabaseArchiveFileExtension) select f;

                d.ProgressValue = 0;
                d.ProgressStep = 100 / (double)(archives.Count());

                foreach (var item in archives)
                {
                    d.Content = item;
                    ArchiveInfo archiveInfo = new ArchiveInfo();
                    try
                    {
                        using (ZipFile zip = new ZipFile(item))
                        {
                            ZipEntry entry = zip.Entries.First(x => x.FileName == "metadata");
                            MemoryStream metadataStream = new MemoryStream();
                            entry.Extract(metadataStream);
                            metadataStream.Position = 0;
                            BinaryReader metadataReader = new BinaryReader(metadataStream);

                            archiveInfo.Date = DateTime.FromBinary(metadataReader.ReadInt64());
                            archiveInfo.Description = metadataReader.ReadString();
                            archiveInfo.Path = item;

                            metadataReader.Dispose();
                        }   

                        result.Add(archiveInfo);
                    }
                    catch
                    {
                        errors.Add(item);

                        if (result.Contains(archiveInfo)) result.Remove(archiveInfo);
                    }
                    d.StepProgress();
                }

                d.ProgressValue = 100;

            }, null, "", GlobalConfig.GetStringResource("lang_LoadingArchives"), GlobalConfig.ActionDialogProgressSize, true);
            GlobalConfig.Dialogs.ShowDialog(progressDialogOwner, dialogViewModel);

            result.Sort((x, y) => x.Date.CompareTo(y.Date));

            if (errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(GlobalConfig.GetStringResource("lang_AnErrorsOccurredWhileReadingSpecifiedArchives"));
                foreach (var error in errors)
                {
                    sb.AppendLine(error);
                }

                GlobalConfig.MessageBox(progressDialogOwner, sb.ToString(), Controls.MessageBoxSuperPredefinedButtons.OK);
            }

            return result;
        }
    }
}
