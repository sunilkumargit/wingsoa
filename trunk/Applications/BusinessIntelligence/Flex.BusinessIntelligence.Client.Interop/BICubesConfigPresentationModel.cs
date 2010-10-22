using System.Collections.ObjectModel;
using Flex.BusinessIntelligence.Data;
using Wing.Client.Sdk;

namespace Flex.BusinessIntelligence.Client.Interop
{
    public class BICubesConfigPresentationModel : ViewPresentationModel
    {
        private ObservableCollection<CubeRegistrationInfo> _cubes;

        public BICubesConfigPresentationModel()
            : base("Cubos", "Cubos registrados no Business Intelligence")
        {
        }


        public ObservableCollection<CubeRegistrationInfo> Cubes
        {
            get { return _cubes; }
            set
            {
                _cubes = value;
                if (_cubes != null)
                    RegisterObservableCollectionProperty(_cubes, "Cubes");
            }
        }
    }
}