// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Models;

namespace YingCaiAiWin.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
{
    public ViewModels.SettingsViewModel ViewModel { get; }
    private INavigationWindow? _navigationWindow;
    public SettingsPage(ViewModels.SettingsViewModel viewModel, INavigationWindow navigationWindow)
    {
        ViewModel = viewModel;
        DataContext = this;
        _navigationWindow = navigationWindow;

        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

    private void SignOut_Click(object sender, RoutedEventArgs e)
    {
        UserStorageHelper.ClearUser();
        AppUser.Instance.Clear();
        if (Application.Current.Windows.OfType<MainWindow>().Any())
        {
            Application.Current.Windows.OfType<Login>().FirstOrDefault()?.Show();
            _navigationWindow.CloseWindow();
           
           
           
        }
    }
}
