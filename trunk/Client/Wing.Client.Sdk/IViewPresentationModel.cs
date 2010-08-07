using System;

namespace Wing.Client.Sdk
{
    public interface IViewPresentationModel : IPresentationModel
    {
        String Caption { get; set; }
        String Title { get; set; }
    }
}
