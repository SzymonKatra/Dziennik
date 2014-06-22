using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class EditMarkViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditMarkResult
        {
            Ok,
            Cancel,
            RemoveMark,
        }

        public EditMarkViewModel(MarkViewModel mark)
        {
            m_mark = mark;

            m_value = m_mark.Value;
            m_description = m_mark.Description;

            m_valueInput = m_value.ToString(CultureInfo.InvariantCulture);

            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeMarkCommand = new RelayCommand(RemoveMark);
        }

        private EditMarkResult m_result;
        public EditMarkResult Result
        {
            get { return m_result; }
        }

        private MarkViewModel m_mark;

        private decimal m_value;
        private bool m_valueInputValid = false;
        private string m_valueInput;
        public string ValueInput
        {
            get { return m_valueInput; }
            set { m_valueInput = value; OnPropertyChanged("ValueInput"); }
        }

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; OnPropertyChanged("Description"); }
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

        private RelayCommand m_removeMarkCommand;
        public ICommand RemoveMarkCommand
        {
            get { return m_removeMarkCommand; }
        }

        private void Ok(object e)
        {
            m_mark.Value = m_value;
            m_mark.Description = m_description;
            m_mark.LastChangeDate = DateTime.Now;

            m_result = EditMarkResult.Ok;
            GlobalConfig.Dialogs.CloseDialog(this);
        }
        private bool CanOk(object e)
        {
            return m_valueInputValid;
        }
        private void Cancel(object e)
        {
            m_result = EditMarkResult.Cancel;
            GlobalConfig.Dialogs.CloseDialog(this);
        }
        private void RemoveMark(object e)
        {
            if(MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Czy na pewno chcesz usunąć ocenę?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes)
            {
                return;
            }

            m_result = EditMarkResult.RemoveMark;
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

            m_value = result;

            m_valueInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
    }
}
