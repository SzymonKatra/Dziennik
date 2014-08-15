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

        public override void CopyDataTo(GlobalSubjectViewModel viewModel)
        {
            viewModel.Number = this.Number;
            viewModel.Name = this.Name;
        }
    }
}
