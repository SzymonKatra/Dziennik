using System;
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

            m_description = m_offDay.Description;
            m_from = m_offDay.Start;
            m_to = m_offDay.End;

            if(isAddingMode)
            {
                m_from = m_to = DateTime.Now.Date;
            }
        }

        private EditOffDayResult m_result = EditOffDayResult.Cancel;
        public EditOffDayResult Result
        {
            get { return m_result; }
        }

        private OffDayViewModel m_offDay;

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

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; RaisePropertyChanged("Name"); }
        }

        private DateTime m_from;
        public DateTime From
        {
            get { return m_from; }
            set
            {
                m_from = value; RaisePropertyChanged("From");
                if (m_to.Ticks == 0) To = m_from;
            }
        }

        private DateTime m_to;
        public DateTime To
        {
            get { return m_to; }
            set { m_to = value; RaisePropertyChanged("To"); }
        }

        private void Ok(object e)
        {
            m_offDay.Description = m_description;
            m_offDay.Start = m_from;
            m_offDay.End = m_to;

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
