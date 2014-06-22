using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.ComponentModel;
using Dziennik.CommandUtils;
using System.Windows.Input;

namespace Dziennik.View
{
    public class EditStudentViewModel : ObservableObject, IDataErrorInfo
    {
        public EditStudentViewModel(StudentViewModel student)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);

            m_student = student;

            m_id = student.Id;
            m_name = student.Name;
            m_surname = student.Surname;
            m_email = student.Email;

            m_idInput = m_id.ToString();
        }

        private StudentViewModel m_student;

        private bool m_result;
        public bool Result
        {
            get { return m_result; }
        }

        private int m_id;
        private bool m_idInputValid = false;
        private string m_idInput;
        public string IdInput
        {
            get { return m_idInput; }
            set { m_idInput = value; OnPropertyChanged("IdInput"); }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
        }

        private string m_surname;
        public string Surname
        {
            get { return m_surname; }
            set { m_surname = value; OnPropertyChanged("Surname"); }
        }

        private string m_email;
        public string Email
        {
            get { return m_email; }
            set { m_email = value; OnPropertyChanged("Email"); }
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
            m_student.Id = m_id;
            m_student.Name = m_name;
            m_student.Surname = m_surname;
            m_student.Email = m_email;

            m_result = true;
            GlobalConfig.Dialogs.CloseDialog(this);
        }
        private bool CanOk(object e)
        {
            return m_idInputValid;
        }
        private void Cancel(object e)
        {
            m_result = false;
            GlobalConfig.Dialogs.CloseDialog(this);
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
                    case "IdInput": return ValidateIdInput();
                }

                return string.Empty;
            }
        }

        public string ValidateIdInput()
        {
            int result;
            if (!int.TryParse(m_idInput, out result))
            {
                m_idInputValid = false;
                m_okCommand.RaiseCanExecuteChanged();
                return "Wprowadź poprawny numer";
            }

            if (result <= 0)
            {
                m_idInputValid = false;
                m_okCommand.RaiseCanExecuteChanged();
                return "Numer musi być większy od 0";
            }

            m_id = result;

            m_idInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
    }
}
