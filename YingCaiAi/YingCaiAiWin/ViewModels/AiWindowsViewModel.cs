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

        [ObservableProperty]
        private List<CardItem> _cardItems = [];


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

            CardItems = new()
    {
        new CardItem
        {
            Title = "客户管理",
            Description = "客户爬虫, 客户资料, 客户标签...",
            IconPath = "#febcd5",
            CommandParam = "1"
        },
        new CardItem
        {
            Title = "AI辅助窗口",
            Description = "AI实时互动，推荐话术...",
            IconPath = "#c8e1c4",
            CommandParam = "2"
        },
        new CardItem
        {
            Title = "知识库管理",
            Description = "查阅专业知识，更新知识库.",
            IconPath = "#aee8f5",
            CommandParam = "3"
        },
          new CardItem
        {
            Title = "话术管理",
            Description = "新建话术，情绪分析，录音管理...",
            IconPath = "#fde491g",
            CommandParam = "4"
        },
             new CardItem
        {
            Title = "数据大屏",
            Description = "销售额数据，新增合同数，各项指标分析新建话术，情绪分析，销售额数据，新增合同数，各项指标分析新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管销售额数据，新增合同数，各项指标分析新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管录音管理...新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管理......",
            IconPath = "#fde491",
            CommandParam = "5"
        }  ,
             new CardItem
        {
            Title = "用户管理",
            Description = "创建用户，权限配置，角色配置...",
            IconPath = "#febcd5",
            CommandParam = "6"
        } ,
             new CardItem
        {
            Title = "机器监控",
            Description = "设备监控，设备预警...",
            IconPath = "#c8e1c4",
            CommandParam = "7"
        } ,
             new CardItem
        {
            Title = "系统设置",
            Description = "设置主题，修改密码，修改字体...",
            IconPath = "#aee8f5",
            CommandParam = "8"
        },
             new CardItem
        {
            Title = "系统设置",
            Description = "设置主题，修改密码，修改字体...",
            IconPath = "#aee8f5",
            CommandParam = "8"
        },
             new CardItem
        {
            Title = "系统设置",
            Description = "设置主题，修改密码，修改字体...",
            IconPath = "#aee8f5",
            CommandParam = "8"
        },
             new CardItem
        {
            Title = "系统设置",
            Description = "设置主题，修改密码，修改字体...",
            IconPath = "#aee8f5",
            CommandParam = "8"
        },
             new CardItem
        {
            Title = "系统设置",
            Description = "设置主题，修改密码，修改字体...",
            IconPath = "#aee8f5",
            CommandParam = "8"
        },
             new CardItem
        {
            Title = "系统设置",
            Description = "设置主题，修改密码，修改字体...",
            IconPath = "#aee8f5",
            CommandParam = "8"
        },
             new CardItem
        {
            Title = "数据大屏",
            Description = "销售额数据，新增合同数，各项指标分析新建话术，情绪分析，销售额数据，新增合同数，各项指标分析新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管销售额数据，新增合同数，各项指标分析新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管录音管理...新建话术，情绪分析，录音管理...新建话术，情绪分析，录音管理......",
            IconPath = "#fde491",
            CommandParam = "5"
        }
        };
            _isInitialized = true;
        }

        // 添加与XAML中控件绑定的属性和命令
        [RelayCommand]
        private void NavigateToDetail(string parameter)
        {
            // 导航逻辑
        }

        [RelayCommand]
        private async void OnCloseCard(string parameter)
        {
            var cardToRemove = CardItems.FindAll(c => c.CommandParam?.ToString() != parameter?.ToString());
            if (cardToRemove != null)
            {
                CardItems = cardToRemove;
            }
        }

        
        public void ShowPopupCommand(string parameter)
        {
            var cardToRemove = CardItems.FindAll(c => c.CommandParam?.ToString() != "7");
            if (cardToRemove != null)
            {
                CardItems = cardToRemove;
            }
        }

    }


    public class ToolItem
    {
        public string Icon { get; set; }       // Symbol 字符串
        public string Title { get; set; }      // 标题
    }
}


