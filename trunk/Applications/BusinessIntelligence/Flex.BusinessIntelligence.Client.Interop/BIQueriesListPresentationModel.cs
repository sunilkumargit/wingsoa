using System.Collections.ObjectModel;
using Flex.BusinessIntelligence.Data;
using Wing.Client.Sdk;

namespace Flex.BusinessIntelligence.Client.Interop
{
    public class BIQueriesListPresentationModel : ViewPresentationModel
    {
        private ObservableCollection<CubeQueryInfo> _queries;
        private ObservableCollection<CubeRegistrationInfo> _cubes;

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

        public ObservableCollection<CubeRegistrationInfo> Cubes
        {
            get { return _cubes; }
            set
            {
                _cubes = value;
                if (_cubes != null)
                    RegisterObservableCollectionProperty(_cubes, "Queries");
            }
        }
    }
}
