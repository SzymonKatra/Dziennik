using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class SelectedHourViewModel : ViewModelBase<SelectedHourViewModel, SelectedHour>
    {
        public SelectedHourViewModel()
            : this(new SelectedHour())
        {
        }
        public SelectedHourViewModel(SelectedHour model)
            : base(model)
        {
            if (this.Model.Id == null) GlobalConfig.GlobalDatabase.AssignId(model);
        }

        public int Hour
        {
            get { return Model.Hour; }
            set { Model.Hour = value; RaisePropertyChanged("Hour"); RaisePropertyChanged("BindingHour"); }
        }
        private SchoolGroupViewModel m_selectedGroup;
        [DatabaseIgnoreSearchRelations]
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set
            {
                if (value == m_selectedGroup) return;
                if (m_selectedGroup != null) m_selectedGroup.Model.Hours.RemoveAll(x => x.Value == this.Model.Id);
                m_selectedGroup = value;
                if (m_selectedGroup != null) m_selectedGroup.Model.Hours.Add(new ValueWrapper<ulong?>(this.Model.Id));
                RaisePropertyChanged("SelectedGroup");
            }
        }
        public string Room
        {
            get { return Model.Room; }
            set
            {
                Model.Room = value;
                RaisePropertyChanged("Room");
            }
        }

        public void InitializeSelectedGroup()
        {
            foreach (var cl in GlobalConfig.Main.OpenedSchoolClasses)
            {
                SchoolGroupViewModel grp = cl.ViewModel.Groups.FirstOrDefault((x) =>
                {
                    return (x.Model.Hours.FirstOrDefault(y => y.Value == this.Model.Id) != null);
                    //if (Model.SelectedGroupId == null) return false;
                    //return x.Model.Id == Model.SelectedGroupId;
                });
                if (grp != null)
                {
                    m_selectedGroup = grp;
                    return;
                }
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Hour);
            pack.Write(this.SelectedGroup);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();
            if (result == WorkingCopyResult.Cancel)
            {
                this.Hour = (int)pack.Read();
                this.SelectedGroup = (SchoolGroupViewModel)pack.Read();
            }
        }
    }
}
