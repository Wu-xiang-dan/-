using Settings.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Settings.ViewModels;

namespace Settings
{
    [Module(ModuleName = "Settings")]
    public class SettingsModule: IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsUcView,SettingsUCViewModel>();
            containerRegistry.RegisterForNavigation<AboutUCView>();
            containerRegistry.RegisterForNavigation<PersonalUcView, PersonalUcViewModel>();
            containerRegistry.RegisterForNavigation<SystemSetUcView>();
        }
    }
}