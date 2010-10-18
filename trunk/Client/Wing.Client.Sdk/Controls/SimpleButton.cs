using System.Windows.Controls;

namespace Wing.Client.Sdk.Controls
{
    public class SimpleButton : Button
    {
        public SimpleButton()
        {
            DefaultStyleKey = typeof(SimpleButton);
            this.Height = 22;
            this.Width = 75;
        }
    }
}
