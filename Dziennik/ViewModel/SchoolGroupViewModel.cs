using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class SchoolGroupViewModel : ObservableObject, IViewModelExposable<SchoolGroup>
    {
        public SchoolGroupViewModel()
            : this(new SchoolGroup())
        {
        }
        public SchoolGroupViewModel(SchoolGroup schoolGroup)
        {
            m_model = schoolGroup;

            m_students = new SynchronizedPerItemObservableCollection<StudentInGroupViewModel, StudentInGroup>(m_model.Students, (m) => { return new StudentInGroupViewModel(m); });
            SubscribeStudents();
        }

        private SchoolGroup m_model;
        public SchoolGroup Model
        {
            get { return m_model; }
        }

        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; RaisePropertyChanged("Name"); }
        }
        private SynchronizedPerItemObservableCollection<StudentInGroupViewModel, StudentInGroup> m_students;
        public SynchronizedPerItemObservableCollection<StudentInGroupViewModel, StudentInGroup> Students
        {
            get { return m_students; }
            set
            {
                UnsubscribeStudents();

                m_students = value;

                SubscribeStudents();

                m_model.Students = value.ModelCollection;
                RaisePropertyChanged("Students");
            }
        }

        public event EventHandler<CommonEventArgs<StudentInGroupViewModel>> StudentInGroupAttachedGlobalIdChanged;

        private void SubscribeStudents()
        {
            m_students.ItemPropertyInCollectionChanged += m_students_ItemPropertyInCollectionChanged;
            m_students.Added += m_students_Added;
            m_students.Removed += m_students_Removed;

            m_students.RaiseAddedForAll();
        }
        private void UnsubscribeStudents()
        {
            m_students.RaiseRemovedForAll();

            m_students.ItemPropertyInCollectionChanged -= m_students_ItemPropertyInCollectionChanged;
            m_students.Added -= m_students_Added;
            m_students.Removed -= m_students_Removed;
        }

        private void m_students_ItemPropertyInCollectionChanged(object sender, ItemPropertyInCollectionChangedEventArgs<StudentInGroupViewModel> e)
        {
            if (e.PropertyName == "GlobalId")
            {
                OnStudentInGroupAttachedGlobalIdChanged(new CommonEventArgs<StudentInGroupViewModel>(e.Item));
            }
        }
        
        private void m_students_Added(object sender, NotifyCollectionChangedSimpleEventArgs<StudentInGroupViewModel> e)
        {
            foreach(StudentInGroupViewModel item in e.Items)
            {
                OnStudentInGroupAttachedGlobalIdChanged(new CommonEventArgs<StudentInGroupViewModel>(item));
            }
        }
        private void m_students_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<StudentInGroupViewModel> e)
        {
            foreach (StudentInGroupViewModel item in e.Items)
            {
                item.GlobalStudent = null;
            }
        }

        protected virtual void OnStudentInGroupAttachedGlobalIdChanged(CommonEventArgs<StudentInGroupViewModel> e)
        {
            EventHandler<CommonEventArgs<StudentInGroupViewModel>> handler = StudentInGroupAttachedGlobalIdChanged;
            if (handler != null) handler(this, e);
        }
    }
}
