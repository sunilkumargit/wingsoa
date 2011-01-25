
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Handler para os eventos de <see cref="HtmlObject"/>
    /// </summary>
    /// <typeparam name="TControl">Tipo concreto do controle.</typeparam>
    /// <param name="control">Controle a que o evento se refere.</param>
    public delegate void TypedControlDelegate<TControl>(TControl control) where TControl : HtmlObject;
}
