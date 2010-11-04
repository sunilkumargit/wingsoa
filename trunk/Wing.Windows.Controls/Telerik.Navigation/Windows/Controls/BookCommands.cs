namespace Telerik.Windows.Controls
{
    using System;

    public static class BookCommands
    {
        private static RoutedUICommand[] internalCommands = new RoutedUICommand[4];

        private static RoutedUICommand EnsureCommand(CommandId commandId)
        {
            if ((commandId < CommandId.FirstPage) || (commandId > CommandId.LastPage))
            {
                return null;
            }
            if (internalCommands[(int) commandId] == null)
            {
                internalCommands[(int) commandId] = new RoutedUICommand(GetPropertyName(commandId), GetPropertyName(commandId), typeof(BookCommands));
            }
            return internalCommands[(int) commandId];
        }

        internal static void EnsureCommands()
        {
            for (byte i = 0; i <= 3; i = (byte) (i + 1))
            {
                EnsureCommand((CommandId) i);
            }
        }

        private static string GetPropertyName(CommandId commandId)
        {
            return commandId.ToString();
        }

        public static RoutedUICommand FirstPage
        {
            get
            {
                return EnsureCommand(CommandId.FirstPage);
            }
        }

        public static RoutedUICommand LastPage
        {
            get
            {
                return EnsureCommand(CommandId.LastPage);
            }
        }

        public static RoutedUICommand NextPage
        {
            get
            {
                return EnsureCommand(CommandId.NextPage);
            }
        }

        public static RoutedUICommand PreviousPage
        {
            get
            {
                return EnsureCommand(CommandId.PreviousPage);
            }
        }

        private enum CommandId : byte
        {
            FirstPage = 0,
            LastPage = 3,
            NextPage = 2,
            PreviousPage = 1
        }
    }
}

