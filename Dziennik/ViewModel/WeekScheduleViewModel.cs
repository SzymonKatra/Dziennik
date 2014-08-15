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
        }

        public int Monday
        {
            get { return Model.Monday; }
            set { Model.Monday = value; RaisePropertyChanged("Monday"); }
        }
        public int Tuesday
        {
            get { return Model.Tuesday; }
            set { Model.Tuesday = value; RaisePropertyChanged("Tuesday"); }
        }
        public int Wednesday
        {
            get { return Model.Wednesday; }
            set { Model.Wednesday = value; RaisePropertyChanged("Wednesday"); }
        }
        public int Thursday
        {
            get { return Model.Thursday; }
            set { Model.Thursday = value; RaisePropertyChanged("Thursday"); }
        }
        public int Friday
        {
            get { return Model.Friday; }
            set { Model.Friday = value; RaisePropertyChanged("Friday"); }
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
