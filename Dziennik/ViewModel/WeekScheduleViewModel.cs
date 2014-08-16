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

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Monday);
            pack.Write(this.Tuesday);
            pack.Write(this.Wednesday);
            pack.Write(this.Thursday);
            pack.Write(this.Friday);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if (result == WorkingCopyResult.Cancel)
            {
                this.Monday = (int)pack.Read();
                this.Tuesday = (int)pack.Read();
                this.Wednesday = (int)pack.Read();
                this.Thursday = (int)pack.Read();
                this.Friday = (int)pack.Read();
            }
        }
    }
}
