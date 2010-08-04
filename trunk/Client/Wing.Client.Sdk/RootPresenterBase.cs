using System;

namespace Wing.Client.Sdk
{
    public abstract class RootViewPresenterBase : IRootViewPresenter
    {

        public IRootView View
        {
            get { throw new NotImplementedException(); }
        }
    }
}
