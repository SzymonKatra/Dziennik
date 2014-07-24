using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Windows.Input;

namespace Dziennik.View
{
    public sealed class SelectGlobalSubjectViewModel : ObservableObject
    {
        public enum SelectedGlobalSubjectResult
        {
            Ok,
            Cancel,
        }

        public class SubjectCategory
        {
            public string Name { get; set; }

            public SubjectCategory(string name)
            {
                Name = name;
            }
        }

        public SelectGlobalSubjectViewModel(ObservableCollection<GlobalSubjectViewModel> subjects)
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);

            m_subjects = subjects;
        }

        private SelectedGlobalSubjectResult m_result = SelectedGlobalSubjectResult.Cancel;
        public SelectedGlobalSubjectResult Result
        {
            get { return m_result; }
        }

        private ObservableCollection<GlobalSubjectViewModel> m_subjects;
        public ObservableCollection<GlobalSubjectViewModel> Subjects
        {
            get { return m_subjects; }
            set { m_subjects = value; RaisePropertyChanged("Subjects"); }
        }

        private GlobalSubjectViewModel m_selectedSubject;
        public GlobalSubjectViewModel SelectedSubject
        {
            get { return m_selectedSubject; }
            set { m_selectedSubject = value; RaisePropertyChanged("SelectedSubject"); }
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

        private void Ok(object e)
        {
            m_result = SelectedGlobalSubjectResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object e)
        {
            m_result = SelectedGlobalSubjectResult.Cancel;
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
    }
}
