using Flex.BusinessIntelligence.Data;
using Wing.Client.Sdk;
using Wing.Utils;

namespace Flex.BusinessIntelligence.WingClient.Views.PivotGrid
{
    public class PivotGridPresentationModel : ViewPresentationModel
    {
        private CubeRegistrationInfo _cubeInfo;
        private CubeQueryInfo _queryInfo;

        public PivotGridPresentationModel(CubeRegistrationInfo cubeInfo, CubeQueryInfo queryInfo)
        {
            Assert.NullArgument(cubeInfo, "cubeInfo");
            _cubeInfo = cubeInfo;
            _queryInfo = queryInfo;
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
        public CubeQueryInfo QueryInfo { get { return _queryInfo; } }
    }
}
