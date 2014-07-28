using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class GlobalSchoolViewModel : ObservableObject, IModelExposable<GlobalSchool>
    {
        public GlobalSchoolViewModel()
            : this(new GlobalSchool())
        {
        }
        public GlobalSchoolViewModel(GlobalSchool model)
        {
            m_model = model;
        }

        private GlobalSchool m_model;
        public GlobalSchool Model
        {
            get { return m_model; }
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
    }
}
