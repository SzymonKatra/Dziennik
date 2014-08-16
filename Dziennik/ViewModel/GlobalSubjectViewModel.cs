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

        public int Number
        {
            get { return Model.Number; }
            set { Model.Number = value; RaisePropertyChanged("Number"); }
        }
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Number);
            pack.Write(this.Name);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.Number = (int)pack.Read();
                this.Name = (string)pack.Read();
            }
        }
    }
}
