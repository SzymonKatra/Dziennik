using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class RealizeSubjectViewModel : ObservableObject
    {
        public enum RealizeSubjectResult
        {
            Ok,
            Cancel,
            RemoveSubject,
        }

        public class StudentPresencePair : ObservableObject
        {
            private StudentInGroupViewModel m_student;
            public StudentInGroupViewModel Student
            {
                get { return m_student; }
                set { m_student = value; RaisePropertyChanged("Student"); }
            }

            private RealizedSubjectPresenceViewModel m_presence;
            public RealizedSubjectPresenceViewModel Presence
            {
                get { return m_presence; }
                set { m_presence = value; RaisePropertyChanged("Presence"); }
            }

            private bool m_wasPresentCache;
            public bool WasPresentCache
            {
                get { return m_wasPresentCache; }
                set { m_wasPresentCache = value; }
            }
        }

        public RealizeSubjectViewModel(RealizedSubjectViewModel realizedSubject, IEnumerable<StudentInGroupViewModel> students, IEnumerable<GlobalSubjectViewModel> availableSubjects, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeSubjectCommand = new RelayCommand(RemoveSubject, CanRemoveSubject);
            m_chooseSubjectCommand = new RelayCommand(ChooseSubject);

            m_isAddingMode = isAddingMode;
            m_realizedSubject = realizedSubject;
            m_students = students;
            m_availableSubjects = new ObservableCollection<GlobalSubjectViewModel>(availableSubjects);

            if (!m_isAddingMode)
            {
                m_selectedSubject = m_realizedSubject.GlobalSubject;
                m_outsideSubject = m_realizedSubject.CustomSubject;
                m_isOutsideCurriculum = m_realizedSubject.IsCustom;
                m_realizeDate = m_realizedSubject.RealizedDate;
            }
            else
            {
                m_realizedSubject = new RealizedSubjectViewModel();
                m_realizeDate = DateTime.Now;
            }
            foreach (StudentInGroupViewModel student in students)
            {
                StudentPresencePair pair = new StudentPresencePair();
                pair.Student = student;
                if (isAddingMode)
                {
                    pair.Presence = new RealizedSubjectPresenceViewModel() { RealizedSubject = m_realizedSubject };
                    pair.Presence.WasPresent = pair.WasPresentCache = true;
                }
                else
                {
                    pair.Presence = student.Presence.First(x => x.RealizedSubject == m_realizedSubject);
                    pair.WasPresentCache = pair.Presence.WasPresent;
                }
                m_pairs.Add(pair);
            }
        }

        private RealizeSubjectResult m_result = RealizeSubjectResult.Cancel;
        public RealizeSubjectResult Result
        {
            get { return m_result; }
        }

        private bool m_isAddingMode;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
            set { m_isAddingMode = value; RaisePropertyChanged("IsAddingMode"); m_removeSubjectCommand.RaiseCanExecuteChanged(); }
        }

        private RealizedSubjectViewModel m_realizedSubject;
        public RealizedSubjectViewModel RealizedSubject
        {
            get { return m_realizedSubject; }
        }
        private IEnumerable<StudentInGroupViewModel> m_students;
        private ObservableCollection<GlobalSubjectViewModel> m_availableSubjects;

        private ObservableCollection<StudentPresencePair> m_pairs = new ObservableCollection<StudentPresencePair>();
        public ObservableCollection<StudentPresencePair> Pairs
        {
            get { return m_pairs; }
            set { m_pairs = value; RaisePropertyChanged("Pairs"); }
        }

        private GlobalSubjectViewModel m_selectedSubject;
        public GlobalSubjectViewModel SelectedSubject
        {
            get { return m_selectedSubject; }
            set { m_selectedSubject = value; RaisePropertyChanged("SelectedSubject"); m_okCommand.RaiseCanExecuteChanged(); }
        }

        private bool m_isOutsideCurriculum;
        public bool IsOutsideCurriculum
        {
            get { return m_isOutsideCurriculum; }
            set { m_isOutsideCurriculum = value; RaisePropertyChanged("IsOutsideCurriculum"); m_okCommand.RaiseCanExecuteChanged(); }
        }

        private string m_outsideSubject;
        public string OutsideSubject
        {
            get { return m_outsideSubject; }
            set { m_outsideSubject = value; RaisePropertyChanged("OutsideCurriculum"); }
        }

        private DateTime m_realizeDate;
        public DateTime RealizeDate
        {
            get { return m_realizeDate; }
            set { m_realizeDate = value; RaisePropertyChanged("RealizeDate"); }
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
        private RelayCommand m_removeSubjectCommand;
        public ICommand RemoveSubjectCommand
        {
            get { return m_removeSubjectCommand; }
        }
        private RelayCommand m_chooseSubjectCommand;
        public ICommand ChooseSubjectCommand
        {
            get { return m_chooseSubjectCommand; }
        }

        private void Ok(object e)
        {
            if(m_isOutsideCurriculum)
            {
                m_realizedSubject.CustomSubject = m_outsideSubject;
            }
            else
            {
                m_realizedSubject.GlobalSubject = m_selectedSubject;
            }
            m_realizedSubject.RealizedDate = m_realizeDate;
            foreach (var pair in m_pairs)
            {
                pair.Presence.WasPresent = pair.WasPresentCache;
            }
            if (m_isAddingMode)
            {
                foreach (var pair in m_pairs)
                {
                    pair.Student.Presence.Add(pair.Presence);
                }
            }
            m_result = RealizeSubjectResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_isOutsideCurriculum || (!m_isOutsideCurriculum && m_selectedSubject != null);
        }
        private void Cancel(object e)
        {
            m_result = RealizeSubjectResult.Cancel;
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void RemoveSubject(object e)
        {
            if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Czy na pewno chcesz usunąć zrealizowany temat?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

            foreach (var pair in m_pairs)
            {
                pair.Student.Presence.Remove(pair.Presence);
            }
            m_result = RealizeSubjectResult.RemoveSubject;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveSubject(object e)
        {
            return !m_isAddingMode;
        }
        private void ChooseSubject(object e)
        {
            SelectGlobalSubjectViewModel dialogViewModel = new SelectGlobalSubjectViewModel(m_availableSubjects);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result != SelectGlobalSubjectViewModel.SelectedGlobalSubjectResult.Ok || dialogViewModel.SelectedSubject == null) return;
            SelectedSubject = dialogViewModel.SelectedSubject;
        }
    }
}
