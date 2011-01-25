
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Item de uma lista não-ordenada. Mesmo que a tag HTML 'li'.
    /// Este controle é usado como filho do contorle <see cref="UListControl"/>
    /// </summary>
    public class UListItem : ContainerControl<UListItem>
    {
        /// <summary>
        /// Cria uma nova instancia de <see cref="UListItem"/>
        /// </summary>
        public UListItem() : base(HtmlTag.Li) { }
    }
}
