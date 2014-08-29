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
        }

        private DayScheduleViewModel m_monday;
        public DayScheduleViewModel Monday
        {
            get { return m_monday; }
            set
            {
                m_monday = value;
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
                m_tuesday = value;
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
                m_wednesday = value;
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
                m_thursday = value;
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
                m_friday = value;
                Model.Friday = value.Model;
                RaisePropertyChanged("Friday");
            }
        }

        protected override void OnPushCopy()
        {
            //ObjectsPack pack = new ObjectsPack();
            //pack.Write(this.Monday);
            //pack.Write(this.Tuesday);
            //pack.Write(this.Wednesday);
            //pack.Write(this.Thursday);
            //pack.Write(this.Friday);

            //CopyStack.Push(pack);

            this.Monday.PushCopy();
            this.Tuesday.PushCopy();
            this.Wednesday.PushCopy();
            this.Thursday.PushCopy();
            this.Friday.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            //ObjectsPack pack = CopyStack.Pop();

            //if (result == WorkingCopyResult.Cancel)
            //{
            //    this.Monday = (int)pack.Read();
            //    this.Tuesday = (int)pack.Read();
            //    this.Wednesday = (int)pack.Read();
            //    this.Thursday = (int)pack.Read();
            //    this.Friday = (int)pack.Read();
            //}

            this.Monday.PopCopy(result);
            this.Tuesday.PopCopy(result);
            this.Wednesday.PopCopy(result);
            this.Thursday.PopCopy(result);
            this.Friday.PopCopy(result);
        }
    }
}
