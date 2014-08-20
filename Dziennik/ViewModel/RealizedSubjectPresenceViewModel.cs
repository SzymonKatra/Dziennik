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
        public RealizedSubjectPresenceViewModel(RealizedSubjectPresence model)
            : base(model)
        {
        }

        private RealizedSubjectViewModel m_realizedSubject;
        [DatabaseRelationProperty("GroupRealizedSubjects", "RealizedSubjectId")]
        public RealizedSubjectViewModel RealizedSubject
        {
            get { return m_realizedSubject; }
            set { m_realizedSubject = value; RaisePropertyChanged("RealizedSubject"); }
        }

        public PresenceType Presence
        {
            get { return Model.Presence; }
            set { Model.Presence = value; RaisePropertyChanged("Presence"); RaisePropertyChanged("WasPresent"); }
        }

        public bool WasPresent
        {
            get { return Model.Presence == PresenceType.Present || Model.Presence == PresenceType.Late; }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.RealizedSubject);
            pack.Write(this.Presence);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.RealizedSubject = (RealizedSubjectViewModel)pack.Read();
                this.Presence = (PresenceType)pack.Read();
            }
        } 
    }
}
