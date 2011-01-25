
namespace Wing.Mvc.Controls
{
    public enum CssClear
    {
        /// <summary>
        /// No floating elements allowed on the left side
        /// </summary>
        Left,

        /// <summary>
        /// No floating elements allowed on the right side
        /// </summary>
        Right,

        /// <summary>
        /// No floating elements allowed on either the left or the right side
        /// </summary>
        Both,

        /// <summary>
        /// Default. Allows floating elements on both sides
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the value of the clear property should be inherited from the parent element
        /// </summary>
        Inherit
    }
}
