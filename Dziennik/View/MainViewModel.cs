﻿using System;
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
using System.Windows;
using System.IO;
using Microsoft.Win32;
using Ionic.Zip;
using System.Timers;
using DziennikAktualizacja;
using System.Diagnostics;

namespace Dziennik.View
{
    public sealed class MainViewModel : ObservableObject, IDisposable
    {
        private class SortClassPriority
        {
            public SchoolClassControlViewModel SchoolClass { get; set; }
            public SchoolGroupViewModel Group { get; set; }
            public int Priority { get; set; }
        }
        public enum CurrentRemainingType
        {
            Lesson,
            Break,
            NoLessons,
        }

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
            m_closeCommand = new RelayCommand<CancelEventArgs>(Close);
            m_checkUpdatesCommand = new RelayCommand<string>(CheckUpdates, CanCheckUpdate);
            m_showSchedulesListCommand = new RelayCommand(ShowSchedulesList);

            m_openedSchoolClasses.Added += m_openedSchoolClasses_Added;
            m_openedSchoolClasses.Removed += m_openedSchoolClasses_Removed;
            m_openedSchoolClasses.RaiseAddedForAll();
        }

        private bool m_disposed = false;

        private ObservableCollectionNotifySimple<SchoolClassControlViewModel> m_openedSchoolClasses = new ObservableCollectionNotifySimple<SchoolClassControlViewModel>();
        public ObservableCollectionNotifySimple<SchoolClassControlViewModel> OpenedSchoolClasses
        {
            get { return m_openedSchoolClasses; }
        }
        private ObservableCollection<SchoolClassControlViewModel> m_sortedOpenedSchoolClasses = new ObservableCollection<SchoolClassControlViewModel>();
        public ObservableCollection<SchoolClassControlViewModel> SortedOpenedSchoolClasses
        {
            get { return m_sortedOpenedSchoolClasses; }
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

        private Timer m_activityTimer;
        private object m_activityTimerLock = new object();
        private bool m_passwordAsking = false;

        private Timer m_idleTimer;
        private object m_idleTimerLock = new object();

        private TimeSpan m_currentRemaining = TimeSpan.MinValue;
        private object m_currentRemainingLock = new object();
        public TimeSpan CurrentRemaining
        {
            get
            {
                TimeSpan temp;
                lock (m_currentRemainingLock) temp = m_currentRemaining;
                return temp;
            }
            set
            {
                lock (m_currentRemainingLock) m_currentRemaining = value;
                RaisePropertyChanged("CurrentRemaining");
                RaisePropertyChanged("CurrentRemainingDisplayed");
            }
        }

        private int m_currentLesson = 1;
        private object m_currentLessonLock = new object();
        public int CurrentLesson
        {
            get
            {
                int temp;
                lock (m_currentLessonLock) temp = m_currentLesson;
                return temp;
            }
            set
            {
                lock (m_currentLessonLock) m_currentLesson = value;
                RaisePropertyChanged("CurrentLesson");
                RaisePropertyChanged("CurrentRemainingDisplayed");
            }
        }

        private CurrentRemainingType m_currentType = CurrentRemainingType.NoLessons;
        private object m_currentTypeLock = new object();
        public CurrentRemainingType CurrentType
        {
            get
            {
                CurrentRemainingType temp;
                lock (m_currentTypeLock) temp = m_currentType;
                return temp;
            }
            set
            {
                lock (m_currentTypeLock) m_currentType = value;
                RaisePropertyChanged("CurrentType");
                RaisePropertyChanged("CurrentRemainingDisplayed");
            }
        }

        public string CurrentRemainingDisplayed
        {
            get
            {
                CurrentRemainingType type = this.CurrentType;
                switch(type)
                {
                    case CurrentRemainingType.Lesson:
                        return string.Format(GlobalConfig.GetStringResource("lang_CurrentLesson"), CurrentLesson.ToString()) + '\t' + string.Format(GlobalConfig.GetStringResource("lang_LessonRemaining"), CurrentRemaining.ToString(GlobalConfig.TimeSpanHMSFormat));

                    case CurrentRemainingType.Break:
                        return string.Format(GlobalConfig.GetStringResource("lang_CurrentLesson"), CurrentLesson.ToString()) + '\t' + string.Format(GlobalConfig.GetStringResource("lang_BreakRemaining"), CurrentRemaining.ToString(GlobalConfig.TimeSpanHMSFormat));

                    case CurrentRemainingType.NoLessons:
                    default:
                        return GlobalConfig.GetStringResource("lang_NoCurrentLessons");
                }
            }
        }

        private List<LessonHourViewModel> m_hoursCopy;
        private object m_hoursCopyLock = new object();

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

        private RelayCommand<CancelEventArgs> m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        private RelayCommand<string> m_checkUpdatesCommand;
        public ICommand CheckUpdatesCommand
        {
            get { return m_checkUpdatesCommand; }
        }

        private RelayCommand m_showSchedulesListCommand;
        public ICommand ShowSchedulesListCommand
        {
            get { return m_showSchedulesListCommand; }
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

        public Action<Action> InvokeWindow;

        private bool m_checkingUpdates = false;
        private object m_checkingUpdatesLock = new object();
        public bool CheckingUpdates
        {
            get
            {
                bool temp;
                lock (m_checkingUpdatesLock) temp = m_checkingUpdates;
                return temp;
            }
            private set
            {
                lock (m_checkingUpdatesLock)
                {
                    m_checkingUpdates = value;
                    RaisePropertyChanged("CheckingUpdates");
                    m_checkUpdatesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public void Init()
        {
            GlobalConfig.InitializeFastJSONCustom();
            GlobalConfig.Notifier.PropertyChanged += Notifier_PropertyChanged;
            GlobalConfig.Dialogs.ViewRegistered += (s, e) =>
            {
                e.View.PreviewKeyDown += View_PreviewKeyDown;
                e.View.PreviewMouseDown += View_PreviewMouseDown;
            };
            GlobalConfig.Dialogs.ViewUnregistered += (s, e) =>
            {
                e.View.PreviewKeyDown -= View_PreviewKeyDown;
                e.View.PreviewMouseDown -= View_PreviewMouseDown;
            };

            //ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            //{
            //    GlobalConfig.Notifier.LoadRegistry();
            //}
            //, null, "Odczytywanie ustawień z rejestru...");
            //GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            Reload(true);

            CheckNotices();
            
            AskPasswordIfNeeded();

            if (DateTime.Now.Date > GlobalConfig.Notifier.LastUpdateCheck.Date)  m_checkUpdatesCommand.Execute(null);

            CopyHours(true);
            lock (m_idleTimerLock)
            {
                m_idleTimer = new Timer(500);
                m_idleTimer.Elapsed += m_idleTimer_Elapsed;
                m_idleTimer.Start();
            }
            
            //string[] args = Environment.GetCommandLineArgs();
            //if (args.Length >= 2) m_openFromPathCommand.Execute(args[1]);
        }
        private void CopyHours(bool createNewCollection=false)
        {
            lock (m_hoursCopyLock)
            {
                if (createNewCollection) m_hoursCopy = new List<LessonHourViewModel>(); else m_hoursCopy.Clear();
                foreach (var item in GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours)
                {
                    LessonHourViewModel hr = new LessonHourViewModel();
                    hr.Number = item.Number;
                    hr.Start = item.Start;
                    hr.End = item.End;
                    m_hoursCopy.Add(hr);
                }
            }
        }

        private void m_idleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan currentRemaining = CurrentRemaining;

            int currentNumber = GlobalConfig.GetCurrentHourNumber(now);
            CurrentLesson = currentNumber;
            if (currentNumber < 0 && currentRemaining.Ticks != 0)
            {
                CurrentRemaining = new TimeSpan(0);
                CurrentType = CurrentRemainingType.NoLessons;
                return;
            }

            DateTime start = new DateTime(0);
            DateTime end = new DateTime(0);
            DateTime nextStart = new DateTime(0);
            lock (m_hoursCopyLock)
            {
                LessonHourViewModel hr = m_hoursCopy.FirstOrDefault(x => x.Number == currentNumber);
                if (hr != null)
                {
                    start = hr.Start;
                    end = hr.End;

                    int index = m_hoursCopy.IndexOf(hr);
                    if (index + 1 < m_hoursCopy.Count - 1)
                    {
                        nextStart = m_hoursCopy[index + 1].Start;
                    }
                }
            }

            if (start.TimeOfDay <= now.TimeOfDay && now.TimeOfDay <= end.TimeOfDay)
            {
                TimeSpan remaining = end.TimeOfDay - now.TimeOfDay;
                if (currentRemaining.Seconds != remaining.Seconds || currentRemaining.Minutes != remaining.Minutes || currentRemaining.Hours != remaining.Hours)
                {
                    CurrentRemaining = remaining;
                    CurrentType = CurrentRemainingType.Lesson;
                }
            }
            else if (nextStart.Ticks != 0)
            {
                TimeSpan remaining = nextStart.TimeOfDay - now.TimeOfDay;
                if (currentRemaining.Seconds != remaining.Seconds || currentRemaining.Minutes != remaining.Minutes || currentRemaining.Hours != remaining.Hours)
                {
                    CurrentRemaining = remaining;
                    CurrentType = CurrentRemainingType.Break;
                }
            }
            else
            {
                if (currentRemaining.Ticks != 0)
                {
                    CurrentRemaining = new TimeSpan(0);
                    CurrentType = CurrentRemainingType.NoLessons;
                }
            }
        }

        private void View_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ResetActivityTimer();
        }
        private void View_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ResetActivityTimer();
        }
        
        private void AskPasswordIfNeeded()
        {
            if (GlobalConfig.Notifier.Password == null) return;

            if (m_passwordAsking) return;
            m_passwordAsking = true;

            TypePasswordViewModel dialogViewModel = new TypePasswordViewModel();
            //Window view = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            //if (view != null && GlobalConfig.Dialogs.IsViewRegistered(view))
            //{
            //    GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Dialogs.GetViewModel(view), dialogViewModel);
            //}
            //else
            //{
            //    GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            //}
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Dialogs.GetActiveViewModel(this), dialogViewModel);
            if (dialogViewModel.Result == TypePasswordViewModel.TypePasswordResult.CloseApplication) GlobalConfig.Dialogs.GetWindow(this).Close();
            m_passwordAsking = false;

            SortOpenedClasses();
        }
        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DatabasesDirectory") m_databasesDirectoryChanged = true;
            else if (e.PropertyName == "BlockingMinutes") ResetActivityTimer();
        }
        private void ResetActivityTimer()
        {
            lock (m_activityTimerLock)
            {
                if (GlobalConfig.Notifier.BlockingMinutes > 0)
                {
                    if (m_activityTimer == null)
                    {
                        m_activityTimer = new Timer();
                        m_activityTimer.AutoReset = true;
                        m_activityTimer.Elapsed += m_activityTimer_Elapsed;
                    }

                    m_activityTimer.Stop();
                    m_activityTimer.Interval = GlobalConfig.Notifier.BlockingMinutes * 60 * 1000; // in miliseconds
                    m_activityTimer.Start();
                }
                else
                {
                    if (m_activityTimer != null)
                    {
                        m_activityTimer.Elapsed += m_activityTimer_Elapsed;
                        m_activityTimer.Dispose();
                        m_activityTimer = null;
                    }
                }
            }
        }

        private void m_activityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (m_activityTimerLock) m_activityTimer.Stop();

            InvokeWindow.Invoke(() => AskPasswordIfNeeded());

            lock (m_activityTimerLock) m_activityTimer.Start();
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
                SortOpenedClasses();
                if (!m_blockSaving) Options(null);
            }
            CopyHours();
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
            DateTime now = DateTime.Now;
            GlobalConfig.GlobalDatabase.ViewModel.LastArchivedDate = now;
            m_saveCommand.Execute(null);

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
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
        private void ShowSchedulesList(object param)
        {
            SchedulesListViewModel dialogViewModel = new SchedulesListViewModel(GlobalConfig.GlobalDatabase.ViewModel.Schedules, GlobalConfig.GlobalDatabase.ViewModel.Calendars, this.OpenedSchoolClasses, GlobalConfig.GlobalDatabaseAutoSaveCommand);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void Close(CancelEventArgs param)
        {
            string lastArchivedDateStr = (GlobalConfig.GlobalDatabase.ViewModel.LastArchivedDate.Year <= 1? GlobalConfig.GetStringResource("lang_Never") : GlobalConfig.GlobalDatabase.ViewModel.LastArchivedDate.ToString(GlobalConfig.DateTimeFormat));
            switch (GlobalConfig.MessageBox(this, string.Format(GlobalConfig.GetStringResource("lang_DoYouWantToArchiveFormat"), lastArchivedDateStr), MessageBoxSuperPredefinedButtons.YesNoCancel))
            {
                case MessageBoxSuperButton.Yes:
                    m_archiveDatabaseCommand.Execute(null);
                    break;

                case MessageBoxSuperButton.No:
                    m_saveCommand.Execute(null);
                    break;

                case MessageBoxSuperButton.Cancel:
                    param.Cancel = true;
                    return;
            }

            if (GlobalConfig.Notifier.UpdateRequest)
            {
                if (!File.Exists(GlobalConfig.AutoUpdaterPath))
                {
                    GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_AutoupdaterNotFound"), MessageBoxSuperPredefinedButtons.OK);
                }
                else
                {
                    Process p = new Process();
                    p.StartInfo.FileName = GlobalConfig.AutoUpdaterPath;
                    p.StartInfo.Arguments = GlobalConfig.CurrentVersion.ToString() + " " + GlobalConfig.UpdateInfoLink;
                    p.Start();
                }
            }
        }
        private void CheckUpdates(string param)
        {
            CheckingUpdates = true;
            VersionChecker.CheckVersionAsync(GlobalConfig.UpdateInfoLink, (x) =>
            {
                InvokeWindow.Invoke(() =>
                {
                    bool error = false;
                    error = x == null;
                    if (error || x.NewestVersion > GlobalConfig.CurrentVersion)
                    {
                        GlobalConfig.Notifier.LastUpdateCheck = DateTime.Now;

                        if (error)
                        {
                            GlobalConfig.MessageBox(GlobalConfig.Dialogs.GetActiveViewModel(this), GlobalConfig.GetStringResource("lang_ErrorOccurredWhileCheckingUpdates"), MessageBoxSuperPredefinedButtons.OK);
                            CheckingUpdates = false;
                            return;
                        }

                        if (GlobalConfig.MessageBox(GlobalConfig.Dialogs.GetActiveViewModel(this), GlobalConfig.GetStringResource("lang_NewVersionAvailable"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
                        GlobalConfig.Notifier.UpdateRequest = true;

                    }
                    if (!error && x.NewestVersion <= GlobalConfig.CurrentVersion && param == "True")
                    {
                        GlobalConfig.MessageBox(GlobalConfig.Dialogs.GetActiveViewModel(this), GlobalConfig.GetStringResource("lang_NoUpdatesFound"), MessageBoxSuperPredefinedButtons.OK);
                    }

                    CheckingUpdates = false;
                });
            });
        }
        private bool CanCheckUpdate(string param)
        {
            return !CheckingUpdates;
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
            m_sortedOpenedSchoolClasses.Clear();
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

                foreach (var sched in GlobalConfig.GlobalDatabase.ViewModel.Schedules)
                {
                    InitializeSelectedHours(sched.Monday);
                    InitializeSelectedHours(sched.Tuesday);
                    InitializeSelectedHours(sched.Wednesday);
                    InitializeSelectedHours(sched.Thursday);
                    InitializeSelectedHours(sched.Friday);
                }

                d.ProgressValue = 100;
            }
           , null, "", GlobalConfig.GetStringResource("lang_Opening"), GlobalConfig.ActionDialogProgressSize,  true);
            GlobalConfig.Dialogs.ShowDialog(this, saveDialogViewModel);

            m_databasesDirectoryChanged = false;

            RaisePropertyChanged("TabWidth");
            if (GlobalConfig.Notifier.Password == null) SortOpenedClasses();
        }
        private void InitializeSelectedHours(DayScheduleViewModel day)
        {
            foreach (var hour in day.HoursSchedule)
            {
                hour.InitializeSelectedGroup();
            }
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

        public void RaiseTabWidthChanged()
        {
            RaisePropertyChanged("TabWidth");
        }

        private void SortOpenedClasses()
        {
            DateTime now = DateTime.Now;
            int hourNumberNow = GlobalConfig.GetCurrentHourNumber(now);

            List<SortClassPriority> priorities = new List<SortClassPriority>();
            foreach (var openedClass in m_sortedOpenedSchoolClasses) priorities.Add((new SortClassPriority() { SchoolClass = openedClass, Priority = int.MaxValue }));
            WeekScheduleViewModel schedule = GlobalConfig.GlobalDatabase.ViewModel.CurrentSchedule;
            
            
            
            bool breakLoop = false;
            bool firstElapsed = false;
            int currentHourNumber = (IsSaturdayOrSunday(now.DayOfWeek) || hourNumberNow < 0 ? 1 : hourNumberNow);
            int selectedHourNumber = currentHourNumber;

            DayOfWeek nowDay = GetClosestWorkingDay(now.DayOfWeek);
            if (hourNumberNow == -2 && !IsSaturdayOrSunday(now.DayOfWeek)) nowDay = GetNextWorkingDayOfWeek(nowDay);
            DayOfWeek currentDay = nowDay;

            int priorityCounter = 0;
            while (!breakLoop)
            {
                if (currentDay == nowDay && firstElapsed) breakLoop = true;
                firstElapsed = true;
                DayScheduleViewModel daySchedule = GetDaySchedule(schedule, currentDay);
                for (int i = 0; i < daySchedule.HoursSchedule.Count; i++)
                {
                    SelectedHourViewModel item = daySchedule.HoursSchedule[i];
                    int hour = i + 1;

                    if (/*item.Hour*/ hour < selectedHourNumber) continue;
                    if (breakLoop && /*item.Hour*/ hour >= currentHourNumber) break;
                    SortClassPriority prior = null;
                    if (item.SelectedGroup != null) prior = priorities.FirstOrDefault(x => x.SchoolClass.ViewModel == item.SelectedGroup.OwnerClass);
                    if (prior != null && priorityCounter < prior.Priority)
                    {
                        prior.Priority = priorityCounter;
                        prior.Group = item.SelectedGroup;
                    }

                    priorityCounter++;
                }

                selectedHourNumber = 1;
                currentDay = GetNextWorkingDayOfWeek(currentDay);
            }

            priorities.Sort((x, y) => { return x.Priority.CompareTo(y.Priority); });

            for (int i = 0; i < priorities.Count; i++)
            {
                m_sortedOpenedSchoolClasses[i] = priorities[i].SchoolClass;
                if (priorities[i].Group != null) priorities[i].SchoolClass.SelectedGroup = priorities[i].Group;
            }

            if (m_sortedOpenedSchoolClasses.Count > 0) SelectedClass = m_sortedOpenedSchoolClasses[0];
        }
        
        private DayScheduleViewModel GetDaySchedule(WeekScheduleViewModel week, DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                case DayOfWeek.Monday: return week.Monday;
                case DayOfWeek.Tuesday: return week.Tuesday;
                case DayOfWeek.Wednesday: return week.Wednesday;
                case DayOfWeek.Thursday: return week.Thursday;
                case DayOfWeek.Friday: return week.Friday;
            }
            return null;
        }
        
        private DayOfWeek GetNextWorkingDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: return DayOfWeek.Tuesday;
                case DayOfWeek.Tuesday: return DayOfWeek.Wednesday;
                case DayOfWeek.Wednesday: return DayOfWeek.Thursday;
                case DayOfWeek.Thursday: return DayOfWeek.Friday;
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                case DayOfWeek.Friday: return DayOfWeek.Monday;
            }

            return dayOfWeek;
        }
        private DayOfWeek GetClosestWorkingDay(DayOfWeek dayOfWeek)
        {
            if (IsSaturdayOrSunday(dayOfWeek)) return DayOfWeek.Monday;
            return dayOfWeek;
        }
        private bool IsSaturdayOrSunday(DayOfWeek dayOfWeek)
        {
            return (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday);
        }

        private void m_openedSchoolClasses_Added(object sender, NotifyCollectionChangedSimpleEventArgs<SchoolClassControlViewModel> e)
        {
            foreach (var item in e.Items)
            {
                m_sortedOpenedSchoolClasses.Add(item);
            }
        }
        private void m_openedSchoolClasses_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<SchoolClassControlViewModel> e)
        {
            foreach (var item in e.Items)
            {
                m_sortedOpenedSchoolClasses.Remove(item);
            }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    //managed
                    lock(m_activityTimerLock)
                    {
                        m_activityTimer.Dispose();
                        m_activityTimer = null;
                    }
                }

                //unmanaged
                m_disposed = true;
            }
        }
        ~MainViewModel()
        {
            Dispose(false);
        }
    }
}
