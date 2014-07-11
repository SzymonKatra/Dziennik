using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.IO;
using System.Xml.Serialization;

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

            //SubscribeStudents();
            //SubscribeGroups();
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
        public SynchronizedObservableCollection<SchoolGroupViewModel, SchoolGroup> Groups
        {
            get { return m_groups; }
            set
            {
                //UnsubscribeGroups();
                m_groups = value;
                //SubscribeGroups();
                m_model.Groups = value.ModelCollection;
                RaisePropertyChanged("Groups");
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
        //private void SubscribeGroups()
        //{
        //    m_groups.Removed += m_groups_Removed;
        //}
        //private void UnsubscribeGroups()
        //{
        //    m_groups.Removed -= m_groups_Removed;
        //}

        //private void m_students_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<GlobalStudentViewModel> e)
        //{
        //    foreach (var item in e.Items)
        //    {
        //        GlobalConfig.Database.GlobalStudents.Remove(item.Model);
        //    }
        //}
        //private void m_groups_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<SchoolGroupViewModel> e)
        //{
        //    foreach (var item in e.Items)
        //    {
        //        GlobalConfig.Database.SchoolGroups.Remove(item.Model);
        //    }
        //}
    }
}
