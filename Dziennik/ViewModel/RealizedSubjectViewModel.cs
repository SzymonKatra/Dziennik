using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class RealizedSubjectViewModel : ObservableObject, IModelExposable<RealizedSubject>
    {
        public RealizedSubjectViewModel()
            : this(new RealizedSubject())
        {
        }
        public RealizedSubjectViewModel(RealizedSubject model)
        {
            m_model = model;
        }

        private RealizedSubject m_model;
        public RealizedSubject Model
        {
            get { return m_model; }
        }

        public DateTime RealizedDate
        {
            get { return m_model.RealizedDate; }
            set { m_model.RealizedDate = value; RaisePropertyChanged("RealizedDate"); }
        }
        public string CustomSubject
        {
            get { return m_model.CustomSubject; }
            set { m_model.CustomSubject = value; m_globalSubject = null; RaisePropertyChanged("CustomSubject"); RaisePropertyChanged("IsCustom"); }
        }
        public bool IsCustom
        {
            get { return !string.IsNullOrWhiteSpace(m_model.CustomSubject); }
        }
        public string Name
        {
            get
            {
                return (IsCustom ? CustomSubject : (m_globalSubject == null ? "ERROR" : m_globalSubject.Name));
            }
        }

        private GlobalSubjectViewModel m_globalSubject;
        [DatabaseRelationProperty("GlobalSubjects", "GlobalSubjectId")]
        public GlobalSubjectViewModel GlobalSubject
        {
            get { return m_globalSubject; }
            set { m_globalSubject = value; m_model.CustomSubject = string.Empty; RaisePropertyChanged("GlobalSubject"); RaisePropertyChanged("IsCustom"); }
        }
    }
}
