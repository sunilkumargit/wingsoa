
namespace Wing.Mvc.Controls
{
    public interface IControlBuildExtension
    {
        void Extend(HtmlObject target, ExtensionStage stage);
    }
}
