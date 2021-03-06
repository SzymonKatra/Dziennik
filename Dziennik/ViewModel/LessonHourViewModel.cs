﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.ComponentModel;

namespace Dziennik.ViewModel
{
    public sealed class LessonHourViewModel : ViewModelBase<LessonHourViewModel, LessonHour>
    {
        public LessonHourViewModel()
            : this(new LessonHour())
        {
        }
        public LessonHourViewModel(LessonHour model)
            : base(model)
        {
        }

        public int Number
        {
            get { return Model.Number; }
            set { Model.Number = value; RaisePropertyChanged("Number"); }
        }
        public DateTime Start
        {
            get { return Model.Start; }
            set { Model.Start = value; RaisePropertyChanged("Start"); End = Start + new TimeSpan(0, 45, 0); RaisePropertyChanged("StartEndDisplayed"); }
        }
        public DateTime End
        {
            get { return Model.End; }
            set { Model.End = value; RaisePropertyChanged("End"); RaisePropertyChanged("StartEndDisplayed"); }
        }

        public string StartEndDisplayed
        {
            get
            {
                return string.Format("{0} - {1}", Start.ToString(GlobalConfig.TimeFormat), End.ToString(GlobalConfig.TimeFormat));
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Number);
            pack.Write(this.Start);
            pack.Write(this.End);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();
            if (result == WorkingCopyResult.Cancel)
            {
                this.Number = (int)pack.Read();
                this.Start = (DateTime)pack.Read();
                this.End = (DateTime)pack.Read();
            }
        }
    }
}
