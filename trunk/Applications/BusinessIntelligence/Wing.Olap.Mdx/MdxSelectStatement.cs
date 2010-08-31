﻿/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxSelectStatement : MdxStatement
    {
        public readonly MdxObjectList<MdxWithClauseItem> With = new MdxObjectList<MdxWithClauseItem>();
        public readonly MdxObjectList<MdxAxis> Axes = new MdxObjectList<MdxAxis>();
        public readonly MdxObjectList<MdxExpression> CellProperties = new MdxObjectList<MdxExpression>();
        MdxObjectReferenceExpression _Cube = null;

        public MdxObjectReferenceExpression Cube
        {
            get
            {
                return _Cube;
            }
            set
            {
                _Cube = value;
                _ChildTokens = null;
            }
        }
        public MdxObject CubeSpecification
        {
            get
            {
                if (_Cube != null)
                    return _Cube;
                else
                    return m_SubSelect;
            }
            set
            {
                m_SubSelect = value as MdxSelectStatement;
                _Cube = value as MdxObjectReferenceExpression;
                _ChildTokens = null;
            }
        }
        private MdxWhereClause m_Where = null;
        public MdxWhereClause Where
        {
            get
            {
                return m_Where;
            }
            set
            {
                m_Where = value;
                _ChildTokens = null;
            }
        }


        private MdxSelectStatement m_SubSelect = null;
        public MdxSelectStatement SubSelect
        {
            get
            {
                return m_SubSelect;
            }
            set
            {
                m_SubSelect = value;
                _ChildTokens = null;
            }
        }
        public MdxSelectStatement()
        {
            With.ListChanged += _ClearChildTokens;
            Axes.ListChanged += _ClearChildTokens;
            CellProperties.ListChanged += _ClearChildTokens;
        }
        public MdxSelectStatement(
                IEnumerable<MdxAxis> axes
                )
            : this(null, axes, null, (MdxObjectReferenceExpression)null)
        {
        }
        public MdxSelectStatement(
                IEnumerable<MdxWithClauseItem> with,
                IEnumerable<MdxAxis> axes,
                MdxWhereClause where)
            : this(with, axes, where, (MdxObjectReferenceExpression)null)
        {
        }
        public MdxSelectStatement(
                IEnumerable<MdxWithClauseItem> with,
                IEnumerable<MdxAxis> axes)
            : this(with, axes, null, (MdxObjectReferenceExpression)null)
        {
        }
        public MdxSelectStatement(
                IEnumerable<MdxAxis> axes,
                MdxWhereClause where)
            : this(null, axes, where, (MdxObjectReferenceExpression)null)
        {
        }
        public MdxSelectStatement(
                IEnumerable<MdxWithClauseItem> with,
                IEnumerable<MdxAxis> axes,
                MdxWhereClause where,
                MdxObject CubeSpecification)
            : this()
        {
            if (with != null)
            {
                With.AddRange(with);
            }
            if (axes != null)
            {
                Axes.AddRange(axes);
            }
            m_Where = where;
            this.CubeSpecification = CubeSpecification;
        }
        public MdxSelectStatement(
                IEnumerable<MdxWithClauseItem> with,
                IEnumerable<MdxAxis> axes,
                MdxWhereClause where,
                MdxSelectStatement subSelect)
            : this()
        {
            if (with != null)
            {
                With.AddRange(with);
            }
            if (axes != null)
            {
                Axes.AddRange(axes);
            }
            m_Where = where;
            m_SubSelect = subSelect;
        }

        public MdxSelectStatement(
                IEnumerable<MdxWithClauseItem> with,
                IEnumerable<MdxAxis> axes,
                MdxWhereClause where,
                MdxObjectReferenceExpression cube)
        {
            if (with != null)
            {
                With.AddRange(with);
            }
            if (axes != null)
            {
                Axes.AddRange(axes);
            }
            _Cube = cube;
            m_Where = where;
        }
        public override string SelfToken
        {
            get { return "SELECT ..."; }
        }
        public static MdxObject WITH = new MdxToken("WITH");
        public static MdxObject SELECT = new MdxToken("SELECT");
        // public static MdxObject ALL = new MdxToken(" *");
        public static MdxObject FROM = new MdxToken("FROM");
        public static MdxObject CELL = new MdxToken("CELL");
        public static MdxObject PROPERTIES = new MdxToken("PROPERTIES");

        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            if (With != null)
                if (With.Count > 0)
                {
                    _ChildTokens.Add(WITH);
                    _ChildTokens.Add(IncShift);
                    foreach (var mdxo in With)
                    {
                        _ChildTokens.Add(NewLine);
                        _ChildTokens.Add(mdxo);
                    }
                    _ChildTokens.Add(DecShift);
                    _ChildTokens.Add(NewLine);
                }
            _ChildTokens.Add(SELECT);
            if (Axes != null)
                if (Axes.Count > 0)
                {
                    _ChildTokens.Add(IncShift);
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(Axes[0]);
                    _ChildTokens.Add(DecShift);
                    for (int i = 1; i < Axes.Count; i++)
                    {
                        _ChildTokens.Add(NewLine);
                        _ChildTokens.Add(COMMA);
                        _ChildTokens.Add(IncShift);
                        _ChildTokens.Add(Axes[i]);
                        _ChildTokens.Add(DecShift);
                    }
                }
            //				else
            //					_ChildTokens.Add(ALL);

            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(FROM);
            if (Cube != null)
            {
                _ChildTokens.Add(Cube);
            }
            else if (SubSelect != null)
            {
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(LPAREN);
                _ChildTokens.Add(IncShift);
                _ChildTokens.Add(SubSelect);
                _ChildTokens.Add(DecShift);
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(RPAREN);
            }
            if (Where != null)
                if (Where.Expression != null)
                {
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(Where);
                }
            if (CellProperties.Count > 0)
            {
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(CELL);
                _ChildTokens.Add(PROPERTIES);
                _ChildTokens.Add(CellProperties[0]);
                for (int i = 1; i < CellProperties.Count; i++)
                {
                    _ChildTokens.Add(COMMA);
                    _ChildTokens.Add(CellProperties[i]);
                }
            }
        }
        public override object Clone()
        {
            if (m_SubSelect != null)
            {
                var res =
                new MdxSelectStatement(
                        (IEnumerable<MdxWithClauseItem>)this.With.Clone(),
                        (IEnumerable<MdxAxis>)this.Axes.Clone(),
                        this.Where != null ? (MdxWhereClause)this.Where.Clone() : null,
                        (MdxSelectStatement)this.SubSelect.Clone());
                res.CellProperties.AddRange((IEnumerable<MdxExpression>)this.CellProperties.Clone());

                return res;
            }
            var res2 = new MdxSelectStatement(
                    (IEnumerable<MdxWithClauseItem>)this.With.Clone(),
                    (IEnumerable<MdxAxis>)this.Axes.Clone(),
                    this.Where != null ? (MdxWhereClause)this.Where.Clone() : null,
                    (MdxObjectReferenceExpression)this.Cube.Clone());
            res2.CellProperties.AddRange((IEnumerable<MdxExpression>)this.CellProperties.Clone());

            return res2;
        }
        internal MdxSelectStatement GenerateCurrentStatement(MdxQueryContext MdxQueryContext)
        {
            var newAxes = Axes;

            for (int i = 0; i < Axes.Count; i++)
            {
                var axis = Axes[i];
                var cntAxis = MdxQueryContext.Axes[i];
                var newAxis = axis.GenerateCurrentAxis(cntAxis);
                if (!object.ReferenceEquals(axis, newAxis))
                {
                    if (newAxes == Axes)
                        newAxes = (MdxObjectList<MdxAxis>)Axes.Clone();

                    newAxes[i] = newAxis;
                }
            }
            if (newAxes == Axes)
                return this;

            var result = new MdxSelectStatement(With, newAxes, Where, CubeSpecification);
            return result;
        }
    }
}
