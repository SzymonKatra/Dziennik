using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.IO;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class EditClassViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditClassResult
        {
            Ok,
            Cancel,
            RemoveClass,
        }

        public EditClassViewModel(SchoolClassViewModel schoolClass)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeClassCommand = new RelayCommand(RemoveClass, CanRemoveClass);
            m_showGlobalStudentsListCommand = new RelayCommand(ShowGlobalStudentsList);
            m_addGroupCommand = new RelayCommand(AddGroup);
            m_editGroupCommand = new RelayCommand(EditGroup, CanEditGroup);

            m_schoolClass = schoolClass;
            m_name = schoolClass.Name;
            m_students = new WorkingCopyCollection<GlobalStudentViewModel>(schoolClass.Students);
            m_groups = new WorkingCopyCollection<SchoolGroupViewModel>(schoolClass.Groups);

            m_selectedCalendar = schoolClass.Calendar;
        } 

        private SchoolGroupViewModel m_selectedGroup;
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set { m_selectedGroup = value; RaisePropertyChanged("SelectedGroup"); m_editGroupCommand.RaiseCanExecuteChanged(); }
        }

        private CalendarViewModel m_selectedCalendar;
        public CalendarViewModel SelectedCalendar
        {
            get { return m_selectedCalendar; }
            set { m_selectedCalendar = value; RaisePropertyChanged("SelectedCalendar"); }
        }

        private EditClassResult m_result = EditClassResult.Cancel;
        public EditClassResult Result
        {
            get { return m_result; }
        }

        private bool m_nameValid = false;
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; RaisePropertyChanged("Name"); }
        }

        private WorkingCopyCollection<GlobalStudentViewModel> m_students;
        public WorkingCopyCollection<GlobalStudentViewModel> Students
        {
            get { return m_students; }
        }

        private WorkingCopyCollection<SchoolGroupViewModel> m_groups;
        public WorkingCopyCollection<SchoolGroupViewModel> Groups
        {
            get { return m_groups; }
        }

        public string Path
        {
            get
            {
                string result = GlobalConfig.Notifier.DatabasesDirectory + @"\" + m_name + GlobalConfig.SchoolClassDatabaseFileExtension;
                foreach (char c in System.IO.Path.GetInvalidPathChars())
                {
                    result = result.Replace(c.ToString(), "");
                }
                return result;
            }
        }

        private bool m_isAddingMode = false;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
            set { m_isAddingMode = value; RaisePropertyChanged("IsAddingMode"); m_removeClassCommand.RaiseCanExecuteChanged(); }
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

        private RelayCommand m_removeClassCommand;
        public ICommand RemoveClassCommand
        {
            get { return m_removeClassCommand; }
        }

        private RelayCommand m_addGroupCommand;
        public ICommand AddGroupCommand
        {
            get { return m_addGroupCommand; }
        }

        private RelayCommand m_editGroupCommand;
        public ICommand EditGroupCommand
        {
            get { return m_editGroupCommand; }
        }

        private RelayCommand m_showGlobalStudentsListCommand;
        public ICommand ShowGlobalStudentsListCommand
        {
            get { return m_showGlobalStudentsListCommand; }
        }

        private SchoolClassViewModel m_schoolClass;
        public SchoolClassViewModel SchoolClass
        {
            get { return m_schoolClass; }
        }

        private void Ok(object param)
        {
            m_schoolClass.Name = m_name;
            m_students.ApplyChangesToOriginalCollection();
            m_groups.ApplyChangesToOriginalCollection();
            m_schoolClass.Calendar = m_selectedCalendar;

            m_result = EditClassResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_nameValid;
        }
        private void Cancel(object e)
        {
            m_result = EditClassResult.Cancel;
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void RemoveClass(object param)
        {
            if (GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_DoYouWantRemoveClass"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

            m_result = EditClassResult.RemoveClass;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveClass(object param)
        {
            return !m_isAddingMode;
        }
        private void AddGroup(object param)
        {
            AddGroupViewModel dialogViewModel = new AddGroupViewModel(m_schoolClass.Students);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result != null)
            {
                m_groups.Add(dialogViewModel.Result);
                SelectedGroup = dialogViewModel.Result;
            }
        }
        private void EditGroup(object param)
        {
            EditGroupViewModel dialogViewModel = new EditGroupViewModel(m_selectedGroup, m_schoolClass.Students);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGroupViewModel.EditGroupResult.RemoveGroup)
            {
                //m_schoolClass.Groups.Remove(m_selectedGroup);
                m_groups.Remove(m_selectedGroup);
                SelectedGroup = null;
            }
            if (dialogViewModel.Result == EditGroupViewModel.EditGroupResult.Ok) m_groups.ApplyChange(m_selectedGroup);
        }
        private bool CanEditGroup(object param)
        {
            return m_selectedGroup != null;
        }
        private void ShowGlobalStudentsList(object param)
        {
            GlobalStudentsListViewModel dialogViewModel = new GlobalStudentsListViewModel(m_students);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
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

        private string ValidateName()
        {
            m_nameValid = false;

            if (string.IsNullOrWhiteSpace(m_name))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_TypeValidClassName");
            }

            m_nameValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
