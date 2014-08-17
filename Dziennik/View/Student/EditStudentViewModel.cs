using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.ComponentModel;
using Dziennik.CommandUtils;
using System.Windows.Input;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class EditStudentViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditStudentResult
        {
            Ok,
            Cancel,
            RemoveStudentCreateHole,
            RemoveStudentCompletly,
        }

        public EditStudentViewModel(GlobalStudentViewModel student)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeStudentCommand = new RelayCommand(RemoveStudent, CanRemoveStudent);

            m_student = student;

            m_numberInput = m_student.Number.ToString();
        }

        private GlobalStudentViewModel m_student;
        public GlobalStudentViewModel Student
        {
            get { return m_student; }
        }

        private EditStudentResult m_result = EditStudentResult.Cancel;
        public EditStudentResult Result
        {
            get { return m_result; }
        }

        private bool m_isAddingMode = false;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
            set { m_isAddingMode = value; RaisePropertyChanged("IsAddingMode"); m_removeStudentCommand.RaiseCanExecuteChanged(); }
        }

        private bool m_numberInputValid = false;
        private string m_numberInput;
        public string NumberInput
        {
            get { return m_numberInput; }
            set { m_numberInput = value; RaisePropertyChanged("IdInput"); }
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

        private RelayCommand m_removeStudentCommand;
        public ICommand RemoveStudentCommand
        {
            get { return m_removeStudentCommand; }
        }

        private void Ok(object e)
        {
            m_result = EditStudentResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_numberInputValid;
        }
        private void Cancel(object e)
        {
            m_result = EditStudentResult.Cancel;
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void RemoveStudent(object e)
        {
            if(MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this),"Czy na pewno chcesz usunąć ucznia?"+Environment.NewLine+"Na liście powstanie luka","Dziennik",MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes)
            {
                return;
            }

            m_result = EditStudentResult.RemoveStudentCreateHole;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveStudent(object e)
        {
            return !m_isAddingMode;
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "NumberInput": return ValidateNumberInput();
                }

                return string.Empty;
            }
        }

        private string ValidateNumberInput()
        {
            int result;
            if (!int.TryParse(m_numberInput, out result))
            {
                m_numberInputValid = false;
                m_okCommand.RaiseCanExecuteChanged();
                return "Wprowadź poprawny numer";
            }

            if (result <= 0)
            {
                m_numberInputValid = false;
                m_okCommand.RaiseCanExecuteChanged();
                return "Numer musi być większy od 0";
            }

            m_student.Number = result;

            m_numberInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
    }
}
