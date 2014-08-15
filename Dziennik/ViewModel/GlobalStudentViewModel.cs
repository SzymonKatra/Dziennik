using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class GlobalStudentViewModel : ViewModelBase<GlobalStudentViewModel, GlobalStudent>
    {
        public GlobalStudentViewModel()
            : this(new GlobalStudent())
        {
        }
        public GlobalStudentViewModel(GlobalStudent model) : base(model)
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
        public string Surname
        {
            get { return Model.Surname; }
            set { Model.Surname = value; RaisePropertyChanged("Surname"); }
        }
        public string Email
        {
            get { return Model.Email; }
            set { Model.Email = value; RaisePropertyChanged("Email"); }
        }
        public string AdditionalInformation
        {
            get { return Model.AdditionalInformation; }
            set { Model.AdditionalInformation = value; RaisePropertyChanged("AdditionalInformation"); }
        }

        public static GlobalStudentViewModel Dummy
        {
            get
            {
                GlobalStudentViewModel result = new GlobalStudentViewModel();

                result.Number = -1;
                result.Name = "----------";
                result.Surname = "----------";
                result.Email = "----------";
                result.AdditionalInformation = string.Empty;

                return result;
            }
        }

        public override void CopyDataTo(GlobalStudentViewModel viewModel)
        {
            viewModel.Number = this.Number;
            viewModel.Name = this.Name;
            viewModel.Surname = this.Surname;
            viewModel.Email = this.Email;
            viewModel.AdditionalInformation = this.AdditionalInformation;
        }
    }
}
