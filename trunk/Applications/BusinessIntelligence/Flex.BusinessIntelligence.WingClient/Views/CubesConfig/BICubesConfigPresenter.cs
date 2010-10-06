﻿using System;
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
using Flex.BusinessIntelligence.Client.Interop;
using Wing.Client.Sdk.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.CubesConfig
{
    public class BICubesConfigPresenter : ViewPresenter<BICubesConfigPresentationModel>
    {
        public BICubesConfigPresenter(BICubesConfigPresentationModel presentationModel, BICubesConfigView view)
            : base(presentationModel, view, null)
        {
        }
    }
}