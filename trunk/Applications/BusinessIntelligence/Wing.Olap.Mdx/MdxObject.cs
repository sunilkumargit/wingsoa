/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public class MdxIncShift : MdxToken { }
    public class MdxDecShift : MdxToken { }
    public class MdxTokenWithNewLineBefore : MdxToken
    {
        public MdxTokenWithNewLineBefore(string SelfToken) : base(SelfToken) { }
        public override bool NewLineBefore { get { return true; } }
    }
    public class MdxToken : MdxObject
    {
        string _SelfToken;

        //static bool needSpaceDelimiter(char c)
        //{
        //  if (c == '_')
        //    return true;
        //  if (c < '0')
        //    return false;
        //  if (c <= '9')
        //    return true;
        //  if (c < 'A')
        //    return false;
        //  if (c <= 'Z')
        //    return true;
        //  if (c < 'a')
        //    return false;
        //  if (c <= 'z')
        //    return true;
        //  if (c < '~')
        //    return false;
        //  return true;
        //}
        //public override bool NeedSpaceAfter
        //{
        //  get
        //  {
        //    if (string.IsNullOrEmpty(_SelfToken))
        //      return false;
        //    var c=_SelfToken[_SelfToken.Length-1];

        //    return needSpaceDelimiter(c);
        //  }
        //}
        //public override bool NeedSpaceBefore
        //{
        //  get
        //  {
        //    if (string.IsNullOrEmpty(_SelfToken))
        //      return false;
        //    var c = _SelfToken[0];
        //    return needSpaceDelimiter(c);
        //  }
        //}
        public MdxToken() { this._SelfToken = string.Empty; }
        public MdxToken(string Self) { this._SelfToken = Self; }
        public override string SelfToken { get { return _SelfToken; } }

        public override object Clone()
        {
            return new MdxToken(this._SelfToken);
        }
    }

    public class MdxRef : MdxObject
    {
        Func<string> StringGetter;
        //public override bool NeedSpaceAfter
        //{
        //  get
        //  {
        //    return true;
        //  }
        //}
        //public override bool NeedSpaceBefore
        //{
        //  get
        //  {
        //    return true;
        //  }
        //}

        public MdxRef(Func<string> StringGetter)
        {
            this.StringGetter = StringGetter;
        }
        public override string SelfToken
        {
            get { return StringGetter(); }
        }
        public override object Clone()
        {
            return new MdxRef(this.StringGetter);
        }
    }

    public abstract class MdxObject // : IMdxFastClonable
    {
        static public readonly MdxToken NewLine = new MdxToken();
        static public readonly MdxToken IncShift = new MdxIncShift();
        static public readonly MdxToken DecShift = new MdxDecShift();
        static public readonly MdxToken DOT = new MdxToken(".");
        static public readonly MdxToken COMMA = new MdxToken(",");
        static public readonly MdxToken EQ = new MdxToken("=");
        static public readonly MdxToken LPAREN = new MdxToken("(");
        static public readonly MdxToken RPAREN = new MdxToken(")");
        static public readonly MdxToken LBRACE = new MdxToken("{");
        static public readonly MdxToken RBRACE = new MdxToken("}");

        protected MdxObject()
        {
        }
        public virtual bool NewLineBefore { get { return false; } }
        //public virtual bool NeedSpaceBefore { get { return false; } }
        //public virtual bool NeedSpaceAfter { get { return false; } }
        public abstract string SelfToken { get; }
        protected List<MdxObject> _ChildTokens = null;
        protected void _ClearChildTokens() { _ChildTokens = null; _Length = -1; }
        public ReadOnlyCollection<MdxObject> ChildTokens
        {
            get
            {
                if (_ChildTokens == null)
                    FillChilds();

                if (_ChildTokens == null)
                    return null;

                return _ChildTokens.AsReadOnly();
            }
        }
        internal int _Length = -1;
        //public virtual int Length
        //{
        //  get
        //  {
        //    if (_Length >= 0)
        //      return _Length;

        //    if (ChildTokens == null)
        //      if (SelfToken == null)
        //        return 0;
        //      else
        //        return SelfToken.Length;

        //    _Length = 0;
        //    foreach (var mdx in ChildTokens)
        //      if (mdx != null)
        //        _Length += mdx.Length;
        //      else
        //        _Length += 80;

        //    return _Length;
        //  }
        //}
        protected virtual void FillChilds()
        {
            _Length = -1;
        }


        private Dictionary<string, object> m_UserData;
        public IDictionary<string, object> UserData
        {
            get
            {
                if (m_UserData == null)
                {
                    m_UserData = new Dictionary<string, object>();
                }
                return m_UserData;
            }
        }
        public abstract object Clone();

    }
}
