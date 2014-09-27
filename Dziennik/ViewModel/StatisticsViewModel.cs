using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Dziennik.ViewModel
{
    public sealed class StatisticsViewModel : ObservableObject
    {
        public class StaticticsItem : ObservableObject
        {
            public StaticticsItem(int month, StatisticsViewModel owner)
            {
                m_month = month;
                m_owner = owner;
            }

            private StatisticsViewModel m_owner;
            private int m_month;
            public int Month
            {
                get { return m_month; }
            }
            public string Name
            {
                get
                {
                    switch(m_month)
                    {
                        case -1: return GlobalConfig.GetStringResource("lang_FirstSemesterNameSmall"); // -1 == first semester
                        case -2: return GlobalConfig.GetStringResource("lang_SecondSemesterNameSmall");  // -2 == second semester
                        case -3: return GlobalConfig.GetStringResource("lang_YearEndingNameSmall"); // -3 == school year
                    }
                    return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m_month);
                }
            }

            public string AverageAttendanceDisplayed
            {
                get
                {
                    int present = 0;
                    int total = 0;

                    foreach (var student in m_owner.m_group.Students)
                    {
                        ComputeValidAttendance(ref present, ref total, student.Presence);
                    }

                    if (total == 0) return string.Empty;

                    decimal attendance = (decimal)present / (decimal)total;
                    attendance = decimal.Round(attendance, 4, MidpointRounding.AwayFromZero);
                    attendance *= 100M;

                    return attendance.ToString("G29", CultureInfo.InvariantCulture) + " %";
                }
            }
            public decimal AverageMark
            {
                get
                {
                    int validMarksWeight = 0;
                    decimal valuesSum = 0;
                    foreach (var student in m_owner.m_group.Students)
                    {
                        ComputeValidMarks(ref validMarksWeight, ref valuesSum, student.FirstSemester);
                        ComputeValidMarks(ref validMarksWeight, ref valuesSum, student.SecondSemester);
                    }
                    if (validMarksWeight <= 0) return 0M;

                    return decimal.Round(valuesSum / (decimal)validMarksWeight, GlobalConfig.DecimalRoundingPoints, MidpointRounding.AwayFromZero);
                }
            }
            public decimal AverageEndingMark
            {
                get
                {
                    if (m_month < 0)
                    {
                        decimal marksSum = 0M;
                        int studentsSum = 0;

                        foreach (var student in m_owner.m_group.Students)
                        {
                            if (m_month == -1)
                            {
                                if (student.HalfEndingMark > 0)
                                {
                                    marksSum += student.HalfEndingMark;
                                    studentsSum++;
                                }
                            }
                            else if (m_month == -2)
                            {
                                if (student.YearEndingMark > 0)
                                {
                                    marksSum += student.YearEndingMark;
                                    studentsSum++;
                                }
                            }
                        }

                        if (studentsSum <= 0) return 0M;

                        return decimal.Round(marksSum / (decimal)studentsSum, GlobalConfig.DecimalRoundingPoints, MidpointRounding.AwayFromZero);
                    }
                    else return 0M;
                }
            }

            public string AverageMarkDisplayed
            {
                get
                {
                    decimal avg = AverageMark;
                    if (avg <= 0M) return string.Empty;

                    return avg.ToString(CultureInfo.InvariantCulture);
                }
            }
            public string AverageEndingMarkDisplayed
            {
                get
                {
                    decimal avg = AverageEndingMark;
                    if (avg <= 0M) return string.Empty;

                    return avg.ToString(CultureInfo.InvariantCulture);
                }
            }
            public string RealizedSubjectsCountDisplayed
            {
                get
                {
                    int count = m_owner.m_group.RealizedSubjects.Count(x => CheckIsValidDate(x.RealizedDate));

                    return count.ToString();
                }
            }
            public string RealizedSubjectsCurriculumCountDisplayed
            {
                get
                {
                    int count = m_owner.m_group.RealizedSubjects.Count(x => CheckIsValidDate(x.RealizedDate) && !x.IsCustom);

                    return count.ToString();
                }
            }
            public System.Windows.FontWeight NameFontWeight
            {
                get
                {
                    if (m_month < 0) return System.Windows.FontWeights.Bold;

                    return System.Windows.FontWeights.Normal;
                }
            }

            private void ComputeValidMarks(ref int weightsSum, ref decimal valuesSum, SemesterViewModel semester)
            {
                foreach (var mark in semester.Marks)
                {
                    if (mark.IsValueValid)
                    {
                        if(CheckIsValidDate(mark.AddDate))
                        {
                            weightsSum += mark.Weight;
                            valuesSum += mark.Value * mark.Weight;
                        }
                    }
                }
            }
            private void ComputeValidAttendance(ref int presentStudentHours, ref int totalStudentHours, IEnumerable<RealizedSubjectPresenceViewModel> presence)
            {
                foreach (var item in presence)
                {
                    if(item.Presence != Model.PresenceType.None && CheckIsValidDate(item.RealizedSubject.RealizedDate))
                    {
                        totalStudentHours++;
                        if (item.WasPresent) presentStudentHours++;
                    }
                }
            }

            private bool CheckIsValidDate(DateTime date)
            {
                if (date.Month == m_month) return true;
                if (m_month == -1 && date >= m_owner.m_group.OwnerClass.Calendar.YearBeginning && date < m_owner.m_group.OwnerClass.Calendar.SemesterSeparator) return true;
                if (m_month == -2 && date >= m_owner.m_group.OwnerClass.Calendar.SemesterSeparator && date <= m_owner.m_group.OwnerClass.Calendar.YearEnding) return true;
                if (m_month == -3 && date >= m_owner.m_group.OwnerClass.Calendar.YearBeginning && date <= m_owner.m_group.OwnerClass.Calendar.YearEnding) return true;
                return false;
            }
        }

        public StatisticsViewModel(SchoolGroupViewModel group)
        {
            m_group = group;

            m_collection = new ObservableCollection<StaticticsItem>();
            //m_firstSemesterMonths = new List<int>();
            //m_secondSemesterMonths = new List<int>();
        }

        private SchoolGroupViewModel m_group;
        //private List<int> m_firstSemesterMonths;
        //private List<int> m_secondSemesterMonths;

        private ObservableCollection<StaticticsItem> m_collection;
        public ObservableCollection<StaticticsItem> Collection
        {
            get { return m_collection; }
        }

        public void Refresh()
        {
            m_collection.Clear();

            CalendarViewModel calendar = m_group.OwnerClass.Calendar;
            if (calendar != null)
            {
                AddMonths(calendar.YearBeginning.Month, GetNextMonth(calendar.SemesterSeparator.Month)); // adding month from YearBeginning(included) to SemesterSeparator(included)
                
                m_collection.Add(new StaticticsItem(-1, this)); // adding first semester

                AddMonths(calendar.SemesterSeparator.Month + 1, calendar.YearEnding.Month); // adding months from SemesterSeparator(excluded, added in first step) to YearEnding(excluded, description below)
                if (m_collection.FirstOrDefault(x => x.Month == calendar.YearEnding.Month) == null) // adding YearEnding month, is case where YearBeginning month and YearEnding month are the same, condition detect it and don't re-add existing month
                {
                    m_collection.Add(new StaticticsItem(calendar.YearEnding.Month, this));
                }
                
                m_collection.Add(new StaticticsItem(-2, this)); // adding second semester
                m_collection.Add(new StaticticsItem(-3, this)); // adding year ending
            }
        }

        private void AddMonths(int startMonthIncluded, int endMonthExcluded)
        {
            while(startMonthIncluded != endMonthExcluded)
            {
                m_collection.Add(new StaticticsItem(startMonthIncluded, this));
                startMonthIncluded = GetNextMonth(startMonthIncluded);
            }
        }
        private int GetNextMonth(int month)
        {
            return (month >= 12 ? 1 : month + 1);
        }
    }
}
