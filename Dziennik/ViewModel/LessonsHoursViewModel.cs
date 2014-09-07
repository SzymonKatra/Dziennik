using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class LessonsHoursViewModel : ViewModelBase<LessonsHoursViewModel, LessonsHours>
    {
        public LessonsHoursViewModel()
            : this(new LessonsHours())
        {
        }
        public LessonsHoursViewModel(LessonsHours model)
            : base(model)
        {
            m_hours = new SynchronizedObservableCollection<LessonHourViewModel, LessonHour>(model.Hours, m => new LessonHourViewModel(m));
            SubscribeHours();
        }

        public event EventHandler HoursAdded;

        private SynchronizedObservableCollection<LessonHourViewModel, LessonHour> m_hours;
        public SynchronizedObservableCollection<LessonHourViewModel, LessonHour> Hours
        {
            get { return m_hours; }
            set
            {
                UnsubscribeHours();

                m_hours = value;

                SubscribeHours();

                Model.Hours = value.ModelCollection;
                RaisePropertyChanged("Hours");
            }
        }
        public bool IsEnabled
        {
            get { return Model.IsEnabled; }
            set { Model.IsEnabled = value; RaisePropertyChanged("IsEnabled"); }
        }

        private void SubscribeHours()
        {
            m_hours.Added += m_hours_Added;
        }
        private void UnsubscribeHours()
        {
            m_hours.Added -= m_hours_Added;
        }

        private void m_hours_Added(object sender, NotifyCollectionChangedSimpleEventArgs<LessonHourViewModel> e)
        {
            OnHoursAdded();
        }

        protected virtual void OnHoursAdded()
        {
            EventHandler handler = HoursAdded;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.IsEnabled);

            CopyStack.Push(pack);

            this.Hours.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();
            if (result == WorkingCopyResult.Cancel)
            {
                this.IsEnabled = (bool)pack.Read();
            }

            this.Hours.PopCopy(result);
        }
    }
}
