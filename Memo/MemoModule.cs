using Memo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Memo
{
    [Module(ModuleName = "Memo")]
    public class MemoModule: IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MemoUcView>();
        }
    }
}