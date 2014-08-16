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

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.RealizedDate);
            pack.Write(this.CustomSubject);
            pack.Write(this.IsCustom);
            pack.Write(this.Name);
            pack.Write(this.GlobalSubject);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.RealizedDate = (DateTime)pack.Read();
                this.CustomSubject = (string)pack.Read();
                this.GlobalSubject = (GlobalSubjectViewModel)pack.Read();
            }
        }
    }
}
