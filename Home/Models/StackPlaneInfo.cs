using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Navigation;

namespace Home.Models
{
    /// <summary>
    /// 统计面板信息
    /// </summary>
    public class StackPlaneInfo:BindableBase
    {
        public string Icon { get; set; }
        public string ItemName { get; set; }
        private string _result; // 私有字段存储实际值

        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value);
                RaisePropertyChanged(nameof(Result));
            }
        }

        public string BackGroundColor{ get; set; }
        public string TextView{ get; set; }
    }
}
