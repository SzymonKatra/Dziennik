using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Dziennik
{
    /// <summary>
    /// ObservableCollection which is synchronised with Model collection
    /// </summary>
    /// <typeparam name="T">ViewModel type</typeparam>
    /// <typeparam name="M">Model type</typeparam>
    public class SynchronizedObservableCollection<VM, M> : ObservableCollection<VM> where VM : IViewModelExposable<M>
    {
        protected SynchronizedObservableCollection()
        {
        }
        public SynchronizedObservableCollection(System.Collections.Generic.IList<M> modelCollection, Func<M, VM> createViewModelFunc)
        {
            m_modelCollection = modelCollection;

            if (m_modelCollection.Count > 0)
            {
                foreach(M model in m_modelCollection)
                {
                    this.Add(createViewModelFunc(model));
                }
            }
        }
        public static SynchronizedObservableCollection<VM, M> CreateFastCopy(System.Collections.Generic.IList<M> modelCollection, Action<System.Collections.Generic.IList<M>, ObservableCollection<VM>> modelToViewModelCopyAction)
        {
            SynchronizedObservableCollection<VM, M> result = new SynchronizedObservableCollection<VM, M>();

            result.m_modelCollection = modelCollection;

            if (result.m_modelCollection.Count > 0) modelToViewModelCopyAction(result.m_modelCollection, result);

            return result;
        }

        private System.Collections.Generic.IList<M> m_modelCollection;
        public System.Collections.Generic.IList<M> ModelCollection
        {
            get { return m_modelCollection; }
            protected set { m_modelCollection = value; }
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
            if (e.OldItems == null || e.OldStartingIndex < 1) return;
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
            m_modelCollection.Clear();
        }
    }
}
