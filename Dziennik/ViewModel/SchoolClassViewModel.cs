using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.IO;
using System.Xml.Serialization;

namespace Dziennik.ViewModel
{
    public sealed class SchoolClassViewModel : ViewModelBase<SchoolClassViewModel, SchoolClass>
    {
        public SchoolClassViewModel()
            : this(new SchoolClass())
        {
        }
        public SchoolClassViewModel(SchoolClass model) : base(model)
        {
            m_students = new SynchronizedPerItemObservableCollection<GlobalStudentViewModel, GlobalStudent>(Model.Students, (m) => { return new GlobalStudentViewModel(m); });
            m_groups = new SynchronizedObservableCollection<SchoolGroupViewModel, SchoolGroup>(Model.Groups, (m) => { return new SchoolGroupViewModel(m); });
            SubscribeGroups();

            m_calendar = GlobalConfig.GlobalDatabase.ViewModel.Calendars.FirstOrDefault(x => x.Model.Id == Model.GlobalCalendarId);
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); }
        }
        private SynchronizedPerItemObservableCollection<GlobalStudentViewModel, GlobalStudent> m_students;
        [DatabaseRelationCollection("GlobalStudents")]
        public SynchronizedPerItemObservableCollection<GlobalStudentViewModel, GlobalStudent> Students
        {
            get { return m_students; }
            set
            {
                //UnsubscribeStudents();
                m_students = value;
                //SubscribeStudents();
                Model.Students = value.ModelCollection;
                RaisePropertyChanged("Students");
            }
        }
        private SynchronizedObservableCollection<SchoolGroupViewModel, SchoolGroup> m_groups;
        [DatabaseInversePropertyOwner("OwnerClass", "SubscribeGroups")]
        public SynchronizedObservableCollection<SchoolGroupViewModel, SchoolGroup> Groups
        {
            get { return m_groups; }
            set
            {
                UnsubscribeGroups();
                m_groups = value;
                SubscribeGroups();
                Model.Groups = value.ModelCollection;
                RaisePropertyChanged("Groups");
            }
        }

        private CalendarViewModel m_calendar;
        public CalendarViewModel Calendar
        {
            get { return m_calendar; }
            set
            {
                m_calendar = value;
                Model.GlobalCalendarId = (m_calendar == null ? null : m_calendar.Model.Id);
                RaisePropertyChanged("Calendar");
            }
        }

        private void SubscribeGroups()
        {
            m_groups.Added += m_groups_Added;
            m_groups.Removed += m_groups_Removed;
        }
        private void UnsubscribeGroups()
        {
            m_groups.Added += m_groups_Added;
            m_groups.Removed += m_groups_Removed;
        }

        private void m_groups_Added(object sender, NotifyCollectionChangedSimpleEventArgs<SchoolGroupViewModel> e)
        {
            foreach (var item in e.Items)
            {
                item.OwnerClass = this;
            }
        }
        private void m_groups_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<SchoolGroupViewModel> e)
        {
            foreach (var item in e.Items)
            {
                item.OwnerClass = null;
            }
        }

        public override void CopyDataTo(SchoolClassViewModel viewModel)
        {
            viewModel.Name = this.Name;
            viewModel.Students = this.Students;
            viewModel.Groups = this.Groups;
            viewModel.Calendar = this.Calendar;
        }
    }
}
