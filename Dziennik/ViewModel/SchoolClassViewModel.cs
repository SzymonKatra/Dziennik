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
    }
}
