
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Identifica o alinhamento vertical do conteúdo de um controle.
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
        /// Alinhar o conteúdo na parte superior do controle.
        /// </summary>
        Top,

        /// <summary>
        /// Alinha o conteúdo na parte inferior do controle.
        /// </summary>
        Bottom,

        /// <summary>
        /// Alinhar o conteúdo no centro do controle.
        /// </summary>
        Middle
    }
}
