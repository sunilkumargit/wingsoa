/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Wing.Olap.Core.Providers
{
    public enum SortTypes
    { 
        None,
        Ascending,
        Descending
    }

    public class MemberInfoCollection : ICollection<MemberInfo>
    {
        //private Dictionary<string, MemberInfo> m_Members = new Dictionary<string, MemberInfo>();
        private List<MemberInfo> m_Members = new List<MemberInfo>();

        public MemberInfoCollection(MemberInfo owner)
        {
            m_Owner = owner;
        }

        public MemberInfoCollection(MemberInfo owner, IEnumerable<MemberInfo> members)
        {
            m_Owner = owner;
            if (members != null)
            {
                foreach (MemberInfo member in members)
                {
                    this.Add(member);
                }
            }
        }

        private int m_MinLevelDepth = 99999;
        public int MinLevelDepth
        {
            get
            {
                if (m_MinLevelDepth == 99999)
                    return 0;
                return m_MinLevelDepth;
            }
        }

        private int m_MaxLevelDepth = 0;
        public int MaxLevelDepth
        {
            get { return m_MaxLevelDepth; }
        }

        public MemberInfo m_Owner = null;

        /*        public MemberInfo this[string id]
                {
                    get
                    {
                        MemberInfo res;
                        m_Members.TryGetValue(id, out res);
                        return res;
                    }
                }*/

        public MemberInfo this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    return m_Members[index];
                }
                return null;
            }
        }

        public bool Contains(string id)
        {
            MemberInfo res = ReverseFind(id);
            return res != null;
        }

        public int ReversePos(MemberInfo member)
        {
            for (int i = m_Members.Count - 1; i >= 0; i--)
            {
                MemberInfo mi = m_Members[i];
                if (member == mi)
                    return i;
            }
            return -1;
        }

        public MemberInfo ReverseFind(string id)
        {
            MemberInfo res = null;

            for (int i = m_Members.Count - 1; i >= 0; i--)
            {
                MemberInfo mi = m_Members[i];
                if (mi.UniqueName == id)
                {
                    res = mi;
                    break;
                }
            }
            return res;
        }

        #region ICollection<MemberInfo> Members

        public void Add(MemberInfo item)
        {
            item.Container = this;
            item.MemberOrder = m_Members.Count;
            item.Parent = this.m_Owner;
            //if (m_Members.ContainsKey(item.UniqueName))
            //{
            //    m_Members[item.UniqueName] = item;
            //}
            //else
            //{
            //m_Members.Add(item.UniqueName, item);
            //}
            m_Members.Add(item);
            m_MaxLevelDepth = Math.Max(item.LevelDepth, m_MaxLevelDepth);
            m_MinLevelDepth = Math.Min(item.LevelDepth, m_MinLevelDepth);
        }

        public void Clear()
        {
            m_Members.Clear();
        }

        public bool Contains(MemberInfo item)
        {
            return m_Members.Contains(item);
        }

        public void CopyTo(MemberInfo[] array, int arrayIndex)
        {
            m_Members.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return m_Members.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(MemberInfo item)
        {
            return m_Members.Remove(item);
        }

        #endregion

        #region IEnumerable<MemberInfo> Members

        public IEnumerator<MemberInfo> GetEnumerator()
        {
            return m_Members.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Members.GetEnumerator();
        }

        #endregion

        public void Sort(SortDescriptor sort)
        {
            m_Members.Sort(new MemberInfo.SortComparer(sort));
        }

        public void Insert(int index, MemberInfo member)
        {
            // Zero-based
            index = Math.Max(0, index);
            // Если индекс слишком большой, то корректируем
            index = Math.Min(m_Members.Count, index);
            member.Container = this;
            member.MemberOrder = index;
            m_Members.Insert(index, member);
        }

        public int IndexOf(MemberInfo mi)
        {
            return m_Members.IndexOf(mi);
        }
    }
}
