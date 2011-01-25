using System;

namespace Wing
{
    public class CustomFormatter : IValueFormatter
    {
        public Func<Object, String> FormaterDelegate { get; set; }

        public CustomFormatter(Func<Object, String> formatterDelegate)
        {
            Assert.NullArgument(formatterDelegate, "formatterDelegate");
            FormaterDelegate = formatterDelegate;
        }

        #region IValueFormatter Members

        public string Format(object value)
        {
            return FormaterDelegate.Invoke(value);
        }

        public virtual TextAlignment Align { get { return TextAlignment.Default; } }

        #endregion
    }
}
