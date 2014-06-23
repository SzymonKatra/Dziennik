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

        public EditStudentViewModel(StudentViewModel student)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeStudentCommand = new RelayCommand(RemoveStudent, CanRemoveStudent);

            m_student = student;

            m_id = student.Id;
            m_name = student.Name;
            m_surname = student.Surname;
            m_email = student.Email;
            m_additionalInformation = student.AdditionalInformation;

            m_idInput = m_id.ToString();
        }

        private StudentViewModel m_student;

        private EditStudentResult m_result = EditStudentResult.Cancel;
        public EditStudentResult Result
        {
            get { return m_result; }
        }

        private bool m_isAddingMode = false;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
            set { m_isAddingMode = value; OnPropertyChanged("IsAddingMode"); m_removeStudentCommand.RaiseCanExecuteChanged(); }
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

        private string m_additionalInformation;
        public string AdditionalInformation
        {
            get { return m_additionalInformation; }
            set { m_additionalInformation = value; OnPropertyChanged("AdditionalInformation"); }
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
            //if(m_isAddingMode && m_student.Id!=m_id)
            //{
            //    if(MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this),"Nr ucznia został ręcznie zmieniony na inny i prawdopodobnie nie będzie zachowana kolejność"+Environment.NewLine+"Jeśli ten sam numer będzie posiadał inny uczeń to wszyscy znajdujący się poniżej uczniowie dostaną numer o 1 wyższy"+Environment.NewLine+"Czy chcesz kontynuować?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes)
            //    {
            //        return;
            //    }
            //}

            //m_student.Id = m_id;
            m_student.Name = m_name;
            m_student.Surname = m_surname;
            m_student.Email = m_email;
            m_student.AdditionalInformation = m_additionalInformation;

            m_result = EditStudentResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_idInputValid;
        }
        private void Cancel(object e)
        {
            m_result = EditStudentResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void RemoveStudent(object e)
        {
            if(MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this),"Czy na pewno chcesz usunąć ucznia?"+Environment.NewLine+"Na liście powstanie luka","Dziennik",MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes)
            {
                return;
            }

            m_student.Name = "----------";
            m_student.Surname = "----------";
            m_student.Email = "----------";
            m_student.FirstSemester.Marks.Clear();
            m_student.SecondSemester.Marks.Clear();

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
