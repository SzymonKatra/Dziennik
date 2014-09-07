using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class DayScheduleViewModel : ViewModelBase<DayScheduleViewModel, DaySchedule>
    {
        public DayScheduleViewModel()
            : this(new DaySchedule())
        {
        }
        public DayScheduleViewModel(DaySchedule viewModel)
            : base(viewModel)
        {
            m_hoursSchedule = new SynchronizedObservableCollection<SelectedHourViewModel, SelectedHour>(Model.HoursSchedule, m => new SelectedHourViewModel(m));
            TypeValidHoursFields();

            SubscribeHoursSchedule();
        }

        //public int HoursCount
        //{
        //    get { return Model.HoursSchedule.Count; }
        //    set
        //    {
        //        //Model.HoursCount = value;
        //        RaisePropertyChanged("HoursCount");
        //        int diff = value - this.HoursSchedule.Count;
        //        if (diff == 0) return;
        //        for (int i = 0; i < Math.Abs(diff); i++)
        //        {
        //            if (diff > 0)
        //            {
        //                this.HoursSchedule.Add(new SelectedHourViewModel());
        //            }
        //            else // diff < 0
        //            {
        //                this.HoursSchedule.RemoveAt(this.HoursSchedule.Count - 1);
        //            }
        //        }
        //    }
        //}
        private SynchronizedObservableCollection<SelectedHourViewModel, SelectedHour> m_hoursSchedule;
        public SynchronizedObservableCollection<SelectedHourViewModel, SelectedHour> HoursSchedule
        {
            get { return m_hoursSchedule; }
            set
            {
                UnsubscribeHoursSchedule();

                m_hoursSchedule = value;
                TypeValidHoursFields();

                SubscribeHoursSchedule();

                Model.HoursSchedule = value.ModelCollection;
                RaisePropertyChanged("HoursSchedule");
            }
        }

        public void PadHours(int count)
        {
            int diff = count - this.HoursSchedule.Count;
            if (diff == 0) return;
            for (int i = 0; i < Math.Abs(diff); i++)
            {
                if (diff > 0)
                {
                    this.HoursSchedule.Add(new SelectedHourViewModel());
                }
                //else // diff < 0
                //{
                //    this.HoursSchedule.RemoveAt(this.HoursSchedule.Count - 1);
                //}
            }

            TypeValidHoursFields();
        }
        private void TypeValidHoursFields()
        {
            for (int i = 0; i < m_hoursSchedule.Count; i++)
            {
                m_hoursSchedule[i].Hour = i + 1;
            }
        }

        private void SubscribeHoursSchedule()
        {
            m_hoursSchedule.CollectionChanged += m_hoursSchedule_CollectionChanged;
        }       
        private void UnsubscribeHoursSchedule()
        {
            m_hoursSchedule.CollectionChanged -= m_hoursSchedule_CollectionChanged;
        }

        private void m_hoursSchedule_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("HoursCount");
        }

        protected override void OnPushCopy()
        {
            //ObjectsPack pack = new ObjectsPack();
            //pack.Write(this.HoursCount);

            //CopyStack.Push(pack);

            this.HoursSchedule.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            //ObjectsPack pack = CopyStack.Pop();
            //if (result == WorkingCopyResult.Cancel)
            //{
            //    this.HoursCount = (int)pack.Read();
            //}

            this.HoursSchedule.PopCopy(result);
        }
    }
}
