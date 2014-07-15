using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class SchoolGroupViewModel : ObservableObject, IModelExposable<SchoolGroup>
    {
        public SchoolGroupViewModel()
            : this(new SchoolGroup())
        {
        }
        public SchoolGroupViewModel(SchoolGroup schoolGroup)
        {
            m_model = schoolGroup;

            m_students = new SynchronizedPerItemObservableCollection<StudentInGroupViewModel, StudentInGroup>(m_model.Students, (m) => { return new StudentInGroupViewModel(m); });;

            //SubscribeStudents();
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
                //UnsubscribeStudents();
                m_students = value;
                //SubscribeStudents();
                m_model.Students = value.ModelCollection;
                RaisePropertyChanged("Students");
            }
        }

        //private void SubscribeStudents()
        //{
        //    m_students.Removed += m_students_Removed;
        //}
        //private void UnsubscribeStudents()
        //{
        //    m_students.Removed -= m_students_Removed;
        //}

        //private void m_students_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<StudentInGroupViewModel> e)
        //{
        //    foreach (var item in e.Items)
        //    {
        //        GlobalConfig.Database.StudentsInGroups.Remove(item.Model);
        //    }
        //}
    }
}
