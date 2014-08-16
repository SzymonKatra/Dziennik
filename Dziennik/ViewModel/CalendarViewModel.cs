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

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Name);
            pack.Write(this.YearBeginning);
            pack.Write(this.SemesterSeparator);
            pack.Write(this.YearEnding);

            CopyStack.Push(pack);
            //m_nameCopy = this.Name;
            //m_yearBeginningCopy = this.YearBeginning;
            //m_semesterSeparatorCopy = this.SemesterSeparator;
            //m_yearEndingCopy = this.YearEnding;

            m_offDays.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if (result == WorkingCopyResult.Cancel)
            {
                this.Name = (string)pack.Read();
                this.YearBeginning = (DateTime)pack.Read();
                this.SemesterSeparator = (DateTime)pack.Read();
                this.YearEnding = (DateTime)pack.Read();
            }

            m_offDays.PopCopy(result);
        }
    }
}
