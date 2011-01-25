using System;
using System.Linq;
using System.Collections.ObjectModel;


namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerStepThrough]
    public class HtmlStyleCollection : ObservableCollection<HtmlStyleItem>
    {
        public void Add(CssProperty style, String value)
        {
            this[style] = value;
        }

        public String this[CssProperty style]
        {
            get
            {
                var item = GetItem(style);
                return item == null ? "" : item.Value;
            }
            set
            {
                var item = GetItem(style);
                if (String.IsNullOrEmpty(value))
                {
                    if (item != null)
                        Remove(item);
                    return;
                }
                else if (item == null)
                {
                    item = new HtmlStyleItem(style, value);
                    Add(item);
                }
                else
                    item.Value = value;
            }
        }

        private HtmlStyleItem GetItem(CssProperty style)
        {
            return this.FirstOrDefault(i => i.Style == style);
        }

        public String ToStyleString()
        {
            String result = "";
            foreach (HtmlStyleItem item in this)
                result += item.StyleTag + ":" + (item.Value ?? "") + ";";
            return result;
        }

        public void Remove(CssProperty htmlStyle)
        {
            var item = GetItem(htmlStyle);
            if (item != null)
                Remove(item);
        }
    }
}
