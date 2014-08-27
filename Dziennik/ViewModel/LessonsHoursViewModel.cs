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
        }

        private SynchronizedObservableCollection<LessonHourViewModel, LessonHour> m_hours;
        public SynchronizedObservableCollection<LessonHourViewModel, LessonHour> Hours
        {
            get { return m_hours; }
            set
            {
                m_hours = value;
                Model.Hours = value.ModelCollection;
                RaisePropertyChanged("Hours");
            }
        }

        protected override void OnPushCopy()
        {
            this.Hours.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            this.Hours.PopCopy(result);
        }
    }
}
