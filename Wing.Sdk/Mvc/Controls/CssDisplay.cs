
namespace Wing.Mvc.Controls
{
    public enum CssDisplay
    {
        /// <summary>
        /// The display property value will be not setted and the default 
        /// behavior for tag will be used by the brownser (like block for
        /// div tag or inline for span tag)
        /// * this value does not exists in css specification.
        /// </summary>
        Default,

        /// <summary>
        /// The element will generate no box at all
        /// </summary>
        None,

        /// <summary>
        /// The element will generate a block box (a line break before and after the element)
        /// </summary>
        Block,

        /// <summary>
        /// The element will generate an inline box (no line break before or after the element). This is default
        /// </summary>
        Inline,

        /// <summary>
        /// The element will generate a block box, laid out as an inline box
        /// </summary>
        InlineBlock,

        /// <summary>
        /// The element will generate an inline box (like "table" element, with no line break before or after)
        /// </summary>
        InlineTable,

        /// <summary>
        /// The element will generate a block box, and an inline box for the list marker
        /// </summary>
        ListItem,

        /// <summary>
        /// Specifies that the value of the display property should be inherited from the parent element
        /// </summary>
        Inherit
    }
}
