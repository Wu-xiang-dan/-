using NoteBook.MegEvents;
using Prism.Events;
using System.Windows.Controls;
namespace NoteBook.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;
        public LoginView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<LoginMegEvents>().Subscribe(Sub);
        }
        private void Sub(string obj)
        {
            ReLoginBar.MessageQueue.Enqueue(obj);
        }
    }
}
