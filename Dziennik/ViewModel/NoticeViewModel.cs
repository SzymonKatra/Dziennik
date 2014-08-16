using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class NoticeViewModel : ViewModelBase<NoticeViewModel, Notice>
    {
        public NoticeViewModel()
            : this(new Notice())
        {
        }
        public NoticeViewModel(Notice model)
            : base(model)
        {
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); RaisePropertyChanged("DisplayedName"); }
        }
        public DateTime Date
        {
            get { return Model.Date; }
            set { Model.Date = value; RaisePropertyChanged("Date"); }
        }
        public TimeSpan NotifyIn
        {
            get { return Model.NotifyIn; }
            set { Model.NotifyIn = value; RaisePropertyChanged("NotifyIn"); }
        }

        public string DisplayedName
        {
            get
            {
                return (Model.Name.Length > 15 ? Model.Name.Remove(15) + "..." : Model.Name);
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();

            pack.Write(this.Name);
            pack.Write(this.Date);
            pack.Write(this.NotifyIn);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.Name = (string)pack.Read();
                this.Date = (DateTime)pack.Read();
                this.NotifyIn = (TimeSpan)pack.Read();
            }
        }
    }
}
