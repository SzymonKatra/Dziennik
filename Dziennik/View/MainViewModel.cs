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
            m_openCommand = new RelayCommand(Open);
            m_openFromPathCommand = new RelayCommand<string>(OpenFromPath);
            m_saveCommand = new RelayCommand(Save, CanSave);
            m_saveAllCommand = new RelayCommand(SaveAll);
            m_closeTabCommand = new RelayCommand<SchoolClassControlViewModel>(CloseTab);
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

        private RelayCommand m_openCommand;
        public ICommand OpenCommand
        {
            get { return m_openCommand; }
        }

        private RelayCommand<string> m_openFromPathCommand;
        public ICommand OpenFromPathCommand
        {
            get { return m_openFromPathCommand; }
        }

        private RelayCommand m_saveCommand;
        public ICommand SaveCommand
        {
            get { return m_saveCommand; }
        }

        private RelayCommand m_saveAllCommand;
        public ICommand SaveAllCommand
        {
            get { return m_saveAllCommand; }
        }

        private RelayCommand<SchoolClassControlViewModel> m_closeTabCommand;
        public ICommand CloseTabCommand
        {
            get { return m_closeTabCommand; }
        }

        private RelayCommand m_optionsCommand;
        public ICommand OptionsCommand
        {
            get { return m_optionsCommand; }
        }

        private bool m_schoolClassesDirectoryChanged = true;

        public void Init()
        {
            GlobalConfig.Notifier.PropertyChanged += Notifier_PropertyChanged;

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                GlobalConfig.Notifier.LoadRegistry();
            }
            , null, "Odczytywanie z ustawień rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            ReloadSchoolClasses();
            
            //string[] args = Environment.GetCommandLineArgs();
            //if (args.Length >= 2) m_openFromPathCommand.Execute(args[1]);
        }
        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SchoolClassesDirectory") m_schoolClassesDirectoryChanged = true;
        }

        private void Open(object param)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = GlobalConfig.FileExtension;
            //sfd.ValidateNames = true;
            ofd.Filter = GlobalConfig.FileDialogFilter;

            bool? result = ofd.ShowDialog(GlobalConfig.Dialogs.GetWindow(this));

            if (result == true)
            {
                m_openFromPathCommand.Execute(ofd.FileName);
            }
        }
        private void OpenFromPath(string path)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                if (!File.Exists(path))
                {
                    MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wybrany plik nie istnieje", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
                }

                SchoolClassControlViewModel searchTab = m_openedSchoolClasses.FirstOrDefault((x) => { return x.ViewModel.Path == path; });
                if (searchTab != null)
                {
                    MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Ta klasa jest już otwarta", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
                    SelectedClass = searchTab;
                    return;
                }

                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    SchoolClassViewModel schoolClass = null;
                    try
                    {
                        schoolClass = SchoolClassViewModel.Deserialize(stream);
                    }
                    catch { MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wystąpił błąd podczas odczytywania pliku", "Dziennik", MessageBoxSuperPredefinedButtons.OK); }

                    if (schoolClass == null) return;
                    schoolClass.Path = path;
                    SchoolClassControlViewModel tab = new SchoolClassControlViewModel(schoolClass);
                    m_openedSchoolClasses.Add(tab);
                    SelectedClass = tab;

                    if (SelectedClass.ViewModel.Groups.Count > 0)
                    {
                        SelectedClass.SelectedGroup = SelectedClass.ViewModel.Groups[0];
                    }
                }
            }
            , null, path, "Otwieranie");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);         
        }
        private void Save(object param)
        {
            m_selectedClass.SaveCommand.Execute(null);
        }
        private bool CanSave(object param)
        {
            return m_selectedClass != null;
        }
        private void SaveAll(object param)
        {
            foreach (SchoolClassControlViewModel tab in m_openedSchoolClasses) PromptSave(tab);

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                GlobalConfig.Notifier.SaveRegistry();
            }
            , null, "Zapisywanie ustawień do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void CloseTab(SchoolClassControlViewModel e)
        {
            PromptSave(e);
            
            m_openedSchoolClasses.Remove(e);
        }
        private void PromptSave(SchoolClassControlViewModel param)
        {
            //if (GlobalConfig.Notifier.AutoSave)
            //{
            param.SaveCommand.Execute(null);
            //}
            //else
            //{
            //    if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Czy chcesz zapisać zmiany w klasie " + param.ViewModel.Name + "?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) == MessageBoxSuperButton.Yes)
            //    {
            //        param.SaveCommand.Execute(null);
            //    }
            //}
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
            if (m_schoolClassesDirectoryChanged) ReloadSchoolClasses();
        }

        private void CloseAllTabs()
        {
            foreach (SchoolClassControlViewModel tab in m_openedSchoolClasses) m_closeTabCommand.Execute(tab);
        }
        private void ReloadSchoolClasses()
        {
            try
            {
                CloseAllTabs();

                IEnumerable<string> files = Directory.EnumerateFiles(GlobalConfig.Notifier.SchoolClassesDirectory);

                var validFiles = from f in files where f.EndsWith(GlobalConfig.FileExtension) select f;

                foreach (string file in validFiles) m_openFromPathCommand.Execute(file);
            }
            catch { }
            m_schoolClassesDirectoryChanged = false;
        }
    }
}
