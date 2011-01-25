
namespace Wing.Mvc.Controls
{
    public enum CssPosition
    {
        /// <summary>
        /// Default. No position, the element occurs in the normal flow (ignores any top, bottom, left, right, or z-index declarations)
        /// </summary>
        Static,

        /// <summary>
        /// Generates an absolutely positioned element, positioned relative to the first parent element that has a position other than static. The element's position is specified with the "left", "top", "right", and "bottom" properties
        /// </summary>
        Absolute,

        /// <summary>
        /// Generates an absolutely positioned element, positioned relative to the browser window. The element's position is specified with the "left", "top", "right", and "bottom" properties
        /// </summary>
        Fixed,

        /// <summary>
        /// Generates a relatively positioned element, positioned relative to its normal position, so "left:20" adds 20 pixels to the element's LEFT position
        /// </summary>
        Relative,

        /// <summary>
        /// Specifies that the value of the position property should be inherited from the parent element        
        /// </summary>
        inherit,
    }
}
