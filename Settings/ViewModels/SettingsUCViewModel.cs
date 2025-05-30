using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.ViewModels
{
    class SettingsUCViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        public DelegateCommand<LeftMenuInfo> NavigateCommand { get; set; }
        private List<LeftMenuInfo> _leftMenuInfos = new List<LeftMenuInfo>();

        public List<LeftMenuInfo> LeftMenuInfos
        {
            get { return _leftMenuInfos; }
            set
            {
                _leftMenuInfos = value;
                RaisePropertyChanged(nameof(LeftMenuInfos));
            }
        }

        public SettingsUCViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<LeftMenuInfo>(Navigate);
            InitSettin();
        }
        public void InitSettin()
        {
            LeftMenuInfos.Add(new LeftMenuInfo() { Icon = "Palette", MenuName = "个性化", ViewName = "PersonalUc" });
            LeftMenuInfos.Add(new LeftMenuInfo() { Icon = "InformationVariant", MenuName = "关于我们", ViewName = "AboutUC" });
            LeftMenuInfos.Add(new LeftMenuInfo() { Icon = "Cog", MenuName = "系统设置", ViewName = "SystemSetUc" });
        }
        private void Navigate(LeftMenuInfo menu)
        {
            if (menu == null || string.IsNullOrEmpty(menu.ViewName)) return;
            _regionManager.RequestNavigate("SetContentRegion", $"{menu.ViewName}View");
        }
    }
}
