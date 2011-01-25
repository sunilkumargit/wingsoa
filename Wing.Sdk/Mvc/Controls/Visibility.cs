
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Identifica a visibilidade de um controle no browser.
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// O controle é visivel, Default.
        /// </summary>
        Visible,

        /// <summary>
        /// O controle não é visivel mas seu espaço na tela é ocupado.
        /// </summary>
        Hidden,

        /// <summary>
        /// O controle não é visivel: mesmo que css 'display:none'.
        /// </summary>
        None
    }
}
