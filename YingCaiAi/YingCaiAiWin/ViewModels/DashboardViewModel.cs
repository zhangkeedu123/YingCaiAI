// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using YingCaiAiService.IService;
using YingCaiAiWin.Models;
namespace YingCaiAiWin.ViewModels;

public partial class DashboardViewModel : ViewModel
{


    private bool _isInitialized = false;

    [ObservableProperty]
    private List<CardItem> _cardItems = [];

    [ObservableProperty]
    private List<DataColor> _colors = [];

     private readonly IUserInfoService _userService;

    public DashboardViewModel(INavigationService navigationService, IUserInfoService _userInfoService)
    {
        if (!_isInitialized)
        {
            _userService = _userInfoService;
            InitializeViewModel();
        }
       
    }
    private void InitializeViewModel()
    {
        var dd = _userService.DeleteUserAsync(1);

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
            Title = "机器监控",
            Description = "设备监控，设备预警...",
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
}

public class CardItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; } // 图片路径
    public string CommandParam { get; set; }
}
