namespace Telerik.Windows.Controls
{
    using System;

    public static class WindowCommands
    {
        private static readonly int CommandsCount = CountCommands();
        private static readonly RoutedUICommand[] InternalCommands = new RoutedUICommand[CommandsCount];

        private static int CountCommands()
        {
            return typeof(CommandId).GetFields().Length;
        }

        private static RoutedUICommand EnsureCommand(CommandId commandId)
        {
            if ((commandId < CommandId.Maximize) || (((int) commandId) >= CommandsCount))
            {
                return null;
            }
            lock (InternalCommands.SyncRoot)
            {
                if (InternalCommands[(int) commandId] == null)
                {
                    RoutedUICommand newCommand = new RoutedUICommand(GetUIText(commandId), commandId.ToString(), typeof(WindowCommands));
                    InternalCommands[(int) commandId] = newCommand;
                }
            }
            return InternalCommands[(int) commandId];
        }

        internal static void EnsureCommandsClassLoaded()
        {
            RoutedUICommand cancel = Cancel;
            RoutedUICommand close = Close;
            RoutedUICommand confirm = Confirm;
            RoutedUICommand maximize = Maximize;
            RoutedUICommand minimize = Minimize;
            RoutedUICommand restore = Restore;
        }

        private static string GetUIText(CommandId commandId)
        {
            string text = string.Empty;
            switch (commandId)
            {
                case CommandId.Maximize:
                    return Telerik.Windows.Controls.SR.GetString("Maximize");

                case CommandId.Minimize:
                    return Telerik.Windows.Controls.SR.GetString("Minimize");

                case CommandId.Close:
                    return Telerik.Windows.Controls.SR.GetString("Close");

                case CommandId.Confirm:
                    return Telerik.Windows.Controls.SR.GetString("Confirm");

                case CommandId.Cancel:
                    return Telerik.Windows.Controls.SR.GetString("Cancel");

                case CommandId.Restore:
                    return Telerik.Windows.Controls.SR.GetString("Restore");
            }
            return text;
        }

        public static RoutedUICommand Cancel
        {
            get
            {
                return EnsureCommand(CommandId.Cancel);
            }
        }

        public static RoutedUICommand Close
        {
            get
            {
                return EnsureCommand(CommandId.Close);
            }
        }

        public static RoutedUICommand Confirm
        {
            get
            {
                return EnsureCommand(CommandId.Confirm);
            }
        }

        public static RoutedUICommand Maximize
        {
            get
            {
                return EnsureCommand(CommandId.Maximize);
            }
        }

        public static RoutedUICommand Minimize
        {
            get
            {
                return EnsureCommand(CommandId.Minimize);
            }
        }

        public static RoutedUICommand Restore
        {
            get
            {
                return EnsureCommand(CommandId.Restore);
            }
        }

        private enum CommandId : byte
        {
            Cancel = 4,
            Close = 2,
            Confirm = 3,
            Maximize = 0,
            Minimize = 1,
            Restore = 5
        }
    }
}

