using System;
namespace Wing.Mvc.Controls.Base
{
    public interface IFormField
    {
        bool IsDisabled { get; set; }
        string Name { get; set; }
    }
}
