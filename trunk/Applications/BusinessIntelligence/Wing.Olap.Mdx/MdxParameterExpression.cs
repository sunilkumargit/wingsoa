/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


namespace Wing.Olap.Mdx
{

    public sealed class MdxParameterExpression : MdxExpression
    {
        public string Literal;
        public MdxParameterExpression(string Literal)
        {
            this.Literal = Literal;
        }
        public override string SelfToken
        {
            get { return "@" + Literal; }
        }

        public override object Clone()
        {
            return new MdxParameterExpression(this.Literal);
        }
    }
}
