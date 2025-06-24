// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using HandyControl.Controls;
using YingCaiAiService.IService;
using YingCaiAiWin.Models;

namespace YingCaiAiWin.ViewModels;

public partial class SettingsViewModel : ViewModel
{
    private bool _isInitialized = false;
    public IUsersService _usersService { get; set; }
    [ObservableProperty]
    private string _Password;
    public SettingsViewModel(IUsersService usersService)
    {
        _usersService = usersService;

    }



    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private Wpf.Ui.Appearance.ApplicationTheme _currentApplicationTheme = Wpf.Ui
        .Appearance
        .ApplicationTheme
        .Unknown;

    public override void OnNavigatedTo()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    private void InitializeViewModel()
    {
        CurrentApplicationTheme = Wpf.Ui.Appearance.ApplicationThemeManager.GetAppTheme();
        AppVersion = $"YingCaiAiWin - {GetAssemblyVersion()}";

        _isInitialized = true;
    }

    private static string GetAssemblyVersion()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
            ?? string.Empty;
    }

    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentApplicationTheme == Wpf.Ui.Appearance.ApplicationTheme.Light)
                {
                    break;
                }

                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Light);
                CurrentApplicationTheme = Wpf.Ui.Appearance.ApplicationTheme.Light;

                break;

            default:
                if (CurrentApplicationTheme == Wpf.Ui.Appearance.ApplicationTheme.Dark)
                {
                    break;
                }

                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
                CurrentApplicationTheme = Wpf.Ui.Appearance.ApplicationTheme.Dark;

                break;
        }
    }

    [RelayCommand]
    private async void OnChangePwd()
    {
        var user = AppUser.Instance.Username;
        var userm = await _usersService.GetUserByNameAsync(user);


        if (!string.IsNullOrWhiteSpace(Password))
        {
            userm.PasswordHash = new Helpers.FileHelper().ToMD5(Password);
        }
        await Task.Run(() => {
            var flag = _usersService.UpdateUserAsync(userm).Status;

            if (flag)
            {
                Growl.Success("操作成功");
            }
            else
            {
                Growl.Error("操作失败！");

                Thread.Sleep(2500);
                Growl.Clear();

            }

        });
      


    }
}
