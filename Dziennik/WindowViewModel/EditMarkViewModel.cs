using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;

namespace Dziennik.WindowViewModel
{
    public sealed class EditMarkViewModel : ObservableObject
    {
        public EditMarkViewModel(MarkViewModel mark)
        {
            m_mark = mark;

            m_value = m_mark.Value;
            m_description = m_mark.Description;

            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
        }

        private bool m_result;
        public bool Result
        {
            get { return m_result; }
        }

        private MarkViewModel m_mark;

        private decimal m_value;
        public decimal Value
        {
            get { return m_value; }
            set { m_value = value; OnPropertyChanged("Value"); m_okCommand.RaiseCanExecuteChanged(); }
        }

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; OnPropertyChanged("Description"); }
        }

        private RelayCommand m_okCommand;
        public RelayCommand OkCommand
        {
            get { return m_okCommand; }
        }

        private RelayCommand m_cancelCommand;
        public RelayCommand CancelCommand
        {
            get { return m_cancelCommand; }
        }

        public void Ok(object e)
        {
            m_mark.Value = m_value;
            m_mark.Description = m_description;
            m_mark.LastChangeDate = DateTime.Now;

            m_result = true;
            GlobalConfig.Dialogs.CloseDialog(this);
        }
        public bool CanOk(object e)
        {
            return (m_value >= 1M && m_value <= 6M);
        }
        public void Cancel(object e)
        {
            m_result = false;
            GlobalConfig.Dialogs.CloseDialog(this);
        }
    }
}
