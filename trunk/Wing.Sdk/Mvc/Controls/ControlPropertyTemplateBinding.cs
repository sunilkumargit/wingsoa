
namespace Wing.Mvc.Controls
{
    internal class ControlPropertyTemplateBinding
    {
        public ControlProperty SourceProperty { get; set; }
        public ControlProperty TargetProperty { get; set; }

        public ControlPropertyTemplateBinding(ControlProperty targetProperty, ControlProperty sourceProperty)
        {
            this.SourceProperty = sourceProperty;
            this.TargetProperty = targetProperty;
        }
    }
}
