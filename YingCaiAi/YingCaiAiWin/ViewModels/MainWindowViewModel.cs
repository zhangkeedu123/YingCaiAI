// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using YingCaiAiWin.Models;

namespace YingCaiAiWin.ViewModels;

public partial class MainWindowViewModel : ViewModel
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private string _applicationTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<object> _navigationItems = [];

    [ObservableProperty]
    private ObservableCollection<object> _navigationFooter = [];

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = [];

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Demo"
    )]
    public MainWindowViewModel(INavigationService navigationService)
    {

        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    public void InitializeViewModel()
    {
        ApplicationTitle = "英才AI工作台";
       
        if (AppUser.Instance.Username!=null&& AppUser.Instance.RoleName != "管理员")
        {
                var naItems = new ObservableCollection<object>
            {
                new NavigationViewItem()
                {
                    Content = "主页",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(Views.Pages.DashboardPage),

                }
            };
            if (AppUser.Instance.Role.Contains("2")|| AppUser.Instance.Role.Contains("3"))
            {
                var MenuItemsSource = new List<NavigationViewItem> ();
                if(AppUser.Instance.Role.Contains("2"))
                     MenuItemsSource.Add(new NavigationViewItem("用户信息", typeof(Views.Pages.UsersPage)));


                if (AppUser.Instance.Role.Contains("3"))
                    MenuItemsSource.Add(new NavigationViewItem("角色管理", typeof(Views.Pages.RolesPage)));

                naItems.Add(new NavigationViewItem()
                {
                    Content = "用户管理",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                    MenuItemsSource = MenuItemsSource
                });
            }
            if (AppUser.Instance.Role.Contains("12") || AppUser.Instance.Role.Contains("13") || AppUser.Instance.Role.Contains("14"))
            {
                var MenuItemsSource = new List<NavigationViewItem>();
                if (AppUser.Instance.Role.Contains("12"))
                    MenuItemsSource.Add(new NavigationViewItem("我的客户", typeof(Views.Pages.UserCustomers)));


                if (AppUser.Instance.Role.Contains("13"))
                    MenuItemsSource.Add(new NavigationViewItem("客户公共库", typeof(Views.Pages.Customers)));
                if (AppUser.Instance.Role.Contains("14"))
                    MenuItemsSource.Add(new NavigationViewItem("客户抓取", typeof(Views.Pages.BrowserCrawlerPage)));

                naItems.Add(new NavigationViewItem()
                {
                    Content = "客户管理",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                    MenuItemsSource = MenuItemsSource
                });
            }
            if (AppUser.Instance.Role.Contains("5") )
            {
                naItems.Add(new NavigationViewItem()
                {
                    Content = "Ai窗口",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                    TargetPageType = typeof(Views.Pages.AIWindows),
                });
            }

            if (AppUser.Instance.Role.Contains("8") || AppUser.Instance.Role.Contains("9"))
            {
                var MenuItemsSource = new List<NavigationViewItem>();
                if (AppUser.Instance.Role.Contains("8"))
                    MenuItemsSource.Add(new NavigationViewItem("知识预览", typeof(Views.Pages.KnowledgeBase)));


                if (AppUser.Instance.Role.Contains("9"))
                    MenuItemsSource.Add(new NavigationViewItem("系统配置", typeof(Views.Pages.SystemConfigPage)));

                naItems.Add(new NavigationViewItem()
                {
                    Content = "知识库管理",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                    MenuItemsSource = MenuItemsSource
                });
            }

       

            if (AppUser.Instance.Role.Contains("16") || AppUser.Instance.Role.Contains("17"))
            {
                var MenuItemsSource = new List<NavigationViewItem>();
                if (AppUser.Instance.Role.Contains("16"))
                    MenuItemsSource.Add(new NavigationViewItem("话术预览", typeof(Views.Pages.TrainingDataPage)));


                if (AppUser.Instance.Role.Contains("17"))
                    MenuItemsSource.Add(new NavigationViewItem("录音管理", typeof(Views.Pages.AudioRecordPage)));

                naItems.Add(new NavigationViewItem()
                {
                    Content = "话术管理",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                    MenuItemsSource = MenuItemsSource
                });
            }

            if (AppUser.Instance.Role.Contains("19") || AppUser.Instance.Role.Contains("20"))
            {
                var MenuItemsSource = new List<NavigationViewItem>();
                if (AppUser.Instance.Role.Contains("19"))
                    MenuItemsSource.Add(new NavigationViewItem("数据大屏", typeof(Views.Pages.DataDashboardPage)));


                if (AppUser.Instance.Role.Contains("20"))
                    MenuItemsSource.Add(new NavigationViewItem("AI使用记录", typeof(Views.Pages.AiRecordPage)));

                naItems.Add(new NavigationViewItem()
                {
                    Content = "数据看板",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                    MenuItemsSource = MenuItemsSource
                });
            }

            NavigationItems = naItems;

        }
        else
        {
            NavigationItems =
     [
         new NavigationViewItem()
            {
                Content = "主页",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage),

            },
            new NavigationViewItem()
            {
                Content = "用户管理",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("用户信息", typeof(Views.Pages.UsersPage)),

                        new NavigationViewItem("角色管理", typeof(Views.Pages.RolesPage))
                  },
            },
             new NavigationViewItem()
            {
               Content = "客户管理",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                       new NavigationViewItem("我的客户", typeof(Views.Pages.UserCustomers)),
                        new NavigationViewItem("客户公共库", typeof(Views.Pages.Customers)),
                        new NavigationViewItem("客户抓取", typeof(Views.Pages.BrowserCrawlerPage)),
                  },
            },
            new NavigationViewItem()
            {
                Content = "Ai窗口",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.AIWindows),
            },
             new NavigationViewItem()
            {
               Content = "知识库管理",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("知识预览", typeof(Views.Pages.KnowledgeBase)),
                        new NavigationViewItem("系统配置", typeof(Views.Pages.SystemConfigPage)),
                  },
            },
             
               new NavigationViewItem()
            {
               Content = "话术管理",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("话术预览", typeof(Views.Pages.TrainingDataPage)),
                        new NavigationViewItem("录音管理", typeof(Views.Pages.AudioRecordPage))
                  },

            },
            new NavigationViewItem()
            {
               Content = "数据看板",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("数据大屏", typeof(Views.Pages.DataDashboardPage)),
                         new NavigationViewItem("AI使用记录", typeof(Views.Pages.AiRecordPage))
                  },
            },
        ];

        }

        NavigationFooter =
        [
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage),
            },
        ];

        TrayMenuItems = [new() { Header = "Home", Tag = "tray_home" }];

        _isInitialized = true;
    }
}
