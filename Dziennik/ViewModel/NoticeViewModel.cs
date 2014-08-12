using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class NoticeViewModel : ObservableObject, IModelExposable<Notice>
    {
        public NoticeViewModel()
            : this(new Notice())
        {
        }
        public NoticeViewModel(Notice model)
        {
            m_model = model;
        }

        private Notice m_model;
        public Notice Model
        {
            get { return m_model; }
        }

        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; RaisePropertyChanged("Name"); RaisePropertyChanged("DisplayedName"); }
        }
        public DateTime Date
        {
            get { return m_model.Date; }
            set { m_model.Date = value; RaisePropertyChanged("Date"); }
        }
        public TimeSpan NotifyIn
        {
            get { return m_model.NotifyIn; }
            set { m_model.NotifyIn = value; RaisePropertyChanged("NotifyIn"); }
        }

        public string DisplayedName
        {
            get
            {
                return (m_model.Name.Length > 15 ? m_model.Name.Remove(15) + "..." : m_model.Name);
            }
        }
    }
}
