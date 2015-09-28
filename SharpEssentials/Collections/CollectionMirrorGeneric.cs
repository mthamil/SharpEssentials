using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace SharpEssentials.Collections
{
    /// <summary>
    /// Synchronizes two generic collections that implement INotifyCollectionChanged.
    /// </summary>
    public class CollectionMirror<T> : CollectionMirror<T, T>
    {
        /// <summary>
        /// Initializes a new collection mirror with transformations.
        /// </summary>
        /// <param name="source">The source collection</param>
        /// <param name="target">The target collection</param>
        public CollectionMirror(IList<T> source, IList<T> target)
            : base(source, target, x => x, y => y) { }
    }

    /// <summary>
    /// Synchronizes two generic collections that implement INotifyCollectionChanged.
    /// </summary>
    public class CollectionMirror<TSource, TTarget> : DisposableBase, IWeakEventListener
    {
        /// <summary>
        /// Initializes a new collection mirror with transformations.
        /// </summary>
        /// <param name="source">The source collection</param>
        /// <param name="target">The target collection</param>
        /// <param name="sourceToTarget">A mapping from source items to target items</param>
        /// <param name="targetToSource">An optional mapping back from target items to source items</param>
        public CollectionMirror(IList<TSource> source, IList<TTarget> target, Func<TSource, TTarget> sourceToTarget, Func<TTarget, TSource> targetToSource = null)
        {
            _source = source;
            _target = target;
            _sourceToTarget = sourceToTarget;
            _targetToSource = targetToSource;

            Synchronize(_source, _target, _sourceToTarget);
            Subscribe(_source);
        }

        private void Unsubscribe<T>(ICollection<T> collection)
        {
            var notifierCollection = collection as INotifyCollectionChanged;
            if (notifierCollection != null)
                CollectionChangedEventManager.RemoveListener(notifierCollection, this);
        }

        private void Subscribe<T>(ICollection<T> collection)
        {
            var notifierCollection = collection as INotifyCollectionChanged;
            if (notifierCollection != null)
                CollectionChangedEventManager.AddListener(notifierCollection, this);
        }

        /// <summary>
        /// Ensures that the target collection matches the source collection.
        /// </summary>
        /// <remarks>The current implementation first clears the target, removing all existing items</remarks>
        private void Synchronize<T1, T2>(IList<T1> source, IList<T2> target, Func<T1, T2> mapping)
        {
            using (SuppressChangeEvents(target))
            {
                target.Clear();
                foreach (var item in source)
                    target.Add(mapping(item));
            }
        }

        private void PropagateAdd<T1, T2>(IList<T2> collection, Func<T1, T2> mapping, NotifyCollectionChangedEventArgs args)
        {
            using (SuppressChangeEvents(collection))
            {
                for (int i = 0; i < args.NewItems.Count; i++)
                {
                    int insertionIndex = i + args.NewStartingIndex;
                    var newItem = (T1)args.NewItems[i];

                    if (insertionIndex > collection.Count)
                        collection.Add(mapping(newItem));
                    else
                        collection.Insert(insertionIndex, mapping(newItem));
                }
            }
        }

        private void PropagateRemove<T>(IList<T> collection, NotifyCollectionChangedEventArgs args)
        {
            using (SuppressChangeEvents(collection))
            {
                for (int i = args.OldItems.Count - 1; i >= 0; i--)
                {
                    int removalIndex = i + args.OldStartingIndex;
                    collection.RemoveAt(removalIndex);
                }
            }
        }

        private void PropagateReplace<T1, T2>(IList<T2> collection, Func<T1, T2> mapping, NotifyCollectionChangedEventArgs args)
        {
            using (SuppressChangeEvents(collection))
            {
                for (int i = 0; i < args.NewItems.Count; i++)
                {
                    int replacementIndex = i + args.NewStartingIndex;
                    collection[replacementIndex] = mapping((T1)args.NewItems[replacementIndex]);
                }
            }
        }

        private void PropagateMove<T1, T2>(IList<T2> collection, Func<T1, T2> mapping, NotifyCollectionChangedEventArgs args)
        {
            PropagateRemove(collection, args);
            PropagateAdd(collection, mapping, args);
        }

        private void PropagateReset<T1, T2>(IList<T1> source, IList<T2> collectionToUpdate, Func<T1, T2> mapping)
        {
            Synchronize(source, collectionToUpdate, mapping);
        }

        #region Implementation of IWeakEventListener

        /// <see cref="IWeakEventListener.ReceiveWeakEvent"/>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var changeArgs = (NotifyCollectionChangedEventArgs)e;
            bool sourceChanged = ReferenceEquals(sender, _source);
            switch (changeArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (sourceChanged)
                        PropagateAdd(_target, _sourceToTarget, changeArgs);
                    else if(_targetToSource != null)
                        PropagateAdd(_source, _targetToSource, changeArgs);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (sourceChanged)
                        PropagateRemove(_target, changeArgs);
                    else if(_targetToSource != null)
                        PropagateRemove(_source, changeArgs);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (sourceChanged)
                        PropagateReplace(_target, _sourceToTarget, changeArgs);
                    else if (_targetToSource != null)
                        PropagateReplace(_source, _targetToSource, changeArgs);
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (sourceChanged)
                        PropagateMove(_target, _sourceToTarget, changeArgs);
                    else if (_targetToSource != null)
                        PropagateMove(_source, _targetToSource, changeArgs);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (sourceChanged)
                        PropagateReset(_source, _target, _sourceToTarget);
                    else if (_targetToSource != null)
                        PropagateReset(_target, _source, _targetToSource);
                    break;
            }

            return true;
        }

        #endregion

        #region Implementation of DisposableBase

        /// <see cref="DisposableBase.OnDisposing"/>
        protected override void OnDisposing()
        {
            Unsubscribe(_source);
            Unsubscribe(_target);
        }

        #endregion

        private readonly IList<TSource> _source;
        private readonly IList<TTarget> _target;
        private readonly Func<TSource, TTarget> _sourceToTarget;
        private readonly Func<TTarget, TSource> _targetToSource;

        /// <summary>
        /// Temporarily suppresses handling of change events for a collection.
        /// </summary>
        private IDisposable SuppressChangeEvents<T>(ICollection<T> collection)
        {
            return new ChangeEventSuppressionToken<T>(collection, this);
        }

        private class ChangeEventSuppressionToken<T> : IDisposable
        {
            public ChangeEventSuppressionToken(ICollection<T> collection, CollectionMirror<TSource, TTarget> listener)
            {
                _collection = collection;
                _listener = listener;

                _listener.Unsubscribe(_collection);
            }

            #region Implementation of IDisposable

            public void Dispose()
            {
                _listener.Subscribe(_collection);
            }

            #endregion

            private readonly ICollection<T> _collection;
            private readonly CollectionMirror<TSource, TTarget> _listener;
        }
    }
}