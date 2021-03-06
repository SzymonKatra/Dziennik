﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;

namespace Dziennik.View
{
    public class EditOffDayViewModel : ObservableObject
    {
        public enum EditOffDayResult
        {
            Ok,
            Cancel,
            Remove,
        }

        public EditOffDayViewModel(OffDayViewModel offDay, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeOffDayCommand = new RelayCommand(RemoveOffDay, CanRemoveOffDay);

            m_offDay = offDay;
            m_isAddingMode = isAddingMode;

            if(isAddingMode)
            {
                offDay.Start = offDay.End = DateTime.Now.Date;
            }
        }

        private EditOffDayResult m_result = EditOffDayResult.Cancel;
        public EditOffDayResult Result
        {
            get { return m_result; }
        }

        private OffDayViewModel m_offDay;
        public OffDayViewModel OffDay
        {
            get { return m_offDay; }
        }

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

        private RelayCommand m_removeOffDayCommand;
        public ICommand RemoveOffDayCommand
        {
            get { return m_removeOffDayCommand; }
        }

        private void Ok(object e)
        {
            m_result = EditOffDayResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object e)
        {
            m_result = EditOffDayResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void RemoveOffDay(object e)
        {
            m_result = EditOffDayResult.Remove;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveOffDay(object e)
        {
            return !m_isAddingMode;
        }
    }
}
