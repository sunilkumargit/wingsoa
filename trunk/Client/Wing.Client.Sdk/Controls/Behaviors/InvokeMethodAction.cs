using System.Windows;
using System.Windows.Interactivity;

namespace Wing.Client.Sdk.Controls.Behaviors
{
    public class InvokeMethodAction : TargetedTriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            if (MethodToInvoke != null)
            {
                MethodToInvoke(Target, null);
            }
        }

        public delegate void Handler(object sender, RoutedEventArgs e);
        public event Handler MethodToInvoke;

    }
}
