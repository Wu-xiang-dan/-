using Prism.Ioc;
using NoteBook.Views;
using System.Windows;
using NoteBook.ViewModels;
using Prism.Services.Dialogs;
using Prism.DryIoc;
using NoteBook.HttpClients;
using DryIoc;
using Prism.Regions;
using Prism.Modularity;
using NoteBook.Data;
using System;
using MaterialDesignThemes.Wpf;
using System.Linq;
namespace NoteBook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {  
            return Container.Resolve<MainWindow>(); 
        }
        //注入服务
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
            containerRegistry.GetContainer().Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.RegisterSingleton<IDataService, DataService>();
            containerRegistry.RegisterSingleton<IJsonSerializerService, JsonSerializerService>();
            containerRegistry.RegisterSingleton<IJsonSerializerService, JsonSerializerService>();
        }
        protected override void OnInitialized()
        {        
            var loginView = Container.Resolve<IDialogService>();
            loginView.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    return;
                }
                base.OnInitialized();
                MainWindowViewModel main = App.Current.MainWindow.DataContext as MainWindowViewModel;
                if (main != null)
                {
                    string accountName = "";
                    accountName = callback.Parameters.GetValue<string>("AccountName");
                    main.DefNavigate(accountName);
                }
            });
        }
        protected override void OnExit(ExitEventArgs e)
        {
            var JasonService = Container.Resolve<IJsonSerializerService>();
            var dataService = Container.Resolve<IDataService>();
            JasonService.SaveWaitsToJson(dataService.ViewWaitList.ToList());
            JasonService.SaveMemosToJson(dataService.ViewMemoList.ToList());
            base.OnExit(e);
        }
        protected override void InitializeModules()
        {
            base.InitializeModules();
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new DirectoryModuleCatalog() { ModulePath = @".\Apps" };
            return catalog;
        }
    }
}
