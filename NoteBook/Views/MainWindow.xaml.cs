using NoteBook.Data;
using NoteBook.HttpClients;
using NoteBook.MegEvents;
using Prism.DryIoc;
using Prism.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NoteBook.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEventAggregator _eventAggregator;
        private IJsonSerializerService _serializerService;
        private IDataService _dataService;
        public MainWindow(IEventAggregator eventAggregator, IJsonSerializerService jsonSerializerService,IDataService dataService)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _serializerService = jsonSerializerService;
           _dataService = dataService;
            _eventAggregator.GetEvent<MesEvent>().Subscribe(ShowMessage);
        }
        private void ShowMessage(string obj)
        {
            this.MessageBar.MessageQueue.Enqueue(obj);
        }

        private void btnmin_Click(object sender, RoutedEventArgs e)
        {
            WindowState= WindowState.Minimized;
        }

        private void btnmax_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private async void btnclose_Click(object sender, RoutedEventArgs e)
        {          
            _serializerService.SaveWaitsToJson(_dataService.ViewWaitList.ToList());
            _serializerService.SaveMemosToJson(_dataService.ViewMemoList.ToList());
            try
            {
                var res = await Task.WhenAll(_dataService.UploadMemosAsync(),_dataService.UploadWaitsAsync());
                var failedResponses = res.SelectMany(t => t)
                           .Where(item => item.ResultCode != Result.Success)
                           .ToList();
                if (failedResponses.Count == 0)
                {
                    _eventAggregator.GetEvent<MesEvent>().Publish("保存成功");
                    await Task.Delay(2000);
                    Application.Current.Shutdown();
                    return;
                }
                foreach (var item in failedResponses)
                {              
                    _eventAggregator.GetEvent<MesEvent>().Publish(item.Message);
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish($"异常:{ex.Data}");
            }
            Application.Current.Shutdown();
        }

        private void ColorZone_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        /// <summary>
        /// 菜单移开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbMenu_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            drawerHost.IsLeftDrawerOpen = false;
        }

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
