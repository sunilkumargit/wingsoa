using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder2;
using Wing.ServiceLocation;
using Wing.Logging;

namespace Wing.UnityServiceLocator
{
    public class UnityContainerLoggerExtension : UnityContainerExtension
    {
        private ILogger _logger;

        public UnityContainerLoggerExtension(ILogger logger)
        {
            _logger = logger;
        }

        protected override void Initialize()
        {
            Context.Registering += new EventHandler<RegisterEventArgs>(Context_Registering);
            Context.RegisteringInstance += new EventHandler<RegisterInstanceEventArgs>(Context_RegisteringInstance);
            var strategy = new ContainerPreCreationLoggerStrategy(_logger);
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }

        void Context_RegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            _logger.Log(String.Format("REG INSTANCE {0}", e.Instance.GetType().Name), Category.Debug, Priority.None);
        }

        void Context_Registering(object sender, RegisterEventArgs e)
        {
            _logger.Log(String.Format("REG TYPE {0}", (e.TypeTo ?? e.TypeFrom).Name), Category.Debug, Priority.None);
        }
    }

    class ContainerPreCreationLoggerStrategy : BuilderStrategy
    {
        private ILogger _logger;

        public ContainerPreCreationLoggerStrategy(ILogger logger)
        {
            _logger = logger;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            base.PreBuildUp(context);
            //novos objetos
            if (context.Existing == null)
                _logger.Log(String.Format("CREATING {0}", context.BuildKey.ToString()), Category.Debug, Priority.None);
        }
    }
}
