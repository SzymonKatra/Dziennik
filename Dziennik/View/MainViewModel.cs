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

namespace Dziennik.View
{
    public sealed class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_saveCommand = new RelayCommand(Save);
            m_optionsCommand = new RelayCommand(Options);
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

        private bool m_databasesDirectoryChanged = true;

        public void Init()
        {
            GlobalConfig.Notifier.PropertyChanged += Notifier_PropertyChanged;

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                GlobalConfig.Notifier.LoadRegistry();
            }
            , null, "Odczytywanie ustawień z rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            ReloadSchoolClasses();
            
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
                GlobalConfig.Notifier.SaveRegistry();
            }
            , null, "Zapisywanie ustawień do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                GlobalConfig.GlobalDatabase.Save();
                foreach (var schoolClass in m_openedSchoolClasses) schoolClass.SaveCommand.Execute(null);
            }
            , null, "Zapisywanie bazy danych...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void Options(object e)
        {
            OptionsViewModel dialogViewModel = new OptionsViewModel(m_openedSchoolClasses);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            ActionDialogViewModel saveDialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                GlobalConfig.Notifier.SaveRegistry();
            }
            , null, "Zapisywanie ustawień do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, saveDialogViewModel);
            if (GlobalConfig.Notifier.AutoSave) GlobalConfig.GlobalDatabase.Save();
            if (m_databasesDirectoryChanged) ReloadSchoolClasses();
        }
        private void OpenFromPath(string path)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
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
            , null, path, "Otwieranie");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void CloseAllTabs()
        {
            if (GlobalConfig.GlobalDatabase != null) GlobalConfig.GlobalDatabase.Save();
            foreach (SchoolClassControlViewModel tab in m_openedSchoolClasses)
            {
                tab.SaveCommand.Execute(null);
                m_openedSchoolClasses.Remove(tab);
            }
        }
        private void ReloadSchoolClasses()
        {
            ActionDialogViewModel saveDialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                CloseAllTabs();

                GlobalConfig.CreateDirectoriesIfNotExists();

                string optionsPath = GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.SchoolOptionsDatabaseFileName;
                if (File.Exists(optionsPath))
                {
                    GlobalConfig.GlobalDatabase = DatabaseGlobal.Load(GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.SchoolOptionsDatabaseFileName);
                }
                else
                {
                    GlobalConfig.GlobalDatabase = new DatabaseGlobal();
                    GlobalConfig.GlobalDatabase.Path = optionsPath;
                    GlobalConfig.GlobalDatabase.Save();
                }

                IEnumerable<string> files = Directory.EnumerateFiles(GlobalConfig.Notifier.DatabasesDirectory);

                var validFiles = from f in files where f.EndsWith(GlobalConfig.SchoolClassDatabaseFileExtension) select f;

                foreach (string file in validFiles) OpenFromPath(file);
            }
           , null, "Odczytywanie klas...");
            GlobalConfig.Dialogs.ShowDialog(this, saveDialogViewModel);

            //foreach (var item in GlobalConfig.Database.SchoolClasses) m_openedSchoolClasses.Add(new SchoolClassControlViewModel(new SchoolClassViewModel(item)));
            m_databasesDirectoryChanged = false;
        }
    }
}
