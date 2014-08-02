using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class CalendarViewModel : ObservableObject, IModelExposable<Calendar>
    {
        public CalendarViewModel()
            : this(new Calendar())
        {
        }
        public CalendarViewModel(Calendar model)
        {
            m_model = model;

            m_offDays = new SynchronizedObservableCollection<OffDayViewModel, OffDay>(m_model.OffDays, m => new OffDayViewModel(m));
        }

        private Calendar m_model;
        public Calendar Model
        {
            get { return m_model; }
        }

        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; RaisePropertyChanged("Name"); }
        }
        public DateTime YearBeginning
        {
            get { return m_model.YearBeginning; }
            set { m_model.YearBeginning = value; RaisePropertyChanged("YearBeginning"); }
        }
        public DateTime SemesterSeparator
        {
            get { return m_model.SemesterSeparator; }
            set { m_model.SemesterSeparator = value; RaisePropertyChanged("SemesterSeparator"); }
        }
        public DateTime YearEnding
        {
            get { return m_model.YearEnding; }
            set { m_model.YearEnding = value; RaisePropertyChanged("YearEnding"); }
        }

        private SynchronizedObservableCollection<OffDayViewModel, OffDay> m_offDays;
        public SynchronizedObservableCollection<OffDayViewModel, OffDay> OffDays
        {
            get { return m_offDays; }
            set
            {
                m_offDays = value;
                m_model.OffDays = value.ModelCollection;
                RaisePropertyChanged("OffDays");
            }
        }
    }
}
