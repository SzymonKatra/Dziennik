using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class SelectedHourViewModel : ViewModelBase<SelectedHourViewModel, SelectedHour>
    {
        public SelectedHourViewModel()
            : this(new SelectedHour())
        {
        }
        public SelectedHourViewModel(SelectedHour model)
            : base(model)
        {
        }

        public int Hour
        {
            get { return Model.Hour; }
            set { Model.Hour = value; RaisePropertyChanged("Hour"); RaisePropertyChanged("BindingHour"); }
        }

        public LessonHourViewModel BindingHour
        {
            get
            {
                return GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.FirstOrDefault(x => x.Number == this.Hour);
            }
            set
            {
                this.Hour = (value != null ? value.Number : 0);
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Hour);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();
            if (result == WorkingCopyResult.Cancel)
            {
                this.Hour = (int)pack.Read();
            }
        }
    }
}
