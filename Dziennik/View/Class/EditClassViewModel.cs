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
            m_nameInput = schoolClass.Name;
            m_selectedCalendar = m_originalCalendar = schoolClass.Calendar;
            m_selectedGroup = (schoolClass.Groups.Count > 0 ? schoolClass.Groups[0] : null);
        }

        private SchoolGroupViewModel m_selectedGroup;
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set { m_selectedGroup = value; RaisePropertyChanged("SelectedGroup"); m_editGroupCommand.RaiseCanExecuteChanged(); }
        }

        private bool m_selectedCalendarValid = false;
        private CalendarViewModel m_originalCalendar;
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

        private bool m_nameInputValid = false;
        private string m_nameInput;
        public string NameInput
        {
            get { return m_nameInput; }
            set { m_nameInput = value; RaisePropertyChanged("NameInput"); }
        }

        public string Path
        {
            get
            {
                string result = GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory + @"\" + m_nameInput + GlobalConfig.SchoolClassDatabaseFileExtension;
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
            m_schoolClass.Name = m_nameInput;
            m_schoolClass.Calendar = m_selectedCalendar;

            if(m_originalCalendar!= m_selectedCalendar)
            {
                foreach (var group in m_schoolClass.Groups)
                {
                    foreach (var student in group.Students)
                    {
                        student.RaiseAttendanceChanged();
                    }
                }
            }

            m_result = EditClassResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_nameInputValid && m_selectedCalendarValid;
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
            if (m_schoolClass.Students.Count <= 0)
            {
                GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_FirstAddStudents"), MessageBoxSuperPredefinedButtons.OK);
                return;
            }

            AddGroupViewModel dialogViewModel = new AddGroupViewModel(m_schoolClass.Students, m_schoolClass);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result != null)
            {
                m_schoolClass.Groups.Add(dialogViewModel.Result);
                SelectedGroup = dialogViewModel.Result;
            }
        }
        private void EditGroup(object param)
        {
            m_selectedGroup.PushCopy();
            EditGroupViewModel dialogViewModel = new EditGroupViewModel(m_selectedGroup, m_schoolClass.Students);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGroupViewModel.EditGroupResult.RemoveGroup)
            {
                m_selectedGroup.PopCopy(WorkingCopyResult.Ok);
                m_schoolClass.Groups.Remove(m_selectedGroup);
                SelectedGroup = null;
            }
            else if(dialogViewModel.Result == EditGroupViewModel.EditGroupResult.Ok)
            {
                m_selectedGroup.PopCopy(WorkingCopyResult.Ok);
            }
            else if(dialogViewModel.Result == EditGroupViewModel.EditGroupResult.Cancel)
            {
                m_selectedGroup.PopCopy(WorkingCopyResult.Cancel);
            }
        }
        private bool CanEditGroup(object param)
        {
            return m_selectedGroup != null;
        }
        private void ShowGlobalStudentsList(object param)
        {
            GlobalStudentsListViewModel dialogViewModel = new GlobalStudentsListViewModel(m_schoolClass.Students);
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
                    case "NameInput": return ValidateNameInput();
                    case "SelectedCalendar": return ValidateSelectedCalendar();
                }

                return string.Empty;
            }
        }

        private string ValidateNameInput()
        {
            m_nameInputValid = false;

            if (string.IsNullOrWhiteSpace(m_nameInput))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_TypeValidClassName");
            }

            m_nameInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
        private string ValidateSelectedCalendar()
        {
            m_selectedCalendarValid = false;

            if(m_selectedCalendar==null)
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_ChooseCalendar");
            }

            m_selectedCalendarValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
