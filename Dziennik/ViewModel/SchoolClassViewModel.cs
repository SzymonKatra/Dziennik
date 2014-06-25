using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class SchoolClassViewModel : ObservableObject, IViewModelExposable<SchoolClass>
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

            SubscribeStudents();
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
            set { m_model.Name = value; OnPropertyChanged("Name"); }
        }
        private SynchronizedPerItemObservableCollection<GlobalStudentViewModel, GlobalStudent> m_students;
        public SynchronizedPerItemObservableCollection<GlobalStudentViewModel, GlobalStudent> Students
        {
            get { return m_students; }
            set
            {
                UnsubscribeStudents();

                m_students = value;

                SubscribeStudents();

                m_model.Students = value.ModelCollection;
                OnPropertyChanged("Students");
            }
        }
        private SynchronizedObservableCollection<SchoolGroupViewModel, SchoolGroup> m_groups;
        public SynchronizedObservableCollection<SchoolGroupViewModel, SchoolGroup> Groups
        {
            get { return m_groups; }
            set
            {
                UnsubscribeGroups();

                m_groups = value;

                SubscribeGroups();

                m_model.Groups = value.ModelCollection;
                OnPropertyChanged("Groups");
            }
        }

        private void SubscribeStudents()
        {
            m_students.ItemPropertyInCollectionChanged += m_students_ItemPropertyInCollectionChanged;
        }
        private void UnsubscribeStudents()
        {
            m_students.ItemPropertyInCollectionChanged -= m_students_ItemPropertyInCollectionChanged;
        }

        private void SubscribeGroups()
        {
            m_groups.Added += m_gropus_Added;
            m_groups.Removed += m_gropus_Removed;

            m_groups.RaiseAddedForAll();
        }
        private void UnsubscribeGroups()
        {
            m_groups.RaiseRemovedForAll();

            m_groups.Added -= m_gropus_Added;
            m_groups.Removed -= m_gropus_Removed;
        }

        private void m_gropus_Added(object sender, NotifyCollectionChangedSimpleEventArgs<SchoolGroupViewModel> e)
        {
            foreach(SchoolGroupViewModel group in e.Items)
            {
                group.StudentInGroupAttachedGlobalIdChanged += group_StudentInGroupAttachedGlobalIdChanged;

                foreach (StudentInGroupViewModel student in group.Students)
                {
                    student.GlobalStudent = FindCorrespondingGlobalStudent(student.GlobalId);
                }
            }
        }
        private void m_gropus_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<SchoolGroupViewModel> e)
        {
            foreach (SchoolGroupViewModel group in e.Items)
            {
                group.StudentInGroupAttachedGlobalIdChanged -= group_StudentInGroupAttachedGlobalIdChanged;

                foreach (StudentInGroupViewModel student in group.Students)
                {
                    student.GlobalStudent = null;
                }
            }
        }

        private void group_StudentInGroupAttachedGlobalIdChanged(object sender, CommonEventArgs<StudentInGroupViewModel> e)
        {
            e.Item.GlobalStudent = FindCorrespondingGlobalStudent(e.Item.GlobalId);
        }

        private void m_students_ItemPropertyInCollectionChanged(object sender, ItemPropertyInCollectionChangedEventArgs<GlobalStudentViewModel> e)
        {
            if (e.PropertyName == "Id")
            {
                foreach (SchoolGroupViewModel group in m_groups)
                {
                    foreach (StudentInGroupViewModel student in group.Students)
                    {
                        if (e.Item.Id == student.GlobalId)
                        {
                            student.GlobalId = e.Item.Id;
                            student.GlobalStudent = e.Item;
                        }
                    }
                }
            }
        }
        
        public GlobalStudentViewModel FindCorrespondingGlobalStudent(int globalId)
        {
            if (m_students.Count >= globalId && m_students[globalId - 1].Id == globalId) return m_students[globalId - 1];

            return m_students.First((gs) => { return gs.Id == globalId; });
        }
    }
}
