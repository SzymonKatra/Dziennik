using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class GlobalSubjectViewModel : ViewModelBase<GlobalSubjectViewModel, GlobalSubject>
    {
        public GlobalSubjectViewModel()
            : this(new GlobalSubject())
        {
        }
        public GlobalSubjectViewModel(GlobalSubject model) : base(model)
        {
        }

        private int m_numberCopy;
        public int Number
        {
            get { return Model.Number; }
            set { Model.Number = value; RaisePropertyChanged("Number"); }
        }
        private string m_nameCopy;
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); }
        }

        protected override void OnPushCopy()
        {
            m_numberCopy = this.Number;
            m_nameCopy = this.Name;
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            if(result == WorkingCopyResult.Cancel)
            {
                this.Number = m_numberCopy;
                this.Name = m_nameCopy;
            }
        }
    }
}
