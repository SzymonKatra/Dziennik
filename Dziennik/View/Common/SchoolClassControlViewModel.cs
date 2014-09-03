using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using Dziennik.Model;
using Dziennik.Controls;
using System.Windows;

namespace Dziennik.View
{
    public sealed class SchoolClassControlViewModel : ObservableObject
    {
        public SchoolClassControlViewModel()
            : this(new DatabaseMain())
        {
        }
        public SchoolClassControlViewModel(DatabaseMain database)
        {
            m_addMarkCommand = new RelayCommand<string>(AddMark);
            m_editMarkCommand = new RelayCommand<string>(EditMark);
            m_editEndingMarkCommand = new RelayCommand<string>(EditEndingMark);
            m_autoSaveCommand = new RelayCommand(AutoSave);
            m_saveCommand = new RelayCommand(Save);
            m_realizeSubjectCommand = new RelayCommand(RealizeSubject, CanRealizeSubject);
            m_editRealizedSubjectCommand = new RelayCommand(EditRealizedSubject);
            m_showOverdueSubjectsCommand = new RelayCommand(ShowOverdueSubjects, CanShowOverdueSubjects);
            m_putAllEndingMarksCommand = new RelayCommand<string>(PutAllEndingMarks);
            m_putNotAllEndingMarksCommand = new RelayCommand<string>(PutNotAllEndingMarks);
            m_cancelAllEndingMarksCommand = new RelayCommand<string>(CancelAllEndingMarks);
            m_addMarksSetCommand = new RelayCommand<string>(AddMarksSet);
            m_refreshStatisticsCommand = new RelayCommand(RefreshStatistics);

            m_database = database;
        }

        private DatabaseMain m_database;
        public DatabaseMain Database
        {
            get { return m_database; }
        }

        public SchoolClassViewModel ViewModel
        {
            get { return m_database.ViewModel; }
            set { m_database.ViewModel = value; RaisePropertyChanged("ViewModel"); }
        }

        private SchoolGroupViewModel m_selectedGroup;
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set { m_selectedGroup = value; RaisePropertyChanged("SelectedGroup"); m_realizeSubjectCommand.RaiseCanExecuteChanged(); }
        }

        private StudentInGroupViewModel m_selectedStudent;
        public StudentInGroupViewModel SelectedStudent
        {
            get { return m_selectedStudent; }
            set { m_selectedStudent = value; RaisePropertyChanged("SelectedStudent"); }
        }

        private MarkViewModel m_selectedMark;
        public MarkViewModel SelectedMark
        {
            get { return m_selectedMark; }
            set { m_selectedMark = value; RaisePropertyChanged("SelectedMark"); }
        }

        private RealizedSubjectViewModel m_selectedSubject;
        public RealizedSubjectViewModel SelectedSubject
        {
            get { return m_selectedSubject; }
            set { m_selectedSubject = value; RaisePropertyChanged("SelectedSubject"); }
        }

        private RelayCommand<string> m_addMarkCommand;
        public ICommand AddMarkCommand
        {
            get { return m_addMarkCommand; }
        }

        private RelayCommand<string> m_editMarkCommand;
        public ICommand EditMarkCommand
        {
            get { return m_editMarkCommand; }
        }

        private RelayCommand<string> m_editEndingMarkCommand;
        public ICommand EditEndingMarkCommand
        {
            get { return m_editEndingMarkCommand; }
        }

        private RelayCommand m_autoSaveCommand;
        public ICommand AutoSaveCommand
        {
            get { return m_autoSaveCommand; }
        }

        private RelayCommand m_saveCommand;
        public ICommand SaveCommand
        {
            get { return m_saveCommand; }
        }

        private RelayCommand m_realizeSubjectCommand;
        public ICommand RealizeSubjectCommand
        {
            get { return m_realizeSubjectCommand; }
        }

        private RelayCommand m_editRealizedSubjectCommand;
        public ICommand EditRealizedSubjectCommand
        {
            get { return m_editRealizedSubjectCommand; }
        }

        private RelayCommand m_showOverdueSubjectsCommand;
        public ICommand ShowOverdueSubjectsCommand
        {
            get { return m_showOverdueSubjectsCommand; }
        }

        private RelayCommand<string> m_putAllEndingMarksCommand;
        public ICommand PutAllEndingMarksCommand
        {
            get { return m_putAllEndingMarksCommand; }
        }
        private RelayCommand<string> m_putNotAllEndingMarksCommand;
        public ICommand PutNotAllEndingMarksCommand
        {
            get { return m_putNotAllEndingMarksCommand; }
        }
        private RelayCommand<string> m_cancelAllEndingMarksCommand;
        public ICommand CancelAllEndingMarksCommand
        {
            get { return m_cancelAllEndingMarksCommand; }
        }
        private RelayCommand<string> m_addMarksSetCommand;
        public RelayCommand<string> AddMarksSetCommand
        {
            get { return m_addMarksSetCommand; }
        }
        private RelayCommand m_refreshStatisticsCommand;
        public ICommand RefreshStatisticsCommand
        {
            get { return m_refreshStatisticsCommand; }
        }

        private void AddMark(string param)
        {
            SemesterViewModel semester = (param == "first" ? m_selectedStudent.FirstSemester : m_selectedStudent.SecondSemester);
            DateTime dStart, dEnd;
            GetMarkParams(param, out dStart, out dEnd);

            MarkViewModel mark = new MarkViewModel();
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(mark, dStart, dEnd, m_selectedStudent, true);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Ok)
            {
                semester.Marks.Add(mark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void EditMark(string param)
        {
            SemesterViewModel semester = (param == "first" ? m_selectedStudent.FirstSemester : m_selectedStudent.SecondSemester);
            DateTime dStart, dEnd;
            GetMarkParams(param, out dStart, out dEnd);

            m_selectedMark.PushCopy();
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(m_selectedMark, dStart, dEnd, m_selectedStudent);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.RemoveMark)
            {
                m_selectedMark.PopCopy(WorkingCopyResult.Ok);
                semester.Marks.Remove(m_selectedMark);
            }
            else if(dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Ok)
            {
                m_selectedMark.PopCopy(WorkingCopyResult.Ok);
            }
            else if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Cancel)
            {
                m_selectedMark.PopCopy(WorkingCopyResult.Cancel);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void GetMarkParams(string param, out DateTime dateStart, out DateTime dateEnd)
        {
            dateStart = (param == "first" ? m_selectedGroup.OwnerClass.Calendar.YearBeginning : m_selectedGroup.OwnerClass.Calendar.SemesterSeparator);
            dateEnd = (param == "first" ? m_selectedGroup.OwnerClass.Calendar.SemesterSeparator.AddDays(-1) : m_selectedGroup.OwnerClass.Calendar.YearEnding);
        }
        private void EditEndingMark(string param)
        {
            decimal initialMark;
            decimal averageMark;

            if (param == "half")
            {
                initialMark = m_selectedStudent.HalfEndingMark;
                averageMark = m_selectedStudent.FirstSemester.AverageMark;
            }
            else
            {
                initialMark = m_selectedStudent.YearEndingMark;
                averageMark = m_selectedStudent.AverageMarkAll;
            }

            EditEndingMarkViewModel dialogViewModel = new EditEndingMarkViewModel(initialMark, averageMark, (param == "half" ? EditEndingMarkViewModel.EndingMarkType.Half : EditEndingMarkViewModel.EndingMarkType.Year));
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);

            if (dialogViewModel.Result == EditEndingMarkViewModel.EditEndingMarkResult.Ok)
            {
                if (param == "half")
                {
                    m_selectedStudent.HalfEndingMark = dialogViewModel.Mark;
                }
                else
                {
                    m_selectedStudent.YearEndingMark = dialogViewModel.Mark;
                }
            }
            if (dialogViewModel.Result != EditEndingMarkViewModel.EditEndingMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void AutoSave(object param)
        {
            if (GlobalConfig.Notifier.AutoSave) m_saveCommand.Execute(null);
        }
        private void Save(object e)
        {
            if (e is NoActionDialogParameter)
            {
                m_database.Save();
            }
            else
            {
                ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
                {
                    m_database.Save();
                }
                , null, "Zapisywanie...");
                GlobalConfig.Dialogs.ShowDialog((e == null ? GlobalConfig.Main : e), dialogViewModel);
            }
        }
        private void RealizeSubject(object e)
        {
            RealizeSubjectViewModel dialogViewModel = new RealizeSubjectViewModel(null, m_selectedGroup.Students, GetAvailableSubjects(m_selectedGroup), m_selectedGroup.OwnerClass.Calendar, true);
            if (e is DateTime) dialogViewModel.RealizeDate = (DateTime)e;
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == RealizeSubjectViewModel.RealizeSubjectResult.Ok)
            {
                m_selectedGroup.RealizedSubjects.Add(dialogViewModel.RealizedSubject);
            }
            if (dialogViewModel.Result != RealizeSubjectViewModel.RealizeSubjectResult.Cancel)
            {
                SortSelectedGroupRealizedSubjects();
                m_autoSaveCommand.Execute(null);
            }
        }
        private bool CanRealizeSubject(object e)
        {
            return m_selectedGroup != null;
        }
        private void EditRealizedSubject(object e)
        {
            RealizeSubjectViewModel dialogViewModel = new RealizeSubjectViewModel(m_selectedSubject, m_selectedGroup.Students, GetAvailableSubjects(m_selectedGroup), m_selectedGroup.OwnerClass.Calendar);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == RealizeSubjectViewModel.RealizeSubjectResult.RemoveSubject)
            {
                m_selectedGroup.RealizedSubjects.Remove(m_selectedSubject);
                m_selectedSubject = null;
            }
            if (dialogViewModel.Result != RealizeSubjectViewModel.RealizeSubjectResult.Cancel)
            {
                SortSelectedGroupRealizedSubjects();
                m_autoSaveCommand.Execute(null);
            }
        }
        private void ShowOverdueSubjects(object param)
        {
            OverdueSubjectsListViewModel dialogViewModel = new OverdueSubjectsListViewModel(m_selectedGroup.OverdueSubjects);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == OverdueSubjectsListViewModel.OverdueSubjectsListResult.RealizeSubject)
            {
                m_realizeSubjectCommand.Execute(dialogViewModel.SelectedSubject);
            }
        }
        private bool CanShowOverdueSubjects(object e)
        {
            return m_selectedGroup != null;
        }
        private IEnumerable<GlobalSubjectViewModel> GetAvailableSubjects(SchoolGroupViewModel group)
        {
            List<GlobalSubjectViewModel> available = new List<GlobalSubjectViewModel>();
            foreach (GlobalSubjectViewModel subject in group.GlobalSubjects)
            {
                if (group.RealizedSubjects.FirstOrDefault(x => x.GlobalSubject == subject) == null) available.Add(subject);
            }
            return available;
        }
        private void PutAllEndingMarks(string e)
        {
            if (!MessageBoxContinue()) return;

            int completedCount = 0;
            foreach (StudentInGroupViewModel student in m_selectedGroup.Students)
            {
                if (student.IsRemoved) continue;
                decimal average = (e == "half" ? student.FirstSemester.AverageMark : student.AverageMarkAll);
                decimal endingMark = SemesterViewModel.ProposeMark(average);
                if (endingMark == 0M) continue;
                if (e == "half")
                {
                    student.HalfEndingMark = endingMark;
                }
                else
                {
                    student.YearEndingMark = endingMark;
                }

                completedCount++;
            }

            MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(GlobalConfig.Main), string.Format("Wystawiono {0} ocen", completedCount), "Dziennik", MessageBoxSuperPredefinedButtons.OK);
        }
        private void PutNotAllEndingMarks(string e)
        {
            if (!MessageBoxContinue()) return;

            int completedCount = 0;
            foreach (StudentInGroupViewModel student in m_selectedGroup.Students)
            {
                if (student.IsRemoved) continue;
                decimal average = (e == "half" ? student.FirstSemester.AverageMark : student.AverageMarkAll);
                decimal endingMark = SemesterViewModel.ProposeMark(average);
                if (endingMark == 0M) continue;
                if (e == "half")
                {
                    if (student.HalfEndingMark != 0M) continue;
                    student.HalfEndingMark = endingMark;
                }
                else
                {
                    if (student.YearEndingMark != 0M) continue;
                    student.YearEndingMark = endingMark;
                }

                completedCount++;
            }

            MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(GlobalConfig.Main), string.Format("Wystawiono {0} ocen", completedCount), "Dziennik", MessageBoxSuperPredefinedButtons.OK);
        }
        private void CancelAllEndingMarks(string e)
        {
            if (!MessageBoxContinue()) return;

            int completedCount = 0;
            foreach (StudentInGroupViewModel student in m_selectedGroup.Students)
            {
                if (student.IsRemoved) continue;
                if (e == "half")
                {
                    if (student.HalfEndingMark == 0M) continue;
                    student.HalfEndingMark = 0M;
                }
                else
                {
                    if (student.YearEndingMark == 0M) continue;
                    student.YearEndingMark = 0M;
                }

                completedCount++;
            }

            MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(GlobalConfig.Main), string.Format("Anulowano {0} ocen", completedCount), "Dziennik", MessageBoxSuperPredefinedButtons.OK);
        }
        private void AddMarksSet(string param)
        {
            AddMarksSetViewModel.SemesterType semester = (param == "first" ? AddMarksSetViewModel.SemesterType.First : AddMarksSetViewModel.SemesterType.Second);
            DateTime dStart, dEnd;
            GetMarkParams(param, out dStart, out dEnd);

            AddMarksSetViewModel dialogViewModel = new AddMarksSetViewModel(m_selectedGroup.Students, semester, dStart, dEnd);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result != AddMarksSetViewModel.AddMarksSetResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void RefreshStatistics(object e)
        {
            m_selectedGroup.Statistics.Refresh();
        }
        private void SortSelectedGroupRealizedSubjects()
        {
            m_selectedGroup.RealizedSubjects.Sort((x, y) => { return x.RealizedDate.CompareTo(y.RealizedDate); });
        }

        private bool MessageBoxContinue()
        {
            if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(GlobalConfig.Main), GlobalConfig.GetStringResource("lang_DoYouWantToContinue"), "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return false;
            return true;
        }
    }
}
