// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

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

    private void InitializeViewModel()
    {
        ApplicationTitle = "WPF UI - MVVM Demo";

        NavigationItems =
        [
            new NavigationViewItem()
            {
                Content = "��ҳ",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage),
               
            },
            new NavigationViewItem()
            {
                Content = "�û�����",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                
                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("�û���Ϣ", typeof(Views.Pages.UsersPage)),
                        new NavigationViewItem("��ɫ����", typeof(Views.Pages.RolesPage))
                  },
            },
            new NavigationViewItem()
            {
                Content = "Ai����",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.AIWindows),
            },
             new NavigationViewItem()
            {
               Content = "֪ʶ�����",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("֪ʶԤ��", typeof(Views.Pages.KnowledgeBase)),
                        new NavigationViewItem("֪ʶ���", typeof(Views.Pages.KnowledgeBase)),
                  },
            },
              new NavigationViewItem()
            {
               Content = "�ͻ�����",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("�ͻ���Ϣ", typeof(Views.Pages.Customers)),
                        new NavigationViewItem("�ͻ�ץȡ", typeof(Views.Pages.BrowserCrawlerPage)),
                  },
            },
               new NavigationViewItem()
            {
               Content = "��������",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },

                 MenuItemsSource = new object[]
                {
                        new NavigationViewItem("����Ԥ��", typeof(Views.Pages.TrainingDataPage))
                  },
            },
        ];

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
