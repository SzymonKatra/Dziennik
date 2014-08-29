using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class WeekScheduleViewModel : ViewModelBase<WeekScheduleViewModel, WeekSchedule>
    {
        public WeekScheduleViewModel()
            : this(new WeekSchedule())
        {
        }
        public WeekScheduleViewModel(WeekSchedule model)
            : base(model)
        {
            m_monday = new DayScheduleViewModel(Model.Monday);
            m_tuesday = new DayScheduleViewModel(Model.Tuesday);
            m_wednesday = new DayScheduleViewModel(Model.Wednesday);
            m_thursday = new DayScheduleViewModel(Model.Thursday);
            m_friday = new DayScheduleViewModel(Model.Friday);

            SubscribeDaySchedule(m_monday);
            SubscribeDaySchedule(m_tuesday);
            SubscribeDaySchedule(m_wednesday);
            SubscribeDaySchedule(m_thursday);
            SubscribeDaySchedule(m_friday);
        }

        public event EventHandler HoursCountChanged;

        public DateTime StartDate
        {
            get { return Model.StartDate; }
            set { Model.StartDate = value; RaisePropertyChanged("StartDate"); }
        }
        private DayScheduleViewModel m_monday;
        public DayScheduleViewModel Monday
        {
            get { return m_monday; }
            set
            {
                UnsubscribeDaySchedule(m_monday);
                m_monday = value;
                SubscribeDaySchedule(m_monday);
                Model.Monday = value.Model;
                RaisePropertyChanged("Monday");
            }
        }
        private DayScheduleViewModel m_tuesday;
        public DayScheduleViewModel Tuesday
        {
            get { return m_tuesday; }
            set
            {
                UnsubscribeDaySchedule(m_tuesday);
                m_tuesday = value;
                SubscribeDaySchedule(m_tuesday);
                Model.Tuesday = value.Model;
                RaisePropertyChanged("Tuesday");
            }
        }
        private DayScheduleViewModel m_wednesday;
        public DayScheduleViewModel Wednesday
        {
            get { return m_wednesday; }
            set
            {
                UnsubscribeDaySchedule(m_wednesday);
                m_wednesday = value;
                SubscribeDaySchedule(m_wednesday);
                Model.Wednesday = value.Model;
                RaisePropertyChanged("Wednesday");
            }
        }
        private DayScheduleViewModel m_thursday;
        public DayScheduleViewModel Thursday
        {
            get { return m_thursday; }
            set
            {
                UnsubscribeDaySchedule(m_thursday);
                m_thursday = value;
                SubscribeDaySchedule(m_thursday);
                Model.Thursday = value.Model;
                RaisePropertyChanged("Thursday");
            }
        }
        private DayScheduleViewModel m_friday;
        public DayScheduleViewModel Friday
        {
            get { return m_friday; }
            set
            {
                UnsubscribeDaySchedule(m_friday);
                m_friday = value;
                SubscribeDaySchedule(m_friday);
                Model.Friday = value.Model;
                RaisePropertyChanged("Friday");
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.StartDate);

            CopyStack.Push(pack);

            this.Monday.PushCopy();
            this.Tuesday.PushCopy();
            this.Wednesday.PushCopy();
            this.Thursday.PushCopy();
            this.Friday.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if (result == WorkingCopyResult.Cancel)
            {
                this.StartDate = (DateTime)pack.Read();
            }

            this.Monday.PopCopy(result);
            this.Tuesday.PopCopy(result);
            this.Wednesday.PopCopy(result);
            this.Thursday.PopCopy(result);
            this.Friday.PopCopy(result);
        }

        private void SubscribeDaySchedule(DayScheduleViewModel daySchedule)
        {
            daySchedule.PropertyChanged += daySchedule_PropertyChanged;
        }
        private void UnsubscribeDaySchedule(DayScheduleViewModel daySchedule)
        {
            daySchedule.PropertyChanged -= daySchedule_PropertyChanged;
        }

        private void daySchedule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="HoursCount")
            {
                OnHoursCountChanged();
            }
        }

        private void OnHoursCountChanged()
        {
            EventHandler handler = HoursCountChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
