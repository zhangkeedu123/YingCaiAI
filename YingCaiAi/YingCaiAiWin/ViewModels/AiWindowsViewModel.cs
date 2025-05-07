using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Wpf.Ui;
using YingCaiAiWin.Views.Pages;


namespace YingCaiAiWin.ViewModels
{
    public partial class AIWindowsViewModel : ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _currentPageTitle = "AI 智能助手";

        [ObservableProperty]
        public List<ToolItem> _toolItems = [];

        public AIWindowsViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
            {
             
                InitializeViewModel();

            }

        }
     

        public void InitializeViewModel()
        {
            ToolItems = new List<ToolItem>()
        {
            new ToolItem { Icon = "People24", Title = "客户资料" },
            new ToolItem { Icon = "Key24", Title = "正在招聘" },
            new ToolItem { Icon = "CheckmarkCircle24", Title = "推荐话术" },
            new ToolItem { Icon = "ClipboardTextLtr20", Title = "企业套餐" },
            new ToolItem { Icon = "List24", Title = "优惠政策" },
            new ToolItem { Icon = "Warning24", Title = "平台数据" },
            new ToolItem { Icon = "Document24", Title = "售后服务" },
            new ToolItem { Icon = "Shield24", Title = "心灵鸡汤" },
        };
            _isInitialized = true;
        }

        // 添加与XAML中控件绑定的属性和命令
        [RelayCommand]
        private void NavigateToDetail(string parameter)
        {
            // 导航逻辑
        }



    }
    public class ToolItem
    {
        public string Icon { get; set; }       // Symbol 字符串
        public string Title { get; set; }      // 标题
    }
}


