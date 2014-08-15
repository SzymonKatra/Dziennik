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

        private string m_nameCopy;
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); RaisePropertyChanged("DisplayedName"); }
        }
        private DateTime m_dateCopy;
        public DateTime Date
        {
            get { return Model.Date; }
            set { Model.Date = value; RaisePropertyChanged("Date"); }
        }
        private TimeSpan m_notifyInCopy;
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

        protected override void OnWorkingCopyStarted()
        {
            m_nameCopy = this.Name;
            m_dateCopy = this.Date;
            m_notifyInCopy = this.NotifyIn;
        }
        protected override void OnWorkingCopyEnded(WorkingCopyResult result)
        {
            if(result == WorkingCopyResult.Cancel)
            {
                this.Name = m_nameCopy;
                this.Date = m_dateCopy;
                this.NotifyIn = m_notifyInCopy;
            }
        }
    }
}
