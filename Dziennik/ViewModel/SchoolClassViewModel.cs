using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.IO;
using System.Xml.Serialization;

namespace Dziennik.ViewModel
{
    public sealed class SchoolClassViewModel : ObservableObject, IModelExposable<SchoolClass>
    {
        public SchoolClassViewModel()
            : this(new SchoolClass())
        {
        }
        public SchoolClassViewModel(SchoolClass schoolClass)
        {
            m_model = schoolClass;

            m_students = new SynchronizedPerItemObservableCollection<GlobalStudentViewModel, GlobalStudent>(m_model.Students, (m) => { return new GlobalStudentViewModel(m); });
            m_groups = new SynchronizedObservableCollection<SchoolGroupViewModel, SchoolGroup>(m_model.Groups, (m) => { return new SchoolGroupViewModel(m); });
            SubscribeGroups();
        }

        private SchoolClass m_model;
        public SchoolClass Model
        {
            get { return m_model; }
        }

        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; RaisePropertyChanged("Name"); }
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
                m_model.Students = value.ModelCollection;
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
                m_model.Groups = value.ModelCollection;
                RaisePropertyChanged("Groups");
            }
        }

        public DateTime YearBeginning
        {
            get { return m_model.YearBeginning; }
            set { m_model.YearBeginning = value; RaisePropertyChanged("YearBeginning"); }
        }
        public DateTime SemesterSeparator
        {
            get { return m_model.SemesterSeparator; }
            set { m_model.SemesterSeparator = value; RaisePropertyChanged("SemesterSeparator"); }
        }
        public DateTime YearEnding
        {
            get { return m_model.YearEnding; }
            set { m_model.YearEnding = value; RaisePropertyChanged("YearEnding"); }
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
    }
}
