/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxObjectList<T> : MdxObject, IList<T> where T : MdxObject
    {
        public delegate void dlgtOnChanged();
        public readonly List<T> List = new List<T>();
        public event dlgtOnChanged ListChanged;
        void onListChanged()
        {
            _ChildTokens = null;
            if (ListChanged != null)
                ListChanged();
        }

        public MdxObjectList(dlgtOnChanged onchanged) { }

        public MdxObjectList() { }
        public MdxObjectList(IEnumerable<T> args)
        {
            if (args != null)
                AddRange(args);
        }
        public void AddRange(IEnumerable<T> items)
        {
            if (items != null)
                List.AddRange(items);

            onListChanged();
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            List.Insert(index, item);
            onListChanged();
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            onListChanged();
        }

        public T this[int index]
        {
            get
            {
                return List[index];
            }
            set
            {
                List[index] = value;
                onListChanged();
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            List.Add(item);
            onListChanged();
        }

        public void Clear()
        {
            List.Clear();
            onListChanged();
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return List.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            try
            {
                return List.Remove(item);
            }
            finally
            {
                onListChanged();
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)List).GetEnumerator();
        }

        #endregion

        public override string SelfToken
        {
            get { return "MDXObjectList"; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            foreach (var o in List)
                _ChildTokens.Add(o);
        }

        public override object Clone()
        {
            var list = new MdxObjectList<T>();
            this.List.ForEach
            (item => list.Add
                (item == null
                ? (T)null
                : (T)item.Clone()
                )
            );

            return list;
        }
    }
}
