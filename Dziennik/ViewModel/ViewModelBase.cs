using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.ViewModel
{
    public abstract class ViewModelBase<VM, M> : ObservableObject, IModelExposable<M>, IWorkingCopyAvailable
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

        //public abstract void ShallowCopyDataTo(VM viewModel);

        private bool m_workingCopyStarted = false;
        public bool WorkingCopyStarted
        {
            get { return m_workingCopyStarted; }
        }

        public void StartWorkingCopy()
        {
            if (m_workingCopyStarted) throw new InvalidOperationException("Already started");
            m_workingCopyStarted = true;

            OnWorkingCopyStarted();
        }
        public void EndWorkingCopy(WorkingCopyResult result)
        {
            if (!m_workingCopyStarted) throw new InvalidOperationException("Must be started to end");
            m_workingCopyStarted = false;

            OnWorkingCopyEnded(result);
        }

        protected virtual void OnWorkingCopyStarted() { }
        protected virtual void OnWorkingCopyEnded(WorkingCopyResult result) { }
        //protected abstract void OnWorkingCopyStarted();
        //protected abstract void OnWorkingCopyEnded(WorkingCopyResult result);
    }
}
