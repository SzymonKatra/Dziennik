﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;

namespace Dziennik.View
{
    public sealed class OverdueSubjectsListViewModel : ObservableObject
    {
        public enum OverdueSubjectsListResult
        {
            Close,
            RealizeSubject,
        }

        public OverdueSubjectsListViewModel(IEnumerable<DateTime> overdueSubjects)
        {
            m_realizeSelectedSubjectCommand = new RelayCommand(RealizeSelectedSubject);

            m_overdueSubjects = overdueSubjects;
        }

        private OverdueSubjectsListResult m_result = OverdueSubjectsListResult.Close;
        public OverdueSubjectsListResult Result
        {
            get { return m_result; }
        }

        private IEnumerable<DateTime> m_overdueSubjects;
        public IEnumerable<DateTime> OverdueSubjects
        {
            get { return m_overdueSubjects; }
        }

        private RelayCommand m_realizeSelectedSubjectCommand;
        public ICommand RealizeSelectedSubjectCommand
        {
            get { return m_realizeSelectedSubjectCommand; }
        }

        private DateTime m_selectedSubject;
        public DateTime SelectedSubject
        {
            get { return m_selectedSubject; }
            set { m_selectedSubject = value; RaisePropertyChanged("SelectedSubject"); }
        }

        private void RealizeSelectedSubject(object param)
        {
            m_result = OverdueSubjectsListResult.RealizeSubject;
            GlobalConfig.Dialogs.Close(this);
        }
    }
}