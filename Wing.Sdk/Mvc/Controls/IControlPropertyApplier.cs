using System;

namespace Wing.Mvc.Controls
{
    public interface IControlPropertyApplier
    {
        /// <summary>
        /// Applies a ControlProperty value to a HtmlControl output.
        /// </summary>
        /// <param name="target">Target control where the property content must be applied</param>
        /// <param name="property">ControlProperty beeing applied</param>
        /// <param name="value">Property value</param>
        /// <param name="result">Context o current rendering. Chagens made in this object will be writed on output stream.</param>
        void ApplyProperty(HtmlObject target,
            ControlProperty property,
            Object value,
            ControlPropertyApplyResult result);
    }
}
