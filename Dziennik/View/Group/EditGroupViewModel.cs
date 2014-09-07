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
            m_restoreStudentsCommand = new RelayCommand(RestoreStudents);
            m_removeGroupCommand = new RelayCommand(RemoveGroup);
            m_showGlobalSubjectsListCommand = new RelayCommand(ShowGlobalSubjectsList);

            m_schoolGroup = schoolGroup;
            m_globalStudents = globalStudents;

            m_nameInput = m_schoolGroup.Name;
        }

        private EditGroupResult m_result = EditGroupResult.Cancel;
        public EditGroupResult Result
        {
            get { return m_result; }
        }

        private SchoolGroupViewModel m_schoolGroup;
        public SchoolGroupViewModel SchoolGroup
        {
            get { return m_schoolGroup; }
        }
        private ObservableCollection<GlobalStudentViewModel> m_globalStudents;

        private bool m_nameInputValid = false;
        private string m_nameInput;
        public string NameInput
        {
            get { return m_nameInput; }
            set { m_nameInput = value; RaisePropertyChanged("NameInput"); }
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

        private RelayCommand m_restoreStudentsCommand;
        public ICommand RestoreStudentsCommand
        {
            get { return m_restoreStudentsCommand; }
        }

        private RelayCommand m_removeGroupCommand;
        public ICommand RemoveGroupCommand
        {
            get { return m_removeGroupCommand; }
        }

        private RelayCommand m_showGlobalSubjectsListCommand;
        public ICommand ShowGlobalSubjectsListCommand
        {
            get { return m_showGlobalSubjectsListCommand; }
        }

        private void Ok(object param)
        {
            m_schoolGroup.Name = m_nameInput;

            m_result = EditGroupResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_nameInputValid;
        }
        private void Cancel(object e)
        {
            m_result = EditGroupResult.Cancel;

            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void AddStudent(object param)
        {
            ObservableCollection<GlobalStudentViewModel> canBeAdded = new ObservableCollection<GlobalStudentViewModel>();
            foreach(GlobalStudentViewModel globalStudent in m_globalStudents)
            {
                var result = m_schoolGroup.Students.FirstOrDefault((x) => { return x.GlobalStudent.Model.Id == globalStudent.Model.Id; });
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

                int selectedGlobalNumber = dialogViewModel.ResultSelection[0];

                StudentInGroupViewModel studentInGroup = new StudentInGroupViewModel();
                studentInGroup.GlobalStudent = m_globalStudents.First(x => x.Number == selectedGlobalNumber);
                studentInGroup.Number = (m_schoolGroup.Students.Count <= 0 ? 1 : m_schoolGroup.Students[m_schoolGroup.Students.Count - 1].Number + 1);
                m_schoolGroup.Students.Add(studentInGroup);
            }
        }
        private void RemoveStudents(object param)
        {
            var toSelect = m_schoolGroup.Students.Where(x => !x.IsRemoved);
            SelectStudentsViewModel dialogViewModel = new SelectStudentsViewModel(toSelect, null);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            List<int> selectionResult = dialogViewModel.ResultSelection;
            if (dialogViewModel.Result && selectionResult.Count > 0)
            {
                if (GlobalConfig.MessageBox(this,
                                           string.Format("{1}{0}{2}{0}{3}",
                                           Environment.NewLine,
                                           string.Format(GlobalConfig.GetStringResource("lang_SelectedStudentsToRemoveFormat"), selectionResult.Count),
                                           GlobalConfig.GetStringResource("lang_OnListWillBeGaps"),
                                           GlobalConfig.GetStringResource("lang_DoYouWantToContinue")),
                                           MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

                foreach(int selRes in selectionResult)
                {
                    StudentInGroupViewModel student = m_schoolGroup.Students.First((x) => { return x.Number == selRes; });
                    student.IsRemoved = true;
                    //student.GlobalStudent = null;
                    //student.FirstSemester.Marks.Clear();
                    //student.SecondSemester.Marks.Clear();
                }
            }
        }
        private void RestoreStudents(object param)
        {
            var toSelect = m_schoolGroup.Students.Where(x => x.IsRemoved);
            SelectStudentsViewModel dialogViewModel = new SelectStudentsViewModel(toSelect, null);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            List<int> selectionResult = dialogViewModel.ResultSelection;
            if (dialogViewModel.Result && selectionResult.Count > 0)
            {
                StringBuilder msgTextBuilder = new StringBuilder();
                msgTextBuilder.AppendLine(GlobalConfig.GetStringResource("lang_DoYouReallyWantToRestoreStudents"));

                List<StudentInGroupViewModel> toRestore = new List<StudentInGroupViewModel>();
                foreach (int selRes in selectionResult)
                {
                    var student = m_schoolGroup.Students.First(x => x.Number == selRes);
                    toRestore.Add(student);

                    msgTextBuilder.AppendLine(string.Format("{0}: {1} {2}", student.Number, student.GlobalStudent.Surname, student.GlobalStudent.Name));
                }

                if (GlobalConfig.MessageBox(this, msgTextBuilder.ToString(), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

                foreach (var student in toRestore)
                {
                    student.IsRemoved = false;
                }
            }
        }
        private void RemoveGroup(object param)
        {
            if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wszyskie dane zostaną stracone" + Environment.NewLine + "Czy napewno chcesz usunąć grupę?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

            m_result = EditGroupResult.RemoveGroup;
            GlobalConfig.Dialogs.Close(this);
        }
        private void ShowGlobalSubjectsList(object param)
        {
            GlobalSubjectsListViewModel dialogViewModel = new GlobalSubjectsListViewModel(m_schoolGroup.GlobalSubjects);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
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
                    case "NameInput": return ValidateNameInput();
                }

                return string.Empty;
            }
        }

        public string ValidateNameInput()
        {
            m_nameInputValid = false;

            if (string.IsNullOrWhiteSpace(m_nameInput))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_TypeGroupName");
            }

            m_nameInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
