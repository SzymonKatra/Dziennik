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
            m_addCommand = new RelayCommand(Add);
            m_openCommand = new RelayCommand(Open);
            m_openFromPathCommand = new RelayCommand<string>(OpenFromPath);
            m_editCommand = new RelayCommand(Edit, CanEdit);
            m_saveCommand = new RelayCommand(Save, CanSave);
            m_saveAllCommand = new RelayCommand(SaveAll);
            m_closeTabCommand = new RelayCommand<SchoolClassControlViewModel>(CloseTab);

            LoadRegistry();
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedSchoolClasses = new ObservableCollection<SchoolClassControlViewModel>();
        public ObservableCollection<SchoolClassControlViewModel> OpenedSchoolClasses
        {
            get { return m_openedSchoolClasses; }
            set { m_openedSchoolClasses = value; OnPropertyChanged("OpenedSchoolClasses"); }
        }
        private SchoolClassControlViewModel m_selectedClass;
        public SchoolClassControlViewModel SelectedClass
        {
            get { return m_selectedClass; }
            set
            {
                m_selectedClass = value;
                OnPropertyChanged("SelectedClass");
                m_editCommand.RaiseCanExecuteChanged();
            }
        }

        private bool m_showName = true;
        public bool ShowName
        {
            get { return m_showName; }
            set { m_showName = value; OnPropertyChanged("ShowName"); }
        }

        private bool m_showSurname = true;
        public bool ShowSurname
        {
            get { return m_showSurname; }
            set { m_showSurname = value; OnPropertyChanged("ShowSurname"); }
        }

        private bool m_showEmail = true;
        public bool ShowEmail
        {
            get { return m_showEmail; }
            set { m_showEmail = value; OnPropertyChanged("ShowEmail"); }
        }

        private bool m_showFirstMarks = true;
        public bool ShowFirstMarks
        {
            get { return m_showFirstMarks; }
            set { m_showFirstMarks = value; OnPropertyChanged("ShowFirstMarks"); }
        }

        private bool m_showFirstAverage = true;
        public bool ShowFirstAverage
        {
            get { return m_showFirstAverage; }
            set { m_showFirstAverage = value; OnPropertyChanged("ShowFirstAverage"); }
        }

        private bool m_showSecondMarks = true;
        public bool ShowSecondMarks
        {
            get { return m_showSecondMarks; }
            set { m_showSecondMarks = value; OnPropertyChanged("ShowSecondMarks"); }
        }

        private bool m_showSecondAverage = true;
        public bool ShowSecondAverage
        {
            get { return m_showSecondAverage; }
            set { m_showSecondAverage = value; OnPropertyChanged("ShowSecondAverage"); }
        }

        private bool m_showEndingAverage = true;
        public bool ShowEndingAverage
        {
            get { return m_showEndingAverage; }
            set { m_showEndingAverage = value; OnPropertyChanged("ShowEndingAverage"); }
        }

        private RelayCommand m_addCommand;
        public ICommand AddCommand
        {
            get { return m_addCommand; }
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

        private RelayCommand m_editCommand;
        public ICommand EditCommand
        {
            get { return m_editCommand; }
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

        private void Add(object param)
        {
            SchoolClassViewModel schoolClass = new SchoolClassViewModel();
            EditClassViewModel dialogViewModel = new EditClassViewModel(schoolClass);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result== EditClassViewModel.EditClassResult.Ok)
            {
                SchoolClassControlViewModel tab = new SchoolClassControlViewModel(this, schoolClass);
                m_openedSchoolClasses.Add(tab);
                SelectedClass = tab;
            }
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
                ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
                {
                    m_openFromPathCommand.Execute(ofd.FileName);
                }
                , null, "Otwieranie...");
                GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            }
        }
        private void OpenFromPath(string path)
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
                SchoolClassControlViewModel tab = new SchoolClassControlViewModel(this, schoolClass);
                m_openedSchoolClasses.Add(tab);
                SelectedClass = tab;
            }
        }
        private void Edit(object param)
        {
            SchoolClassControlViewModel tab = m_selectedClass;
            EditClassViewModel dialogViewModel = new EditClassViewModel(tab.ViewModel);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.RemoveClass)
            {
                if (System.IO.File.Exists(tab.ViewModel.Path))
                {
                    System.IO.File.Delete(tab.ViewModel.Path);
                }

                m_selectedClass = null;
                m_openedSchoolClasses.Remove(tab);
                return;
            }
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.Ok) m_selectedClass.SaveCommand.Execute(null);
        }
        private bool CanEdit(object param)
        {
            return m_selectedClass != null;
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
            foreach (SchoolClassControlViewModel tab in m_openedSchoolClasses) tab.SaveCommand.Execute(null);

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                SaveRegistry();
            }
            , null, "Zapisywanie do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void CloseTab(SchoolClassControlViewModel e)
        {
            e.SaveCommand.Execute(null);
            m_openedSchoolClasses.Remove(e);
        }

        private void LoadRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(GlobalConfig.RegistryKeyName);
            object showNameReg = key.GetValue(GlobalConfig.RegistryValueNameShowName);
            object showSurnameReg = key.GetValue(GlobalConfig.RegistryValueNameShowSurname);
            object showEmailReg = key.GetValue(GlobalConfig.RegistryValueNameShowEmail);
            object showFirstMarksReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstMarks);
            object showFirstAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowFirstAverage);
            object showSecondMarksReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondMarks);
            object showSecondAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowSecondAverage);
            object showEndingAverageReg = key.GetValue(GlobalConfig.RegistryValueNameShowEndingAverage);
            object lastOpenedReg = key.GetValue(GlobalConfig.RegistryValueNameLastOpened);
            key.Close();

            if (showNameReg != null) bool.TryParse(showNameReg.ToString(), out m_showName);
            if (showSurnameReg != null) bool.TryParse(showSurnameReg.ToString(), out m_showSurname);
            if (showEmailReg != null) bool.TryParse(showEmailReg.ToString(), out m_showEmail);
            if (showFirstMarksReg != null) bool.TryParse(showFirstMarksReg.ToString(), out m_showFirstMarks);
            if (showFirstAverageReg != null) bool.TryParse(showFirstAverageReg.ToString(), out m_showFirstAverage);
            if (showSecondMarksReg != null) bool.TryParse(showSecondMarksReg.ToString(), out m_showSecondMarks);
            if (showSecondAverageReg != null) bool.TryParse(showSecondAverageReg.ToString(), out m_showSecondAverage);
            if (showEndingAverageReg != null) bool.TryParse(showEndingAverageReg.ToString(), out m_showEndingAverage);
            if (lastOpenedReg != null)
            {
                string lastOpened = lastOpenedReg.ToString();
                string[] tokens = lastOpened.Split(';');
                foreach (string file in tokens) m_openFromPathCommand.Execute(file);
            }
        }
        private void SaveRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(GlobalConfig.RegistryKeyName);
            key.SetValue(GlobalConfig.RegistryValueNameShowName, m_showName);
            key.SetValue(GlobalConfig.RegistryValueNameShowSurname, m_showSurname);
            key.SetValue(GlobalConfig.RegistryValueNameShowEmail, m_showEmail);
            key.SetValue(GlobalConfig.RegistryValueNameShowFirstMarks, m_showFirstMarks);
            key.SetValue(GlobalConfig.RegistryValueNameShowFirstAverage, m_showFirstAverage);
            key.SetValue(GlobalConfig.RegistryValueNameShowSecondMarks, m_showSecondMarks);
            key.SetValue(GlobalConfig.RegistryValueNameShowSecondAverage, m_showSecondAverage);
            key.SetValue(GlobalConfig.RegistryValueNameShowEndingAverage, m_showEndingAverage);
            StringBuilder builder = new StringBuilder();
            foreach (SchoolClassControlViewModel item in m_openedSchoolClasses)
            {
                builder.Append(item.ViewModel.Path);
                builder.Append(";");
            }
            builder.Remove(builder.Length - 1, 1); // remove last ;
            key.SetValue(GlobalConfig.RegistryValueNameLastOpened, builder.ToString());
            key.Close();
        }
    }
}
