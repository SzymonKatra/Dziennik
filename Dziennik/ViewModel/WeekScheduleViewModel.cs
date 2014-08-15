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
        public WeekScheduleViewModel(WeekSchedule model) : base(model)
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

        public override void CopyDataTo(WeekScheduleViewModel viewModel)
        {
            viewModel.Monday = this.Monday;
            viewModel.Tuesday = this.Tuesday;
            viewModel.Wednesday = this.Wednesday;
            viewModel.Thursday = this.Thursday;
            viewModel.Friday = this.Friday;
        }
    }
}
