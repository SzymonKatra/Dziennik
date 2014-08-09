using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class WeekScheduleViewModel : ObservableObject, IModelExposable<WeekSchedule>
    {
        public WeekScheduleViewModel()
            : this(new WeekSchedule())
        {
        }
        public WeekScheduleViewModel(WeekSchedule model)
        {
            m_model = model;
        }

        private WeekSchedule m_model;
        public WeekSchedule Model
        {
            get { return m_model; }
        }

        public int Monday
        {
            get { return m_model.Monday; }
            set { m_model.Monday = value; RaisePropertyChanged("Monday"); }
        }
        public int Tuesday
        {
            get { return m_model.Tuesday; }
            set { m_model.Tuesday = value; RaisePropertyChanged("Tuesday"); }
        }
        public int Wednesday
        {
            get { return m_model.Wednesday; }
            set { m_model.Wednesday = value; RaisePropertyChanged("Wednesday"); }
        }
        public int Thursday
        {
            get { return m_model.Thursday; }
            set { m_model.Thursday = value; RaisePropertyChanged("Thursday"); }
        }
        public int Friday
        {
            get { return m_model.Friday; }
            set { m_model.Friday = value; RaisePropertyChanged("Friday"); }
        }

        public void CopyTo(WeekScheduleViewModel other)
        {
            other.Monday = this.Monday;
            other.Tuesday = this.Tuesday;
            other.Wednesday = this.Wednesday;
            other.Thursday = this.Thursday;
            other.Friday = this.Friday;
        }
    }
}
