
namespace Wing.Mvc.Controls
{
    public class SelectOptionGroup : ContainerControl<SelectOptionGroup, SelectOption>
    {
        public SelectOptionGroup()
            : base(HtmlTag.OptGroup)
        {
            var customizer = GetAdapter();
            customizer.SetCustomApplier(TextProperty, new HtmlAttributePropertyApplier(HtmlAttr.Label));
        }
    }
}
