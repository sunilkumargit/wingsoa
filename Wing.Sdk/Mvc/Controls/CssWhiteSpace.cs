
namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Specifies the white space behavior on an hmtl element.
    /// </summary>
    /// <remarks>
    /// Specifies what behavior the HTML element will be adopt when a white-space is encountered.
    /// </remarks>
    public enum CssWhiteSpace
    {
        /// <summary>
        /// Sequences of whitespace will collapse into a single whitespace. Text will wrap when necessary. This is default.
        /// </summary>
        Normal,

        /// <summary>
        /// Sequences of whitespace will collapse into a single whitespace. Text will never wrap to the next line. 
        /// The text continues on the same line until a <br /> tag is encountered
        /// </summary>
        NoWrap,

        /// <summary>
        /// Whitespace is preserved by the browser. Text will only wrap on line breaks Acts like the PRE tag in HTML
        /// </summary>
        Pre,

        /// <summary>
        /// Sequences of whitespace will collapse into a single whitespace. Text will wrap when necessary, and on line breaks
        /// </summary>
        PreLine,

        /// <summary>
        /// Whitespace is preserved by the browser. Text will wrap when necessary, and on line breaks
        /// </summary>
        PreWrap,

        /// <summary>
        /// Specifies that the value of the white-space property should be inherited from the parent element
        /// </summary>
        Inherit
    }
}
