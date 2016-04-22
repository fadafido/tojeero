using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tojeero.Forms.Controls
{
    public class ObservedCollection<T>
    {
        public readonly ObservableCollection<T> Source;

        public ObservedCollection()
            : this(new ObservableCollection<T>())
        {
        }

        public ObservedCollection(ObservableCollection<T> aSource)
        {
            Source = aSource;
            Source.CollectionChanged += Source_CollectionChanged;
        }

        public delegate void ObservedCollectionAddDelegate(ObservableCollection<T> aSender, int aIndex, T aItem);

        public event ObservedCollectionAddDelegate OnItemAdded;

        public delegate void ObservedCollectionMoveDelegate(
            ObservableCollection<T> aSender, int aOldIndex, int aNewIndex, T aItem);

        public event ObservedCollectionMoveDelegate OnItemMoved;

        public delegate void ObservedCollectionRemoveDelegate(ObservableCollection<T> aSender, int aIndex, T aItem);

        public event ObservedCollectionRemoveDelegate OnItemRemoved;

        public delegate void ObservedCollectionReplaceDelegate(
            ObservableCollection<T> aSender, int aIndex, T aOldItem, T aNewItem);

        public event ObservedCollectionReplaceDelegate OnItemReplaced;

        public delegate void ObservedCollectionResetDelegate(ObservableCollection<T> aSender);

        public event ObservedCollectionResetDelegate OnCleared;

        protected void CheckOneOrNone(IList aList)
        {
            if (aList.Count > 1)
            {
                throw new Exception("Holy cow. Someone changed ObservableCollection - update ObservedCollection.");
            }
        }

        void Source_CollectionChanged(object aSender, NotifyCollectionChangedEventArgs aArgs)
        {
            switch (aArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (OnItemAdded != null)
                    {
                        CheckOneOrNone(aArgs.NewItems);
                        OnItemAdded(Source, aArgs.NewStartingIndex, (T) aArgs.NewItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (OnItemMoved != null)
                    {
                        CheckOneOrNone(aArgs.NewItems);
                        OnItemMoved(Source, aArgs.OldStartingIndex, aArgs.NewStartingIndex, (T) aArgs.NewItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (OnItemRemoved != null)
                    {
                        CheckOneOrNone(aArgs.OldItems);
                        OnItemRemoved(Source, aArgs.OldStartingIndex, (T) aArgs.OldItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (OnItemReplaced != null)
                    {
                        CheckOneOrNone(aArgs.NewItems);
                        OnItemReplaced(Source, aArgs.OldStartingIndex, (T) aArgs.OldItems[0], (T) aArgs.NewItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (OnCleared != null)
                    {
                        OnCleared(Source);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}