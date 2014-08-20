using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.Controls;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using Ionic.Zip;

namespace Dziennik.View
{
    public sealed class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_saveCommand = new RelayCommand(Save, CanSave);
            m_optionsCommand = new RelayCommand(Options);
            m_infoCommand = new RelayCommand(Info);
            m_archiveDatabaseCommand = new RelayCommand<string>(ArchiveDatabase);
            m_closeArchivePreviewCommand = new RelayCommand(CloseArchivePreview, CanCloseArchivePreview);
            m_showClassesListCommand = new RelayCommand(ShowClassesList);
            m_showMarksCategoriesListCommand = new RelayCommand(ShowMarksCategoriesList);
            m_showCalendarsListCommand = new RelayCommand(ShowCalendarsList);
            m_showNoticesListCommand = new RelayCommand(ShowNoticesList);
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedSchoolClasses = new ObservableCollection<SchoolClassControlViewModel>();
        public ObservableCollection<SchoolClassControlViewModel> OpenedSchoolClasses
        {
            get { return m_openedSchoolClasses; }
            set { m_openedSchoolClasses = value; RaisePropertyChanged("OpenedSchoolClasses"); }
        }
        private SchoolClassControlViewModel m_selectedClass;
        public SchoolClassControlViewModel SelectedClass
        {
            get { return m_selectedClass; }
            set
            {
                m_selectedClass = value;
                RaisePropertyChanged("SelectedClass");
                m_saveCommand.RaiseCanExecuteChanged();
            }
        }

        private int m_windowWidth;
        public int WindowWidth
        {
            get { return m_windowWidth; }
            set { m_windowWidth = value; RaisePropertyChanged("WindowWidth"); RaisePropertyChanged("TabWidth"); }
        }
        public int TabWidth
        {
            get
            {
                int result = ((m_windowWidth - 70 - m_openedSchoolClasses.Count * 26) / m_openedSchoolClasses.Count);
                if (result < 0) return 0;
                return result;
            }
        }

        private RelayCommand m_saveCommand;
        public ICommand SaveCommand
        {
            get { return m_saveCommand; }
        }

        private RelayCommand m_optionsCommand;
        public ICommand OptionsCommand
        {
            get { return m_optionsCommand; }
        }

        private RelayCommand m_infoCommand;
        public ICommand InfoCommand
        {
            get { return m_infoCommand; }
        }

        private RelayCommand<string> m_archiveDatabaseCommand;
        public ICommand ArchiveDatabaseCommand
        {
            get { return m_archiveDatabaseCommand; }
        }

        private RelayCommand m_closeArchivePreviewCommand;
        public ICommand CloseArchivePreviewCommand
        {
            get { return m_closeArchivePreviewCommand; }
        }

        private RelayCommand m_showClassesListCommand;
        public ICommand ShowClassesListCommand
        {
            get { return m_showClassesListCommand; }
        }

        private RelayCommand m_showMarksCategoriesListCommand;
        public ICommand ShowMarksCategoriesListCommand
        {
            get { return m_showMarksCategoriesListCommand; }
        }

        private RelayCommand m_showCalendarsListCommand;
        public ICommand ShowCalendarsListCommand
        {
            get { return m_showCalendarsListCommand; }
        }

        private RelayCommand m_showNoticesListCommand;
        public ICommand ShowNoticesListCommand
        {
            get { return m_showNoticesListCommand; }
        }

        private string m_originalDatabasePath;
        public string OriginalDatabasePath
        {
            get { return m_originalDatabasePath; }
            set
            {
                if (!m_blockSaving) throw new InvalidOperationException();
                m_originalDatabasePath = value; RaisePropertyChanged("OriginalDatabasePath");
            }
        }

        private bool m_blockSaving;
        public bool BlockSaving
        {
            get { return m_blockSaving; }
            set { m_blockSaving = value; RaisePropertyChanged("BlockSaving"); m_saveCommand.RaiseCanExecuteChanged(); m_closeArchivePreviewCommand.RaiseCanExecuteChanged(); }
        }

        private bool m_databasesDirectoryChanged = true;

        public void Init()
        {
            GlobalConfig.InitializeFastJSONCustom();
            GlobalConfig.Notifier.PropertyChanged += Notifier_PropertyChanged;

            //ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            //{
            //    GlobalConfig.Notifier.LoadRegistry();
            //}
            //, null, "Odczytywanie ustawień z rejestru...");
            //GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            Reload(true);
            
            //string[] args = Environment.GetCommandLineArgs();
            //if (args.Length >= 2) m_openFromPathCommand.Execute(args[1]);
        }
        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DatabasesDirectory") m_databasesDirectoryChanged = true;
        }

        private void Save(object param)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                d.ProgressValue = 0;
                d.ProgressStep = 100 / (double)(m_openedSchoolClasses.Count + 2); 

                d.Content = GlobalConfig.GetStringResource("lang_Registry");
                GlobalConfig.Notifier.SaveRegistry();
                d.StepProgress();

                d.Content = GlobalConfig.GetStringResource("lang_GlobalDatabase");
                GlobalConfig.GlobalDatabase.Save();
                d.StepProgress();

                foreach (var schoolClass in m_openedSchoolClasses)
                {
                    d.Content = schoolClass.ViewModel.Name;
                    schoolClass.SaveCommand.Execute(NoActionDialogParameter.Instance);
                    d.StepProgress();
                }

                d.ProgressValue = 100;
                
            }, null, "", GlobalConfig.GetStringResource("lang_Saving"), GlobalConfig.ActionDialogProgressSize, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private bool CanSave(object param)
        {
            return !m_blockSaving;
        }
        private void Options(object param)
        {
            OptionsViewModel dialogViewModel = new OptionsViewModel(m_openedSchoolClasses);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            ActionDialogViewModel saveDialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                if (!m_blockSaving) GlobalConfig.Notifier.SaveRegistry();
            }
            , null, "Zapisywanie ustawień do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, saveDialogViewModel);
            if (GlobalConfig.Notifier.AutoSave) GlobalConfig.GlobalDatabase.Save();
            if (m_databasesDirectoryChanged)
            {
                Reload();
                if (!m_blockSaving) Options(null);
            }
            RaisePropertyChanged("TabWidth");
        }
        private void Info(object e)
        {
            //GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_AuthorInfo"), MessageBoxSuperPredefinedButtons.OK);
            InfoDialogViewModel dialogViewModel = new InfoDialogViewModel();
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void OpenFromPath(string path)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                OpenFromPathRaw(path);
            }
            , null, path, "Otwieranie");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void OpenFromPathRaw(string path)
        {
            if (!File.Exists(path))
            {
                MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wybrany plik nie istnieje", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
            }

            SchoolClassControlViewModel searchTab = m_openedSchoolClasses.FirstOrDefault((x) => { return x.Database.Path == path; });
            if (searchTab != null)
            {
                MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Ta klasa jest już otwarta", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
                SelectedClass = searchTab;
                return;
            }

            DatabaseMain database = null;
            try
            {
                database = DatabaseMain.Load(path);
            }
            catch (Exception exception)
            {
                MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wystąpił błąd podczas odczytywania pliku" + Environment.NewLine + "Treść błędu:" + Environment.NewLine + exception.ToString(), "Dziennik", MessageBoxSuperPredefinedButtons.OK);
            }

            if (database == null) return;
            database.Path = path;
            SchoolClassControlViewModel tab = new SchoolClassControlViewModel(database);
            m_openedSchoolClasses.Add(tab);
            SelectedClass = tab;

            if (SelectedClass.ViewModel.Groups.Count > 0)
            {
                SelectedClass.SelectedGroup = SelectedClass.ViewModel.Groups[0];
            }
        }
        private void ArchiveDatabase(string param)
        {
            m_saveCommand.Execute(null);

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                DateTime now = DateTime.Now;
                IEnumerable<string> files = Directory.EnumerateFiles(GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory);
                var validFiles = from f in files where f.EndsWith(GlobalConfig.SchoolClassDatabaseFileExtension) || f.EndsWith(GlobalConfig.SchoolOptionsDatabaseFileExtension) select f;

                d.ProgressValue = 0;
                d.ProgressStep = 100 / (double)(validFiles.Count() + 1);

                d.Content = GlobalConfig.GetStringResource("lang_Starting");

                string path = GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.ArchiveDatabasesSubdirectory + @"\" + now.ToString(GlobalConfig.FileDateTimeFormat) + GlobalConfig.DatabaseArchiveFileExtension;
                int count = 1;
                while (File.Exists(path))
                {
                    path = GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.ArchiveDatabasesSubdirectory + @"\" + now.ToString(GlobalConfig.FileDateTimeFormat) + "(" + count.ToString() + ")" + GlobalConfig.DatabaseArchiveFileExtension;
                    ++count;
                }

                using (ZipFile zip = new ZipFile(path))
                {
                    MemoryStream metadataStream = new MemoryStream();
                    BinaryWriter metadataWriter = new BinaryWriter(metadataStream);

                    metadataWriter.Write(now.ToBinary());
                    metadataWriter.Write((param == null ? GlobalConfig.GetStringResource("lang_Archive") + (count > 1 ? (count - 1).ToString() : "") : param));
                    metadataStream.Position = 0;
                    zip.AddEntry("metadata", metadataStream);

                    foreach (var file in validFiles)
                    {
                        zip.AddFile(file, string.Empty);
                    }
                    d.StepProgress();

                    ZipEntry entryTemp = null;
                    zip.SaveProgress += (s, e) =>
                    {
                        if (e.CurrentEntry != entryTemp)
                        {
                            if (e.CurrentEntry == null) return;
                            d.Content = e.CurrentEntry.FileName;
                            entryTemp = e.CurrentEntry;
                            d.StepProgress();
                        }
                    };

                    zip.Save();

                    metadataWriter.Dispose();
                }

                d.Content = GlobalConfig.GetStringResource("lang_Ending");

                d.ProgressValue = 100;

            }, null, "", GlobalConfig.GetStringResource("lang_Archiving"), GlobalConfig.ActionDialogProgressSize, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);         
        }
        public static void UnpackArchive(object viewModel, string archivePath, string unpackPath)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                d.Content = GlobalConfig.GetStringResource("lang_Starting");
                //Archiver archive = new Archiver(archivePath, Archiver.ArchiverMode.Read);
                //archive.Start();
                //archive.ReadMetadataString();
                //int filesCount = BitConverter.ToInt32(archive.ReadMetadataArray(), 0);

                using (ZipFile zip = new ZipFile(archivePath))
                {
                    IEnumerable<ZipEntry> validEntries = from e in zip.Entries
                                                         where e.FileName.EndsWith(GlobalConfig.SchoolClassDatabaseFileExtension) ||
                                                               e.FileName == GlobalConfig.SchoolOptionsDatabaseFileName
                                                         select e;

                    d.ProgressValue = 0;
                    d.ProgressStep = 100 / (double)(validEntries.Count());

                    foreach (var entry in validEntries)
                    {
                        entry.Extract(unpackPath, ExtractExistingFileAction.OverwriteSilently);
                        d.StepProgress();
                    }
                }

                

                //for (int i = 0; i < filesCount; i++)
                //{
                //    archive.ReadFile(unpackPath);
                //}

                d.Content = GlobalConfig.GetStringResource("lang_Ending");
                //archive.End();
                //d.StepProgress();

                d.ProgressValue = 100;

            }, null, "", GlobalConfig.GetStringResource("lang_ArchiveUnpacking"), GlobalConfig.ActionDialogProgressSize, true);
            GlobalConfig.Dialogs.ShowDialog(viewModel, dialogViewModel);  
        }
        private void CloseArchivePreview(object e)
        {
            GlobalConfig.Notifier.DatabasesDirectory = m_originalDatabasePath;
            Reload();
            this.BlockSaving = false;
        }
        private bool CanCloseArchivePreview(object e)
        {
            return m_blockSaving;
        }
        private void ShowClassesList(object e)
        {
            ClassesListViewModel dialogViewModel = new ClassesListViewModel(m_openedSchoolClasses);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void ShowMarksCategoriesList(object e)
        {
            MarksCategoriesListViewModel dialogViewModel = new MarksCategoriesListViewModel(GlobalConfig.GlobalDatabase.ViewModel.MarksCategories, m_openedSchoolClasses);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void ShowCalendarsList(object e)
        {
            GlobalCalendarListViewModel dialogViewModel = new GlobalCalendarListViewModel(GlobalConfig.GlobalDatabase.ViewModel.Calendars, m_openedSchoolClasses, GlobalConfig.GlobalDatabaseAutoSaveCommand);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void ShowNoticesList(object e)
        {
            NoticesListViewModel dialogViewModel = new NoticesListViewModel();
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void CloseAllTabs()
        {
            if (!m_blockSaving)
            {
                if (GlobalConfig.GlobalDatabase != null) GlobalConfig.GlobalDatabase.Save();
                foreach (SchoolClassControlViewModel tab in m_openedSchoolClasses)
                {
                    tab.SaveCommand.Execute(null);
                }
            }
            m_openedSchoolClasses.Clear();
        }
        public void Reload(bool loadRegistry = false)
        {
            ActionDialogViewModel saveDialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                CloseAllTabs();

                d.ProgressValue = 0;

                if (loadRegistry)
                {
                    d.Content = GlobalConfig.GetStringResource("lang_Registry");
                    GlobalConfig.Notifier.LoadRegistry();
                }

                GlobalConfig.CreateDirectoriesIfNotExists();

                IEnumerable<string> files = Directory.EnumerateFiles(GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory);
                var validFiles = from f in files where f.EndsWith(GlobalConfig.SchoolClassDatabaseFileExtension) select f;

                d.ProgressStep = 100.0 / (double)(validFiles.Count() + 1 + (loadRegistry ? 1 : 0));

                d.Content = GlobalConfig.GetStringResource("lang_GlobalDatabase");

                string optionsPath = GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory + @"\" + GlobalConfig.SchoolOptionsDatabaseFileName;
                if (File.Exists(optionsPath))
                {
                    GlobalConfig.GlobalDatabase = DatabaseGlobal.Load(optionsPath);
                }
                else
                {
                    GlobalConfig.GlobalDatabase = new DatabaseGlobal();
                    GlobalConfig.GlobalDatabase.Path = optionsPath;
                    GlobalConfig.GlobalDatabase.Save();
                }

                d.StepProgress();

                foreach (string file in validFiles)
                {
                    d.Content = file;
                    OpenFromPathRaw(file);
                    d.StepProgress();
                }

                d.ProgressValue = 100;
            }
           , null, "", GlobalConfig.GetStringResource("lang_Opening"), GlobalConfig.ActionDialogProgressSize,  true);
            GlobalConfig.Dialogs.ShowDialog(this, saveDialogViewModel);

            m_databasesDirectoryChanged = false;

            RaisePropertyChanged("TabWidth");
            CheckNotices();
        }
        private void CheckNotices()
        {
            List<NoticeViewModel> toRemove = new List<NoticeViewModel>();
            StringBuilder sb = new StringBuilder();

            DateTime today = DateTime.Now.Date;
            foreach (NoticeViewModel notice in GlobalConfig.GlobalDatabase.ViewModel.Notices)
            {
                DateTime noticeDate = notice.Date.Date;
                if (noticeDate >= today)
                {
                    if (noticeDate - notice.NotifyIn <= today)
                    {
                        sb.Clear();
                        if (noticeDate == today)
                        {
                            sb.AppendLine(GlobalConfig.GetStringResource("lang_Today"));
                        }
                        else
                        {
                            sb.AppendLine(string.Format(GlobalConfig.GetStringResource("lang_InDaysFormat"), (noticeDate - today).Days));
                        }
                        sb.Append(notice.Name);

                        GlobalConfig.MessageBox(this, sb.ToString(), MessageBoxSuperPredefinedButtons.OK);
                    }
                }
                else
                {
                    toRemove.Add(notice);
                }
            }

            foreach (var rem in toRemove) GlobalConfig.GlobalDatabase.ViewModel.Notices.Remove(rem);
        }

        public void SearchAndSelectClass(object comboBoxCollection)
        {
            foreach (var openedClass in m_openedSchoolClasses)
            {
                if (openedClass.ViewModel.Groups == comboBoxCollection)
                {
                    if (m_selectedClass == openedClass) return;
                    SelectedClass = openedClass;
                }
            }
        }
    }
}
