using System;


namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Delega a aplicação de uma propriedade.
    /// </summary>
    public class DelegateControlPropertyApplier : IControlPropertyApplier
    {
        /// <summary>
        /// Cria uma nova instancia de <see cref="DelegateControlPropertyApplier"/>
        /// </summary>
        /// <param name="applyCallback">Callback para aplicalção da propriedade. Este operador será invocado sempre que seja necessário aplicar a propriedade a um controle.</param>
        public DelegateControlPropertyApplier(Action<HtmlObject, ControlProperty, Object, ControlPropertyApplyResult> applyCallback)
        {
            Assert.NullArgument(applyCallback, "applyCallback");
            this.Callback = applyCallback;
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
            Callback.Invoke(target, property, value, result);
        }

        /// <summary>
        /// Callback para a aplicação da propriedade.
        /// </summary>
        public Action<HtmlObject, ControlProperty, object, ControlPropertyApplyResult> Callback { get; private set; }
    }
}
