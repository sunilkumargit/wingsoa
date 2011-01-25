using System;


namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Aplica um propriedade css que tenha o valor representado como um enumerador. 
    /// Um exemplo de propriedade enumeradas: <see cref="HtmlControl.DisplayProperty"/>, <see cref="HtmlControl.CursorProperty"/>.
    /// </summary>
    public class CssEnumPropertyApplier : IControlPropertyApplier
    {
        /// <summary>
        /// Cria uma nova instancia de <see cref="CssEnumPropertyApplier"/>
        /// </summary>
        /// <param name="cssAttribute">Properpiedade css que será renderizada na saída.</param>
        /// <param name="defaultValue">Valor default para a propriedade. Caso o valor da propriedade no controle for igual ao valor default, então a propriedade não é gravada na saída.</param>
        public CssEnumPropertyApplier(CssProperty cssAttribute, Object defaultValue = null)
        {
            this.CssProperty = cssAttribute;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Propriedade css
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
                result.Styles[CssProperty] = CssConfig.GetItemNameFromEnum(property.PropertyType, value);
        }
    }
}
