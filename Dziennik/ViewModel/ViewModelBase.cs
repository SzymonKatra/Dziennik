using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.ViewModel
{
    public abstract class ViewModelBase<VM, M> : ObservableObject, IModelExposable<M>, IViewModelShallowCopyable<VM>
    {
        private M m_model;
        public M Model
        {
            get { return m_model; }
            protected set { m_model = value; }
        }

        public ViewModelBase(M model)
        {
            m_model = model;
        }

        public abstract void ShallowCopyDataTo(VM viewModel);
    }
}
