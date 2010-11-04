namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Input;

    public sealed class CommandManager : DependencyObject
    {
        public static readonly Telerik.Windows.RoutedEvent CanExecuteEvent = EventManager.RegisterRoutedEvent("CanExecute", RoutingStrategy.Bubble, typeof(CanExecuteRoutedEventHandler), typeof(CommandManager));
        private static Dictionary<Type, CommandBindingCollection> classCommandBindings = new Dictionary<Type, CommandBindingCollection>();
        private static Dictionary<Type, InputBindingCollection> classInputBindings = new Dictionary<Type, InputBindingCollection>();
        public static readonly DependencyProperty CommandBindingsProperty = DependencyProperty.RegisterAttached("CommandBindings", typeof(CommandBindingCollection), typeof(CommandManager), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(CommandManager.OnCommandBindingsChanged)));
        private static CommandManager commandManager;
        public static readonly Telerik.Windows.RoutedEvent ExecutedEvent = EventManager.RegisterRoutedEvent("Executed", RoutingStrategy.Bubble, typeof(ExecutedRoutedEventHandler), typeof(CommandManager));
        public static readonly DependencyProperty InputBindingsProperty = DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(CommandManager), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(CommandManager.OnInputBindingsChanged)));
        public static readonly Telerik.Windows.RoutedEvent PreviewCanExecuteEvent = EventManager.RegisterRoutedEvent("PreviewCanExecute", RoutingStrategy.Tunnel, typeof(CanExecuteRoutedEventHandler), typeof(CommandManager));
        public static readonly Telerik.Windows.RoutedEvent PreviewExecutedEvent = EventManager.RegisterRoutedEvent("PreviewExecuted", RoutingStrategy.Tunnel, typeof(ExecutedRoutedEventHandler), typeof(CommandManager));
        private List<WeakReference> requerySuggestedHandlers;
        private DispatcherOperation requerySuggestedOperation;

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Requery")]
        public static  event EventHandler RequerySuggested
        {
            add
            {
                AddWeakReferenceHandler(ref Current.requerySuggestedHandlers, value);
            }
            remove
            {
                RemoveWeakReferenceHandler(Current.requerySuggestedHandlers, value);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static CommandManager()
        {
            EventManager.RegisterClassHandler(typeof(UIElement), CanExecuteEvent, new CanExecuteRoutedEventHandler(CommandManager.OnCanExecute));
            EventManager.RegisterClassHandler(typeof(UIElement), ExecutedEvent, new ExecutedRoutedEventHandler(CommandManager.OnExecuted));
        }

        private CommandManager()
        {
        }

        public static void AddCanExecuteHandler(DependencyObject element, CanExecuteRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.AddHandler(CanExecuteEvent, handler);
        }

        public static void AddExecutedHandler(DependencyObject element, ExecutedRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.AddHandler(ExecutedEvent, handler);
        }

        public static void AddPreviewCanExecuteHandler(DependencyObject element, CanExecuteRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.AddHandler(PreviewCanExecuteEvent, handler);
        }

        public static void AddPreviewExecutedHandler(DependencyObject element, ExecutedRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.AddHandler(PreviewExecutedEvent, handler);
        }

        internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers == null)
            {
                handlers = new List<WeakReference>();
            }
            handlers.Add(new WeakReference(handler));
        }

        internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                EventHandler[] handlerArray = new EventHandler[handlers.Count];
                int index = 0;
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler target = reference.Target as EventHandler;
                    if (target == null)
                    {
                        handlers.RemoveAt(i);
                    }
                    else
                    {
                        handlerArray[index] = target;
                        index++;
                    }
                }
                for (int j = 0; j < index; j++)
                {
                    EventHandler handler2 = handlerArray[j];
                    handler2(null, EventArgs.Empty);
                }
            }
        }

        private static bool CanExecuteCommandBinding(object sender, CanExecuteRoutedEventArgs e, CommandBinding commandBinding)
        {
            commandBinding.OnCanExecute(sender, e);
            if (!e.CanExecute)
            {
                return e.Handled;
            }
            return true;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="inputEventArgs")]
        private static void ExecuteCommand(RoutedCommand routedCommand, object parameter, UIElement target, RoutedEventArgs inputEventArgs)
        {
            routedCommand.Execute(parameter, target);
        }

        private static bool ExecuteCommandBinding(object sender, ExecutedRoutedEventArgs e, CommandBinding commandBinding)
        {
            commandBinding.OnExecuted(sender, e);
            return e.Handled;
        }

        private static void FindCommandBinding(object sender, RoutedEventArgs e, ICommand command, bool execute)
        {
            CommandBindingCollection commandBindings = GetCommandBindings(sender as DependencyObject);
            if (commandBindings != null)
            {
                FindCommandBinding(commandBindings, sender, e, command, execute);
            }
            lock (((ICollection) classCommandBindings).SyncRoot)
            {
                if (classCommandBindings.Count != 0)
                {
                    for (Type type = sender.GetType(); type != null; type = type.BaseType)
                    {
                        if (classCommandBindings.ContainsKey(type))
                        {
                            CommandBindingCollection bindings2 = classCommandBindings[type];
                            FindCommandBinding(bindings2, sender, e, command, execute);
                        }
                    }
                }
            }
        }

        private static void FindCommandBinding(CommandBindingCollection commandBindings, object sender, RoutedEventArgs e, ICommand command, bool execute)
        {
            CommandBinding binding;
            int index = 0;
            do
            {
                binding = commandBindings.FindMatch(command, ref index);
            }
            while (((binding != null) && (!execute || !ExecuteCommandBinding(sender, (ExecutedRoutedEventArgs) e, binding))) && (execute || !CanExecuteCommandBinding(sender, (CanExecuteRoutedEventArgs) e, binding)));
        }

        public static CommandBindingCollection GetCommandBindings(DependencyObject element)
        {
            return (CommandBindingCollection) element.GetValue(CommandBindingsProperty);
        }

        public static InputBindingCollection GetInputBindings(DependencyObject element)
        {
            return (InputBindingCollection) element.GetValue(InputBindingsProperty);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Requery")]
        public static void InvalidateRequerySuggested()
        {
            Current.RaiseRequerySuggested();
        }

        internal static void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (((sender != null) && (e != null)) && (e.Command != null))
            {
                FindCommandBinding(sender, e, e.Command, false);
            }
        }

        private static void OnCommandBindingsChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null)
            {
                RemoveCanExecuteHandler(element, new CanExecuteRoutedEventHandler(CommandManager.OnCanExecute));
                RemoveExecutedHandler(element, new ExecutedRoutedEventHandler(CommandManager.OnExecuted));
            }
            if (args.NewValue != null)
            {
                AddCanExecuteHandler(element, new CanExecuteRoutedEventHandler(CommandManager.OnCanExecute));
                AddExecutedHandler(element, new ExecutedRoutedEventHandler(CommandManager.OnExecuted));
            }
        }

        internal static void OnElementClickEvent(object sender, RoutedEventArgs e)
        {
            TranslateInput(sender as UIElement, new TestMouseEventArgs(MouseButton.Left, System.Windows.Input.Keyboard.Modifiers));
        }

        internal static void OnElementEvent(object sender, RoutedEventArgs e)
        {
            TranslateInput(sender as UIElement, e);
        }

        internal static void OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (((sender != null) && (e != null)) && (e.Command != null))
            {
                FindCommandBinding(sender, e, e.Command, true);
            }
        }

        private static void OnInputBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            UIElement element = d as UIElement;
            ButtonBase buttonBase = d as ButtonBase;
            if (args.OldValue != null)
            {
                Mouse.RemoveMouseWheelHandler(element, new EventHandler<Telerik.Windows.Input.MouseWheelEventArgs>(CommandManager.OnElementEvent));
                Mouse.RemoveMouseDownHandler(element, new EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>(CommandManager.OnElementEvent));
                Mouse.RemoveMouseUpHandler(element, new EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>(CommandManager.OnElementEvent));
                element.KeyDown -= new KeyEventHandler(CommandManager.OnElementEvent);
                if (buttonBase != null)
                {
                    buttonBase.Click -= new RoutedEventHandler(CommandManager.OnElementClickEvent);
                }
                RemoveCanExecuteHandler(element, new CanExecuteRoutedEventHandler(CommandManager.OnCanExecute));
                RemoveExecutedHandler(element, new ExecutedRoutedEventHandler(CommandManager.OnExecuted));
            }
            if (args.NewValue != null)
            {
                element.MouseWheel += new MouseWheelEventHandler(CommandManager.OnElementEvent);
                element.KeyDown += new KeyEventHandler(CommandManager.OnElementEvent);
                if (buttonBase != null)
                {
                    buttonBase.Click += new RoutedEventHandler(CommandManager.OnElementClickEvent);
                }
                else
                {
                    Mouse.AddMouseDownHandler(element, new EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>(CommandManager.OnElementEvent));
                    Mouse.AddMouseUpHandler(element, new EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>(CommandManager.OnElementEvent));
                    Mouse.AddMouseWheelHandler(element, new EventHandler<Telerik.Windows.Input.MouseWheelEventArgs>(CommandManager.OnElementEvent));
                }
                AddCanExecuteHandler(element, new CanExecuteRoutedEventHandler(CommandManager.OnCanExecute));
                AddExecutedHandler(element, new ExecutedRoutedEventHandler(CommandManager.OnExecuted));
            }
        }

        private void RaiseRequerySuggested()
        {
            if (this.requerySuggestedOperation == null)
            {
                Dispatcher currentDispatcher = base.Dispatcher;
                if (currentDispatcher != null)
                {
                    this.requerySuggestedOperation = currentDispatcher.BeginInvoke(delegate {
                        this.RaiseRequerySuggested(null);
                    });
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obj")]
        private object RaiseRequerySuggested(object obj)
        {
            this.requerySuggestedOperation = null;
            CallWeakReferenceHandlers(this.requerySuggestedHandlers);
            return null;
        }

        public static void RegisterClassCommandBinding(Type type, CommandBinding commandBinding)
        {
            type.TestNotNull("type");
            commandBinding.TestNotNull("commandBinding");
            lock (((ICollection) classCommandBindings).SyncRoot)
            {
                CommandBindingCollection bindings = null;
                if (classCommandBindings.ContainsKey(type))
                {
                    bindings = classCommandBindings[type];
                }
                if (bindings == null)
                {
                    bindings = new CommandBindingCollection();
                    classCommandBindings[type] = bindings;
                }
                bindings.Add(commandBinding);
            }
        }

        public static void RegisterClassInputBinding(Type type, InputBinding inputBinding)
        {
            type.TestNotNull("type");
            inputBinding.TestNotNull("inputBinding");
            lock (((ICollection) classInputBindings).SyncRoot)
            {
                InputBindingCollection bindings = null;
                if (classCommandBindings.ContainsKey(type))
                {
                    bindings = classInputBindings[type];
                }
                if (bindings == null)
                {
                    bindings = new InputBindingCollection();
                    classInputBindings[type] = bindings;
                }
                bindings.Add(inputBinding);
            }
        }

        public static void RemoveCanExecuteHandler(DependencyObject element, CanExecuteRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.RemoveHandler(CanExecuteEvent, handler);
        }

        public static void RemoveExecutedHandler(DependencyObject element, ExecutedRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.RemoveHandler(ExecutedEvent, handler);
        }

        public static void RemovePreviewCanExecuteHandler(DependencyObject element, CanExecuteRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.RemoveHandler(PreviewCanExecuteEvent, handler);
        }

        public static void RemovePreviewExecutedHandler(DependencyObject element, ExecutedRoutedEventHandler handler)
        {
            element.TestNotNull("element");
            handler.TestNotNull("handler");
            element.RemoveHandler(PreviewExecutedEvent, handler);
        }

        internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler target = reference.Target as EventHandler;
                    if ((target == null) || (target == handler))
                    {
                        handlers.RemoveAt(i);
                    }
                }
            }
        }

        public static void SetCommandBindings(DependencyObject element, CommandBindingCollection value)
        {
            element.SetValue(CommandBindingsProperty, value);
        }

        public static void SetInputBindings(DependencyObject element, InputBindingCollection value)
        {
            element.SetValue(InputBindingsProperty, value);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static void TranslateInput(UIElement targetElement, RoutedEventArgs args)
        {
            if ((targetElement == null) || (args == null))
            {
                return;
            }
            ICommand command = null;
            UIElement target = null;
            object parameter = null;
            InputBindingCollection inputBindings = GetInputBindings(targetElement);
            if (inputBindings != null)
            {
                InputBinding inputBinding = inputBindings.FindMatch(targetElement, args);
                if (inputBinding != null)
                {
                    command = inputBinding.Command;
                    target = inputBinding.CommandTarget;
                    parameter = inputBinding.CommandParameter;
                }
            }
            if (command == null)
            {
                lock (((ICollection) classInputBindings).SyncRoot)
                {
                    for (Type type = targetElement.GetType(); type != null; type = type.BaseType)
                    {
                        if (classInputBindings.ContainsKey(type))
                        {
                            InputBindingCollection bindings4 = classInputBindings[type];
                            if (bindings4 != null)
                            {
                                InputBinding binding = bindings4.FindMatch(targetElement, args);
                                if (binding != null)
                                {
                                    command = binding.Command;
                                    target = binding.CommandTarget;
                                    parameter = binding.CommandParameter;
                                    goto Label_00C8;
                                }
                            }
                        }
                    }
                }
            }
        Label_00C8:
            if (command == null)
            {
                CommandBindingCollection commandBindings = GetCommandBindings(targetElement);
                if (commandBindings != null)
                {
                    command = commandBindings.FindMatch(targetElement, args);
                }
            }
            if (command == null)
            {
                lock (((ICollection) classCommandBindings).SyncRoot)
                {
                    for (Type type = targetElement.GetType(); type != null; type = type.BaseType)
                    {
                        if (classCommandBindings.ContainsKey(type))
                        {
                            CommandBindingCollection bindings3 = classCommandBindings[type];
                            if (bindings3 != null)
                            {
                                command = bindings3.FindMatch(targetElement, args);
                                if (command != null)
                                {
                                    goto Label_014D;
                                }
                            }
                        }
                    }
                }
            }
        Label_014D:
            if (command != null)
            {
                if (target == null)
                {
                    target = targetElement;
                }
                bool continueRouting = false;
                RoutedCommand routedCommand = command as RoutedCommand;
                if (routedCommand != null)
                {
                    if (routedCommand.CanExecute(parameter, target))
                    {
                        ExecuteCommand(routedCommand, parameter, target, args);
                    }
                }
                else if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
                System.Windows.Input.MouseButtonEventArgs mouseEventArgs = args as System.Windows.Input.MouseButtonEventArgs;
                if (mouseEventArgs != null)
                {
                    mouseEventArgs.Handled = !continueRouting;
                    return;
                }
                KeyEventArgs keyEventArgs = args as KeyEventArgs;
                if (keyEventArgs != null)
                {
                    keyEventArgs.Handled = !continueRouting;
                    return;
                }
                Telerik.Windows.Input.MouseButtonEventArgs telerikMouseArgs = args as Telerik.Windows.Input.MouseButtonEventArgs;
                if (telerikMouseArgs != null)
                {
                    telerikMouseArgs.Handled = !continueRouting;
                }
            }
        }

        private static CommandManager Current
        {
            get
            {
                if (commandManager == null)
                {
                    commandManager = new CommandManager();
                }
                return commandManager;
            }
        }
    }
}

