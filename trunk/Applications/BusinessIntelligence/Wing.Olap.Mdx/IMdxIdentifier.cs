/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

namespace Wing.Olap.Mdx
{
    public interface IMdxIdentifier
    {
        string UniqueName { get; }
        string Caption { get; }
    }
}
