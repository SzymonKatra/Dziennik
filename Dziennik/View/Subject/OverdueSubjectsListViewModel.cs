using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.View
{
    public sealed class OverdueSubjectsListViewModel
    {
        public OverdueSubjectsListViewModel(IEnumerable<DateTime> overdueSubjects)
        {
            m_overdueSubjects = overdueSubjects;
        }

        private IEnumerable<DateTime> m_overdueSubjects;
        public IEnumerable<DateTime> OverdueSubjects
        {
            get { return m_overdueSubjects; }
        }
    }
}
