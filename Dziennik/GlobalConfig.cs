using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Dziennik.View;
using Microsoft.Win32;
using Dziennik.Model;

namespace Dziennik
{
    public static class GlobalConfig
    {
        public class GlobalConfigNotifier : ObservableObject
        {
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

            private bool m_showEndingAverage = true;
            public bool ShowEndingAverage
            {
                get { return m_showEndingAverage; }
                set { m_showEndingAverage = value; RaisePropertyChanged("ShowEndingAverage"); }
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

            private string m_databasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DziennikBaza.sdf";
            public string DatabasePath
            {
                get { return m_databasePath; }
                set { m_databasePath = value; RaisePropertyChanged("SchoolClassesDirectory"); }
            }

            private string m_databaseArchiveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Dziennik_Archiwum";
            public string DatabaseArchiveDirectory
            {
                get { return m_databaseArchiveDirectory; }
                set { m_databaseArchiveDirectory = value; RaisePropertyChanged("SchoolClassesBackupDirectory"); }
            }

            public void LoadRegistry()
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(GlobalConfig.RegistryKeyName);
                object showNameReg = key.GetValue(GlobalConfig.RegistryValueNameShowName);
                object showSurnameReg = key.GetValue(GlobalConfig.RegistryValueNameShowSurname);
                object showEmailReg = key.GetValue(GlobalConfig.RegistryValueNameShowEmail);
                object showFirstMarksReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstMarks);
                object showFirstAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstAverage);
                object showHalfEndingMarkReg = key.GetValue(GlobalConfig.RegistryValueNameShowHalfEndingMark);
                object showSecondMarksReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondMarks);
                object showSecondAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondAverage);
                object showEndingAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowEndingAverage);
                object showYearEndingMarkReg = key.GetValue(GlobalConfig.RegistryValueNameShowYearEndingMark);
                object autoSaveReg = key.GetValue(GlobalConfig.RegistryValueNameAutoSave);
                object databasePathReg = key.GetValue(GlobalConfig.RegistryValueNameDatabasePath);
                object databaseArchiveDirectoryReg = key.GetValue(GlobalConfig.RegistryValueNameDatabaseArchiveDirectory);
                key.Close();

                if (showNameReg != null) ShowName = Ext.BoolParseOrDefault(showNameReg.ToString(), m_showName);
                if (showSurnameReg != null) ShowSurname = Ext.BoolParseOrDefault(showSurnameReg.ToString(), m_showSurname);
                if (showEmailReg != null) ShowEmail = Ext.BoolParseOrDefault(showEmailReg.ToString(), m_showEmail);
                if (showFirstMarksReg != null) ShowFirstMarks = Ext.BoolParseOrDefault(showFirstMarksReg.ToString(), m_showFirstMarks);
                if (showFirstAverageReg != null) ShowFirstAverage = Ext.BoolParseOrDefault(showFirstAverageReg.ToString(), m_showFirstAverage);
                if (showHalfEndingMarkReg != null) ShowHalfEndingMark = Ext.BoolParseOrDefault(showHalfEndingMarkReg.ToString(), m_showHalfEndingMark);
                if (showSecondMarksReg != null) ShowSecondMarks = Ext.BoolParseOrDefault(showSecondMarksReg.ToString(), m_showSecondMarks);
                if (showSecondAverageReg != null) ShowSecondAverage = Ext.BoolParseOrDefault(showSecondAverageReg.ToString(), m_showSecondAverage);
                if (showEndingAverageReg != null) ShowEndingAverage = Ext.BoolParseOrDefault(showEndingAverageReg.ToString(), m_showEndingAverage);
                if (showYearEndingMarkReg != null) ShowYearEndingMark = Ext.BoolParseOrDefault(showYearEndingMarkReg.ToString(), m_showYearEndingMark);
                if (autoSaveReg != null) AutoSave = Ext.BoolParseOrDefault(autoSaveReg.ToString(), m_autoSave);
                if (databasePathReg != null) DatabasePath = databasePathReg.ToString();
                if (databaseArchiveDirectoryReg != null) DatabaseArchiveDirectory = databaseArchiveDirectoryReg.ToString();
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
                key.SetValue(GlobalConfig.RegistryValueNameShowHalfEndingMark, m_showHalfEndingMark);
                key.SetValue(GlobalConfig.RegistryValueNameShowSecondMarks, m_showSecondMarks);
                key.SetValue(GlobalConfig.RegistryValueNameShowSecondAverage, m_showSecondAverage);
                key.SetValue(GlobalConfig.RegistryValueNameShowEndingAverage, m_showEndingAverage);
                key.SetValue(GlobalConfig.RegistryValueNameShowYearEndingMark, m_showYearEndingMark);
                key.SetValue(GlobalConfig.RegistryValueNameAutoSave, m_autoSave);
                key.SetValue(GlobalConfig.RegistryValueNameDatabasePath, m_databasePath);
                key.SetValue(GlobalConfig.RegistryValueNameDatabaseArchiveDirectory, m_databaseArchiveDirectory);
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
        public static readonly string FileExtension = ".dzi";
        public static readonly string FileDialogFilter = "Pliki dziennika (.dzi)|*.dzi|Dokumenty XML (.xml)|*.xml|Wszystkie pliki (*.*)|*.*";
        public static readonly string RegistryKeyName = @"Software\Dziennik_Katra";
        public static readonly string RegistryValueNameShowName = "ShowName";
        public static readonly string RegistryValueNameShowSurname = "ShowSurname";
        public static readonly string RegistryValueNameShowEmail = "ShowEmail";
        public static readonly string RegistryValueNameShowFirstMarks = "ShowFirstMarks";
        public static readonly string RegistryValueNameShowFirstAverage = "ShowFirstAverage";
        public static readonly string RegistryValueNameShowHalfEndingMark = "ShowHalfEndingMark";
        public static readonly string RegistryValueNameShowSecondMarks = "ShowSecondMarks";
        public static readonly string RegistryValueNameShowSecondAverage = "ShowSecondAverage";
        public static readonly string RegistryValueNameShowEndingAverage = "ShowEndingAverage";
        public static readonly string RegistryValueNameShowYearEndingMark = "ShowYearEndingMark";
        public static readonly string RegistryValueNameAutoSave = "AutoSave";
        public static readonly string RegistryValueNameLastOpened = "LastOpened";
        public static readonly string RegistryValueNameDatabasePath = "DatabasePath";
        public static readonly string RegistryValueNameDatabaseArchiveDirectory = "DatabaseArchiveDirectory";

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

        private static DatabaseContext m_database;
        public static DatabaseContext Database
        {
            get { return m_database; }
        }

        public static void InitializeDatabase()
        {
            if (m_database != null) m_database.Dispose();
            m_database = new DatabaseContext(Notifier.DatabasePath);
        }

        static GlobalConfig()
        {
            Dictionary<Type, Func<object, Window>> windowViewModelMappings = new Dictionary<Type, Func<object, Window>>();

            windowViewModelMappings.Add(typeof(EditMarkViewModel), (vm) => { return new EditMarkWindow((EditMarkViewModel)vm); });
            windowViewModelMappings.Add(typeof(EditStudentViewModel), (vm) => { return new EditStudentWindow((EditStudentViewModel)vm); });
            windowViewModelMappings.Add(typeof(ActionDialogViewModel), (vm) => { return new ActionDialogWindow((ActionDialogViewModel)vm); });
            windowViewModelMappings.Add(typeof(GlobalStudentsListViewModel), (vm) => { return new GlobalStudentsListWindow((GlobalStudentsListViewModel)vm); });
            windowViewModelMappings.Add(typeof(AddGroupViewModel), (vm) => { return new AddGroupWindow((AddGroupViewModel)vm); });
            windowViewModelMappings.Add(typeof(SelectStudentsViewModel), (vm) => { return new SelectStudentsWindow((SelectStudentsViewModel)vm); });
            windowViewModelMappings.Add(typeof(EditGroupViewModel), (vm) => { return new EditGroupWindow((EditGroupViewModel)vm); });
            windowViewModelMappings.Add(typeof(EditClassViewModel), (vm) => { return new EditClassWindow((EditClassViewModel)vm); });
            windowViewModelMappings.Add(typeof(OptionsViewModel), (vm) => { return new OptionsWindow((OptionsViewModel)vm); });
            windowViewModelMappings.Add(typeof(EditEndingMarkViewModel), (vm) => { return new EditEndingMarkWindow((EditEndingMarkViewModel)vm); });

            Dialogs = new DialogService(windowViewModelMappings);
        }

        public static void InitializeNotifier()
        {
            if (m_notifier != null) throw new InvalidOperationException("Notifier already initialized");
            m_notifier = new GlobalConfigNotifier();
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
