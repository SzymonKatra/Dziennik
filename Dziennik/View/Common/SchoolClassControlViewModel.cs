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
            m_addMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(AddMark);
            m_editMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(EditMark);
            m_editEndingMarkCommand = new RelayCommand<string>(EditEndingMark);
            m_autoSaveCommand = new RelayCommand(AutoSave);
            m_saveCommand = new RelayCommand(Save);
            m_realizeSubjectCommand = new RelayCommand(RealizeSubject, CanRealizeSubject);
            m_editRealizedSubjectCommand = new RelayCommand(EditRealizedSubject);
            m_putAllEndingMarksCommand = new RelayCommand<string>(PutAllEndingMarks);
            m_putNotAllEndingMarksCommand = new RelayCommand<string>(PutNotAllEndingMarks);
            m_cancelAllEndingMarksCommand = new RelayCommand<string>(CancelAllEndingMarks);
            m_addMarksSetCommand = new RelayCommand<string>(AddMarksSet);

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

        private RelayCommand<ObservableCollection<MarkViewModel>> m_addMarkCommand;
        public ICommand AddMarkCommand
        {
            get { return m_addMarkCommand; }
        }

        private RelayCommand<ObservableCollection<MarkViewModel>> m_editMarkCommand;
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

        private void AddMark(ObservableCollection<MarkViewModel> param)
        {
            MarkViewModel mark = new MarkViewModel();
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(mark, m_selectedStudent, true);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Ok)
            {
                param.Add(mark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void EditMark(ObservableCollection<MarkViewModel> param)
        {
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(m_selectedMark, m_selectedStudent);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.RemoveMark)
            {
                param.Remove(m_selectedMark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_autoSaveCommand.Execute(null);
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

            EditEndingMarkViewModel dialogViewModel = new EditEndingMarkViewModel(initialMark, averageMark);
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
            RealizeSubjectViewModel dialogViewModel = new RealizeSubjectViewModel(null, m_selectedGroup.Students, GetAvailableSubjects(m_selectedGroup), true);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == RealizeSubjectViewModel.RealizeSubjectResult.Ok)
            {
                m_selectedGroup.RealizedSubjects.Add(dialogViewModel.RealizedSubject);
            }
            if (dialogViewModel.Result != RealizeSubjectViewModel.RealizeSubjectResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private bool CanRealizeSubject(object e)
        {
            return m_selectedGroup != null;
        }
        private void EditRealizedSubject(object e)
        {
            RealizeSubjectViewModel dialogViewModel = new RealizeSubjectViewModel(m_selectedSubject, m_selectedGroup.Students, GetAvailableSubjects(m_selectedGroup));
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == RealizeSubjectViewModel.RealizeSubjectResult.RemoveSubject)
            {
                m_selectedGroup.RealizedSubjects.Remove(m_selectedSubject);
                m_selectedSubject = null;
            }
            if (dialogViewModel.Result != RealizeSubjectViewModel.RealizeSubjectResult.Cancel) m_autoSaveCommand.Execute(null);
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
        private void AddMarksSet(string e)
        {
            AddMarksSetViewModel dialogViewModel = new AddMarksSetViewModel(m_selectedGroup.Students, (e == "first" ? AddMarksSetViewModel.SemesterType.First : AddMarksSetViewModel.SemesterType.Second));
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result != AddMarksSetViewModel.AddMarksSetResult.Cancel) m_autoSaveCommand.Execute(null);
        }

        private bool MessageBoxContinue()
        {
            if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(GlobalConfig.Main), GlobalConfig.GetStringResource("lang_DoYouWantToContinue"), "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return false;
            return true;
        }
    }
}
