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
