using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class RealizedSubjectViewModel : ObservableObject, IModelExposable<RealizedSubject>
    {
        public RealizedSubjectViewModel()
            : this(new RealizedSubject())
        {
        }
        public RealizedSubjectViewModel(RealizedSubject model)
        {
            m_model = model;
        }

        private RealizedSubject m_model;
        public RealizedSubject Model
        {
            get { return m_model; }
        }

        public DateTime RealizedDate
        {
            get { return m_model.RealizedDate; }
            set { m_model.RealizedDate = value; RaisePropertyChanged("RealizedDate"); }
        }

        private GlobalSubjectViewModel m_globalSubject;
        [DatabaseRelationProperty("GlobalSubjects", "GlobalSubjectId")]
        public GlobalSubjectViewModel GlobalSubject
        {
            get { return m_globalSubject; }
            set { m_globalSubject = value; RaisePropertyChanged("GlobalSubject"); }
        }
    }
}
