
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Aplica o atributo de visibilidade a um controle.
    /// </summary>
    public class VisibilityAttributeApplier : IControlPropertyApplier
    {
        /// <summary>
        /// Aplica o atributo de visibilidade a um controle.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            var state = (Visibility)value;
            switch (state)
            {
                case Visibility.Visible:
                    result.Styles.Remove(CssProperty.Visibility);
                    break;
                case Visibility.Hidden:
                    result.Styles[CssProperty.Visibility] = state.ToString().ToLower();
                    break;
                case Visibility.None:
                    result.Styles.Remove(CssProperty.Visibility);
                    result.Styles[CssProperty.Display] = "none";
                    break;
            }
        }
    }
}
