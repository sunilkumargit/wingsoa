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
using Wing.Client.Sdk;
using System.Collections.Generic;
using Flex.BusinessIntelligence.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Flex.BusinessIntelligence.Client.Interop
{
    public class BIQueriesListPresentationModel : ViewPresentationModel
    {
        private ObservableCollection<CubeQueryInfo> _queries;
        public BIQueriesListPresentationModel()
            : base("Consultas", "Selecione uma consulta da lista ou cria uma nova consultando um cubo diretamente") { }

        public ObservableCollection<CubeQueryInfo> Queries
        {
            get { return _queries; }
            set
            {
                _queries = value;
                if (_queries != null)
                    RegisterObservableCollectionProperty(_queries, "Queries");
            }
        }
    }
}
