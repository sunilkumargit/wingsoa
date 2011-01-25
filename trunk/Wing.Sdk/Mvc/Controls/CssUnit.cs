
namespace Wing.Mvc.Controls
{
    public enum CssUnit
    {
        /// <summary>
        /// Percentage
        /// </summary>
        Percent,

        /// <summary>
        /// Inch
        /// </summary>
        In,

        /// <summary>
        /// Centimeter
        /// </summary>
        Cm,

        /// <summary>
        /// Milimeter
        /// </summary>
        Mm,

        /// <summary>
        /// 1em is equal to the current font size. 2em means 2 times the size of the current font. 
        /// E.g., if an element is displayed with a font of 12 pt, then '2em' is 24 pt. 
        /// The 'em' is a very useful unit in CSS, since it can adapt automatically to the font that the reader uses
        /// </summary>
        Em,

        /// <summary>
        /// one ex is the x-height of a font (x-height is usually about half the font-size)
        /// </summary>
        Ex,

        /// <summary>
        /// point (1 pt is the same as 1/72 inch)
        /// </summary>
        Pt,

        /// <summary>
        /// pica (1 pc is the same as 12 points)
        /// </summary>
        Pc,

        /// <summary>
        /// pixels (a dot on the computer screen)    
        /// </summary>
        Px,
    }
}
