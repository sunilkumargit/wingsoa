
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Aplica uma propriedade do tipo <see cref="Thickness"/> a saída de um controle.
    /// </summary>
    public class ThicknessCssPropertyApplier : IControlPropertyApplier
    {
        /// <summary>
        /// Cria uma nova instancia de <see cref="ThicknessCssPropertyApplier"/>
        /// </summary>
        /// <param name="cssAttribute">Propriedade css que será gravada na saída. Por exemplo: margin, margin, etc.</param>
        public ThicknessCssPropertyApplier(CssProperty cssAttribute)
        {
            this.CssAttribute = cssAttribute;
        }

        /// <summary>
        /// Aplica a propriedade à saída de um controle.
        /// </summary>
        /// <param name="target">Controle alvo</param>
        /// <param name="property">Propriedade sendo aplicada</param>
        /// <param name="value">Valor da propriedade sendo aplicada</param>
        /// <param name="result">Resultado da saida da renderização atual.</param>
        public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            var thickness = value as Thickness;
            if (value != null && thickness.IsSetted())
            {
                if (thickness.AllValuesIsSetted())
                    result.Styles[CssAttribute] = thickness.GetAllValuesString();
                else
                {
                    var str = CssAttribute.ToString().ToLower();
                    if (thickness.Top.HasValue)
                        result.Styles[CssConfig.GetEnumItemFromCssName<CssProperty>(str + "-top")] = thickness.GetTopStringValue();
                    if (thickness.Bottom.HasValue)
                        result.Styles[CssConfig.GetEnumItemFromCssName<CssProperty>(str + "-bottom")] = thickness.GetBottomStringValue();
                    if (thickness.Left.HasValue)
                        result.Styles[CssConfig.GetEnumItemFromCssName<CssProperty>(str + "-left")] = thickness.GetLeftStringValue();
                    if (thickness.Right.HasValue)
                        result.Styles[CssConfig.GetEnumItemFromCssName<CssProperty>(str + "-right")] = thickness.GetRightStringValue();
                }
            }
        }


        /// <summary>
        /// Propriedade css.
        /// </summary>
        public CssProperty CssAttribute { get; private set; }
    }
}
