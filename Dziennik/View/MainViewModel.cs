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

        private bool m_autoSave = true;
        public bool AutoSave
        {
            get { return m_autoSave; }
            set { m_autoSave = value; OnPropertyChanged("AutoSave"); }
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

        public void Init()
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                LoadRegistry();
            }
            , null, "Odczytywanie z ustawień rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2) m_openFromPathCommand.Execute(args[1]);
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
                m_selectedClass.SaveCommand.Execute(null);
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
                    SchoolClassControlViewModel tab = new SchoolClassControlViewModel(this, schoolClass);
                    m_openedSchoolClasses.Add(tab);
                    SelectedClass = tab;
                }
            }
            , null, path, "Otwieranie");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);         
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
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.Ok) m_selectedClass.AutoSaveCommand.Execute(null);
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
            foreach (SchoolClassControlViewModel tab in m_openedSchoolClasses) PromptSave(tab);

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                SaveRegistry();
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
            if (m_autoSave)
            {
                param.SaveCommand.Execute(null);
            }
            else
            {
                if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Czy chcesz zapisać zmiany w klasie " + param.ViewModel.Name + "?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) == MessageBoxSuperButton.Yes)
                {
                    param.SaveCommand.Execute(null);
                }
            }
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
            object autoSaveReg = key.GetValue(GlobalConfig.RegistryValueNameAutoSave);
            object lastOpenedReg = key.GetValue(GlobalConfig.RegistryValueNameLastOpened);
            key.Close();

            if (showNameReg != null) ShowName= Ext.BoolParseOrDefault(showNameReg.ToString(), m_showName);
            if (showSurnameReg != null) ShowSurname= Ext.BoolParseOrDefault(showSurnameReg.ToString(), m_showSurname);
            if (showEmailReg != null) ShowEmail = Ext.BoolParseOrDefault(showEmailReg.ToString(), m_showEmail);
            if (showFirstMarksReg != null) ShowFirstMarks = Ext.BoolParseOrDefault(showFirstMarksReg.ToString(), m_showFirstMarks);
            if (showFirstAverageReg != null) ShowFirstAverage = Ext.BoolParseOrDefault(showFirstAverageReg.ToString(), m_showFirstAverage);
            if (showSecondMarksReg != null) ShowSecondMarks = Ext.BoolParseOrDefault(showSecondMarksReg.ToString(), m_showSecondMarks);
            if (showSecondAverageReg != null) ShowSecondAverage = Ext.BoolParseOrDefault(showSecondAverageReg.ToString(), m_showSecondAverage);
            if (showEndingAverageReg != null) ShowEndingAverage = Ext.BoolParseOrDefault(showEndingAverageReg.ToString(), m_showEndingAverage);
            if (autoSaveReg != null) AutoSave = Ext.BoolParseOrDefault(autoSaveReg.ToString(), m_autoSave);
            if (lastOpenedReg != null)
            {
                string lastOpened = lastOpenedReg.ToString();
                string[] tokens = lastOpened.Split(';');
                foreach (string file in tokens) if (!string.IsNullOrWhiteSpace(file)) m_openFromPathCommand.Execute(file);
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
            key.SetValue(GlobalConfig.RegistryValueNameAutoSave, m_autoSave);
            StringBuilder builder = new StringBuilder();
            foreach (SchoolClassControlViewModel item in m_openedSchoolClasses)
            {
                builder.Append(item.ViewModel.Path);
                builder.Append(";");
            }
            if (builder.Length >= 1) builder.Remove(builder.Length - 1, 1); // remove last ;
            key.SetValue(GlobalConfig.RegistryValueNameLastOpened, builder.ToString());
            key.Close();
        }
    }
}
