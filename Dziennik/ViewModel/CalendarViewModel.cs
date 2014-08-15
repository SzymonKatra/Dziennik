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

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); }
        }
        public DateTime YearBeginning
        {
            get { return Model.YearBeginning; }
            set { Model.YearBeginning = value; RaisePropertyChanged("YearBeginning"); }
        }
        public DateTime SemesterSeparator
        {
            get { return Model.SemesterSeparator; }
            set { Model.SemesterSeparator = value; RaisePropertyChanged("SemesterSeparator"); }
        }
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

        public override void CopyDataTo(CalendarViewModel viewModel)
        {
            viewModel.Name = this.Name;
            viewModel.YearBeginning = this.YearBeginning;
            viewModel.SemesterSeparator = this.SemesterSeparator;
            viewModel.YearEnding = this.YearEnding;
            viewModel.OffDays = this.OffDays;
        }
    }
}
