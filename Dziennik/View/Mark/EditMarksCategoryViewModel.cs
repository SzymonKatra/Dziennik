using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Dziennik.CommandUtils;
using System.Windows.Input;
using System.ComponentModel;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public sealed class EditMarksCategoryViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditMarkCategoryResult
        {
            Ok,
            Cancel,
            RemoveCategory,
        }

        public EditMarksCategoryViewModel(MarksCategoryViewModel marksCategory, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeCategoryCommand = new RelayCommand(RemoveCategory, CanRemoveCategory);

            m_isAddingMode = isAddingMode;

            m_marksCategory = marksCategory;
        }

        private EditMarkCategoryResult m_result = EditMarkCategoryResult.Cancel;
        public EditMarkCategoryResult Result
        {
            get { return m_result; }
        }

        private MarksCategoryViewModel m_marksCategory;

        private bool m_isAddingMode;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
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

        private RelayCommand m_removeCategoryCommand;
        public ICommand RemoveCategoryCommand
        {
            get { return m_removeCategoryCommand; }
        }

        private bool m_nameValid = false;
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; RaisePropertyChanged("Name"); }
        }

        private System.Windows.Media.Color m_selectedColor;
        public System.Windows.Media.Color SelectedColor
        {
            get { return m_selectedColor; }
            set { m_selectedColor = value; RaisePropertyChanged("SelectedColor"); }
        }

        private void Ok(object e)
        {
            m_marksCategory.Name = m_name;
            m_marksCategory.Color = m_selectedColor;

            m_result = EditMarkCategoryResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_nameValid;
        }
        private void Cancel(object e)
        {
            m_result = EditMarkCategoryResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void RemoveCategory(object e)
        {
            m_result = EditMarkCategoryResult.RemoveCategory;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveCategory(object e)
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
                    case "Name": return ValidateName();
                }
                return string.Empty;
            }
        }
        public string ValidateName()
        {
            m_nameValid = false;

            if(string.IsNullOrWhiteSpace(m_name))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_TypeValidCategoryName");
            }

            m_nameValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
