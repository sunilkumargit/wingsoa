/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of UILibrary.OLAP
 
    UILibrary.OLAP is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with UILibrary.OLAP.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides UILibrary.OLAP under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;

namespace Wing.Olap.Mdx
{
    public enum MdxConstantKind
    {
        String,
        Integer,
        Float,
        Variable,
        Unknown
    }
    public sealed class MdxConstantExpression : MdxExpression
    {
        public MdxConstantExpression(string value)
            : this(value, MdxConstantKind.String)
        {
        }
        public MdxConstantExpression(string value, MdxConstantKind kind)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_Value = value;
            m_Kind = kind;
        }


        private MdxConstantKind m_Kind = MdxConstantKind.Unknown;

        public MdxConstantKind Kind
        {
            get
            {
                return m_Kind;
            }
        }
        private string m_Value;
        public string Value
        {
            get
            {
                return m_Value;
            }
        }
        public string AsString
        {
            get
            {
                return m_Value;
            }
        }
        public override string SelfToken
        {
            get { return AsString; }
        }

        public override object Clone()
        {
            return new MdxConstantExpression(
                    this.Value,
                    this.Kind);
        }
    }
}
