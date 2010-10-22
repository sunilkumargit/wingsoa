/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Controls.General.ItemControls;

namespace Wing.Olap.Controls.MemberChoice.Filter
{
    public enum CustomControlTypes 
    {
        None,
        Delete,
        Clear,
        AddOperation,
        AddOperand
    }

    public class CustomItemEventArgs : EventArgs
    {
        public readonly CustomControlTypes Type = CustomControlTypes.None;

        public CustomItemEventArgs(CustomControlTypes type)
        {
            Type = type;
        }
    }

    public class CustomItemControl : ItemControlBase
    {
        public CustomItemControl(CustomControlTypes type)
        {
            Type = type;
            Text = type.ToString();

            switch (type)
            { 
                case CustomControlTypes.Delete:
                    Icon = UriResources.Images.RemoveGroup16;
                    Text = Localization.Delete;
                    break;
                case CustomControlTypes.Clear:
                    Icon = UriResources.Images.Clear16;
                    Text = Localization.Clear;
                    break;
                case CustomControlTypes.AddOperand:
                    Icon = UriResources.Images.AddOperand16;
                    Text = Localization.Filter_AddOperand;
                    break;
                case CustomControlTypes.AddOperation:
                    Icon = UriResources.Images.AddOperation16;
                    Text = Localization.Filter_AddOperation;
                    break;

            }
            
        }

        public CustomControlTypes Type = CustomControlTypes.None;
    }
}