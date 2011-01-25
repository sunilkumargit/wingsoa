using System;
using System.Collections.ObjectModel;


namespace Wing.Mvc.Controls
{
    public class StyleLinkCollection : ObservableCollection<StyleLink>
    {
        public void AddUrl(String url, String media)
        {
            Add(new StyleLink() { Url = url, Media = media });
        }

        public void AddContent(String content, String media)
        {
            Add(new StyleLink() { Content = content, Media = media });
        }

        public void AddUrl(String url)
        {
            AddUrl(url, "All");
        }

        public void AddContent(String content)
        {
            AddContent(content, "All");
        }

        public void AddContentFromFile(String filePath)
        {
            Add(new StyleLink() { FromFile = filePath });
        }

    }
}
