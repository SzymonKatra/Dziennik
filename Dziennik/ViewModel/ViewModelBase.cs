using System.Collections.Generic;

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

        protected ViewModelBase(M model)
        {
            m_model = model;
        }

        //public abstract void ShallowCopyDataTo(VM viewModel);
        private Stack<ObjectsPack> m_copyStack = new Stack<ObjectsPack>();
        protected Stack<ObjectsPack> CopyStack
        {
            get { return m_copyStack; }
        }
        public int CopyDepth
        {
            get { return m_copyStack.Count; }
        }

        public void PushCopy()
        {
            OnPushCopy();
        }
        public void PopCopy(WorkingCopyResult result)
        {
            OnPopCopy(result);
        }

        protected abstract void OnPushCopy();
        protected abstract void OnPopCopy(WorkingCopyResult result);
        //protected abstract void OnWorkingCopyStarted();
        //protected abstract void OnWorkingCopyEnded(WorkingCopyResult result);
    }
}
