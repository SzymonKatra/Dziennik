using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public EditMarkViewModel(MarkViewModel mark, DateTime dateStart, DateTime dateEnd, StudentInGroupViewModel owner = null, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeMarkCommand = new RelayCommand(RemoveMark, CanRemoveMark);

            m_availableCategories = new ObservableCollection<MarksCategoryViewModel>(GlobalConfig.GlobalDatabase.ViewModel.MarksCategories);
            m_availableCategories.Insert(0, NoSelectionMarksCategory);
            m_selectedCategory = NoSelectionMarksCategory;

            m_isAddingMode = isAddingMode;
            m_dateStart = dateStart;
            m_dateEnd = dateEnd;

            m_mark = mark;

            if(isAddingMode)
            {
                m_mark.Weight = 1;
            }
            m_selectedCategory = (m_mark.Category == null ? NoSelectionMarksCategory : m_mark.Category);

            m_valueInput = (isAddingMode ? string.Empty : MarkViewModel.GetValidDisplayedMark(m_mark.Value));
            m_noteInput = m_mark.Note;
            m_addDateInput = (isAddingMode ? DateTime.Now.Date : m_mark.AddDate);
            //m_weightInput = m_mark.Weight.ToString();

            m_noteSelected = !mark.IsValueValid;         

            m_title = string.Format("{0} - {1} {2}", owner.Number, owner.GlobalStudent.Name, owner.GlobalStudent.Surname);
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

        private DateTime m_dateStart;
        private DateTime m_dateEnd;

        private MarkViewModel m_mark;
        public MarkViewModel Mark
        {
            get { return m_mark; }
        }

        private string m_title = "Edytuj ocenę";
        public string Title
        {
            get { return m_title; }
            set { m_title = value; RaisePropertyChanged("Title"); }
        }

        private bool m_valueInputValid = false;
        private string m_valueInput;
        public string ValueInput
        {
            get { return m_valueInput; }
            set { m_valueInput = value; RaisePropertyChanged("ValueInput"); }
        }

        private bool m_noteInputValid = false;
        private string m_noteInput;
        public string NoteInput
        {
            get { return m_noteInput; }
            set { m_noteInput = value; RaisePropertyChanged("NoteInput"); }
        }

        private bool m_addDateInputValid = false;
        private DateTime m_addDateInput;
        public DateTime AddDateInput
        {
            get { return m_addDateInput; }
            set { m_addDateInput = value; RaisePropertyChanged("AddDateInput"); }
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

        private ObservableCollection<MarksCategoryViewModel> m_availableCategories;
        public ObservableCollection<MarksCategoryViewModel> AvailableCategories
        {
            get { return m_availableCategories; }
        }

        public static readonly MarksCategoryViewModel NoSelectionMarksCategory = new MarksCategoryViewModel() { Name = GlobalConfig.GetStringResource("lang_NoneSmall"), DefaultWeight=1 };
        private MarksCategoryViewModel m_selectedCategory;
        public MarksCategoryViewModel SelectedCategory
        {
            get { return m_selectedCategory; }
            set
            {
                m_selectedCategory = value; RaisePropertyChanged("SelectedCategory");
                Mark.Weight = m_selectedCategory.DefaultWeight;
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
                m_mark.Value = 0;
            }
            else
            {
                m_mark.Note = string.Empty;
            }
            m_mark.Category = (m_selectedCategory == NoSelectionMarksCategory ? null : m_selectedCategory);
            if(m_isAddingMode)
            {
                m_mark.AddDate = m_mark.LastChangeDate = m_addDateInput.Add(DateTime.Now.TimeOfDay);
            }

            m_result = EditMarkResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return (m_noteSelected ? m_noteInputValid : m_valueInputValid) && m_addDateInputValid;
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
                    case "AddDateInput": return ValidateAddDateInput();
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

            m_mark.Value = result;
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

            m_mark.Note = m_noteInput;
            m_noteInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
        private string ValidateAddDateInput()
        {
            m_addDateInputValid = false;

            string errorResult = GlobalValidateAddDate(m_addDateInput, m_dateStart, m_dateEnd);
            if (!string.IsNullOrEmpty(errorResult))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return errorResult;
            }

            m_mark.AddDate = m_addDateInput;
            m_addDateInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();

            return string.Empty;
        }
        //private string ValidateWeightInput()
        //{
        //    if (m_noteSelected) return string.Empty;
        //    m_weightInputValid = false;

        //    int result;

        //    string errorResult = GlobalValidateWeightInput(m_weightInput, out result);
        //    if(!string.IsNullOrEmpty(errorResult))
        //    {
        //        m_okCommand.RaiseCanExecuteChanged();
        //        return errorResult;
        //    }

        //    m_mark.Weight = result;
        //    m_weightInputValid = true;
        //    m_okCommand.RaiseCanExecuteChanged();

        //    return string.Empty;
        //}

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

            foreach (var c in input)
            {
                if(!char.IsLetter(c))
                {
                    return GlobalConfig.GetStringResource("lang_OnlyLetters");
                }
            }

            //bool hasLetter = false;
            //for (int i = 0; i < input.Length; i++)
            //{
            //    if (!char.IsLetterOrDigit(input[i]))
            //    {
            //        return GlobalConfig.GetStringResource("lang_InvalidCharacters");
            //    }
            //    if (char.IsLetter(input[i]))
            //    {
            //        hasLetter = true;
            //    }
            //}

            //if (!hasLetter)
            //{
            //    return GlobalConfig.GetStringResource("lang_TypeAtLeastOneLetter");
            //}

            return string.Empty;
        }
        public static string GlobalValidateAddDate(DateTime input, DateTime dateStart, DateTime dateEnd)
        {
            DateTime dateStartDay = dateStart.Date;
            DateTime dateEndDay = dateEnd.Date;

            if (input < dateStart || input > dateEnd)
            {
                return string.Format(GlobalConfig.GetStringResource("lang_AddDateOnlyInRange"), dateStartDay.ToString(GlobalConfig.DateFormat), dateEndDay.ToString(GlobalConfig.DateFormat));
            }

            return string.Empty;
        }
        //public static string GlobalValidateWeightInput(string input, out int result)
        //{
        //    if (!int.TryParse(input, out result))
        //    {
        //        return GlobalConfig.GetStringResource("lang_TypeValidInteger");
        //    }

        //    if (result < 0 || result > 9)
        //    {
        //        return GlobalConfig.GetStringResource("lang_TypeWeightRange0-9");
        //    }

        //    return string.Empty;
        //}
    }
}
