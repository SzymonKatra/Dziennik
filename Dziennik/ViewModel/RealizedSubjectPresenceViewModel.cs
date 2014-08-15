using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class RealizedSubjectPresenceViewModel : ObservableObject, IModelExposable<RealizedSubjectPresence>
    {
        public RealizedSubjectPresenceViewModel()
            : this(new RealizedSubjectPresence())
        {
        }
        public RealizedSubjectPresenceViewModel(RealizedSubjectPresence model)
        {
            m_model = model;
        }

        private RealizedSubjectPresence m_model;
        public RealizedSubjectPresence Model
        {
            get { return m_model; }
        }

        private RealizedSubjectViewModel m_realizedSubject;
        [DatabaseRelationProperty("GroupRealizedSubjects", "RealizedSubjectId")]
        public RealizedSubjectViewModel RealizedSubject
        {
            get { return m_realizedSubject; }
            set { m_realizedSubject = value; RaisePropertyChanged("RealizedSubject"); }
        }

        public bool WasPresent
        {
            get { return m_model.WasPresent; }
            set { m_model.WasPresent = value; RaisePropertyChanged("WasPresent"); }
        }
    }
}
