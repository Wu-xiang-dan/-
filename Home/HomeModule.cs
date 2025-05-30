using Home.ViewModels.Dialogs;
using Home.Views;
using Home.Views.Dialogs;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using System.Windows;
namespace Home
{
    [Module(ModuleName = "Home")]
    public class HomeModule: IModule
    {
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogService _dialogService;
        public HomeModule(IContainerProvider containerProvider, IDialogService dialogService)
        {  
            _containerProvider = containerProvider;
            _dialogService = dialogService;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {         
        }
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomeUcView>();
            containerRegistry.RegisterDialog<AddWaitUcView, AddWaitUcViewModel>();
            containerRegistry.RegisterDialog<EditWaitUcView, EditWaitUcViewModel>();
            containerRegistry.RegisterDialog<EditMemoUcView, EditMemoUcViewModel>();
            containerRegistry.RegisterDialog<AddMemoUcView, AddMemoUcViewModel>();
        }
        
    }
}