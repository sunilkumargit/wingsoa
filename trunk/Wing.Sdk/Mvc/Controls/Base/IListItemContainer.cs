
namespace Wing.Mvc.Controls.Base
{
    public interface IListItemContainer
    {
        void NotifyItemSelectedPropertyChanged(ListItemBase item, bool isSelected);
    }
}
