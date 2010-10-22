using Flex.BusinessIntelligence.Data;
using Wing.Client.Sdk;
using Wing.Utils;

namespace Flex.BusinessIntelligence.WingClient.Views.PivotGrid
{
    public class PivotGridPresentationModel : ViewPresentationModel
    {
        private CubeRegistrationInfo _cubeInfo;

        public PivotGridPresentationModel(CubeRegistrationInfo cubeInfo)
        {
            Assert.NullArgument(cubeInfo, "cubeInfo");
            _cubeInfo = cubeInfo;
        }

        public override string Caption
        {
            get
            {
                return _cubeInfo.Description;
            }
            set { }
        }

        public CubeRegistrationInfo CubeInfo { get { return _cubeInfo; } }
    }
}
