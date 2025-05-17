using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Models;
using YingCaiAiWin.Services;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace YingCaiAiWin.Views
{
    public partial class Login : FluentWindow  // 修改基类为WPF UI的Window
    {
        private INavigationWindow? _navigationWindow;
        private IUsersService _usersService;
        public Login(INavigationWindow navigationWindow,IUsersService usersService)
        {
            _navigationWindow = navigationWindow;
            _usersService = usersService;
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                load();
            };
        }

        private void load()
        {
            
            if (!string.IsNullOrWhiteSpace(AppUser.Instance.Username))
            {
                UsernameTextBox.Text = AppUser.Instance.Username;
                PasswordBox.Password = AppUser.Instance.Token;
            }
        }


        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            ErrorInfoBar.IsOpen = false ;

            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            e.Handled = !_validRegex.IsMatch(username)|| !_validRegex.IsMatch(password);
            if (e.Handled)
            {
                System.Media.SystemSounds.Beep.Play();
                ErrorInfoBar.Message = "只能输入数字和英文!";
                ErrorInfoBar.IsOpen = true;
                return;
            }
            var flag = await _usersService.LoginUsersAsync(username, new FileHelper().ToMD5(password));
            if (flag != null && flag.Id > 0)
            {
                AppUser.Instance.Username = flag.UserName;
                AppUser.Instance.Token = password;
                AppUser.Instance.Role =flag.PerIds;

                UserStorageHelper.SaveUser(AppUser.Instance); // 保存持久化

                if (Application.Current.Windows.OfType<MainWindow>().Any())
                {

                    _navigationWindow!.ShowWindow();

                    _ = _navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));
                    // 关闭登录窗口
                    Application.Current.Windows.OfType<Login>().FirstOrDefault()?.Close();
                }
            }
            else
            {
                ErrorInfoBar.Message = "用户名或密码错误！";
                ErrorInfoBar.IsOpen = true;
            }
           
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            //Wpf.Ui.Controls.MessageBox.Show(
            //    "提示",
            //    "忘记密码功能正在开发中...",
            //    MessageBoxButton.OK,
            //    Wpf.Ui.Controls.MessageBoxImage.Information);
        }

        private static readonly Regex _validRegex = new Regex("^[a-zA-Z0-9]+$");

        // 阻止键盘输入非法字符（包括中文符号、汉字）
        private void UsernameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_validRegex.IsMatch(e.Text);
            if (e.Handled)
            {
                System.Media.SystemSounds.Beep.Play();
                ErrorInfoBar.Message = "只能输入数字和英文！";
                ErrorInfoBar.IsOpen = true;
            }
                
        }
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
            _navigationWindow.CloseWindow();
        }

    }
}