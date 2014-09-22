using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;
using Dziennik.Controls;
using System.ComponentModel;

namespace Dziennik.View
{
    public sealed class RealizeSubjectViewModel : ObservableObject, IDataErrorInfo
    {
        public enum RealizeSubjectResult
        {
            Ok,
            Cancel,
            RemoveSubject,
        }

        public class StudentPresencePair : ObservableObject
        {
            public StudentPresencePair()
            {
                m_justifyCommand = new RelayCommand(Justify, CanJustify);
                m_cancelJustifyCommand = new RelayCommand(CancelJustify, CanCancelJustify);
            }

            private RelayCommand m_justifyCommand;
            public ICommand JustifyCommand
            {
                get { return m_justifyCommand; }
            }

            private RelayCommand m_cancelJustifyCommand;
            public ICommand CancelJustifyCommand
            {
                get { return m_cancelJustifyCommand; }
            }

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

            public bool IsRemoved
            {
                get { return m_student.IsRemoved; }
            }

            private Dziennik.Model.PresenceType m_presenceCache;
            public Dziennik.Model.PresenceType PresenceCache
            {
                get { return m_presenceCache; }
                set { m_presenceCache = value; RaiseAllPresenceChanged(); }
            }

            public bool WasPresent
            {
                get { return m_presenceCache == Model.PresenceType.Present; }
                set { m_presenceCache = Model.PresenceType.Present; RaiseAllPresenceChanged(); }
            }
            public bool WasAbsent
            {
                get { return m_presenceCache == Model.PresenceType.Absent || m_presenceCache == Model.PresenceType.AbsentJustified; }
                set { m_presenceCache = Model.PresenceType.Absent; RaiseAllPresenceChanged(); }
            }
            public bool WasLate
            {
                get { return m_presenceCache == Model.PresenceType.Late; }
                set { m_presenceCache = Model.PresenceType.Late; RaiseAllPresenceChanged(); }
            }

            private void Justify(object param)
            {
                PresenceCache = Model.PresenceType.AbsentJustified;
            }
            private bool CanJustify(object param)
            {
                return m_presenceCache == Model.PresenceType.Absent;
            }
            private void CancelJustify(object param)
            {
                PresenceCache = Model.PresenceType.Absent;
            }
            private bool CanCancelJustify(object param)
            {
                return m_presenceCache == Model.PresenceType.AbsentJustified;
            }

            private void RaiseAllPresenceChanged()
            {
                RaisePropertyChanged("PresenceCache");
                RaisePropertyChanged("WasPresent");
                RaisePropertyChanged("WasAbsent");
                RaisePropertyChanged("WasLate");
                m_justifyCommand.RaiseCanExecuteChanged();
                m_cancelJustifyCommand.RaiseCanExecuteChanged();
            }
        }

        public RealizeSubjectViewModel(RealizedSubjectViewModel realizedSubject, IEnumerable<StudentInGroupViewModel> students, IEnumerable<GlobalSubjectViewModel> availableSubjects, CalendarViewModel calendar, IEnumerable<RealizedSubjectViewModel> alreadyRealized, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeSubjectCommand = new RelayCommand(RemoveSubject, CanRemoveSubject);
            m_chooseSubjectCommand = new RelayCommand(ChooseSubject);

            m_isAddingMode = isAddingMode;
            m_realizedSubject = realizedSubject;
            m_students = students;
            m_availableSubjects = new ObservableCollection<GlobalSubjectViewModel>(availableSubjects);
            m_calendar = calendar;
            m_alreadyRealized = alreadyRealized;

            if (!m_isAddingMode)
            {
                m_selectedSubject = m_realizedSubject.GlobalSubject;
                m_outsideSubject = m_realizedSubject.CustomSubject;
                m_isOutsideCurriculum = m_realizedSubject.IsCustom;
                m_realizeDate = m_realizedSubject.RealizedDate;
                m_realizeHour = GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.FirstOrDefault(x => x.Number == m_realizedSubject.RealizedHour);
            }
            else
            {
                m_realizedSubject = new RealizedSubjectViewModel();
                m_realizeDate = DateTime.Now;
                m_realizeHour = (GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.Count > 0 ? GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours[0] : null);
                int currentHour = GlobalConfig.GetCurrentHourNumber(m_realizeDate);
                LessonHourViewModel findResult = GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.FirstOrDefault(x => x.Number == currentHour);
                if (findResult != null) m_realizeHour = findResult;

                m_selectedSubject = (m_availableSubjects.Count > 0 ? m_availableSubjects[0] : null);
            }
            foreach (StudentInGroupViewModel student in students)
            {
                StudentPresencePair pair = new StudentPresencePair();
                pair.Student = student;
                pair.PropertyChanged += pair_PropertyChanged;
                if (isAddingMode)
                {
                    pair.Presence = new RealizedSubjectPresenceViewModel() { RealizedSubject = m_realizedSubject };
                    //pair.Presence.WasPresent = pair.WasPresentCache = true;
                    pair.Presence.Presence = pair.PresenceCache = (student.IsRemoved ? Model.PresenceType.None : Model.PresenceType.Present);
                }
                else
                {
                    pair.Presence = student.Presence.FirstOrDefault(x => x.RealizedSubject == m_realizedSubject);
                    if (pair.Presence == null) continue;
                    //pair.WasPresentCache = pair.Presence.WasPresent;
                    pair.PresenceCache = pair.Presence.Presence;
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

        private IEnumerable<RealizedSubjectViewModel> m_alreadyRealized;
        private CalendarViewModel m_calendar;

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

        private LessonHourViewModel m_realizeHour;
        public LessonHourViewModel RealizeHour
        {
            get { return m_realizeHour; }
            set { m_realizeHour = value; RaisePropertyChanged("RealizeHour"); }
        }

        private bool m_realizeDateValid = false;
        private DateTime m_realizeDate;
        public DateTime RealizeDate
        {
            get { return m_realizeDate; }
            set { m_realizeDate = value; RaisePropertyChanged("RealizeDate"); }
        }

        private string m_semesterTypeText = string.Empty;
        public string SemesterTypeText
        {
            get { return m_semesterTypeText; }
            private set { m_semesterTypeText = value; RaisePropertyChanged("SemesterTypeText"); }
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

        public string StudentsPresentFormalDisplayed
        {
            get
            {
                int present = m_pairs.Count(x => x.WasPresent);
                int lates = m_pairs.Count(x => x.WasLate);
                int absents = m_pairs.Count(x => x.WasAbsent);

                return string.Format(GlobalConfig.GetStringResource("lang_StudentsPresentFormalFormat"), present + lates, absents, lates);
            }
        }
        public string StudentsPresentDisplayed
        {
            get
            {
                int present = m_pairs.Count(x => x.WasPresent);
                int lates = m_pairs.Count(x => x.WasLate);

                return string.Format(GlobalConfig.GetStringResource("lang_StudentsPresentFormat"), present + lates, lates);
            }
        }
        public string StudentsAbsentDisplayed
        {
            get
            {
                int absents = m_pairs.Count(x => x.WasAbsent);

                return string.Format(GlobalConfig.GetStringResource("lang_StudentsAbsentFormat"), absents);
            }
        }
        public string StudentsSumDisplayed
        {
            get
            {
                int sum = m_pairs.Count(x => x.PresenceCache != Model.PresenceType.None);

                return string.Format(GlobalConfig.GetStringResource("lang_StudentsSumFormat"), sum);
            }
        }

        private void Ok(object e)
        {
            if(!m_isAddingMode)
            {
                if (GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_DoYouWantToContinue"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
            }
            if (IsRealizeDateHourChanged() &&
                m_alreadyRealized.FirstOrDefault(x => x.RealizedDate.Date == m_realizeDate.Date && m_realizeHour != null && x.RealizedHour == m_realizeHour.Number) != null)
            {
                if (GlobalConfig.MessageBox(this, string.Format(GlobalConfig.GetStringResource("lang_SubjectAlreadyRealizedFormat"), m_realizeDate.ToString(GlobalConfig.DateFormat), m_realizeHour.Number), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
            }

            if (m_isOutsideCurriculum)
            {
                m_realizedSubject.CustomSubject = m_outsideSubject;
            }
            else
            {
                m_realizedSubject.GlobalSubject = m_selectedSubject;
            }
            m_realizedSubject.RealizedHour = (m_realizeHour == null ? 0 : m_realizeHour.Number);
            m_realizedSubject.RealizedDate = m_realizeDate;
            foreach (var pair in m_pairs)
            {
                //pair.Presence.WasPresent = pair.WasPresentCache;
                pair.Presence.Presence = pair.PresenceCache;
                if (!m_isAddingMode) pair.Student.RaiseAttendanceChanged();
            }
            if (m_isAddingMode)
            {
                foreach (var pair in m_pairs)
                {
                    pair.Student.Presence.Add(pair.Presence);
                    pair.Student.RaiseAttendanceChanged();
                }
            }
            m_result = RealizeSubjectResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_isOutsideCurriculum || (!m_isOutsideCurriculum && m_selectedSubject != null) && m_realizeDateValid;
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
            if (GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_DoYouWantToRemoveRealizedSubject"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

            foreach (var pair in m_pairs)
            {
                pair.Student.Presence.Remove(pair.Presence);
                pair.Student.RaiseAttendanceChanged();
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
        private void pair_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("StudentsPresentFormalDisplayed");
            RaisePropertyChanged("StudentsPresentDisplayed");
            RaisePropertyChanged("StudentsAbsentDisplayed");
            RaisePropertyChanged("StudentsSumDisplayed");
        }

        private bool IsRealizeDateHourChanged()
        {
            return (m_realizeHour == null ? 0 : m_realizeHour.Number) != m_realizedSubject.RealizedHour || m_realizeDate != m_realizedSubject.RealizedDate;
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
                    case "RealizeDate": return ValidateRealizeDate();
                }

                return string.Empty;
            }
        }

        private string ValidateRealizeDate()
        {
            m_realizeDateValid = false;

            if (m_realizeDate < m_calendar.YearBeginning || m_realizeDate > m_calendar.YearEnding)
            {
                m_okCommand.RaiseCanExecuteChanged();
                return string.Format(GlobalConfig.GetStringResource("lang_RealizedSubjectDateNoticeFormat"),
                                     m_calendar.YearBeginning.ToString(GlobalConfig.DateFormat),
                                     m_calendar.SemesterSeparator.AddDays(-1).ToString(GlobalConfig.DateFormat),
                                     m_calendar.SemesterSeparator.ToString(GlobalConfig.DateFormat),
                                     m_calendar.YearEnding.ToString(GlobalConfig.DateFormat));
            }

            SemesterTypeText = GlobalConfig.GetStringResource((m_realizeDate < m_calendar.SemesterSeparator ? "lang_FirstSemesterName" : "lang_SecondSemesterName"));

            m_realizeDateValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
