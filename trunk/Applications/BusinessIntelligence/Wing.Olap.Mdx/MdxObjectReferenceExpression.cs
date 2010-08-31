/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


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
