using System;

namespace Wing.Mvc.Controls
{
    public interface IValuePropertyApplier
    {
        void ApplyValueProperty(HtmlObject target, ControlProperty property, Object value, ControlPropertyApplyResult result);
    }
}
