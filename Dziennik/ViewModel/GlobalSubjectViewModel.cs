using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class GlobalSubjectViewModel : ObservableObject, IModelExposable<GlobalSubject>
    {
        public GlobalSubjectViewModel()
            : this(new GlobalSubject())
        {
        }
        public GlobalSubjectViewModel(GlobalSubject model)
        {
            m_model = model;
        }

        private GlobalSubject m_model;
        public GlobalSubject Model
        {
            get { return m_model; }
        }

        public int Number
        {
            get { return m_model.Number; }
            set { m_model.Number = value; RaisePropertyChanged("Number"); }
        }
        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; RaisePropertyChanged("Name"); }
        }
        public string Category
        {
            get { return m_model.Category; }
            set { m_model.Category = value; RaisePropertyChanged("Category"); }
        }
    }
}
