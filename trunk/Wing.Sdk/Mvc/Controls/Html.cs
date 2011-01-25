
namespace Wing.Mvc.Controls
{
    public static class Html
    {
        public class Br : BrTag { }

        public class Div : HtmlTagControl
        {
            public Div() : base(HtmlTag.Div) { }
        }

        public class Span : HtmlTagControl
        {
            public Span() : base(HtmlTag.Span) { }
        }

        public class P : HtmlTagControl
        {
            public P() : base(HtmlTag.P) { }
        }

        public class H1 : HtmlTagControl
        {
            public H1() : base(HtmlTag.H1) { }
        }

        public class H2 : HtmlTagControl
        {
            public H2() : base(HtmlTag.H2) { }
        }

        public class H3 : HtmlTagControl
        {
            public H3() : base(HtmlTag.H3) { }
        }

        public class H4 : HtmlTagControl
        {
            public H4() : base(HtmlTag.H4) { }
        }

        public class H5 : HtmlTagControl
        {
            public H5() : base(HtmlTag.H5) { }
        }

        public class H6 : HtmlTagControl
        {
            public H6() : base(HtmlTag.H6) { }
        }

        public class Pre : HtmlTagControl
        {
            public Pre() : base(HtmlTag.Pre) { }
        }

        public class B : HtmlTagControl
        {
            public B() : base(HtmlTag.B) { }
        }

        public class Strong : HtmlTagControl
        {
            public Strong() : base(HtmlTag.Strong) { }
        }

        public class Ul : HtmlTagControl
        {
            public Ul() : base(HtmlTag.Ul) { }
        }

        public class Li : HtmlTagControl
        {
            public Li() : base(HtmlTag.Li) { }
        }
    }
}
