using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core.Providers.ClientServer;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core;
using System.Collections.Generic;
using System.Text;

namespace Ranet.AgOlap.Controls
{
    public class TransactionStateButton : AgControlBase
    {
        public String Text
        {
            get { return m_btnState.Content != null ? m_btnState.Content.ToString() : String.Empty; }
            set { m_btnState.Content = value; }
        }

        private String m_Connection = String.Empty;
        public String Connection
        {
            get { return m_Connection; }
            set
            {
                m_Connection = value;
                this.IsEnabled = OlapTransactionManager.HasPendingChanges(value);
            }
        }

        Button m_btnState;

        public TransactionStateButton()
        {
            Grid grdRoot = new Grid();
            m_btnState = new Button();
            m_btnState.Click += new RoutedEventHandler(m_btnState_Click);
            grdRoot.Children.Add(m_btnState);
            this.IsEnabled = false;

            this.Content = grdRoot;

            OlapDataLoader = GetOlapDataLoader();
            OlapTransactionManager.AfterCommandComplete += new EventHandler<TransactionCommandResultEventArgs>(AnalysisTransactionManager_AfterCommandComplete);
            OlapTransactionManager.PendingChangesModified += new EventHandler<TransactionCommandResultEventArgs>(AnalysisTransactionManager_AfterCommandComplete);
        }

        ~TransactionStateButton()
        {
            OlapTransactionManager.AfterCommandComplete -= new EventHandler<TransactionCommandResultEventArgs>(AnalysisTransactionManager_AfterCommandComplete);
            OlapTransactionManager.PendingChangesModified -= new EventHandler<TransactionCommandResultEventArgs>(AnalysisTransactionManager_AfterCommandComplete);
        }

        void AnalysisTransactionManager_AfterCommandComplete(object sender, TransactionCommandResultEventArgs e)
        {
            if (e.Connection == this.Connection)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    this.IsEnabled = OlapTransactionManager.HasPendingChanges(this.Connection);// !e.Succeess;
                }
                else
                {
                    this.Dispatcher.BeginInvoke(() => this.IsEnabled = OlapTransactionManager.HasPendingChanges(this.Connection));
                }
            }            
        }

        IDataLoader m_OlapDataLoader = null;
        public IDataLoader OlapDataLoader
        {
            set
            {
                if (m_OlapDataLoader != null)
                {
                    m_OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                }

                m_OlapDataLoader = value;
                if (m_OlapDataLoader != null)
                {
                    m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                }
            }
            get
            {
                return m_OlapDataLoader;
            }
        }

        void OlapDataLoader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            // Exception
            if (e.Error != null)
            {
                LogManager.LogError(this, e.Error.ToString());
                return;
            }

            // Exception or Message from Olap-Service
            if (e.Result.ContentType == InvokeContentType.Error)
            {
                LogManager.LogError(this, e.Result.Content);
                return;
            }

            if (e.Result != null)
            {
                String connectionId = String.Empty;
                if (e.Result.Headers.Contains(InvokeResultDescriptor.CONNECTION_ID))
                {
                    Header connection_header = e.Result.Headers[InvokeResultDescriptor.CONNECTION_ID];
                    if (connection_header != null)
                    {
                        connectionId = connection_header.Value;
                    }
                }

                if (connectionId == Connection)
                {
                    // Commit or Rollback Transaction
                    MdxQueryArgs query_args = e.UserState as MdxQueryArgs;
                    if (query_args != null)
                    {
                        if (query_args.Type == QueryTypes.CommitTransaction ||
                            query_args.Type == QueryTypes.RollbackTransaction)
                        {
                            if (e.Result.ContentType == InvokeContentType.UpdateResult)
                            {
                                List<String> results = XmlSerializationUtility.XmlStr2Obj<List<String>>(e.Result.Content);
                                if (results != null)
                                {
                                    var errors = results.Where(res => !String.IsNullOrEmpty(res));
                                    StringBuilder sb = new StringBuilder();
                                    //if (errors.Count() == 0)
                                    //    AnalysisTransactionManager.CloseTransaction(Connection);
                                    //else
                                    //{
                                        foreach (var error in errors)
                                        {
                                            sb.AppendLine(error);
                                        }
                                        if (!String.IsNullOrEmpty(sb.ToString()))
                                        {
                                            LogManager.LogError(this, sb.ToString());
                                        }
                                    //}

                                    // В случае ошибки считаем что транзакция закрыта. И кэш чистим.
                                    OlapTransactionManager.CloseTransaction(Connection);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual IDataLoader GetOlapDataLoader()
        {
            return new OlapDataLoader(URL);
        }

        void m_btnState_Click(object sender, RoutedEventArgs e)
        {
            if (Type != TransactionStateActionTypes.None)
            {
                MdxQueryArgs args = new MdxQueryArgs();
                args.Connection = Connection;

                switch (Type)
                { 
                    case TransactionStateActionTypes.Commit:
                        args.Queries.Add("COMMIT TRANSACTION");
                        args.Type = QueryTypes.CommitTransaction;
                        LogManager.LogInformation(this, this.Name + " - COMMIT TRANSACTION");
                        break;
                    case TransactionStateActionTypes.Rollback:
                        args.Queries.Add("ROLLBACK TRANSACTION");
                        args.Type = QueryTypes.RollbackTransaction;
                        LogManager.LogInformation(this, this.Name + " - ROLLBACK TRANSACTION");
                        break;
                }

                OlapDataLoader.LoadData(args, args);
            }
        }

        public TransactionStateActionTypes Type = TransactionStateActionTypes.None;
    }

    public enum TransactionStateActionTypes
    {
        Commit,
        Rollback,
        None
    }
}
