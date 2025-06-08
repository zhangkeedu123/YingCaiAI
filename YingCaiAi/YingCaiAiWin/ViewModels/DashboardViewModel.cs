// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Controls;
using YingCaiAiService.IService;
using YingCaiAiWin.Models;
using YingCaiAiWin.Views;
namespace YingCaiAiWin.ViewModels;

public partial class DashboardViewModel : ViewModel
{


    private bool _isInitialized = false;

    [ObservableProperty]
    private List<CardItem> _cardItems = [];

    [ObservableProperty]
    private List<DataColor> _colors = [];

     private readonly IUsersService _userService;

    public DashboardViewModel(INavigationService navigationService, IUsersService _userInfoService)
    {
        if (!_isInitialized)
        {
            _userService = _userInfoService;
            InitializeViewModel();
        }
       
    }
    private void InitializeViewModel()
    {
        CardItems = new()
    {
        new CardItem
        {
            Title = "客户管理",
            Description = "客户爬虫, 客户资料, 客户标签...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/user.png",
            CommandParam = "BasicInput"
        },
        new CardItem
        {
            Title = "AI辅助窗口",
            Description = "AI实时互动，推荐话术...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/ai.png",
            CommandParam = "DialogsAndFlyouts"
        },
        new CardItem
        {
            Title = "知识库管理",
            Description = "查阅专业知识，更新知识库.",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/books.png",
            CommandParam = "Navigation"
        },
          new CardItem
        {
            Title = "话术管理",
            Description = "新建话术，情绪分析，录音管理...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/huashu.png",
            CommandParam = "Navigation"
        },
             new CardItem
        {
            Title = "客户抓取",
            Description = "自动化获取客户数据资料...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/data.png",
            CommandParam = "Navigation"
        },
             new CardItem
        {
            Title = "数据大屏",
            Description = "销售额数据，新增合同数，各项指标分析...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/data.png",
            CommandParam = "Navigation"
        }  ,
             new CardItem
        {
            Title = "用户管理",
            Description = "创建用户，权限配置，角色配置...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/users.png",
            CommandParam = "Navigation"
        } ,
             new CardItem
        {
            Title = "录音管理",
            Description = "上传录音，NLP分析...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/jiqijiankong.png",
            CommandParam = "Navigation"
        } ,
             new CardItem
        {
            Title = "系统设置",
            Description = "设置主题，修改密码，修改字体...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/xitongshezhi.png",
            CommandParam = "Navigation"
        }
        };

        _isInitialized = true;
    }

    [RelayCommand]
    private void OnCardClick(string parameter)
    {
        if (Application.Current.Windows.OfType<MainWindow>().Any())
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().First();
           if(parameter == "客户管理")
               _ = mainWindow.Navigate(typeof(Views.Pages.Customers));
            if (parameter == "AI辅助窗口")
                _ = mainWindow.Navigate(typeof(Views.Pages.AIWindows));
            if (parameter == "知识库管理")
                _ = mainWindow.Navigate(typeof(Views.Pages.KnowledgeBase));
            if (parameter == "话术管理")
                _ = mainWindow.Navigate(typeof(Views.Pages.TrainingDataPage));
            if (parameter == "数据大屏")
                _ = mainWindow.Navigate(typeof(Views.Pages.DataDashboardPage));
            if (parameter == "客户管理")
                _ = mainWindow.Navigate(typeof(Views.Pages.Customers));
            if (parameter == "用户管理")
                _ = mainWindow.Navigate(typeof(Views.Pages.UsersPage));
            if (parameter == "录音管理")
                _ = mainWindow.Navigate(typeof(Views.Pages.AudioRecordPage));
            if (parameter == "客户抓取")
                _ = mainWindow.Navigate(typeof(Views.Pages.BrowserCrawlerPage));
            if (parameter == "系统设置")
                _ = mainWindow.Navigate(typeof(Views.Pages.SettingsPage));

        }
    }
}

public class CardItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; } // 图片路径
    public string CommandParam { get; set; }

}
