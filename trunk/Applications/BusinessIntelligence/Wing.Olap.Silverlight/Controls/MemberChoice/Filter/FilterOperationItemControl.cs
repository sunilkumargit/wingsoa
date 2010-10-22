/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Controls.General.ItemControls;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.MemberChoice.Filter
{
    public class FilterOperationItemControl : ItemControlBase
    {
        public FilterOperationItemControl(OperationTypes operation)
        {
            Operation = operation;
            Text = operation.ToString();

            switch (operation)
            { 
                case OperationTypes.And:
                    Icon = UriResources.Images.And16;
                    break;
                case OperationTypes.Or:
                    Icon = UriResources.Images.Or16;
                    break;
            }
            
        }

        public OperationTypes Operation = OperationTypes.And;
    }
}