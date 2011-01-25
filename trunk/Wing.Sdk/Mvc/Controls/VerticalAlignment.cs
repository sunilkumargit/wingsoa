
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Identifica o alinhamento vertical do conte�do de um controle.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// O alinhamento deve ser herdado do controle pai.
        /// </summary>
        inherit,

        /// <summary>
        /// O texto deve ser alinhado abaixo.
        /// </summary>
        BaseLine,

        /// <summary>
        /// Alinhar o conte�do na parte superior do controle.
        /// </summary>
        Top,

        /// <summary>
        /// Alinha o conte�do na parte inferior do controle.
        /// </summary>
        Bottom,

        /// <summary>
        /// Alinhar o conte�do no centro do controle.
        /// </summary>
        Middle
    }
}
