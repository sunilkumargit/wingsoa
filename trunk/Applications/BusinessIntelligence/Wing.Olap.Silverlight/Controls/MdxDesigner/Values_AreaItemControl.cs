/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


namespace Wing.Olap.Controls.MdxDesigner
{
    public class Values_AreaItemControl : AreaItemControl
    {
        public Values_AreaItemControl()
            : base (UriResources.Images.DataArea16, Localization.MdxDesigner_ValuesNode_Caption)
        {
            UseDragDrop = true;
        }
    }
}
