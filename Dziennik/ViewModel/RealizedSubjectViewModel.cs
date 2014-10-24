using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class RealizedSubjectViewModel : ViewModelBase<RealizedSubjectViewModel,RealizedSubject>
    {
        public RealizedSubjectViewModel()
            : this(new RealizedSubject())
        {
        }
        public RealizedSubjectViewModel(RealizedSubject model)
            : base(model)
        {
        }

        private SchoolGroupViewModel m_ownerGroup;
        [DatabaseIgnoreSearchRelations]
        public SchoolGroupViewModel OwnerGroup
        {
            get { return m_ownerGroup; }
            set { m_ownerGroup = value; }
        }

        public int RealizedHour
        {
            get { return Model.RealizedHour; }
            set { Model.RealizedHour = value; RaisePropertyChanged("RealizedHour"); }
        }
        public DateTime RealizedDate
        {
            get { return Model.RealizedDate; }
            set { Model.RealizedDate = value; RaisePropertyChanged("RealizedDate"); }
        }
        public string CustomSubject
        {
            get { return Model.CustomSubject; }
            set { Model.CustomSubject = value; m_globalSubject = null; RaisePropertyChanged("CustomSubject"); RaisePropertyChanged("IsCustom"); }
        }
        private GlobalSubjectViewModel m_globalSubject;
        [DatabaseRelationProperty("GlobalSubjects", "GlobalSubjectId")]
        public GlobalSubjectViewModel GlobalSubject
        {
            get { return m_globalSubject; }
            set { m_globalSubject = value; Model.CustomSubject = string.Empty; RaisePropertyChanged("GlobalSubject"); RaisePropertyChanged("IsCustom"); }
        }

        public bool IsCustom
        {
            get { return m_globalSubject == null; }
        }
        public string Name
        {
            get
            {
                return (IsCustom ? CustomSubject : (m_globalSubject == null ? "ERROR" : m_globalSubject.Name));
            }
        }
        public string AbsentsDisplay
        {
            get
            {
                string result = string.Empty;
                if (m_ownerGroup == null) return result;

                foreach (var student in m_ownerGroup.Students)
                {
                    foreach (var presence in student.Presence)
                    {
                        if (presence.RealizedSubject == this && !presence.WasPresent && presence.Presence != PresenceType.None)
                        {
                            result += student.Number + ", ";
                        }
                    }
                }

                if (result.Length > 0) result = result.Remove(result.Length - 2);

                return result;
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.RealizedHour);
            pack.Write(this.RealizedDate);
            pack.Write(this.CustomSubject);
            pack.Write(this.GlobalSubject);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.RealizedHour = (int)pack.Read();
                this.RealizedDate = (DateTime)pack.Read();
                this.CustomSubject = (string)pack.Read();
                this.GlobalSubject = (GlobalSubjectViewModel)pack.Read();
            }
        }
    }
}
