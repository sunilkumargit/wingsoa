

namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Aplica uma propriedade com um texto dentro da tag do controle.
    /// </summary>
    public class TextContentApplier : IControlPropertyApplier
    {
        private bool Raw;

        /// <summary>
        /// Cria uma nova instancia de <see cref="TextContentApplier"/>
        /// </summary>
        /// <param name="raw">Informa se a saída é para ser gravada sem alterações ou deve ser codificada para saída HTML.</param>
        public TextContentApplier(bool raw = false)
        {
            this.Raw = raw;
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
            if (value.AsString().HasValue())
            {
                if (Raw)
                    result.RawInnerText.Append(value.AsString());
                else
                    result.InnerText.Append(value.AsString());
            }
        }
    }
}
