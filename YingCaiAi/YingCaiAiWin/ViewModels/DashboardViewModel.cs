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
            Title = "�ͻ�����",
            Description = "�ͻ�����, �ͻ�����, �ͻ���ǩ...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/user.png",
            CommandParam = "BasicInput"
        },
        new CardItem
        {
            Title = "AI��������",
            Description = "AIʵʱ�������Ƽ�����...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/ai.png",
            CommandParam = "DialogsAndFlyouts"
        },
        new CardItem
        {
            Title = "֪ʶ�����",
            Description = "����רҵ֪ʶ������֪ʶ��.",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/books.png",
            CommandParam = "Navigation"
        },
          new CardItem
        {
            Title = "��������",
            Description = "�½�����������������¼������...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/huashu.png",
            CommandParam = "Navigation"
        },
             new CardItem
        {
            Title = "���ݴ���",
            Description = "���۶����ݣ�������ͬ��������ָ�����...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/data.png",
            CommandParam = "Navigation"
        }  ,
             new CardItem
        {
            Title = "�û�����",
            Description = "�����û���Ȩ�����ã���ɫ����...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/users.png",
            CommandParam = "Navigation"
        } ,
             new CardItem
        {
            Title = "�������",
            Description = "�豸��أ��豸Ԥ��...",
            IconPath = "pack://application:,,,/Assets/WinUiGallery/jiqijiankong.png",
            CommandParam = "Navigation"
        } ,
             new CardItem
        {
            Title = "ϵͳ����",
            Description = "�������⣬�޸����룬�޸�����...",
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
    public string IconPath { get; set; } // ͼƬ·��
    public string CommandParam { get; set; }
}
