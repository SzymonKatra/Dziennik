using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class CalendarViewModel : ViewModelBase<CalendarViewModel, Calendar>
    {
        public CalendarViewModel()
            : this(new Calendar())
        {
        }
        public CalendarViewModel(Calendar model) : base(model)
        {
            m_offDays = new SynchronizedObservableCollection<OffDayViewModel, OffDay>(Model.OffDays, m => new OffDayViewModel(m));
        }

        private string m_nameCopy;
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); }
        }
        private DateTime m_yearBeginningCopy;
        public DateTime YearBeginning
        {
            get { return Model.YearBeginning; }
            set { Model.YearBeginning = value; RaisePropertyChanged("YearBeginning"); }
        }
        private DateTime m_semesterSeparatorCopy;
        public DateTime SemesterSeparator
        {
            get { return Model.SemesterSeparator; }
            set { Model.SemesterSeparator = value; RaisePropertyChanged("SemesterSeparator"); }
        }
        private DateTime m_yearEndingCopy;
        public DateTime YearEnding
        {
            get { return Model.YearEnding; }
            set { Model.YearEnding = value; RaisePropertyChanged("YearEnding"); }
        }

        private SynchronizedObservableCollection<OffDayViewModel, OffDay> m_offDays;
        public SynchronizedObservableCollection<OffDayViewModel, OffDay> OffDays
        {
            get { return m_offDays; }
            set
            {
                m_offDays = value;
                Model.OffDays = value.ModelCollection;
                RaisePropertyChanged("OffDays");
            }
        }

        //public override void ShallowCopyDataTo(CalendarViewModel viewModel)
        //{
        //    viewModel.Name = this.Name;
        //    viewModel.YearBeginning = this.YearBeginning;
        //    viewModel.SemesterSeparator = this.SemesterSeparator;
        //    viewModel.YearEnding = this.YearEnding;
        //    viewModel.OffDays = this.OffDays;
        //}

        protected override void OnWorkingCopyStarted()
        {
            m_nameCopy = this.Name;
            m_yearBeginningCopy = this.YearBeginning;
            m_semesterSeparatorCopy = this.SemesterSeparator;
            m_yearEndingCopy = this.YearEnding;

            m_offDays.StartWorkingCopy();
        }
        protected override void OnWorkingCopyEnded(WorkingCopyResult result)
        {
            if (result == WorkingCopyResult.Cancel)
            {
                this.Name = m_nameCopy;
                this.YearBeginning = m_yearBeginningCopy;
                this.SemesterSeparator = m_semesterSeparatorCopy;
                this.YearEnding = m_yearEndingCopy;
            }

            m_offDays.EndWorkingCopy(result);
        }
    }
}
