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
    }
}
