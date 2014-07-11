using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.ComponentModel;
using System.Diagnostics;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class AddGroupViewModel : ObservableObject, IDataErrorInfo
    {
        public AddGroupViewModel(ObservableCollection<GlobalStudentViewModel> globalStudentCollection)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_selectStudentsCommand = new RelayCommand(SelectStudents);

            m_globalStudentCollection = globalStudentCollection;
        }

        private SchoolGroupViewModel m_result = null;
        public SchoolGroupViewModel Result
        {
            get { return m_result; }
        }

        private ObservableCollection<GlobalStudentViewModel> m_globalStudentCollection;

        private bool m_nameValid = false;
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; RaisePropertyChanged("Name"); }
        }

        private List<int> m_selectedStudents = new List<int>();
        private bool m_selectedStudentsInputValid = false;
        private string m_selectedStudentsInput;
        public string SelectedStudentsInput
        {
            get { return m_selectedStudentsInput; }
            set { m_selectedStudentsInput = value; RaisePropertyChanged("SelectedStudentsInput"); }
        }

        private bool m_renumberFromOne = true;
        public bool RenumberFromOne
        {
            get { return m_renumberFromOne; }
            set { m_renumberFromOne = value; RaisePropertyChanged("RenumberFromOne"); }
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

        private RelayCommand m_selectStudentsCommand;
        public ICommand SelectStudentsCommand
        {
            get { return m_selectStudentsCommand; }
        }

        private void Ok(object param)
        {
            if (m_selectedStudents.Count <= 0)
            {
                if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Uwaga! Nie dodano żadnego ucznia do grupy." + Environment.NewLine + "Czy chcesz kontynuować?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
            }

            if (!m_renumberFromOne) m_selectedStudents.Sort();

            int index = 1;

            m_result = new SchoolGroupViewModel();

            m_result.Name = m_name;
            foreach (int selStudent in m_selectedStudents)
            {
                StudentInGroupViewModel studentInGroup = new StudentInGroupViewModel();
                studentInGroup.GlobalStudent = m_globalStudentCollection.First(x => x.Number == selStudent);
                studentInGroup.Number = (m_renumberFromOne ? index++ : selStudent);

                m_result.Students.Add(studentInGroup);
            }

            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_nameValid && m_selectedStudentsInputValid;
        }
        private void Cancel(object param)
        {
            m_result = null;

            GlobalConfig.Dialogs.Close(this);
        }
        private void SelectStudents(object param)
        {
            SelectStudentsViewModel dialogViewModel = new SelectStudentsViewModel(m_globalStudentCollection, m_selectedStudents);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result)
            {
                SelectedStudentsInput = dialogViewModel.ResultSelectionString;
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Name": return ValidateName();
                    case "SelectedStudentsInput": return ValidateSelectedStudentsInput();
                }

                return string.Empty;
            }
        }

        public string ValidateName()
        {
            m_nameValid = false;

            if (string.IsNullOrWhiteSpace(m_name))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return "Wprowadź nazwę grupy";
            }

            m_nameValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
        public string ValidateSelectedStudentsInput()
        {
            m_selectedStudentsInputValid = false;

            string error;
            List<int> tempColl = SelectStudentsViewModel.ParseSelection(m_selectedStudentsInput, out error);
            if (tempColl == null)
            {
                m_okCommand.RaiseCanExecuteChanged();
                return error;
            }
            m_selectedStudents = tempColl;

            foreach (int selStudent in m_selectedStudents)
            {
                if (m_globalStudentCollection.FirstOrDefault((gs) => { return gs.Number == selStudent; }) == null)
                {
                    m_okCommand.RaiseCanExecuteChanged();
                    return string.Format("Ucznia o numerze {0} nie ma w bazie", selStudent);
                }
            }

            m_selectedStudentsInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
       
    }
}
