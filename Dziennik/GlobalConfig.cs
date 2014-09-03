using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Dziennik.View;
using Microsoft.Win32;
using Dziennik.Model;
using System.IO;
using Dziennik.Controls;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using fastJSON;
using DziennikAktualizacja;
using System.Reflection;
using System.Diagnostics;

namespace Dziennik
{
    public static class GlobalConfig
    {
        public class GlobalConfigNotifier : ObservableObject
        {
            private bool m_updateRequest = false;
            public bool UpdateRequest
            {
                get { return m_updateRequest; }
                set { m_updateRequest = value; RaisePropertyChanged("UpdateRequest"); }
            }

            //to registry
            private bool m_showName = true;
            public bool ShowName
            {
                get { return m_showName; }
                set { m_showName = value; RaisePropertyChanged("ShowName"); }
            }

            private bool m_showSurname = true;
            public bool ShowSurname
            {
                get { return m_showSurname; }
                set { m_showSurname = value; RaisePropertyChanged("ShowSurname"); }
            }

            private bool m_showEmail = true;
            public bool ShowEmail
            {
                get { return m_showEmail; }
                set { m_showEmail = value; RaisePropertyChanged("ShowEmail"); }
            }

            private bool m_showFirstMarks = true;
            public bool ShowFirstMarks
            {
                get { return m_showFirstMarks; }
                set { m_showFirstMarks = value; RaisePropertyChanged("ShowFirstMarks"); }
            }

            private bool m_showFirstAverage = true;
            public bool ShowFirstAverage
            {
                get { return m_showFirstAverage; }
                set { m_showFirstAverage = value; RaisePropertyChanged("ShowFirstAverage"); }
            }

            private bool m_showFirstAttendance = true;
            public bool ShowFirstAttendance
            {
                get { return m_showFirstAttendance; }
                set { m_showFirstAttendance = value; RaisePropertyChanged("ShowFirstAttendance"); }
            }

            private bool m_showFirstJustified = true;
            public bool ShowFirstJustified
            {
                get { return m_showFirstJustified; }
                set { m_showFirstJustified = value; RaisePropertyChanged("ShowFirstJustified"); }
            }

            private bool m_showHalfEndingMark = true;
            public bool ShowHalfEndingMark
            {
                get { return m_showHalfEndingMark; }
                set { m_showHalfEndingMark = value; RaisePropertyChanged("ShowHalfEndingMark"); }
            }

            private bool m_showSecondMarks = true;
            public bool ShowSecondMarks
            {
                get { return m_showSecondMarks; }
                set { m_showSecondMarks = value; RaisePropertyChanged("ShowSecondMarks"); }
            }

            private bool m_showSecondAverage = true;
            public bool ShowSecondAverage
            {
                get { return m_showSecondAverage; }
                set { m_showSecondAverage = value; RaisePropertyChanged("ShowSecondAverage"); }
            }

            private bool m_showSecondAttendance = true;
            public bool ShowSecondAttendance
            {
                get { return m_showSecondAttendance; }
                set { m_showSecondAttendance = value; RaisePropertyChanged("ShowSecondAttendance"); }
            }

            private bool m_showSecondJustified = true;
            public bool ShowSecondJustified
            {
                get { return m_showSecondJustified; }
                set { m_showSecondJustified = value; RaisePropertyChanged("ShowSecondJustified"); }
            }

            private bool m_showEndingAverage = true;
            public bool ShowEndingAverage
            {
                get { return m_showEndingAverage; }
                set { m_showEndingAverage = value; RaisePropertyChanged("ShowEndingAverage"); }
            }

            private bool m_showYearAttendance = true;
            public bool ShowYearAttendance
            {
                get { return m_showYearAttendance; }
                set { m_showYearAttendance = value; RaisePropertyChanged("ShowYearAttendance"); }
            }

            private bool m_showYearJustified = true;
            public bool ShowYearJustified
            {
                get { return m_showYearJustified; }
                set { m_showYearJustified = value; RaisePropertyChanged("ShowYearJustified"); }
            }

            private bool m_showYearEndingMark = true;
            public bool ShowYearEndingMark
            {
                get { return m_showYearEndingMark; }
                set { m_showYearEndingMark = value; RaisePropertyChanged("ShowYearEndingMark"); }
            }

            private bool m_autoSave = true;
            public bool AutoSave
            {
                get { return m_autoSave; }
                set { m_autoSave = value; RaisePropertyChanged("AutoSave"); }
            }

            private bool m_showWeights = true;
            public bool ShowWeights
            {
                get { return m_showWeights; }
                set { m_showWeights = value; RaisePropertyChanged("ShowWeights"); }
            }

            private string m_databasesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Dziennik";
            public string DatabasesDirectory
            {
                get { return m_databasesDirectory; }
                set { m_databasesDirectory = value; RaisePropertyChanged("DatabasesDirectory"); }
            }

            private byte[] m_password;
            public byte[] Password
            {
                get { return m_password; }
                set { m_password = value; RaisePropertyChanged("Password"); }
            }

            private int m_blockingMinutes = 0;
            public int BlockingMinutes
            {
                get { return m_blockingMinutes; }
                set { m_blockingMinutes = value; RaisePropertyChanged("BlockingMinutes"); }
            }

            private DateTime m_lastUpdateCheck;
            public DateTime LastUpdateCheck
            {
                get { return m_lastUpdateCheck; }
                set { m_lastUpdateCheck = value; RaisePropertyChanged("LastUpdateCheck"); }
            }

            public void LoadRegistry()
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(GlobalConfig.RegistryKeyName);
                object showNameReg = key.GetValue(GlobalConfig.RegistryValueNameShowName);
                object showSurnameReg = key.GetValue(GlobalConfig.RegistryValueNameShowSurname);
                object showEmailReg = key.GetValue(GlobalConfig.RegistryValueNameShowEmail);
                object showFirstMarksReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstMarks);
                object showFirstAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstAverage);
                object showFirstAttendanceReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstAttendance);
                object showFirstJustifiedReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstJustified);
                object showHalfEndingMarkReg = key.GetValue(GlobalConfig.RegistryValueNameShowHalfEndingMark);
                object showSecondMarksReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondMarks);
                object showSecondAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondAverage);
                object showSecondAttendanceReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondAttendance);
                object showSecondJustifiedReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondJustified);
                object showEndingAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowEndingAverage);
                object showYearAttendanceReg = key.GetValue(GlobalConfig.RegistryValueNameShowYearAttendance);
                object showYearJustifiedReg = key.GetValue(GlobalConfig.RegistryValueNameShowYearJustified);
                object showYearEndingMarkReg = key.GetValue(GlobalConfig.RegistryValueNameShowYearEndingMark);
                object autoSaveReg = key.GetValue(GlobalConfig.RegistryValueNameAutoSave);
                object showWeightsReg = key.GetValue(GlobalConfig.RegistryValueNameShowWeights);
                object databasesDirectoryReg = key.GetValue(GlobalConfig.RegistryValueNameDatabasesDirectory);
                object passwordReg = key.GetValue(GlobalConfig.RegistryValueNameDatabasesPassword);
                object blockingMinutesReg = key.GetValue(GlobalConfig.RegistryValueNameBlockingMinutes);
                object lastUpdateCheckReg = key.GetValue(GlobalConfig.RegistryValueNameLastUpdateCheck);

                key.Close();

                if (showNameReg != null) ShowName = Ext.BoolParseOrDefault(showNameReg.ToString(), m_showName);
                if (showSurnameReg != null) ShowSurname = Ext.BoolParseOrDefault(showSurnameReg.ToString(), m_showSurname);
                if (showEmailReg != null) ShowEmail = Ext.BoolParseOrDefault(showEmailReg.ToString(), m_showEmail);
                if (showFirstMarksReg != null) ShowFirstMarks = Ext.BoolParseOrDefault(showFirstMarksReg.ToString(), m_showFirstMarks);
                if (showFirstAverageReg != null) ShowFirstAverage = Ext.BoolParseOrDefault(showFirstAverageReg.ToString(), m_showFirstAverage);
                if (showFirstAttendanceReg != null) ShowFirstAttendance = Ext.BoolParseOrDefault(showFirstAttendanceReg.ToString(), m_showFirstAttendance);
                if (showFirstJustifiedReg != null) ShowFirstJustified = Ext.BoolParseOrDefault(showFirstJustifiedReg.ToString(), m_showFirstJustified);
                if (showHalfEndingMarkReg != null) ShowHalfEndingMark = Ext.BoolParseOrDefault(showHalfEndingMarkReg.ToString(), m_showHalfEndingMark);
                if (showSecondMarksReg != null) ShowSecondMarks = Ext.BoolParseOrDefault(showSecondMarksReg.ToString(), m_showSecondMarks);
                if (showSecondAverageReg != null) ShowSecondAverage = Ext.BoolParseOrDefault(showSecondAverageReg.ToString(), m_showSecondAverage);
                if (showSecondAttendanceReg != null) ShowSecondAttendance = Ext.BoolParseOrDefault(showSecondAttendanceReg.ToString(), m_showSecondAttendance);
                if (showSecondJustifiedReg != null) ShowSecondJustified = Ext.BoolParseOrDefault(showSecondJustifiedReg.ToString(), m_showSecondJustified);
                if (showEndingAverageReg != null) ShowEndingAverage = Ext.BoolParseOrDefault(showEndingAverageReg.ToString(), m_showEndingAverage);
                if (showYearAttendanceReg != null) ShowYearAttendance = Ext.BoolParseOrDefault(showYearAttendanceReg.ToString(), m_showYearAttendance);
                if (showYearJustifiedReg != null) ShowYearJustified = Ext.BoolParseOrDefault(showYearJustifiedReg.ToString(), m_showYearJustified);
                if (showYearEndingMarkReg != null) ShowYearEndingMark = Ext.BoolParseOrDefault(showYearEndingMarkReg.ToString(), m_showYearEndingMark);
                if (autoSaveReg != null) AutoSave = Ext.BoolParseOrDefault(autoSaveReg.ToString(), m_autoSave);
                if (showWeightsReg != null) ShowWeights = Ext.BoolParseOrDefault(showWeightsReg.ToString(), m_showWeights);
                if (databasesDirectoryReg != null) DatabasesDirectory = databasesDirectoryReg.ToString();
                if (passwordReg != null) Password = (byte[])passwordReg;
                if (blockingMinutesReg != null) BlockingMinutes = Ext.IntParseOrDefault(blockingMinutesReg.ToString(), m_blockingMinutes);
                if (lastUpdateCheckReg != null) LastUpdateCheck = DateTime.FromBinary(Ext.LongParseOrDefault(lastUpdateCheckReg.ToString(), m_lastUpdateCheck.ToBinary()));
                //if (lastOpenedReg != null)
                //{
                //    string lastOpened = lastOpenedReg.ToString();
                //    string[] tokens = lastOpened.Split(';');
                //    foreach (string file in tokens) if (!string.IsNullOrWhiteSpace(file)) m_openFromPathCommand.Execute(file);
                //}
            }
            public void SaveRegistry()
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(GlobalConfig.RegistryKeyName);
                key.SetValue(GlobalConfig.RegistryValueNameShowName, m_showName);
                key.SetValue(GlobalConfig.RegistryValueNameShowSurname, m_showSurname);
                key.SetValue(GlobalConfig.RegistryValueNameShowEmail, m_showEmail);
                key.SetValue(GlobalConfig.RegistryValueNameShowFirstMarks, m_showFirstMarks);
                key.SetValue(GlobalConfig.RegistryValueNameShowFirstAverage, m_showFirstAverage);
                key.SetValue(GlobalConfig.RegistryValueNameShowFirstAttendance, m_showFirstAttendance);
                key.SetValue(GlobalConfig.RegistryValueNameShowFirstJustified, m_showFirstJustified);
                key.SetValue(GlobalConfig.RegistryValueNameShowHalfEndingMark, m_showHalfEndingMark);
                key.SetValue(GlobalConfig.RegistryValueNameShowSecondMarks, m_showSecondMarks);
                key.SetValue(GlobalConfig.RegistryValueNameShowSecondAverage, m_showSecondAverage);
                key.SetValue(GlobalConfig.RegistryValueNameShowSecondAttendance, m_showSecondAttendance);
                key.SetValue(GlobalConfig.RegistryValueNameShowSecondJustified, m_showSecondJustified);
                key.SetValue(GlobalConfig.RegistryValueNameShowEndingAverage, m_showEndingAverage);
                key.SetValue(GlobalConfig.RegistryValueNameShowYearAttendance, m_showYearAttendance);
                key.SetValue(GlobalConfig.RegistryValueNameShowYearJustified, m_showYearJustified);
                key.SetValue(GlobalConfig.RegistryValueNameShowYearEndingMark, m_showYearEndingMark);
                key.SetValue(GlobalConfig.RegistryValueNameAutoSave, m_autoSave);
                key.SetValue(GlobalConfig.RegistryValueNameShowWeights, m_showWeights);
                key.SetValue(GlobalConfig.RegistryValueNameDatabasesDirectory, m_databasesDirectory);
                if (m_password != null)
                {
                    key.SetValue(GlobalConfig.RegistryValueNameDatabasesPassword, m_password);
                }
                else
                {
                    key.DeleteValue(GlobalConfig.RegistryValueNameDatabasesPassword, false);
                }
                key.SetValue(GlobalConfig.RegistryValueNameBlockingMinutes, m_blockingMinutes);
                key.SetValue(GlobalConfig.RegistryValueNameLastUpdateCheck, m_lastUpdateCheck.ToBinary());
                //StringBuilder builder = new StringBuilder();
                //foreach (SchoolClassControlViewModel item in m_openedSchoolClasses)
                //{
                //    builder.Append(item.ViewModel.Path);
                //    builder.Append(";");
                //}
                //if (builder.Length >= 1) builder.Remove(builder.Length - 1, 1); // remove last ;
                //key.SetValue(GlobalConfig.RegistryValueNameLastOpened, builder.ToString());
                key.Close();
            }
        }

        public static readonly int DecimalRoundingPoints = 2;
        public static readonly string DateTimeFormat = "dd.MM.yyyy HH:mm";
        public static readonly string DateTimeWithSecondsFormat = "dd.MM.yyyy HH:mm:ss";
        public static readonly string DateFormat = "dd.MM.yyyy";
        public static readonly string TimeFormat = "HH:mm";
        public static readonly string FileDateTimeFormat = "ddMMyyy_HHmmss";
        public static readonly string SchoolClassDatabaseFileExtension = ".dzs";
        public static readonly string SchoolOptionsDatabaseFileExtension = ".dzo";
        public static readonly string DatabaseArchiveFileExtension = ".dza";
        public static readonly string FileDialogFilter = "Pliki dziennika (.dzs)|*.dzs|Dokumenty XML (.xml)|*.xml|Wszystkie pliki (*.*)|*.*";
        public static readonly string SchoolOptionsDatabaseFileName = "options" + SchoolOptionsDatabaseFileExtension;
        public static readonly string ErrorLogFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + '\\' + "Dziennik_Error.log";
        public static readonly Size ActionDialogProgressSize = new Size(400, 200);
        public static readonly int DefaultWeightMinValue = 0;
        public static readonly int DefaultWeightMaxValue = 9;
        public static readonly int MaxLessonHour = 15;
        public static readonly Version CurrentVersion;
        public static readonly string UpdateInfoLink = "https://www.dropbox.com/s/z7phk39d6wyi9jn/update_info.xml?dl=1";
        public static readonly string AutoUpdaterPath = "DziennikAktualizacja.exe";

        public static readonly string RegistryKeyName = @"Software\Dziennik_Katra";
        public static readonly string RegistryValueNameShowName = "ShowName";
        public static readonly string RegistryValueNameShowSurname = "ShowSurname";
        public static readonly string RegistryValueNameShowEmail = "ShowEmail";
        public static readonly string RegistryValueNameShowFirstMarks = "ShowFirstMarks";
        public static readonly string RegistryValueNameShowFirstAverage = "ShowFirstAverage";
        public static readonly string RegistryValueNameShowFirstAttendance = "ShowFirstAttendance";
        public static readonly string RegistryValueNameShowFirstJustified = "ShowFirstJustified";
        public static readonly string RegistryValueNameShowHalfEndingMark = "ShowHalfEndingMark";
        public static readonly string RegistryValueNameShowSecondMarks = "ShowSecondMarks";
        public static readonly string RegistryValueNameShowSecondAverage = "ShowSecondAverage";
        public static readonly string RegistryValueNameShowSecondAttendance = "ShowSecondAttendance";
        public static readonly string RegistryValueNameShowSecondJustified = "ShowSecondJustified";
        public static readonly string RegistryValueNameShowEndingAverage = "ShowEndingAverage";
        public static readonly string RegistryValueNameShowYearAttendance = "ShowYearAttendance";
        public static readonly string RegistryValueNameShowYearJustified = "ShowYearJustified";
        public static readonly string RegistryValueNameShowYearEndingMark = "ShowYearEndingMark";
        public static readonly string RegistryValueNameAutoSave = "AutoSave";
        public static readonly string RegistryValueNameShowWeights = "ShowWeights";
        public static readonly string RegistryValueNameDatabasesDirectory = "DatabasesDirectory";
        public static readonly string RegistryValueNameDatabasesPassword = "Password";
        public static readonly string RegistryValueNameBlockingMinutes = "BlockingMinutes";
        public static readonly string RegistryValueNameLastUpdateCheck = "LastUpdateCheck";

        public static readonly string CurrentDatabaseSubdirectory = "Baza";
        public static readonly string ArchiveDatabasesSubdirectory = "Archiwum";

        public static readonly DialogService Dialogs;

        private static MainViewModel m_main;
        public static MainViewModel Main
        {
            get { return m_main; }
            set
            {
                if (m_main != null) throw new InvalidOperationException("Main window already assigned");
                m_main = value;
            }
        }

        private static DatabaseGlobal m_globalDatabase;
        public static DatabaseGlobal GlobalDatabase
        {
            get { return m_globalDatabase; }
            set { m_globalDatabase = value; }
        }

        private static RelayCommand m_globalDatabaseAutoSaveCommand;
        public static ICommand GlobalDatabaseAutoSaveCommand
        {
            get { return GlobalConfig.m_globalDatabaseAutoSaveCommand; }
        }

        static GlobalConfig()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            CurrentVersion = Version.Parse(fileVersion.ProductVersion);

            m_globalDatabaseAutoSaveCommand = new RelayCommand(x => { if (m_notifier.AutoSave) m_globalDatabase.Save(); });

            Dictionary<Type, Func<object, Window>> windowViewModelMappings = new Dictionary<Type, Func<object, Window>>();

            windowViewModelMappings.Add(typeof(EditMarkViewModel), vm => new EditMarkWindow((EditMarkViewModel)vm));
            windowViewModelMappings.Add(typeof(EditStudentViewModel), vm => new EditStudentWindow((EditStudentViewModel)vm));
            windowViewModelMappings.Add(typeof(ActionDialogViewModel), vm => new ActionDialogWindow((ActionDialogViewModel)vm));
            windowViewModelMappings.Add(typeof(GlobalStudentsListViewModel), vm => new GlobalStudentsListWindow((GlobalStudentsListViewModel)vm));
            windowViewModelMappings.Add(typeof(AddGroupViewModel), vm => new AddGroupWindow((AddGroupViewModel)vm));
            windowViewModelMappings.Add(typeof(SelectStudentsViewModel), vm => new SelectStudentsWindow((SelectStudentsViewModel)vm));
            windowViewModelMappings.Add(typeof(EditGroupViewModel), vm => new EditGroupWindow((EditGroupViewModel)vm));
            windowViewModelMappings.Add(typeof(EditClassViewModel), vm => new EditClassWindow((EditClassViewModel)vm));
            windowViewModelMappings.Add(typeof(OptionsViewModel), vm => new OptionsWindow((OptionsViewModel)vm));
            windowViewModelMappings.Add(typeof(EditEndingMarkViewModel), vm => new EditEndingMarkWindow((EditEndingMarkViewModel)vm));
            windowViewModelMappings.Add(typeof(GlobalSubjectsListViewModel), vm => new GlobalSubjectsListWindow((GlobalSubjectsListViewModel)vm));
            windowViewModelMappings.Add(typeof(EditGlobalSubjectViewModel), vm => new EditGlobalSubjectWindow((EditGlobalSubjectViewModel)vm));
            windowViewModelMappings.Add(typeof(SelectGlobalSubjectViewModel), vm => new SelectGlobalSubjectWindow((SelectGlobalSubjectViewModel)vm));
            windowViewModelMappings.Add(typeof(RealizeSubjectViewModel), vm => new RealizeSubjectWindow((RealizeSubjectViewModel)vm));
            windowViewModelMappings.Add(typeof(EditCalendarViewModel), vm => new EditCalendarWindow((EditCalendarViewModel)vm));
            windowViewModelMappings.Add(typeof(GlobalCalendarListViewModel), vm => new GlobalCalendarListWindow((GlobalCalendarListViewModel)vm));
            windowViewModelMappings.Add(typeof(EditOffDayViewModel), vm => new EditOffDayWindow((EditOffDayViewModel)vm));
            windowViewModelMappings.Add(typeof(AddMarksSetViewModel), vm => new AddMarksSetWindow((AddMarksSetViewModel)vm));
            windowViewModelMappings.Add(typeof(InfoDialogViewModel), vm => new InfoDialogWindow((InfoDialogViewModel)vm));
            windowViewModelMappings.Add(typeof(EditMarksCategoryViewModel), vm => new EditMarksCategoryWindow((EditMarksCategoryViewModel)vm));
            windowViewModelMappings.Add(typeof(EditNoticeViewModel), vm => new EditNoticeWindow((EditNoticeViewModel)vm));
            windowViewModelMappings.Add(typeof(NoticesListViewModel), vm => new NoticesListWindow((NoticesListViewModel)vm));
            windowViewModelMappings.Add(typeof(ArchivesListViewModel), vm => new ArchivesListWindow((ArchivesListViewModel)vm));
            windowViewModelMappings.Add(typeof(ClassesListViewModel), vm => new ClassesListWindow((ClassesListViewModel)vm));
            windowViewModelMappings.Add(typeof(MarksCategoriesListViewModel), vm => new MarksCategoriesListWindow((MarksCategoriesListViewModel)vm));
            windowViewModelMappings.Add(typeof(ChangePasswordViewModel), vm => new ChangePasswordWindow((ChangePasswordViewModel)vm));
            windowViewModelMappings.Add(typeof(TypePasswordViewModel), vm => new TypePasswordWindow((TypePasswordViewModel)vm));
            windowViewModelMappings.Add(typeof(EditLessonsHoursViewModel), vm => new EditLessonsHoursWindow((EditLessonsHoursViewModel)vm));
            windowViewModelMappings.Add(typeof(EditScheduleViewModel), vm => new EditScheduleWindow((EditScheduleViewModel)vm));
            windowViewModelMappings.Add(typeof(SchedulesListViewModel), vm => new SchedulesListWindow((SchedulesListViewModel)vm));
            windowViewModelMappings.Add(typeof(SelectGroupViewModel), vm => new SelectGroupWindow((SelectGroupViewModel)vm));
            windowViewModelMappings.Add(typeof(OverdueSubjectsListViewModel), vm => new OverdueSubjectsListWindow((OverdueSubjectsListViewModel)vm));

            Dialogs = new DialogService(windowViewModelMappings);
        }

        public static void InitializeFastJSONCustom()
        {
            JSON.RegisterCustomType(typeof(TimeSpan), SerializeTimeSpan, DeserializeTimeSpan);
        }
        private static string SerializeTimeSpan(object data)
        {
            return ((TimeSpan)data).Ticks.ToString();
        }
        private static object DeserializeTimeSpan(string data)
        {
            return new TimeSpan(long.Parse(data));
        }
        public static void InitializeNotifier()
        {
            if (m_notifier != null) throw new InvalidOperationException("Notifier already initialized");
            m_notifier = new GlobalConfigNotifier();
        }
        public static void CreateDirectoriesIfNotExists()
        {
            if (!Directory.Exists(Notifier.DatabasesDirectory)) Directory.CreateDirectory(Notifier.DatabasesDirectory);
            if (!Directory.Exists(Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory)) Directory.CreateDirectory(Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory);
            if (!Directory.Exists(Notifier.DatabasesDirectory + @"\" + GlobalConfig.ArchiveDatabasesSubdirectory)) Directory.CreateDirectory(Notifier.DatabasesDirectory + @"\" + GlobalConfig.ArchiveDatabasesSubdirectory);
        }

        public static string GetStringResource(object key)
        {
            object result = Application.Current.TryFindResource(key);
            return (result == null ? key + " NOT FOUND - RESOURCE ERROR" : (string)result);
        }
        public static MessageBoxSuperButton MessageBox(object viewModel, string text, MessageBoxSuperPredefinedButtons buttons)
        {
            return MessageBoxSuper.ShowBox(Dialogs.GetWindow(viewModel), text, GetStringResource("lang_AppName"), buttons);
        }

        //SINGLETON
        private static GlobalConfigNotifier m_notifier = null;
        public static GlobalConfigNotifier Notifier
        {
            get
            {
                if (m_notifier == null) m_notifier = new GlobalConfigNotifier();
                return m_notifier;
            }
        }
    }
}
