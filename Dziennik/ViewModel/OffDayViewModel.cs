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

        private DateTime m_startCopy;
        public DateTime Start
        {
            get { return Model.Start; }
            set { Model.Start = value; RaisePropertyChanged("Start"); RaisePropertyChanged("IsOneDay"); }
        }
        private DateTime m_endCopy;
        public DateTime End
        {
            get { return Model.End; }
            set { Model.End = value; RaisePropertyChanged("End"); RaisePropertyChanged("IsOneDay"); }
        }
        private string m_descriptionCopy;
        public string Description
        {
            get { return Model.Description; }
            set { Model.Description = value; RaisePropertyChanged("Description"); }
        }

        public bool IsOneDay
        {
            get { return Model.Start == Model.End; }
        }

        //public override void ShallowCopyDataTo(OffDayViewModel viewModel)
        //{
        //    viewModel.Description = this.Description;
        //    viewModel.Start = this.Start;
        //    viewModel.End = this.End;
        //}

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Start);
            pack.Write(this.End);
            pack.Write(this.Description);

            CopyStack.Push(pack);
            //m_startCopy = this.Start;
            //m_endCopy = this.End;
            //m_descriptionCopy = this.Description;
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.Start = (DateTime)pack.Read();
                this.End = (DateTime)pack.Read();
                this.Description = (string)pack.Read();
            }
        }
    }
}
