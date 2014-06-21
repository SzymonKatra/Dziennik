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

            m_students = new SynchronizedObservableCollection<StudentViewModel, Student>(m_model.Students, (m) => { return new StudentViewModel(m); });
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
        private SynchronizedObservableCollection<StudentViewModel, Student> m_students;
        public SynchronizedObservableCollection<StudentViewModel, Student> Students
        {
            get { return m_students; }
            set
            {
                m_students = value;
                m_model.Students = value.ModelCollection;
                OnPropertyChanged("Students");
            }
        }
    }
}
