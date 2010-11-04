namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    public sealed class InputBindingCollection : Collection<InputBinding>
    {
        internal InputBinding FindMatch(object targetElement, RoutedEventArgs inputEventArgs)
        {
            for (int i = base.Count - 1; i >= 0; i--)
            {
                InputBinding binding = base[i];
                if (((binding.Command != null) && (binding.Gesture != null)) && binding.Gesture.Matches(targetElement, inputEventArgs))
                {
                    return binding;
                }
            }
            return null;
        }
    }
}

