using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Dziennik
{
    /// <summary>
    /// ObservableCollection which is synchronised with Model collection and can notify about changed property of item in this collection via event in class.
    /// Remember that synchronization is only in one-way. Changes made in ViewModel appears in Model but not vice versa.
    /// To synchronize ViewModel with Model use ResynchronizeWithModel method, but in general you should make changes only in ViewModel.
    /// </summary>
    /// <typeparam name="T">ViewModel type</typeparam>
    /// <typeparam name="M">Model type</typeparam>
    public class SynchronizedPerItemObservableCollection<VM, M> : SynchronizedPerItemObservableCollection<VM, M, System.Collections.Generic.List<M>>
                                                                  where VM : IModelExposable<M>, INotifyPropertyChanged, IWorkingCopyAvailable
    {
        public SynchronizedPerItemObservableCollection(System.Collections.Generic.List<M> modelCollection, Func<M, VM> createViewModelFunc)
            : base(modelCollection, createViewModelFunc)
        {
        }
    }

    /// <summary>
    /// ObservableCollection which is synchronised with Model collection and can notify about changed property of item in this collection via event in class.
    /// Remember that synchronization is only in one-way. Changes made in ViewModel appears in Model but not vice versa.
    /// To synchronize ViewModel with Model use ResynchronizeWithModel method, but in general you should make changes only in ViewModel.
    /// </summary>
    /// <typeparam name="T">ViewModel type</typeparam>
    /// <typeparam name="M">Model type</typeparam>
    /// <typeparam name="MC">Model collection</typeparam>
    public class SynchronizedPerItemObservableCollection<VM, M, MC> : SynchronizedObservableCollection<VM, M, MC>
                                                                      where VM : IModelExposable<M>, INotifyPropertyChanged, IWorkingCopyAvailable
                                                                      where MC : System.Collections.Generic.IList<M>
    {
        protected SynchronizedPerItemObservableCollection()
            : base()
        {
        }
        public SynchronizedPerItemObservableCollection(MC modelCollection, Func<M, VM> createViewModelFunc)
        {
            ConstructorImpl(this, modelCollection, createViewModelFunc);
        }
        public static new SynchronizedPerItemObservableCollection<VM, M, MC> CreateFastCopy(MC modelCollection, Action<MC, ObservableCollection<VM>> modelToViewModelCopyAction)
        {
            SynchronizedPerItemObservableCollection<VM, M, MC> result = new SynchronizedPerItemObservableCollection<VM, M, MC>();

            CreateFastCopyImpl(result, modelCollection, modelToViewModelCopyAction);

            return result;
        }

        public event EventHandler<ItemPropertyInCollectionChangedEventArgs<VM>> ItemPropertyInCollectionChanged;

        protected override void ClearItems()
        {
            foreach (VM item in this) item.PropertyChanged -= item_PropertyChanged;
            base.ClearItems();
        }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move)
            {
                if (e.OldItems != null)
                {
                    foreach (VM item in e.OldItems) item.PropertyChanged -= item_PropertyChanged;
                }

                if (e.NewItems != null)
                {
                    foreach (VM item in e.NewItems) item.PropertyChanged += item_PropertyChanged;
                }
            }

            base.OnCollectionChanged(e);
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnItemPropertyInCollectionChanged(new ItemPropertyInCollectionChangedEventArgs<VM>((VM)sender, e));
        }

        protected virtual void OnItemPropertyInCollectionChanged(ItemPropertyInCollectionChangedEventArgs<VM> e)
        {
            EventHandler<ItemPropertyInCollectionChangedEventArgs<VM>> handler = ItemPropertyInCollectionChanged;
            if (handler != null) handler(this, e);
        }
    }
}
