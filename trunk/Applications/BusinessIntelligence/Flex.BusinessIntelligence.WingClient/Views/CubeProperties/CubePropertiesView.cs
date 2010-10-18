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
using Wing.Client.Sdk.Controls;
using Wing.Client.Sdk;
using Flex.BusinessIntelligence.Data;

namespace Flex.BusinessIntelligence.WingClient.Views.CubeProperties
{
    public class CubePropertiesView : HeaderedPage
    {
        public CubePropertiesView()
        {

        }
    }

    public class CubePropertiesPresentationModel : ViewPresentationModel
    {
        private CubeRegistrationInfo _cubeInfo;

        public CubeRegistrationInfo CubeInfo
        {
            get { return _cubeInfo; }
            set { _cubeInfo = value; NotifyPropertyChanged("CubeInfo"); }
        }
    }

    public class CubePropertiesPresenter : PopupWindowPresenter<CubePropertiesPresentationModel>
    {
        public CubePropertiesPresenter()
            : base(new CubePropertiesPresentationModel() { Caption = "Propriedades do cubo" }, new CubePropertiesView(), null)
        {
        }
    }
}
