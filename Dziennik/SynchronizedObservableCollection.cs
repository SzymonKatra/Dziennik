using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Dziennik
{
    /// <summary>
    /// ObservableCollection which is synchronised with Model collection
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
    /// ObservableCollection which is synchronised with Model collection
    /// </summary>
    /// <typeparam name="T">ViewModel type</typeparam>
    /// <typeparam name="M">Model type</typeparam>
    /// <typeparam name="MC">Model collection</typeparam>
    public class SynchronizedObservableCollection<VM, M, MC> : ObservableCollection<VM>
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

            if (target.m_modelCollection.Count > 0)
            {
                foreach (M model in target.m_modelCollection)
                {
                    target.Add(createViewModelFunc(model));
                }
            }
        }
        protected static void CreateFastCopyImpl(SynchronizedObservableCollection<VM, M, MC> target, MC modelCollection, Action<MC, ObservableCollection<VM>> modelToViewModelCopyAction)
        {
            target.m_modelCollection = modelCollection;
            if (target.m_modelCollection.Count > 0) modelToViewModelCopyAction(target.m_modelCollection, target);
        }

        private MC m_modelCollection;
        public MC ModelCollection
        {
            get { return m_modelCollection; }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: OnAdd(e); break;
                case NotifyCollectionChangedAction.Move: OnMove(e); break;
                case NotifyCollectionChangedAction.Remove: OnRemove(e); break;
                case NotifyCollectionChangedAction.Replace: OnReplace(e); break;
                case NotifyCollectionChangedAction.Reset: OnReset(e); break;
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
            Debug.Assert(e.OldStartingIndex >= 0 && e.OldItems.Count > 1, "SynchronizedObservableCollection.OnRemove - OldStartingIndex valid while OldItems.Count > 1");
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
            Debug.Assert(e.OldStartingIndex >= 0 && e.OldItems.Count > 1, "SynchronizedObservableCollection.OnReplace - OldStartingIndex valid while NewItems.Count > 1");
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
