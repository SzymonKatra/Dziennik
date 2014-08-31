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
        public bool IsEnabled
        {
            get { return Model.IsEnabled; }
            set { Model.IsEnabled = value; RaisePropertyChanged("IsEnabled"); }
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
