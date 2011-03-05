//
// Copyright (C) 2010-2011 Leon Breedt
// ljb -at- bitserf [dot] org
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using System;
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Base class for an item collection.
    /// </summary>
    public abstract class ItemCollection
    {
    }

    /// <summary>
    /// Item collection for implementing collections of objects with the XML model.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class ItemCollection<T> : ItemCollection, IList<T>, IList
    {
        #region Fields
        readonly List<T> _items;
        Dictionary<string, T> _itemsById;
        #endregion

        public ItemCollection()
        {
            _items = new List<T>();
        }

        public ItemCollection(IEnumerable<T> items)
        {
            _items = new List<T>(items);
        }

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }

        public T this[string key]
        {
            get
            {
                if (_itemsById != null)
                    return _itemsById[key];
                throw new KeyNotFoundException(string.Format("No {0} found with key {1}", typeof(T), key));
            }
        }

        public bool ContainsItem(string key)
        {
            if (_itemsById == null)
                return false;
            return _itemsById.ContainsKey(key);
        }

        public bool TryGetItem(string key, out T item)
        {
            item = default(T);
            if (_itemsById == null)
                return false;
            return _itemsById.TryGetValue(key, out item);
        }

        public void Add(T item)
        {
            if (item is IItemWithId)
            {
                if (_itemsById == null)
                    _itemsById = new Dictionary<string, T>();
                _itemsById[((IItemWithId)item).Id] = item;
            }
            _items.Add(item);
        }

        public void AddRange(IEnumerable<T> range)
        {
            if (range != null)
            {
                foreach (var item in range)
                    Add(item);
            }
        }

        public void Clear()
        {
            if (_itemsById != null)
                _itemsById.Clear();
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(T item)
        {
            if (_itemsById != null)
            {
                if (item is IItemWithId)
                    _itemsById.Remove(((IItemWithId)item).Id);
            }
            return _items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        object ICollection.SyncRoot { get { return ((ICollection)_items).SyncRoot; } }

        bool ICollection.IsSynchronized { get { return ((ICollection)_items).IsSynchronized; } }

        void ICollection.CopyTo(Array array, int index)
        {
            _items.CopyTo((T[])array, index);
        }

        int IList.Add(object value)
        {
            _items.Add((T)value);
            return _items.Count;
        }

        bool IList.Contains(object value)
        {
            return _items.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return _items.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            _items.Insert(index, (T)value);
        }

        bool IList.IsFixedSize { get { return ((IList)_items).IsFixedSize; } }

        void IList.Remove(object value)
        {
            _items.Remove((T)value);
        }

        object IList.this[int index] { get { return _items[index]; } set { _items[index] = (T)value; } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}

