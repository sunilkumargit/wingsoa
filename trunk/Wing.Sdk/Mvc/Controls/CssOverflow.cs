
namespace Wing.Mvc.Controls
{
    public enum CssOverflow
    {
        /// <summary>
        /// The overflow is not clipped. It renders outside the element's box. This is default
        /// </summary>
        Visible,

        /// <summary>
        /// The overflow is clipped, and the rest of the content will be invisible
        /// </summary>
        Hidden,

        /// <summary>
        /// The overflow is clipped, but a scroll-bar is added to see the rest of the content
        /// </summary>
        Scroll,

        /// <summary>
        /// If overflow is clipped, a scroll-bar should be added to see the rest of the content 
        /// </summary>
        Auto,
    }
}
