using NoteBook.MegEvents;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            _eventAggregator.GetEvent<MesEvent>().Subscribe(Sub);
        }

        private void Sub(string obj)
        {
            ReLoginBar.MessageQueue.Enqueue(obj);
        }
    }
}
