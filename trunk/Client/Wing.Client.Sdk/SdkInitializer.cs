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
using Wing.ServiceLocation;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk
{
    public static class SdkInitializer
    {
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.Current;

            //registrar os serviços default
            serviceLocator.Register<IGlobalCommandsManager, DefaultGlobalCommandsManager>(true);
            CommandsManager.SetCommandsManager(serviceLocator.GetInstance<IGlobalCommandsManager>());
        }
    }
}
