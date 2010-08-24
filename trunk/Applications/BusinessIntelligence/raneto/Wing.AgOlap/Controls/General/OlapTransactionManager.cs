using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Ranet.AgOlap.Providers;
using Ranet.AgOlap.Controls.PivotGrid;

namespace Ranet.AgOlap.Controls.General
{
    public static class OlapTransactionManager
    {
        public static event EventHandler<TransactionCommandResultEventArgs> AfterCommandComplete;
        public static event EventHandler<TransactionCommandResultEventArgs> PendingChangesModified;
        private static Dictionary<string, CellChangesCache> PendingChanges = new Dictionary<string, CellChangesCache>();

        public static CellChangesCache GetPendingChanges(string connectionName)
        {
            lock (PendingChanges)
            {
                if (PendingChanges.ContainsKey(connectionName))
                {
                    return PendingChanges[connectionName];
                }
                else
                {
                    CellChangesCache cache = new CellChangesCache();
                    PendingChanges.Add(connectionName, cache);
                    return cache;
                }
            }
        }

        //public static void SetPendingChanges(string connectionName, CellChangesCache cache)
        //{
        //    lock (PendingChanges)
        //    {
        //        if (PendingChanges.ContainsKey(connectionName))
        //        {
        //            PendingChanges[connectionName] = cache;
        //        }
        //        else
        //        {
        //            PendingChanges.Add(connectionName, cache);
        //        }
        //        OnPendingChangesModified(connectionName);
        //    }
        //}

        public static void AddPendingChanges(string connectionName, List<UpdateEntry> changes)
        {
            if (changes != null)
            {
                lock (PendingChanges)
                {
                    CellChangesCache cache = GetPendingChanges(connectionName);
                    foreach (var entry in changes)
                    {
                        cache.Add(entry);
                    }
                    OnPendingChangesModified(connectionName);
                }
            }
        }

        //public static void RemovePendingChanges(string connectionName, List<UpdateEntry> changes)
        //{
        //    if (changes != null)
        //    {
        //        lock (PendingChanges)
        //        {
        //            CellChangesCache cache = GetPendingChanges(connectionName);
        //            foreach (var entry in changes)
        //            {
        //                cache.RemoveChange(entry);
        //            }
        //            OnPendingChangesModified(connectionName);
        //        }
        //    }
        //}

        public static bool HasPendingChanges(string connectionName)
        {
            lock (PendingChanges)
            {
                if (PendingChanges.ContainsKey(connectionName))
                {
                    if (PendingChanges[connectionName] != null)
                    {
                        return PendingChanges[connectionName].Count > 0;
                    }
                }
                return false;
            }
        }

        public static void ClearPendingChanges(string connectionName)
        {
            lock (PendingChanges)
            {
                if (PendingChanges.ContainsKey(connectionName))
                {
                    PendingChanges[connectionName].Clear();
                    OnPendingChangesModified(connectionName);
                }
            }
        }

        private static void OnAfterCommandComplete(string connectionName)
        {
            var handler = AfterCommandComplete;
            if (handler != null)
            {
                handler(null, new TransactionCommandResultEventArgs(connectionName));
            }
        }

        private static void OnPendingChangesModified(string connectionName)
        {
            var handler = PendingChangesModified;
            if (handler != null)
            {
                handler(null, new TransactionCommandResultEventArgs(connectionName));
            }
        }

        public static void CloseTransaction(string connectionName)
        {
            ClearPendingChanges(connectionName);
            OnAfterCommandComplete(connectionName);
        }

    }

    public class TransactionCommandResultEventArgs : EventArgs
    {
        public TransactionCommandResultEventArgs(string connection)
        {
            this.Connection = connection;
        }

        public readonly string Connection;
    }
}
