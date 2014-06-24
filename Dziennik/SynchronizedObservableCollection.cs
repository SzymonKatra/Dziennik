using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Dziennik
{
    /// <summary>
    /// ObservableCollection which is synchronised with Model collection.
    /// Remember that synchronization is only in one-way. Changes made in ViewModel appears in Model but not vice versa.
    /// To synchronize ViewModel with Model use ResynchronizeWithModel method, but in general you should make changes only in ViewModel.
    /// </summary>
    /// <typeparam name="T">ViewModel type</typeparam>
    /// <typeparam name="M">Model type</typeparam>
    public class SynchronizedObservableCollection<VM, M> : SynchronizedObservableCollection<VM, M, System.Collections.Generic.List<M>>
                                                           where VM : IViewModelExposable<M>
    {
        public SynchronizedObservableCollection(System.Collections.Generic.List<M> modelCollection, Func<M, VM> createViewModelFunc)
            : base(modelCollection, createViewModelFunc)
        {
        }
    }

    /// <summary>
    /// ObservableCollection which is synchronised with Model collection.
    /// Remember that synchronization is only in one-way. Changes made in ViewModel appears in Model but not vice versa.
    /// To synchronize ViewModel with Model use ResynchronizeWithModel method, but in general you should make changes only in ViewModel.
    /// </summary>
    /// <typeparam name="T">ViewModel type</typeparam>
    /// <typeparam name="M">Model type</typeparam>
    /// <typeparam name="MC">Model collection</typeparam>
    public class SynchronizedObservableCollection<VM, M, MC> : ObservableCollectionNotifySimple<VM>
                                                               where VM : IViewModelExposable<M>
                                                               where MC : System.Collections.Generic.IList<M>
    {
        protected SynchronizedObservableCollection()
        {
        }
        public SynchronizedObservableCollection(MC modelCollection, Func<M, VM> createViewModelFunc)
        {
            //m_modelCollection = modelCollection;

            //if (m_modelCollection.Count > 0)
            //{
            //    foreach(M model in m_modelCollection)
            //    {
            //        this.Add(createViewModelFunc(model));
            //    }
            //}
            ConstructorImpl(this, modelCollection, createViewModelFunc);
        }
        public static SynchronizedObservableCollection<VM, M, MC> CreateFastCopy(MC modelCollection, Action<MC, ObservableCollection<VM>> modelToViewModelCopyAction)
        {
            SynchronizedObservableCollection<VM, M, MC> result = new SynchronizedObservableCollection<VM, M, MC>();

            CreateFastCopyImpl(result, modelCollection, modelToViewModelCopyAction);
            //result.m_modelCollection = modelCollection;
            //if (result.m_modelCollection.Count > 0) modelToViewModelCopyAction(result.m_modelCollection, result);

            return result;
        }

        protected static void ConstructorImpl(SynchronizedObservableCollection<VM, M, MC> target, MC modelCollection, Func<M, VM> createViewModelFunc)
        {
            target.m_modelCollection = modelCollection;
            target.m_createViewModelFunc = createViewModelFunc;
            target.ResynchronizeWithModel();
            //if (target.m_modelCollection.Count > 0)
            //{
            //    foreach (M model in target.m_modelCollection)
            //    {
            //        target.Add(createViewModelFunc(model));
            //    }
            //}
        }
        protected static void CreateFastCopyImpl(SynchronizedObservableCollection<VM, M, MC> target, MC modelCollection, Action<MC, ObservableCollection<VM>> modelToViewModelCopyAction)
        {
            target.m_modelCollection = modelCollection;
            target.m_modelToViewModelCopyAction = modelToViewModelCopyAction;
            target.ResynchronizeWithModel();
            //if (target.m_modelCollection.Count > 0) modelToViewModelCopyAction(target.m_modelCollection, target);
        }

        private Func<M, VM> m_createViewModelFunc;
        private Action<MC, ObservableCollection<VM>> m_modelToViewModelCopyAction;

        private bool m_synchronizationPaused = false;

        private MC m_modelCollection;
        /// <summary>
        /// Shouldn't be used to modify collection. If it required, first modify collecion(Model) and call ResynchronizeWithModel method.
        /// Should be used to get new model collection to set it in backend.
        /// </summary>
        public MC ModelCollection
        {
            get { return m_modelCollection; }
        }

        public void ResynchronizeWithModel()
        {
            this.PauseSynchronization();

            this.Clear();

            if (this.m_modelCollection.Count > 0)
            {
                if (m_modelToViewModelCopyAction != null)
                {
                    m_modelToViewModelCopyAction(this.m_modelCollection, this);
                }
                else if (m_createViewModelFunc != null)
                {
                    foreach (M model in this.m_modelCollection)
                    {
                        this.Add(m_createViewModelFunc(model));
                    }
                }
                else Debug.Assert(true, "SynchronizedObservableCollection - m_createViewModelFunc and m_modelToViewModelCopyAction are both null!");
            }

            this.ResumeSynchronization();
        }
        public void CopyFrom(System.Collections.Generic.IEnumerable<VM> source)
        {
            foreach (VM item in source) this.Add(item);
        }

        protected void PauseSynchronization()
        {
            m_synchronizationPaused = true;
        }
        protected void ResumeSynchronization()
        {
            m_synchronizationPaused = false;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!m_synchronizationPaused)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add: OnAdd(e); break;
                    case NotifyCollectionChangedAction.Move: OnMove(e); break;
                    case NotifyCollectionChangedAction.Remove: OnRemove(e); break;
                    case NotifyCollectionChangedAction.Replace: OnReplace(e); break;
                    case NotifyCollectionChangedAction.Reset: OnReset(e); break;
                }
            }

            base.OnCollectionChanged(e);
        }

        protected virtual void OnAdd(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            int insertIndex = e.NewStartingIndex;
            if (insertIndex < 0)
            {
                foreach (VM viewModelItem in e.NewItems) m_modelCollection.Add(viewModelItem.Model);
            }
            else
            {
                foreach (VM viewModelItem in e.NewItems)
                {
                    m_modelCollection.Insert(insertIndex, viewModelItem.Model);
                    insertIndex++;
                }
            }
        }
        protected virtual void OnMove(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null || e.OldItems.Count < 1) return;
            VM viewModelItem = (VM)e.OldItems[0];
            m_modelCollection.RemoveAt(e.OldStartingIndex);
            m_modelCollection.Insert(e.NewStartingIndex, viewModelItem.Model);
        }
        protected virtual void OnRemove(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;
            if (e.OldStartingIndex >= 0)
            {
                m_modelCollection.RemoveAt(e.OldStartingIndex);
            }
            else
            {
                foreach (VM viewModelItem in e.OldItems)
                {
                    m_modelCollection.Remove(viewModelItem.Model);
                }
            }
        }
        protected virtual void OnReplace(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null || e.NewItems == null || e.NewItems.Count < 1) return;

            if (e.OldStartingIndex >= 0)
            {
                m_modelCollection[e.OldStartingIndex] = ((VM)e.NewItems[0]).Model;
            }
            else
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    VM oldViewModelItem = (VM)e.OldItems[i];
                    int index = m_modelCollection.IndexOf(oldViewModelItem.Model);
                    if (index >= 0)
                    {
                        VM newViewModelItem = (VM)e.NewItems[i];
                        m_modelCollection[index] = newViewModelItem.Model;
                    }
                }
            }
        }
        protected virtual void OnReset(NotifyCollectionChangedEventArgs e)
        {
            //Reset != Clear. Reset == treat it as new list

            m_modelCollection.Clear();

            if (this.Count > 0)
            {
                Debug.Assert(this.Count > 0, "SynchronizedObservableCollection.OnReset - Count > 0 while resetting collection");
                foreach (VM viewModelItem in this) m_modelCollection.Add(viewModelItem.Model);
            }
        }
    }
}
