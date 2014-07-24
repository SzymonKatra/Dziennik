using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dziennik.CommandUtils;
using System.Windows.Input;
using System.Globalization;

namespace Dziennik.View
{
    public class EditEndingMarkViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditEndingMarkResult
        {
            Ok,
            Cancel,
        }

        public EditEndingMarkViewModel(decimal initialMark, decimal averageMark)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);

            m_mark = initialMark;
            if (initialMark == 0M)
            {
                m_mark = decimal.Round(averageMark - 0.1M, MidpointRounding.AwayFromZero);
            }

            m_markInput = m_mark.ToString(CultureInfo.InvariantCulture);
        }

        private EditEndingMarkResult m_result = EditEndingMarkResult.Cancel;
        public EditEndingMarkResult Result
        {
            get { return m_result; }
        }

        private decimal m_mark;
        public decimal Mark
        {
            get { return m_mark; }
        }
        private bool m_markInputValid = false;
        private string m_markInput;
        public string MarkInput
        {
            get { return m_markInput; }
            set { m_markInput = value; RaisePropertyChanged("MarkInput"); }
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
            m_result = EditEndingMarkResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_markInputValid;
        }
        private void Cancel(object e)
        {
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
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
                    case "MarkInput": return ValidateMarkInput();
                }

                return string.Empty;
            }
        }

        private string ValidateMarkInput()
        {
            m_markInputValid = false;

            int res;
            if (!int.TryParse(m_markInput, out res))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return "Wprowadź poprawną liczbę całkowitą";
            }
            if (res < 0 || res > 6)
            {
                m_okCommand.RaiseCanExecuteChanged();
                return "Wprowadź ocenę z zakresu <1; 6> lub 0 jeśli chcesz ją anulować";
            }

            m_mark = res;
            m_markInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
