using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class EditGlobalSubjectViewModel : ObservableObject
    {
        public enum EditGlobalSubjectResult
        {
            Ok,
            Cancel,
            Remove,
        }

        public EditGlobalSubjectViewModel(GlobalSubjectViewModel subject, IEnumerable<GlobalSubjectViewModel> existingSubjects, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeSubjectCommand = new RelayCommand(RemoveSubject, CanRemoveSubject);

            m_subject = subject;
            m_existingSubjects = existingSubjects;
            m_existingCategories = GlobalSubjectsListViewModel.GetExistingCategories(existingSubjects);

            m_number = m_firstNumber = (isAddingMode ? GlobalSubjectsListViewModel.GetNextSubjectNumber(existingSubjects, subject.Category) : subject.Number);
            m_category = m_firstCategory = subject.Category;
            m_name = subject.Name;

            m_isAddingMode = isAddingMode;
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
        private RelayCommand m_removeSubjectCommand;
        public ICommand RemoveSubjectCommand
        {
            get { return m_removeSubjectCommand; }
        }

        private EditGlobalSubjectResult m_result = EditGlobalSubjectResult.Cancel;
        public EditGlobalSubjectResult Result
        {
            get { return m_result; }
        }

        private GlobalSubjectViewModel m_subject;
        private IEnumerable<GlobalSubjectViewModel> m_existingSubjects;
        private IEnumerable<string> m_existingCategories;

        private bool m_isAddingMode = false;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
        }

        private int m_firstNumber;
        private int m_number;
        public int Number
        {
            get { return m_number; }
            set { m_number = value; RaisePropertyChanged("Number"); }
        }

        private string m_firstCategory;
        private string m_category;
        public string Category
        {
            get { return m_category; }
            set
            {
                m_category = value;
                if (m_isAddingMode)
                {
                    Number = GlobalSubjectsListViewModel.GetNextSubjectNumber(m_existingSubjects, m_category);
                }
                else
                {
                    if (GlobalSubjectsListViewModel.CheckCategory(m_category, m_firstCategory))
                    {
                        Number = m_firstNumber;
                    }
                    else
                    {
                        Number = GlobalSubjectsListViewModel.GetNextSubjectNumber(m_existingSubjects, m_category);
                    }
                }
                RaisePropertyChanged("Category");
                RaisePropertyChanged("Number");
            }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; RaisePropertyChanged("Name"); }
        }

        private void Ok(object e)
        {
            m_subject.Number = GlobalSubjectsListViewModel.GetNextSubjectNumber(m_existingSubjects, m_category);
            m_subject.Category = m_category;
            m_subject.Name = m_name;
            m_result = EditGlobalSubjectResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object e)
        {
            m_result = EditGlobalSubjectResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void RemoveSubject(object e)
        {
            if(MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Czy na pewno chcesz usunąć temat?", "Dziennik", MessageBoxSuperPredefinedButtons.OKCancel) != MessageBoxSuperButton.OK)
            {
                return;
            }

            m_result = EditGlobalSubjectResult.Remove;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveSubject(object e)
        {
            return !m_isAddingMode;
        }
    }
}
