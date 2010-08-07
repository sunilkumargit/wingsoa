using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Utils;

namespace Wing.Client.Sdk.Events
{
    public class ViewBagActiveViewChangedEventArgs
    {
        public IViewBagPresenter Presenter { get; private set; }

        public ViewBagActiveViewChangedEventArgs(IViewBagPresenter presenter)
        {
            Assert.NullArgument(presenter, "presenter");
            Presenter = presenter;
        }
    }
}
