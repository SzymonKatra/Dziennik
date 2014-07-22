using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.CommandUtils;
using System.Windows.Input;

namespace Dziennik.View
{
    public sealed class TypeSubjectCategoryViewModel : ObservableObject
    {
        public enum TypeSubjectCategoryResult
        {
            Ok,
            Cancel,
        }

        public TypeSubjectCategoryViewModel()
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
        }

        private TypeSubjectCategoryResult m_result = TypeSubjectCategoryResult.Cancel;
        public TypeSubjectCategoryResult Result
        {
            get { return m_result; }
        }

        private string m_category;
        public string Category
        {
            get { return m_category; }
            set { m_category = value; RaisePropertyChanged("Category"); }
        }

        private RelayCommand m_okCommand;
        public ICommand OkCommand
        {
            get { return m_okCommand; }
        }

        private RelayCommand m_cancelCommand;
        public ICommand CancelCommand
        {
            get { return m_cancelCommand; }
        }

        private void Ok(object e)
        {
            m_result = TypeSubjectCategoryResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object e)
        {
            m_result = TypeSubjectCategoryResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
    }
}
