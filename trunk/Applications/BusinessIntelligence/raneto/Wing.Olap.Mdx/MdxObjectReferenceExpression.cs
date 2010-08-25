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

namespace Wing.Olap.Mdx
{
    public enum MdxObjectReferenceKind
    {
        Cube,
        Dimension,
        Hierarchy,
        Level,
        Member,
        Measure,
        NamedSet,
        Kpi,
        Unknown
    }
    public sealed class MdxObjectReferenceExpression : MdxExpression, IMdxIdentifier
    {
        public MdxObjectReferenceExpression(string uniqueName, string caption, MdxObjectReferenceKind kind)
        {
            m_Caption = caption;
            m_UniqueName = uniqueName;
            m_Kind = kind;
        }

        public MdxObjectReferenceExpression(string uniqueName, string caption)
            : this(uniqueName, caption, MdxObjectReferenceKind.Unknown)
        {
        }

        public MdxObjectReferenceExpression(string uniqueName)
            : this(uniqueName, uniqueName, MdxObjectReferenceKind.Unknown)
        {
        }

        //public override bool NeedSpaceBefore { get { return true; } }
        //public override bool NeedSpaceAfter { get { return true; } }

        private MdxObjectReferenceKind m_Kind = MdxObjectReferenceKind.Unknown;
        public MdxObjectReferenceKind Kind
        {
            get
            {
                return m_Kind;
            }
            set
            {
                m_Kind = value;
            }
        }

        public void AppendName(string term)
        {
            m_UniqueName += "." + term;
        }

        #region IMdxIdentifier Members

        private string m_UniqueName;
        public string UniqueName
        {
            get
            {
                return m_UniqueName;
            }
        }

        private string m_Caption;
        public string Caption
        {
            get
            {
                return m_Caption;
            }
        }

        #endregion

        //public override bool Equals(object obj)
        //{
        //    MdxObjectReferenceExpression objRef = obj as MdxObjectReferenceExpression;
        //    if (objRef != null)
        //    {
        //        return objRef.UniqueName.Equals(this.UniqueName);
        //    }
        //    return base.Equals(obj);
        //}

        //public override int GetHashCode()
        //{
        //    return this.UniqueName.GetHashCode();
        //}

        //public override string ToString()
        //{
        //    return string.Format("{0} ({1})", m_Caption, m_UniqueName);
        //}

        public override string SelfToken
        {
            get { return UniqueName; }
        }

        public override object Clone()
        {
            return new MdxObjectReferenceExpression(
                    this.UniqueName,
                    this.Caption,
                    this.Kind);
        }
    }
}
