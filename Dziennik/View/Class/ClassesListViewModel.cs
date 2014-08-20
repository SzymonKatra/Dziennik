using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public sealed class ClassesListViewModel : ObservableObject
    {
        public ClassesListViewModel(ObservableCollection<SchoolClassControlViewModel> openedClasses)
        {
            m_addClassCommand = new RelayCommand(AddClass);
            m_editClassCommand = new RelayCommand(EditClass);

            m_openedClasses = openedClasses;
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedClasses;
        public ObservableCollection<SchoolClassControlViewModel> OpenedClasses
        {
            get { return m_openedClasses; }
        }

        private SchoolClassControlViewModel m_selectedClass;
        public SchoolClassControlViewModel SelectedClass
        {
            get { return m_selectedClass; }
            set { m_selectedClass = value; RaisePropertyChanged("SelectedClass"); }
        }

        private RelayCommand m_addClassCommand;
        public ICommand AddClassCommand
        {
            get { return m_addClassCommand; }
        }

        private RelayCommand m_editClassCommand;
        public ICommand EditClassCommand
        {
            get { return m_editClassCommand; }
        }

        private void AddClass(object e)
        {
            SchoolClassViewModel schoolClass = new SchoolClassViewModel();
            EditClassViewModel dialogViewModel = new EditClassViewModel(schoolClass);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.Ok)
            {
                //GlobalConfig.Database.SchoolClasses.Add(schoolClass.Model);
                SchoolClassControlViewModel tab = new SchoolClassControlViewModel(new DatabaseMain());
                tab.Database.ViewModel = schoolClass;
                tab.Database.Path = dialogViewModel.Path;
                m_openedClasses.Add(tab);
                SelectedClass = tab;
            }
            if (dialogViewModel.Result != EditClassViewModel.EditClassResult.Cancel) m_selectedClass.AutoSaveCommand.Execute(this);
        }
        private void EditClass(object e)
        {
            SchoolClassControlViewModel tab = m_selectedClass;
            tab.ViewModel.PushCopy();
            EditClassViewModel dialogViewModel = new EditClassViewModel(tab.ViewModel);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.RemoveClass)
            {
                tab.ViewModel.PopCopy(WorkingCopyResult.Ok);
                //GlobalConfig.Database.SchoolClasses.Remove(m_selectedClass.ViewModel.Model);
                SelectedClass = null;
                m_openedClasses.Remove(tab);
                return;
            }
            else if (dialogViewModel.Result == EditClassViewModel.EditClassResult.Ok)
            {
                tab.ViewModel.PopCopy(WorkingCopyResult.Ok);
            }
            else if (dialogViewModel.Result == EditClassViewModel.EditClassResult.Cancel)
            {
                tab.ViewModel.PopCopy(WorkingCopyResult.Cancel);
            }

            if (dialogViewModel.Result != EditClassViewModel.EditClassResult.Cancel) m_selectedClass.AutoSaveCommand.Execute(this);
        }
    }
}
