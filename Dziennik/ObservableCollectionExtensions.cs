using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public static class ObservableCollectionExtensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection)
        {
            List<T> list = collection.ToList();
            list.Sort();
            CompleteSort(collection, list);
        }
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            List<T> list = collection.ToList();
            list.Sort(comparison);
            CompleteSort(collection, list);
        }
        public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer)
        {
            List<T> list = collection.ToList();
            list.Sort(comparer);
            CompleteSort(collection, list);
        }
        public static void Sort<T>(this ObservableCollection<T> collection, int index, int count, IComparer<T> comparer)
        {
            List<T> list = collection.ToList();
            list.Sort(index, count, comparer);
            CompleteSort(collection, list);
        }
        private static void CompleteSort<T>(ObservableCollection<T> collection, List<T> newItems)
        {
            collection.Clear();
            foreach (T item in newItems) collection.Add(item);
        }
    }
}
