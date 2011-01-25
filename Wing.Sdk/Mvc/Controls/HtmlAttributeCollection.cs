using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;


namespace Wing.Mvc.Controls
{
    public class HtmlAttributeCollection : ObservableCollection<HtmlAttributeItem>
    {
        public void Add(HtmlAttr name, String value)
        {
            if (!String.IsNullOrEmpty(value))
                this[name] = value;
        }

        public void AddRange(IDictionary<String, String> values)
        {
            foreach (var el in values)
                this[el.Key] = el.Value;
        }

        public String this[HtmlAttr attribute]
        {
            get { return this[attribute.ToString().ToLower()]; }
            set { this[attribute.ToString().ToLower()] = value; }
        }

        public String this[String attributeName]
        {
            get
            {
                var item = GetItem(attributeName);
                return item == null ? "" : item.Value;
            }
            set
            {
                var item = GetItem(attributeName);
                if (String.IsNullOrEmpty(value))
                {
                    if (item != null)
                        Remove(item);
                    return;
                }
                else if (item == null)
                {
                    item = new HtmlAttributeItem() { Name = attributeName, Value = value };
                    Add(item);
                }
                else
                    item.Value = value;
            }
        }

        private HtmlAttributeItem GetItem(String attributeName)
        {
            return this.FirstOrDefault((i) => i.Name.EqualsIgnoreCase(attributeName));
        }

        public void Remove(HtmlAttr attribute)
        {
            Remove(attribute.ToString().ToLower());
        }

        public void Remove(String attributeName)
        {
            var item = GetItem(attributeName);
            if (item != null)
                Remove(item);

        }
    }

}
