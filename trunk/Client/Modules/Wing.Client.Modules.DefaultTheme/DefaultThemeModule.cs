using Wing.Client.Core;
using Wing.Modularity;
using Wing.ServiceLocation;

namespace Wing.Client.Modules.DefaultTheme
{
    [Module("Theme")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Low)]
    [ModuleDescription("Tema padrão da interface do usuário")]
    public class DefaultThemeModule : ModuleBase
    {
        public override void Initialize()
        {
            var visualManager = ServiceLocator.Current.GetInstance<IRootVisualManager>();
            visualManager.AddResourceDictionary("Wing.Client.Modules.DefaultTheme", "WingTheme");
        }
    }
}
