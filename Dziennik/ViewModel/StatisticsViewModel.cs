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
                        case -1: return GlobalConfig.GetStringResource("lang_SemesterSeparatorName"); // -1 == school semester
                        case -2: return GlobalConfig.GetStringResource("lang_YearEndingName");  // -2 == school year
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
                        if (student.IsRemoved) continue;
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
                        if (student.IsRemoved) continue;
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

            private void ComputeValidMarks(ref int weightsSum, ref decimal valuesSum, SemesterViewModel semester)
            {
                foreach (var mark in semester.Marks)
                {
                    if (mark.IsValueValid)
                    {
                        if (CheckIsValidMonth(mark.AddDate.Month))
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
                    if (CheckIsValidMonth(item.RealizedSubject.RealizedDate.Month))
                    {
                        totalStudentHours++;
                        if (item.WasPresent) presentStudentHours++;
                    }
                }
            }

            private bool CheckIsValidMonth(int month)
            {
                if (month == m_month) return true;
                if (m_month == -1 && m_owner.m_firstSemesterMonths.Contains(month)) return true;
                if (m_month == -2 && (m_owner.m_firstSemesterMonths.Contains(month) || m_owner.m_secondSemesterMonths.Contains(month))) return true;
                return false;
            }
        }

        public StatisticsViewModel(SchoolGroupViewModel group)
        {
            m_group = group;

            m_collection = new ObservableCollection<StaticticsItem>();
            m_firstSemesterMonths = new List<int>();
            m_secondSemesterMonths = new List<int>();
        }

        private SchoolGroupViewModel m_group;
        private List<int> m_firstSemesterMonths;
        private List<int> m_secondSemesterMonths;

        private ObservableCollection<StaticticsItem> m_collection;
        public ObservableCollection<StaticticsItem> Collection
        {
            get { return m_collection; }
        }

        public void Refresh()
        {
            m_collection.Clear();
            m_firstSemesterMonths.Clear();
            m_secondSemesterMonths.Clear();

            CalendarViewModel calendar = m_group.OwnerClass.Calendar;
            if (calendar != null)
            {
                int currentMonth = calendar.YearBeginning.Month;
                while (currentMonth != calendar.SemesterSeparator.Month)
                {
                    m_collection.Add(new StaticticsItem(currentMonth,this));
                    m_firstSemesterMonths.Add(currentMonth);
                    currentMonth = (currentMonth >= 12 ? 1 : currentMonth + 1);
                }
                m_collection.Add(new StaticticsItem(calendar.SemesterSeparator.Month, this));
                m_firstSemesterMonths.Add(calendar.SemesterSeparator.Month);
                m_collection.Add(new StaticticsItem(-1, this));
                currentMonth = calendar.SemesterSeparator.Month + 1;
                while (currentMonth != calendar.YearEnding.Month)
                {
                    m_collection.Add(new StaticticsItem(currentMonth, this));
                    m_secondSemesterMonths.Add(currentMonth);
                    currentMonth = (currentMonth >= 12 ? 1 : currentMonth + 1);
                }
                if (m_collection.FirstOrDefault(x => x.Month == calendar.YearEnding.Month) == null)
                {
                    m_collection.Add(new StaticticsItem(calendar.YearEnding.Month, this));
                    m_secondSemesterMonths.Add(calendar.YearEnding.Month);
                }
                m_collection.Add(new StaticticsItem(-2, this));
            }
        }
    }
}
