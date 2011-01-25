
namespace Wing.Mvc.Controls
{
    public interface IControlInstanceAdapter
    {
        void SetCustomApplier(ControlProperty property, IControlPropertyApplier applier);
        void IgnoreApplier(params ControlProperty[] properties);
    }
}
