namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    public sealed class InputGestureCollection : Collection<InputGesture>
    {
        internal InputGesture FindMatch(object targetElement, RoutedEventArgs inputEventArgs)
        {
            for (int i = 0; i < base.Count; i++)
            {
                InputGesture gesture = base[i];
                if (gesture.Matches(targetElement, inputEventArgs))
                {
                    return gesture;
                }
            }
            return null;
        }
    }
}

