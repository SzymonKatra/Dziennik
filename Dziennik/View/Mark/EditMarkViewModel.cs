﻿using System;
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

        public EditMarkViewModel(MarkViewModel mark, StudentInGroupViewModel owner = null, bool isAddingMode = false)
        {
            m_isAddingMode = isAddingMode;

            m_mark = mark;

            m_value = m_mark.Value;
            m_note = m_mark.Note;
            m_weight = (isAddingMode ? 1 : m_mark.Weight);
            m_description = m_mark.Description;

            m_valueInput = (isAddingMode ? string.Empty : MarkViewModel.GetValidDisplayedMark(m_value));
            m_noteInput = m_note;
            m_weightInput = m_weight.ToString();

            m_noteSelected = !mark.IsValueValid;

            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeMarkCommand = new RelayCommand(RemoveMark, CanRemoveMark);

            if (owner != null) m_title = string.Format("{0} - {1} {2}", owner.Number, owner.GlobalStudent.Name, owner.GlobalStudent.Surname);
        }

        private EditMarkResult m_result = EditMarkResult.Cancel;
        public EditMarkResult Result
        {
            get { return m_result; }
        }

        private bool m_isAddingMode = false;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
            //set { m_isAddingMode = value; RaisePropertyChanged("IsAddingMode"); m_removeMarkCommand.RaiseCanExecuteChanged(); }
        }

        private MarkViewModel m_mark;

        private string m_title = "Edytuj ocenę";
        public string Title
        {
            get { return m_title; }
            set { m_title = value; RaisePropertyChanged("Title"); }
        }

        private decimal m_value;
        private bool m_valueInputValid = false;
        private string m_valueInput;
        public string ValueInput
        {
            get { return m_valueInput; }
            set { m_valueInput = value; RaisePropertyChanged("ValueInput"); }
        }

        private string m_note;
        private bool m_noteInputValid = false;
        private string m_noteInput;
        public string NoteInput
        {
            get { return m_noteInput; }
            set { m_noteInput = value; RaisePropertyChanged("NoteInput"); }
        }

        private int m_weight;
        private bool m_weightInputValid = false;
        private string m_weightInput;
        public string WeightInput
        {
            get { return m_weightInput; }
            set { m_weightInput = value; RaisePropertyChanged("WeightInput"); }
        }

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; RaisePropertyChanged("Description"); }
        }

        private bool m_noteSelected;
        public bool NoteSelected
        {
            get { return m_noteSelected; }
            set
            {
                m_noteSelected = value;
                RaisePropertyChanged("NoteSelected");
                RaisePropertyChanged("ValueInput"); // to remove red border if is error in textbox
                RaisePropertyChanged("WeightInput"); // as above
                RaisePropertyChanged("NoteInput"); // validation returns string.Empty if textbox is not selected by radiobutton
                m_okCommand.RaiseCanExecuteChanged();
            }
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
            if (m_noteSelected)
            {
                m_mark.Note = m_note;
                m_mark.Value = 0;
            }
            else
            {
                m_mark.Value = m_value;
                m_mark.Note = string.Empty;
            }
            m_mark.Weight = m_weight;
            m_mark.Description = m_description;
            m_mark.LastChangeDate = DateTime.Now;
            if (m_isAddingMode) m_mark.AddDate = m_mark.LastChangeDate;

            m_result = EditMarkResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return (m_noteSelected ? m_noteInputValid : m_valueInputValid && m_weightInputValid);
        }
        private void Cancel(object e)
        {
            m_result = EditMarkResult.Cancel;
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void RemoveMark(object e)
        {
            if(MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Czy na pewno chcesz usunąć ocenę?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes)
            {
                return;
            }

            m_result = EditMarkResult.RemoveMark;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveMark(object e)
        {
            return !m_isAddingMode;
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ValueInput": return ValidateValueInput();
                    case "NoteInput": return ValidateNoteInput();
                    case "WeightInput": return ValidateWeightInput();
                }

                return string.Empty;
            }
        }

        private string ValidateValueInput()
        {
            if (m_noteSelected) return string.Empty;
            m_valueInputValid = false;

            decimal result;
            string errorResult = GlobalValidateValue(m_valueInput, out result);
            if(!string.IsNullOrEmpty(errorResult))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return errorResult;
            }

            m_value = result;
            m_valueInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
        private string ValidateNoteInput()
        {
            if (!m_noteSelected) return string.Empty;
            m_noteInputValid = false;

            string errorResult = GlobalValidateNote(m_noteInput);
            if(!string.IsNullOrEmpty(errorResult))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return errorResult;
            }

            m_note = m_noteInput;
            m_noteInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
        private string ValidateWeightInput()
        {
            if (m_noteSelected) return string.Empty;
            m_weightInputValid = false;

            int result;

            string errorResult = GlobalValidateWeightInput(m_weightInput, out result);
            if(!string.IsNullOrEmpty(errorResult))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return errorResult;
            }

            m_weight = result;
            m_weightInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }

        public static string GlobalValidateValue(string input, out decimal result)
        {
            result = 0M;
            int integralResult;

            string toParse = Ext.RemoveAllWhitespaces(input);
            bool hadDigit = false;
            bool hadPlus = false;
            foreach (char c in toParse)
            {
                if (char.IsDigit(c))
                {
                    hadDigit = true;
                    continue;
                }
                if (c == '+')
                {
                    if (!hadDigit)
                    {
                        return GlobalConfig.GetStringResource("lang_PlusMustBeAfterValue");
                    }
                    hadPlus = true;
                    break;
                }
            }
            if (toParse.Count(x => x == '+') > 1)
            {
                return GlobalConfig.GetStringResource("lang_PlusCanBeOnlyOne");
            }
            toParse = toParse.Replace("+", "");

            if (!int.TryParse(toParse, out integralResult))
            {
                return GlobalConfig.GetStringResource("lang_InvalidCharacters");
            }

            if (integralResult < 1M || integralResult > 6M)
            {
                return GlobalConfig.GetStringResource("lang_TypeIntegerMarkRange1-6Plus");
            }

            result = (decimal)integralResult + (hadPlus ? 0.5M : 0M);

            return string.Empty;
        }
        public static string GlobalValidateNote(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length < 1 || input.Length > 2)
            {
                return GlobalConfig.GetStringResource("lang_LengthMustBe1Or2");
            }

            bool hasLetter = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsLetterOrDigit(input[i]))
                {
                    return GlobalConfig.GetStringResource("lang_InvalidCharacters");
                }
                if (char.IsLetter(input[i]))
                {
                    hasLetter = true;
                }
            }

            if (!hasLetter)
            {
                return GlobalConfig.GetStringResource("lang_TypeAtLeastOneLetter");
            }

            return string.Empty;
        }
        public static string GlobalValidateWeightInput(string input, out int result)
        {
            if (!int.TryParse(input, out result))
            {
                return GlobalConfig.GetStringResource("lang_TypeValidInteger");
            }

            if (result < 0)
            {
                return GlobalConfig.GetStringResource("lang_TypePositiveValue");
            }

            return string.Empty;
        }
    }
}
