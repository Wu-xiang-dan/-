using Wait.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Wait
{
    [Module(ModuleName = "Wait")]
    public class WaitModule: IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var regionManager = containerProvider.Resolve<IRegionManager>();
            ////通过ContentRegion管理导航默认初始页面ContactView
            //var contentRegion = regionManager.Regions["ContentRegion"];
            //contentRegion.RequestNavigate(nameof(WaitUcView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WaitUcView>();
        }
    }
}