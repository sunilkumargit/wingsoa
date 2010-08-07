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
using Wing.Modularity;
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.WingClient.Views.Root;

namespace Flex.BusinessIntelligence.WingClient
{
    [Module("FlexBI")]
    [ModuleDescription("Flex Business Intelligence - UI")]
    [ModuleCategory(ModuleCategory.Common)]
    [ModulePriority(ModulePriority.High)]
    public class FlexBusinessIntelligenceModule : ModuleBase
    {
        public override void Initialize()
        {
            ServiceLocator.Current.Register<IBIRootPresenter, BIRootPresenter>();
        }

        public override void Initialized()
        {
            // ativar o presenter;
            var presenter = ServiceLocator.Current.GetInstance<IBIRootPresenter>();
        }
    }

}
