using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class OffDayViewModel : ViewModelBase<OffDayViewModel, OffDay>
    {
        public OffDayViewModel()
            : this(new OffDay())
        {
        }
        public OffDayViewModel(OffDay model)
            : base(model)
        {
        }

        public DateTime Start
        {
            get { return Model.Start; }
            set { Model.Start = value; RaisePropertyChanged("Start"); RaisePropertyChanged("IsOneDay"); }
        }
        public DateTime End
        {
            get { return Model.End; }
            set { Model.End = value; RaisePropertyChanged("End"); RaisePropertyChanged("IsOneDay"); }
        }
        public bool IsOneDay
        {
            get { return Model.Start == Model.End; }
        }
        public string Description
        {
            get { return Model.Description; }
            set { Model.Description = value; RaisePropertyChanged("Description"); }
        }

        public override void CopyDataTo(OffDayViewModel viewModel)
        {
            viewModel.Description = this.Description;
            viewModel.Start = this.Start;
            viewModel.End = this.End;
        }
    }
}
