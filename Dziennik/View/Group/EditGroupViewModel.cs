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

        public EditGroupViewModel(SchoolGroupViewModel schoolGroup, ObservableCollection<GlobalStudentViewModel> globalStudents, ICommand autoSaveCommand)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_addStudentCommand = new RelayCommand(AddStudent);
            m_removeStudentsCommand = new RelayCommand(RemoveStudents);
            m_removeGroupCommand = new RelayCommand(RemoveGroup);
            m_showGlobalSubjectsListCommand = new RelayCommand(ShowGlobalSubjectsList);

            m_autoSaveCommand = autoSaveCommand;

            m_schoolGroup = schoolGroup;
            m_globalStudents = globalStudents;

            m_name = schoolGroup.Name;
            m_schedule = new WeekScheduleViewModel();
            m_schedule.Monday = schoolGroup.Schedule.Monday;
            m_schedule.Tuesday = schoolGroup.Schedule.Tuesday;
            m_schedule.Wednesday = schoolGroup.Schedule.Wednesday;
            m_schedule.Thursday = schoolGroup.Schedule.Thursday;
            m_schedule.Friday= schoolGroup.Schedule.Friday;
        }

        private EditGroupResult m_result = EditGroupResult.Cancel;
        public EditGroupResult Result
        {
            get { return m_result; }
        }

        private ICommand m_autoSaveCommand;
        private SchoolGroupViewModel m_schoolGroup;
        private ObservableCollection<GlobalStudentViewModel> m_globalStudents;

        private bool m_nameValid = false;
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; RaisePropertyChanged("Name"); }
        }

        private WeekScheduleViewModel m_schedule;
        public WeekScheduleViewModel Schedule
        {
            get { return m_schedule; }
            set { m_schedule = value; RaisePropertyChanged("Schedule"); }
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

        private RelayCommand m_showGlobalSubjectsListCommand;
        public ICommand ShowGlobalSubjectsListCommand
        {
            get { return m_showGlobalSubjectsListCommand; }
        }

        private void Ok(object param)
        {
            m_result = EditGroupResult.Ok;
            m_schoolGroup.Name = m_name;
            m_schoolGroup.Schedule.Monday = m_schedule.Monday;
            m_schoolGroup.Schedule.Tuesday = m_schedule.Tuesday;
            m_schoolGroup.Schedule.Wednesday = m_schedule.Wednesday;
            m_schoolGroup.Schedule.Thursday = m_schedule.Thursday;
            m_schoolGroup.Schedule.Friday = m_schedule.Friday;

            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_nameValid;
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
                    StudentInGroupViewModel student = m_schoolGroup.Students.First((x) => { return x.Number == selRes; });
                    student.GlobalStudent = GlobalStudentViewModel.Dummy;
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
        private void ShowGlobalSubjectsList(object e)
        {
            GlobalSubjectsListViewModel dialogViewModel = new GlobalSubjectsListViewModel(m_schoolGroup.GlobalSubjects, m_autoSaveCommand);
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
