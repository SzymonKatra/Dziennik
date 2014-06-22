using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.ComponentModel;
using System.Globalization;

namespace Dziennik.View
{
    public sealed class EditMarkViewModel : ObservableObject, IDataErrorInfo
    {
        public EditMarkViewModel(MarkViewModel mark)
        {
            m_mark = mark;

            m_value = m_mark.Value;
            m_description = m_mark.Description;

            m_valueInput = m_value.ToString(CultureInfo.InvariantCulture);

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
        private bool m_valueInputValid = false;
        private string m_valueInput;
        public string ValueInput
        {
            get { return m_valueInput.ToString(); }
            set { m_valueInput = value; OnPropertyChanged("ValueInput"); }
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
            return m_valueInputValid;
        }
        public void Cancel(object e)
        {
            m_result = false;
            GlobalConfig.Dialogs.CloseDialog(this);
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "ValueInput": return ValidateValueInput();
                }

                return string.Empty;
            }
        }

        private string ValidateValueInput()
        {
            decimal result;

            if (!decimal.TryParse(m_valueInput, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
            {
                m_valueInputValid = false;
                m_okCommand.RaiseCanExecuteChanged();
                return "Wprowadź poprawną liczbę. Oddziel liczby kropką (.)";
            }

            if(result <1M || result > 6M)
            {
                m_valueInputValid = false;
                m_okCommand.RaiseCanExecuteChanged();
                return "Wprowadź ocenę z zakresu <1; 6>";
            }

            m_valueInputValid = true;
            Value = result;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
    }
}
