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
    public sealed class MdxAxis : MdxObject
    {
        public bool NonEmpty = false;
        bool getNonEmty() { return NonEmpty; }
        public string Name;

        public readonly MdxObjectList<MdxExpression> DimensionProperties = new MdxObjectList<MdxExpression>();

        MdxExpression _Expression = null;

        string getName() { return Name; }
        public MdxExpression Expression
        {
            get { return _Expression; }
            set { _Expression = value; _ClearChildTokens(); }
        }
        MdxExpression _Having = null;
        public MdxExpression Having
        {
            get { return _Having; }
            set { _Having = value; _ClearChildTokens(); }
        }

        public MdxAxis()
        {
            DimensionProperties.ListChanged += _ClearChildTokens;
        }
        public MdxAxis(string name, MdxExpression expression)
            : this()
        {
            this.Name = name;
            this.Expression = expression;
        }
        public MdxAxis(
                string name,
                MdxExpression expression,
                MdxExpression having,
                IEnumerable<MdxExpression> DimensionProperties)
            : this(name, expression)
        {
            this.Having = having;
            if (DimensionProperties != null)
                this.DimensionProperties.AddRange(DimensionProperties);
        }

        public static MdxToken ON = new MdxToken("ON");
        public static MdxToken NON_EMPTY = new MdxToken("NON EMPTY");
        public static MdxToken DIMENSION_PROPERTIES = new MdxToken("DIMENSION PROPERTIES");

        public override string SelfToken
        {
            get { return "Axis " + Name; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            if (NonEmpty)
                _ChildTokens.Add(NON_EMPTY);

            _ChildTokens.Add(Expression);
            if (DimensionProperties.Count > 0)
            {
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(DIMENSION_PROPERTIES);
                _ChildTokens.Add(DimensionProperties[0]);
                for (int i = 1; i < DimensionProperties.Count; i++)
                {
                    _ChildTokens.Add(COMMA);
                    _ChildTokens.Add(DimensionProperties[i]);
                }
            }
            _ChildTokens.Add(ON);
            _ChildTokens.Add(new MdxRef(getName));
        }

        public override object Clone()
        {
            return new MdxAxis(
                    this.Name,
                    (MdxExpression)this.Expression.Clone(),
                    this.Having != null ? (MdxExpression)this.Having.Clone() : null,
                    (IEnumerable<MdxExpression>)this.DimensionProperties.Clone())
            {
                NonEmpty = this.NonEmpty
            };
        }
        static readonly MdxConstantExpression RECURSIVE = new MdxConstantExpression("RECURSIVE");
        static readonly MdxConstantExpression ONE = new MdxConstantExpression("1");
        internal MdxAxis GenerateCurrentAxis(AxisInfo AxesInfo)
        {
            var newExpression = this.Expression;
            foreach (var Hierarchy in AxesInfo.Hierarchies)
            {
                if (Hierarchy.ExpandedCells.Count > 0)
                {
                    var SetExpanded = new MdxSetExpression();
                    foreach (var Tuple in Hierarchy.ExpandedCells.Keys)
                    {
                        SetExpanded.Expressions.Add(new MdxTupleExpression(Tuple));
                    }
                    newExpression = new MdxFunctionExpression
                    ("ORDER"
                    , new MdxFunctionExpression("DRILLDOWNMEMBER", newExpression, SetExpanded, RECURSIVE)
                    , ONE
                    );
                }
                if (Hierarchy.CollapsedCells.Count > 0)
                {
                    var SetCollapsed = new MdxSetExpression();
                    foreach (var Tuple in Hierarchy.CollapsedCells.Keys)
                    {
                        SetCollapsed.Expressions.Add(new MdxTupleExpression(Tuple));
                    }
                    newExpression = new MdxFunctionExpression("DRILLUPMEMBER", newExpression, SetCollapsed);
                }
            }
            if (newExpression == this.Expression)
                return this;

            return new MdxAxis(this.Name, newExpression, this.Having, this.DimensionProperties);
        }
    }
}
