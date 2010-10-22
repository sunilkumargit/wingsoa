
namespace Wing.Olap.Controls.General.ItemControls
{
    public class NoneItemControl : ItemControlBase
    {
        public NoneItemControl()
            : base(false)
        {
            Text = Localization.ComboBoxItem_None;
        }
    }
}
