/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

using System.Text;

namespace Wing.Olap.Mdx.Compiler
{
    public class MdxGeneratorOptions
    {
        public MdxGeneratorOptions()
        {
        }
        public bool EvaluateConstantExpressions = false;
        public bool EveryBlockFromNewLine = true;
    }
}
