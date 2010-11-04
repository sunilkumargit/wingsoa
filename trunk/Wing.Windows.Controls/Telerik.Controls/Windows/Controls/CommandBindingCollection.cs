namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;

    public sealed class CommandBindingCollection : Collection<CommandBinding>
    {
        internal ICommand FindMatch(object targetElement, RoutedEventArgs inputEventArgs)
        {
            for (int i = 0; i < base.Count; i++)
            {
                CommandBinding binding = base[i];
                RoutedCommand command = binding.Command as RoutedCommand;
                if (command != null)
                {
                    InputGestureCollection inputGesturesInternal = command.InputGestures;
                    if ((inputGesturesInternal != null) && (inputGesturesInternal.FindMatch(targetElement, inputEventArgs) != null))
                    {
                        return command;
                    }
                }
            }
            return null;
        }

        internal CommandBinding FindMatch(ICommand command, ref int index)
        {
            while (index < base.Count)
            {
                CommandBinding binding = base[index++];
                if (binding.Command == command)
                {
                    return binding;
                }
            }
            return null;
        }
    }
}

