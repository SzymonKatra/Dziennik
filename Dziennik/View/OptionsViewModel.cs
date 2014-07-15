using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public class OptionsViewModel : ObservableObject
    {
        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }
        private RelayCommand m_editClassCommand;
        public ICommand EditClassCommand
        {
            get { return m_editClassCommand; }
        }
        private RelayCommand m_addClassCommand;
        public ICommand AddClassCommand
        {
            get { return m_addClassCommand; }
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedSchoolClasses;
        public ObservableCollection<SchoolClassControlViewModel> OpenedSchoolClasses
        {
            get { return m_openedSchoolClasses; }
            set { m_openedSchoolClasses = value; RaisePropertyChanged("OpenedSchoolClasses"); }
        }

        private SchoolClassControlViewModel m_selectedClass;
        public SchoolClassControlViewModel SelectedClass
        {
            get { return m_selectedClass; }
            set { m_selectedClass = value; RaisePropertyChanged("SelectedClass"); m_editClassCommand.RaiseCanExecuteChanged(); }
        }

        public OptionsViewModel(ObservableCollection<SchoolClassControlViewModel> openedSchoolClasses)
        {
            m_closeCommand = new RelayCommand(Close);
            m_editClassCommand = new RelayCommand(EditClass, CanEditClass);
            m_addClassCommand = new RelayCommand(AddClass);

            m_openedSchoolClasses = openedSchoolClasses;
        }

        private void Close(object e)
        {
            if (e == null) // if window is closed by X icon, e will be not null
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void EditClass(object e)
        {
            SchoolClassControlViewModel tab = m_selectedClass;
            EditClassViewModel dialogViewModel = new EditClassViewModel(tab.ViewModel, tab.AutoSaveCommand);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.RemoveClass)
            {
                //GlobalConfig.Database.SchoolClasses.Remove(m_selectedClass.ViewModel.Model);
                m_selectedClass = null;
                m_openedSchoolClasses.Remove(tab);
                return;
            }
            if (dialogViewModel.Result != EditClassViewModel.EditClassResult.Cancel) m_selectedClass.AutoSaveCommand.Execute(this);
        }
        private bool CanEditClass(object e)
        {
            return m_selectedClass != null;
        }
        private void AddClass(object e)
        {
            SchoolClassViewModel schoolClass = new SchoolClassViewModel();
            EditClassViewModel dialogViewModel = new EditClassViewModel(schoolClass, new RelayCommand((x) => { }));
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.Ok)
            {
                //GlobalConfig.Database.SchoolClasses.Add(schoolClass.Model);
                SchoolClassControlViewModel tab = new SchoolClassControlViewModel(schoolClass);
                m_openedSchoolClasses.Add(tab);
                SelectedClass = tab;
            }
            if (dialogViewModel.Result != EditClassViewModel.EditClassResult.Cancel) m_selectedClass.AutoSaveCommand.Execute(this);
        }
    }
}
