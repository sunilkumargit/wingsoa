namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public abstract class PaneGroupBase : RadTabControl, IThemable
    {
        static PaneGroupBase()
        {
            EventManager.RegisterClassHandler(typeof(PaneGroupBase), RadContextMenu.ClosedEvent, new RoutedEventHandler(PaneGroupBase.OnContextMenuClose), true);
            CommandManager.RegisterClassCommandBinding(typeof(PaneGroupBase), new CommandBinding(RadDockingCommands.ContextMenuOpen, new ExecutedRoutedEventHandler(PaneGroupBase.OnContextMenuCommandInvoke)));
            CommandManager.RegisterClassCommandBinding(typeof(PaneGroupBase), new CommandBinding(RadDockingCommands.PaneHeaderMenuOpen, new ExecutedRoutedEventHandler(PaneGroupBase.OnPaneHeaderMenuOpen)));
            CommandManager.RegisterClassCommandBinding(typeof(PaneGroupBase), new CommandBinding(RadDockingCommands.Close, new ExecutedRoutedEventHandler(PaneGroupBase.OnPaneCommandInvoke), new CanExecuteRoutedEventHandler(PaneGroupBase.OnPaneCommandCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(PaneGroupBase), new CommandBinding(RadDockingCommands.Pin, new ExecutedRoutedEventHandler(PaneGroupBase.OnPaneCommandInvoke), new CanExecuteRoutedEventHandler(PaneGroupBase.OnPaneCommandCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(PaneGroupBase), new CommandBinding(RadDockingCommands.Floating, new ExecutedRoutedEventHandler(PaneGroupBase.OnPaneCommandInvoke), new CanExecuteRoutedEventHandler(PaneGroupBase.OnPaneCommandCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(PaneGroupBase), new CommandBinding(RadDockingCommands.Dockable, new ExecutedRoutedEventHandler(PaneGroupBase.OnPaneCommandInvoke), new CanExecuteRoutedEventHandler(PaneGroupBase.OnPaneCommandCanExecute)));
            CommandManager.RegisterClassCommandBinding(typeof(PaneGroupBase), new CommandBinding(RadDockingCommands.TabbedDocument, new ExecutedRoutedEventHandler(PaneGroupBase.OnPaneCommandInvoke), new CanExecuteRoutedEventHandler(PaneGroupBase.OnPaneCommandCanExecute)));
        }

        protected PaneGroupBase()
        {
        }

        private static void ExecuteCommand(RadPane pane, ICommand command, PaneGroupBase groupBase)
        {
            if (command == RadDockingCommands.Close)
            {
                pane.IsHidden = true;
            }
            else if (command == RadDockingCommands.Pin)
            {
                pane.IsPinned = !pane.IsPinned;
            }
            else if (command == RadDockingCommands.Floating)
            {
                pane.MakeFloatingOnly();
            }
            else if (command == RadDockingCommands.Dockable)
            {
                pane.MakeDockable();
            }
            else if (command == RadDockingCommands.TabbedDocument)
            {
                pane.MoveToDocumentHost();
            }
            groupBase.UpdateCheckedState(false);
        }

        private static void OnContextMenuClose(object sender, RoutedEventArgs e)
        {
            PaneGroupBase paneGroup = sender as PaneGroupBase;
            if (paneGroup != null)
            {
                paneGroup.UpdateCheckedState(false);
            }
        }

        private static void OnContextMenuCommandInvoke(object sender, ExecutedRoutedEventArgs e)
        {
            PaneGroupBase paneGroup = sender as PaneGroupBase;
            if (paneGroup != null)
            {
                paneGroup.UpdateCheckedState(true);
                ShowContextMenu(paneGroup, e);
            }
        }

        private static void OnPaneCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            PaneGroupBase tabControl = sender as PaneGroupBase;
            if (tabControl != null)
            {
                RadPane selectedPane = (e.Parameter ?? tabControl.SelectedItem) as RadPane;
                if (selectedPane != null)
                {
                    if (e.Command == RadDockingCommands.Close)
                    {
                        e.CanExecute = selectedPane.CanUserClose;
                    }
                    else if (e.Command == RadDockingCommands.Pin)
                    {
                        e.CanExecute = selectedPane.CanPin;
                    }
                    else if (e.Command == RadDockingCommands.Floating)
                    {
                        e.CanExecute = selectedPane.IsPinned && selectedPane.CanFloat;
                    }
                    else if (e.Command == RadDockingCommands.Dockable)
                    {
                        e.CanExecute = selectedPane.IsPinned && (!selectedPane.IsInDocumentHost || selectedPane.CanFloat);
                    }
                    else if (e.Command == RadDockingCommands.TabbedDocument)
                    {
                        e.CanExecute = selectedPane.IsPinned && selectedPane.CanDockInDocumentHost;
                    }
                }
            }
        }

        private static void OnPaneCommandInvoke(object sender, ExecutedRoutedEventArgs e)
        {
            RadPane selectedPane;
            PaneGroupBase tabControl = sender as PaneGroupBase;
            if (tabControl != null)
            {
                selectedPane = (e.Parameter ?? tabControl.SelectedItem) as RadPane;
                if (selectedPane != null)
                {
                    tabControl.Dispatcher.BeginInvoke(delegate {
                        ExecuteCommand(selectedPane, e.Command, tabControl);
                    });
                }
            }
        }

        private static void OnPaneHeaderMenuOpen(object sender, ExecutedRoutedEventArgs e)
        {
            PaneGroupBase paneGroup = sender as PaneGroupBase;
            if (paneGroup != null)
            {
                paneGroup.UpdateCheckedState(false);
                ShowContextMenu(paneGroup, e);
            }
        }

        public void ResetTheme()
        {
            foreach (RadPane pane in base.Items.OfType<RadPane>())
            {
                pane.CopyValue(this, StyleManager.ThemeProperty);
            }
            if (this.PaneHeader != null)
            {
                this.PaneHeader.CopyValue(this, StyleManager.ThemeProperty);
            }
        }

        internal static void ShowContextMenu(PaneGroupBase paneGroup, ExecutedRoutedEventArgs e)
        {
            if (paneGroup != null)
            {
                RadPane selectedPane = (e.Parameter ?? paneGroup.SelectedItem) as RadPane;
                paneGroup.Focus();
                RadContextMenu contextMenu = RadContextMenu.GetContextMenu(selectedPane);
                if (contextMenu != null)
                {
                    FrameworkElement element = e.OriginalSource as FrameworkElement;
                    contextMenu.PlacementTarget = selectedPane;
                    contextMenu.SetPlacementTarget(element);
                    contextMenu.IsOpen = true;
                }
            }
        }

        protected void UpdateCheckedState(bool isChecked)
        {
            if (this.PaneHeader != null)
            {
                this.PaneHeader.UpdateCheckedState(isChecked);
            }
        }

        protected Telerik.Windows.Controls.Docking.PaneHeader PaneHeader { get; set; }
    }
}

