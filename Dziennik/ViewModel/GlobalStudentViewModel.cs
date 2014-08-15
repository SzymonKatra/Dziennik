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
        private string m_surnameCopy;
        public string Surname
        {
            get { return Model.Surname; }
            set { Model.Surname = value; RaisePropertyChanged("Surname"); }
        }
        private string m_emailCopy;
        public string Email
        {
            get { return Model.Email; }
            set { Model.Email = value; RaisePropertyChanged("Email"); }
        }
        private string m_additionalInformationCopy;
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

        protected override void OnWorkingCopyStarted()
        {
            m_numberCopy = this.Number;
            m_nameCopy = this.Name;
            m_surnameCopy = this.Surname;
            m_emailCopy = this.Email;
            m_additionalInformationCopy = this.AdditionalInformation;
        }
        protected override void OnWorkingCopyEnded(WorkingCopyResult result)
        {
            if(result == WorkingCopyResult.Cancel)
            {
                this.Number = m_numberCopy;
                this.Name = m_nameCopy;
                this.Surname = m_surnameCopy;
                this.Email = m_emailCopy;
                this.AdditionalInformation = m_additionalInformationCopy;
            }
        }
    }
}
