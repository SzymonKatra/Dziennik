﻿using System;
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
            m_showGlobalSubjectsListCommand = new RelayCommand(ShowGlobalSubjectsList);

            m_schoolGroup = schoolGroup;
            m_globalStudents = globalStudents;

            m_name = schoolGroup.Name;
            m_schedule = new WeekScheduleViewModel();
            m_students = new WorkingCopyCollection<StudentInGroupViewModel>(schoolGroup.Students);
            m_globalSubjects = new WorkingCopyCollection<GlobalSubjectViewModel>(schoolGroup.GlobalSubjects);
            m_realizedSubjects = new WorkingCopyCollection<RealizedSubjectViewModel>(schoolGroup.RealizedSubjects);
            schoolGroup.Schedule.CopyDataTo(m_schedule);
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
            set { m_name = value; RaisePropertyChanged("Name"); }
        }

        private WorkingCopyCollection<StudentInGroupViewModel> m_students;
        public WorkingCopyCollection<StudentInGroupViewModel> Students
        {
            get { return m_students; }
        }

        private WorkingCopyCollection<GlobalSubjectViewModel> m_globalSubjects;
        public WorkingCopyCollection<GlobalSubjectViewModel> GlobalSubjects
        {
            get { return m_globalSubjects; }
        }

        private WorkingCopyCollection<RealizedSubjectViewModel> m_realizedSubjects;
        public WorkingCopyCollection<RealizedSubjectViewModel> RealizedSubjects
        {
            get { return m_realizedSubjects; }
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
            m_schoolGroup.Name = m_name;
            m_students.ApplyChangesToOriginalCollection();
            m_globalSubjects.ApplyChangesToOriginalCollection();
            m_realizedSubjects.ApplyChangesToOriginalCollection();
            m_schedule.CopyDataTo(m_schoolGroup.Schedule);

            m_result = EditGroupResult.Ok;
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
                var result = m_students.FirstOrDefault((x) => { return x.GlobalStudent.Model.Id == globalStudent.Model.Id; });
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
                studentInGroup.Number = (m_students.Count <= 0 ? 1 : m_students[m_students.Count - 1].Number + 1);
                m_students.Add(studentInGroup);
            }
        }
        private void RemoveStudents(object param)
        {
            SelectStudentsViewModel dialogViewModel = new SelectStudentsViewModel(m_students, null);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            List<int> selectionResult = dialogViewModel.ResultSelection;
            if (dialogViewModel.Result && selectionResult.Count > 0)
            {
                if (GlobalConfig.MessageBox(this,
                                           string.Format("{1}{0}{2}{0}{3}{0}{4}",
                                           Environment.NewLine,
                                           string.Format(GlobalConfig.GetStringResource("lang_SelectedStudentsToRemoveFormat"), selectionResult.Count),
                                           GlobalConfig.GetStringResource("lang_TheyMarksAreGoingToBeRemove"),
                                           GlobalConfig.GetStringResource("lang_OnListWillBeGaps"),
                                           GlobalConfig.GetStringResource("lang_DoYouWantToContinue")),
                                           MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

                foreach(int selRes in selectionResult)
                {
                    StudentInGroupViewModel student = m_students.First((x) => { return x.Number == selRes; });
                    student.GlobalStudent = null;
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
            GlobalSubjectsListViewModel dialogViewModel = new GlobalSubjectsListViewModel(m_globalSubjects);
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
                return GlobalConfig.GetStringResource("lang_TypeGroupName");
            }

            m_nameValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
