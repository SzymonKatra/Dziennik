using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class EditGlobalSubjectViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditGlobalSubjectResult
        {
            Ok,
            Cancel,
            Remove,
        }

        public EditGlobalSubjectViewModel(GlobalSubjectViewModel subject, IEnumerable<GlobalSubjectViewModel> existingSubjects, int minNumber, int maxNumber, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeSubjectCommand = new RelayCommand(RemoveSubject, CanRemoveSubject);

            m_subject = subject;
            m_existingSubjects = existingSubjects;

            //m_number = m_firstNumber = (isAddingMode ? GlobalSubjectsListViewModel.GetNextSubjectNumber(existingSubjects) : subject.Number);
            //m_name = subject.Name;
            m_minNumber = minNumber;
            m_maxNumber = maxNumber;

            m_isAddingMode = isAddingMode;
            if (isAddingMode)
            {
                m_subject.Number = GlobalSubjectsListViewModel.GetNextSubjectNumber(existingSubjects);
            }

            m_oldNumber = m_subject.Number;
            m_number = m_subject.Number;
            m_numberInput = m_subject.Number.ToString();
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

        private int m_oldNumber;
        private int m_minNumber;
        private int m_maxNumber;

        private int m_number;
        private bool m_numberInputValid = false;
        private string m_numberInput;
        public string NumberInput
        {
            get { return m_numberInput; }
            set { m_numberInput = value; RaisePropertyChanged("NumberInput"); }
        }

        private GlobalSubjectViewModel m_subject;

        public GlobalSubjectViewModel Subject
        {
            get { return m_subject; }
        }

        private IEnumerable<GlobalSubjectViewModel> m_existingSubjects;

        private bool m_isAddingMode = false;

        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
        }

        //private int m_firstNumber;

        private void Ok(object e)
        {
            m_subject.Number = m_number;

            m_result = EditGlobalSubjectResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_numberInputValid;
        }

        private void Cancel(object e)
        {
            m_result = EditGlobalSubjectResult.Cancel;
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void RemoveSubject(object e)
        {
            if(GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_DoYouWantToRemoveSubject"), MessageBoxSuperPredefinedButtons.OKCancel) != MessageBoxSuperButton.OK)
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

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "NumberInput": return ValidateNameInput();
                }

                return string.Empty;
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }

        private string ValidateNameInput()
        {
            m_numberInputValid = false;

            int result;
            if (!int.TryParse(m_numberInput, out result))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_InvalidValue");
            }

            if (!(!m_isAddingMode && result == m_oldNumber))
            {
                if (result < m_minNumber || result > m_maxNumber)
                {
                    m_okCommand.RaiseCanExecuteChanged();
                    return string.Format(GlobalConfig.GetStringResource("lang_MustBeInRange"), m_minNumber, m_maxNumber);
                }
            }

            m_number = result;

            m_numberInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
