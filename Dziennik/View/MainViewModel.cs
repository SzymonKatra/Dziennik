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

        private bool m_databasePathChanged = true;

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
            if (e.PropertyName == "DatabasePath") m_databasePathChanged = true;
        }

        private void Save(object param)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                GlobalConfig.Notifier.SaveRegistry();
            }
            , null, "Zapisywanie ustawień do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            ActionDialogViewModel dialogViewModel2 = new ActionDialogViewModel((d, p) =>
            {
                //GlobalConfig.Database.SaveChanges();
            }
            , null, "Zapisywanie bazy danych...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel2);
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
            if (m_databasePathChanged) ReloadSchoolClasses();
        }

        private void ReloadSchoolClasses()
        {
            ActionDialogViewModel saveDialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                //if (GlobalConfig.Database != null) m_saveCommand.Execute(null);
                m_openedSchoolClasses.Clear();

                //GlobalConfig.InitializeDatabase();
            }
           , null, "Odczytywanie bazy danch...");
            GlobalConfig.Dialogs.ShowDialog(this, saveDialogViewModel);

            //foreach (var item in GlobalConfig.Database.SchoolClasses) m_openedSchoolClasses.Add(new SchoolClassControlViewModel(new SchoolClassViewModel(item)));
            m_databasePathChanged = false;
        }
    }
}
