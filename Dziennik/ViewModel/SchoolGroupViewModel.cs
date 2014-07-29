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
            m_globalSubjects = new SynchronizedObservableCollection<GlobalSubjectViewModel, GlobalSubject>(m_model.Subjects, m => new GlobalSubjectViewModel(m));
            m_realizedSubjects = new SynchronizedObservableCollection<RealizedSubjectViewModel, RealizedSubject>(m_model.RealizedSubjects, m => new RealizedSubjectViewModel(m));
            SubscribeRealizedSubjects();
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
        private SynchronizedObservableCollection<GlobalSubjectViewModel, GlobalSubject> m_globalSubjects;
        [DatabaseRelationCollection("GlobalSubjects")]
        public SynchronizedObservableCollection<GlobalSubjectViewModel, GlobalSubject> GlobalSubjects
        {
            get { return m_globalSubjects; }
            set
            {
                m_globalSubjects = value;
                m_model.Subjects = value.ModelCollection;
                RaisePropertyChanged("GlobalSubjects");
            }
        }
        private SynchronizedObservableCollection<RealizedSubjectViewModel, RealizedSubject> m_realizedSubjects;
        [DatabaseRelationCollection("GroupRealizedSubjects")]
        public SynchronizedObservableCollection<RealizedSubjectViewModel, RealizedSubject> RealizedSubjects
        {
            get { return m_realizedSubjects; }
            set
            {
                UnsubscribeRealizedSubjects();
                m_realizedSubjects = value;
                SubscribeRealizedSubjects();
                m_model.RealizedSubjects = value.ModelCollection;
                RaisePropertyChanged("RealizedSubjects");
            }
        }

        public string RealizedSubjectsDisplay
        {
            get
            {
                int realizedFromCurriculum = m_realizedSubjects.Count(x => !x.IsCustom);
                int realizedOutsideCurriculum = m_realizedSubjects.Count(x => x.IsCustom);

                return string.Format(GlobalConfig.GetStringResource("lang_RealizedSubjectsCountFormat"), realizedFromCurriculum, m_globalSubjects.Count, realizedOutsideCurriculum);
            }
        }

        private void SubscribeRealizedSubjects()
        {
            m_realizedSubjects.CollectionChanged += m_realizedSubjects_CollectionChanged;
        }
        private void UnsubscribeRealizedSubjects()
        {
            m_realizedSubjects.CollectionChanged -= m_realizedSubjects_CollectionChanged;
        }

        private void m_realizedSubjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("RealizedSubjectsDisplay");
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
