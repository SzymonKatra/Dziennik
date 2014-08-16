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

        public GlobalStudentViewModel DeepClone()
        {
            GlobalStudentViewModel result = new GlobalStudentViewModel();

            result.Number = this.Number;
            result.Name = this.Name;
            result.Surname = this.Surname;
            result.Email = this.Email;
            result.AdditionalInformation = this.AdditionalInformation;

            return result;
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Number);
            pack.Write(this.Name);
            pack.Write(this.Surname);
            pack.Write(this.Email);
            pack.Write(this.AdditionalInformation);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.Number = (int)pack.Read();
                this.Name = (string)pack.Read();
                this.Surname = (string)pack.Read();
                this.Email = (string)pack.Read();
                this.AdditionalInformation = (string)pack.Read();
            }
        }
    }
}
