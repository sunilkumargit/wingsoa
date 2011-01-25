using System;


namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Aplica uma propriedade css a saída de um controle.
    /// </summary>
    public class CssPropertyApplier : IControlPropertyApplier
    {
        /// <summary>
        /// Cria uma nova instancia de <see cref="CssPropertyApplier"/>
        /// </summary>
        /// <param name="cssAttribute">Propriedade css que será gravada na saída.</param>
        /// <param name="defaultValue">Valor default para a propriedade. Caso o valor da propriedade no controle seja igual ao valor default, então a propriedade não é gravada na saída.</param>
        public CssPropertyApplier(CssProperty cssAttribute, Object defaultValue = null)
        {
            this.CssProperty = cssAttribute;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Propriedade css.
        /// </summary>
        public CssProperty CssProperty { get; private set; }


        /// <summary>
        /// Valor default.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Aplica a propriedade à saída de um controle.
        /// </summary>
        /// <param name="target">Controle alvo</param>
        /// <param name="property">Propriedade sendo aplicada</param>
        /// <param name="value">Valor da propriedade sendo aplicada</param>
        /// <param name="result">Resultado da saida da renderização atual.</param>
        public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            if (value != null && value.AsString() != DefaultValue.AsString())
            {
                var strValue = (value ?? "").ToString();
                if (strValue.HasValue())
                    result.Styles[CssProperty] = strValue.ToLowerInvariant();
            }
        }
    }
}
