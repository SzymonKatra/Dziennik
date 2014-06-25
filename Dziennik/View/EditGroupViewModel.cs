using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dziennik.CommandUtils;
using System.ComponentModel;
using System.Windows.Input;
using Dziennik.ViewModel;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class EditGroupViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditGroupResult
        {
            Ok,
            Cancel,
            RemoveGroup,
        }

        public EditGroupViewModel(SchoolGroupViewModel schoolGroup, ObservableCollection<GlobalStudentViewModel> globalStudents)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_addStudentCommand = new RelayCommand(AddStudent);
            m_removeStudentsCommand = new RelayCommand(RemoveStudents);
            m_removeGroupCommand = new RelayCommand(RemoveGroup);

            m_schoolGroup = schoolGroup;
            m_globalStudents = globalStudents;

            m_name = schoolGroup.Name;
        }

        private EditGroupResult m_result = EditGroupResult.Cancel;
        public EditGroupResult Result
        {
            get { return m_result; }
        }

        private SchoolGroupViewModel m_schoolGroup;
        private ObservableCollection<GlobalStudentViewModel> m_globalStudents;

        private bool m_nameValid = false;
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
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

        private RelayCommand m_addStudentCommand;
        public ICommand AddStudentCommand
        {
            get { return m_addStudentCommand; }
        }

        private RelayCommand m_removeStudentsCommand;
        public ICommand RemoveStudentsCommand
        {
            get { return m_removeStudentsCommand; }
        }

        private RelayCommand m_removeGroupCommand;
        public ICommand RemoveGroupCommand
        {
            get { return m_removeGroupCommand; }
        }

        private void Ok(object param)
        {
            m_result = EditGroupResult.Ok;
            m_schoolGroup.Name = m_name;

            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_nameValid;
        }
        private void Cancel(object param)
        {
            m_result = EditGroupResult.Cancel;

            GlobalConfig.Dialogs.Close(this);
        }
        private void AddStudent(object param)
        {
            ObservableCollection<GlobalStudentViewModel> canBeAdded = new ObservableCollection<GlobalStudentViewModel>();
            foreach(GlobalStudentViewModel globalStudent in m_globalStudents)
            {
                var result = m_schoolGroup.Students.FirstOrDefault((x) => { return x.GlobalId == globalStudent.Id; });
                if (result == null) canBeAdded.Add(globalStudent);
            }

            SelectStudentsViewModel dialogViewModel = new SelectStudentsViewModel(canBeAdded, null);
            dialogViewModel.SingleSelection = true;
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result && dialogViewModel.ResultSelection.Count == 1)
            {
                if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this),
                                           "Uczeń zostanie dopisany na koniec listy i otrzyma numer o jeden wyższy od poprzedniego ucznia który znajduje się na liście" + Environment.NewLine + "Czy chcesz kontynuować?",
                                           "Dziennik",
                                           MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

                int selectedGlobalId = dialogViewModel.ResultSelection[0];

                StudentInGroupViewModel studentInGroup = new StudentInGroupViewModel();
                studentInGroup.GlobalId = selectedGlobalId;
                studentInGroup.Id = m_schoolGroup.Students[m_schoolGroup.Students.Count - 1].Id + 1;
                m_schoolGroup.Students.Add(studentInGroup);
            }
        }
        private void RemoveStudents(object param)
        {
            SelectStudentsViewModel dialogViewModel = new SelectStudentsViewModel(m_schoolGroup.Students, null);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            List<int> selectionResult = dialogViewModel.ResultSelection;
            if (dialogViewModel.Result && selectionResult.Count > 0)
            {
                if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this),
                                           "Zaznaczono " + selectionResult.Count + " uczniów do usunięcia." + Environment.NewLine + "Ich dotychczasowe oceny zostaną usunięte" + Environment.NewLine + "Na liście powstaną luki" + Environment.NewLine + "Czy chcesz kontynuować?",
                                           "Dziennik",
                                           MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

                foreach(int selRes in selectionResult)
                {
                    StudentInGroupViewModel student = m_schoolGroup.Students.First((x) => { return x.Id == selRes; });
                    student.GlobalId = -1;
                    student.FirstSemester.Marks.Clear();
                    student.SecondSemester.Marks.Clear();
                }
            }
        }
        private void RemoveGroup(object param)
        {
            if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wszyskie dane zostaną stracone" + Environment.NewLine + "Czy napewno chcesz usunąć grupę?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

            m_result = EditGroupResult.RemoveGroup;
            GlobalConfig.Dialogs.Close(this);
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
    }
}
