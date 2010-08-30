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
using System.IO;

namespace Wing.Olap.Mdx.Compiler
{
    internal sealed class DefaultMdxGenerator : IMdxGenerator
    {
        TextWriter tw;
        int curShift = 0;
        string curShiftStr = "";
        bool NoSpaceBefore = true;
        MdxGeneratorOptions options = new MdxGeneratorOptions();
        public void GenerateMdxFromDom(MdxObject mdx, TextWriter tw, MdxGeneratorOptions op)
        {
            this.tw = tw;
            this.options = op;
            this.NoSpaceBefore = true;
            curShift = 0;
            curShiftStr = "";
            mdx = CalculateLength(mdx);
            GenerateMdx(mdx);
        }
        void inc()
        {
            curShift++;
            curShiftStr = new string(' ', curShift << 1);
        }
        void dec()
        {
            curShift--;
            curShiftStr = new string(' ', curShift << 1);
        }
        void Write(string str)
        {
            tw.Write(str);
        }

        MdxObject CalculateLength(MdxObject MdxObject)
        {
            if (MdxObject == null)
                return null;

            MdxObject = tryReduceIIF(MdxObject);

            if (MdxObject.ChildTokens != null)
            {
                MdxObject._Length = 0;
                foreach (var mdx in MdxObject.ChildTokens)
                {
                    var mdx1 = CalculateLength(mdx);
                    if (mdx1 != null)
                    {
                        MdxObject._Length += mdx1._Length;
                    }
                    else
                        MdxObject._Length += 80;
                }
            }
            else if (MdxObject.SelfToken == null)
                MdxObject._Length = 0;
            else
                MdxObject._Length = MdxObject.SelfToken.Length + 1; // 1 - SPACE before token

            return MdxObject;

        }
        static string ConstAsString(MdxConstantExpression cnst)
        {
            var result = cnst.Value;
            if (result == null)
                return null;

            if (cnst.Kind != MdxConstantKind.String)
                return result;
            return result.Substring(1, result.Length - 2);
        }
        MdxObject tryReduceIIF(MdxObject MdxObject)
        {
            var tuple = MdxObject as MdxTupleExpression;
            if (tuple != null)
                if (tuple.Members.Count == 1)
                    return tryReduceIIF(tuple.Members[0]);

            if (!options.EvaluateConstantExpressions)
                return MdxObject;

            var Func = MdxObject as MdxFunctionExpression;
            if (Func == null)
                return MdxObject;

            if (Func.Name == null)
                return MdxObject;
            if (Func.Name.ToLower() != "iif")
                return MdxObject;
            if (Func.Arguments.Count != 3)
                return MdxObject;

            var cond = tryReduceIIF(Func.Arguments[0]) as MdxBinaryExpression;
            if (cond == null)
                return MdxObject;
            if (cond.Operator != "=")
                return MdxObject;

            var left = tryReduceIIF(cond.Left) as MdxConstantExpression;
            if (left == null)
                return MdxObject;
            var right = tryReduceIIF(cond.Right) as MdxConstantExpression;
            if (right == null)
                return MdxObject;

            if (left.Value == null)
                return MdxObject;
            if (right.Value == null)
                return MdxObject;

            if (ConstAsString(left) == ConstAsString(right))
                return tryReduceIIF(Func.Arguments[1]);
            else
                return tryReduceIIF(Func.Arguments[2]);
        }

        // bool LastNeedSpace = false;
        bool LastNeedNewLine = false;
        public void GenerateMdx(MdxObject mdx)
        {
            if (null == mdx)
            {
                Write("!ERROR while parsing MDX!");
                LastNeedNewLine = true;
                return;
            }
            mdx = tryReduceIIF(mdx);
            int curShiftSave = this.curShift;
            string curShiftStrSave = this.curShiftStr;
            try
            {
                if (mdx.ChildTokens != null)
                {
                    bool NeedNewLine = (mdx._Length + curShift) > 80;
                    foreach (MdxObject mdxChild in mdx.ChildTokens)
                        if (MdxObject.IncShift == mdxChild)
                            inc();
                        else if (MdxObject.DecShift == mdxChild)
                            dec();
                        else if (mdxChild == MdxObject.NewLine)
                        {
                            if (NeedNewLine)
                                LastNeedNewLine = true;
                        }
                        else GenerateMdx(mdxChild);
                }
                else if (mdx.SelfToken != null)
                {
                    if (mdx.SelfToken.Length > 0)
                    {
                        if (LastNeedNewLine)
                        {
                            tw.WriteLine();
                            tw.Write(curShiftStr);
                        }
                        else if (!NoSpaceBefore)
                        {
                            if (mdx.SelfToken != ".")
                            {
                                tw.Write(" ");
                            }
                        }
                        Write(mdx.SelfToken);
                        if (mdx.SelfToken == ".")
                            NoSpaceBefore = true;
                        else
                            NoSpaceBefore = false;

                        LastNeedNewLine = false;
                        //LastNeedSpace = mdx.NeedSpaceAfter;
                    }
                }
                else
                {
                    tw.WriteLine();
                    tw.Write(curShiftStr);
                    Write("?Unknown token=" + mdx.ToString() + "?");
                    LastNeedNewLine = true;
                }
            }
            finally
            {
                this.curShift = curShiftSave;
                this.curShiftStr = curShiftStrSave;
            }
        }
    }
}
