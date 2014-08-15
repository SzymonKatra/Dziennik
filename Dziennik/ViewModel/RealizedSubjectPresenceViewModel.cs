using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class RealizedSubjectPresenceViewModel : ViewModelBase<RealizedSubjectPresenceViewModel, RealizedSubjectPresence>
    {
        public RealizedSubjectPresenceViewModel()
            : this(new RealizedSubjectPresence())
        {
        }
        public RealizedSubjectPresenceViewModel(RealizedSubjectPresence model) : base(model)
        {
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
            get { return Model.WasPresent; }
            set { Model.WasPresent = value; RaisePropertyChanged("WasPresent"); }
        }

        public override void CopyDataTo(RealizedSubjectPresenceViewModel viewModel)
        {
            viewModel.RealizedSubject = this.RealizedSubject;
            viewModel.WasPresent = this.WasPresent;
        }
    }
}
