using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class OffDayViewModel : ObservableObject, IModelExposable<OffDay>
    {
        public OffDayViewModel() : this(new OffDay())
        {
        }
        public OffDayViewModel(OffDay model)
        {
            m_model = model;
        }

        private OffDay m_model;
        public OffDay Model
        {
            get { return m_model; }
        }

        public DateTime Start
        {
            get { return m_model.Start; }
            set { m_model.Start = value; RaisePropertyChanged("Start"); RaisePropertyChanged("IsOneDay"); }
        }
        public DateTime End
        {
            get { return m_model.End; }
            set { m_model.End = value; RaisePropertyChanged("End"); RaisePropertyChanged("IsOneDay"); }
        }
        public bool IsOneDay
        {
            get { return m_model.Start == m_model.End; }
        }
        public string Description
        {
            get { return m_model.Description; }
            set { m_model.Description = value; RaisePropertyChanged("Description"); }
        }
    }
}
