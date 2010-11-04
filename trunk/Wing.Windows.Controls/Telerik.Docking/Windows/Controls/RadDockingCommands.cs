namespace Telerik.Windows.Controls
{
    using System;

    public static class RadDockingCommands
    {
        private static readonly int CommandsCount = CountCommands();
        private static readonly RoutedUICommand[] InternalCommands = new RoutedUICommand[CommandsCount];

        private static int CountCommands()
        {
            return typeof(CommandId).GetFields().Length;
        }

        private static RoutedUICommand EnsureCommand(CommandId commandId)
        {
            if ((commandId < CommandId.Close) || (((int) commandId) >= CommandsCount))
            {
                return null;
            }
            lock (InternalCommands.SyncRoot)
            {
                if (InternalCommands[(int) commandId] == null)
                {
                    RoutedUICommand newCommand = new RoutedUICommand(GetUIText(commandId), commandId.ToString(), typeof(RadDockingCommands));
                    InternalCommands[(int) commandId] = newCommand;
                }
            }
            return InternalCommands[(int) commandId];
        }

        internal static void EnsureCommandsClassLoaded()
        {
            WindowCommands.EnsureCommandsClassLoaded();
            RoutedUICommand close = Close;
            RoutedUICommand contextMenuOpen = ContextMenuOpen;
            RoutedUICommand dockable = Dockable;
            RoutedUICommand floating = Floating;
            RoutedUICommand paneHeaderMenuOpen = PaneHeaderMenuOpen;
            RoutedUICommand pin = Pin;
            RoutedUICommand tabbedDocument = TabbedDocument;
        }

        private static string GetUIText(CommandId commandId)
        {
            string text = string.Empty;
            switch (commandId)
            {
                case CommandId.Close:
                    return LocalizationManager.GetString("Hide");

                case CommandId.Pin:
                    return LocalizationManager.GetString("Auto_hide");

                case CommandId.Floating:
                    return LocalizationManager.GetString("Floating");

                case CommandId.Dockable:
                    return LocalizationManager.GetString("Dockable");

                case CommandId.TabbedDocument:
                    return LocalizationManager.GetString("Tabbed_document");

                case CommandId.ContextMenuOpen:
                    return text;
            }
            return text;
        }

        public static RoutedUICommand Close
        {
            get
            {
                return EnsureCommand(CommandId.Close);
            }
        }

        public static RoutedUICommand ContextMenuOpen
        {
            get
            {
                return EnsureCommand(CommandId.ContextMenuOpen);
            }
        }

        public static RoutedUICommand Dockable
        {
            get
            {
                return EnsureCommand(CommandId.Dockable);
            }
        }

        public static RoutedUICommand Floating
        {
            get
            {
                return EnsureCommand(CommandId.Floating);
            }
        }

        public static RoutedUICommand PaneHeaderMenuOpen
        {
            get
            {
                return EnsureCommand(CommandId.PaneHeaderMenuOpen);
            }
        }

        public static RoutedUICommand Pin
        {
            get
            {
                return EnsureCommand(CommandId.Pin);
            }
        }

        public static RoutedUICommand TabbedDocument
        {
            get
            {
                return EnsureCommand(CommandId.TabbedDocument);
            }
        }

        private enum CommandId : byte
        {
            Close = 0,
            ContextMenuOpen = 5,
            Dockable = 3,
            Floating = 2,
            PaneHeaderMenuOpen = 6,
            Pin = 1,
            TabbedDocument = 4
        }
    }
}

