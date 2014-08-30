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
    public class SelectGroupViewModel : ObservableObject
    {
        public enum SelectGroupResult
        {
            Ok,
            Cancel,
        }

        public SelectGroupViewModel(ObservableCollection<SchoolClassControlViewModel> schoolClasses)
        {
            ObservableCollection<SchoolClassViewModel> validClasses = new ObservableCollection<SchoolClassViewModel>();
            foreach (var item in schoolClasses)
            {
                validClasses.Add(item.Database.ViewModel);
            }

            ConstructorImpl(validClasses);
        }
        public SelectGroupViewModel(ObservableCollection<SchoolClassViewModel> schoolClasses)
        {
            ConstructorImpl(schoolClasses);
        }
        private void ConstructorImpl(ObservableCollection<SchoolClassViewModel> schoolClasses)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);

            m_schoolClasses = schoolClasses;
        }

        private SelectGroupResult m_result = SelectGroupResult.Cancel;
        public SelectGroupResult Result
        {
            get { return m_result; }
        }

        private ObservableCollection<SchoolClassViewModel> m_schoolClasses;
        public ObservableCollection<SchoolClassViewModel> SchoolClasses
        {
            get { return m_schoolClasses; }
        }

        private SchoolGroupViewModel m_selectedGroup;
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set { m_selectedGroup = value; RaisePropertyChanged("SelectedGroup"); m_okCommand.RaiseCanExecuteChanged(); }
        }

        private RelayCommand m_okCommand;
        public ICommand OkCommand
        {
            get { return m_okCommand; }
        }
        private RelayCommand m_cancelCommand;
        public ICommand CancelCommand
        {
            get { return m_cancelCommand; }
        }

        private void Ok(object param)
        {
            m_result = SelectGroupResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_selectedGroup != null;
        }
        private void Cancel(object param)
        {
            m_result = SelectGroupResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
    }
}
