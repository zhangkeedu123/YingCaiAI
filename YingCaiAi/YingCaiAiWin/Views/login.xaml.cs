using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace YingCaiAiWin.Views
{
    public partial class Login : FluentWindow  // 修改基类为WPF UI的Window
    {
        public Login()
        {
           // InitializeComponent();
            
            // 添加系统主题适配支持
            //Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
        //    string username = UsernameTextBox.Text;
        //    string password = PasswordBox.Password;

        //    // 检查用户名是否为空
        //    if (string.IsNullOrEmpty(username))
        //    {
        //        Wpf.Ui.Controls.MessageBox.Show(
        //            "提示",
        //            "请输入用户名或手机号",
        //            MessageBoxButton.OK,
        //            Wpf.Ui.Controls.MessageBoxImage.Warning);
        //        return;
        //    }

        //    // 检查密码是否为空
        //    if (string.IsNullOrEmpty(password))
        //    {
        //        Wpf.Ui.Controls.MessageBox.Show(
        //            "提示",
        //            "请输入密码",
        //            MessageBoxButton.OK,
        //            Wpf.Ui.Controls.MessageBoxImage.Warning);
        //        return;
        //    }

        //    // 这里添加实际的登录验证逻辑
        //    if (username == "admin" && password == "admin")
        //    {
        //        // 登录成功，打开主窗口
        //        MainWindow mainWindow = new MainWindow();
        //        mainWindow.Show();
        //        this.Close();
        //    }
        //    else
        //    {
        //        //Wpf.Ui.Controls.MessageBox.Show(
        //        //    "登录失败",
        //        //    "用户名或密码错误，请重试",
        //        //    MessageBoxButton.OK,
        //        //    Wpf.Ui.Controls.MessageBoxImage.Error);
        //    }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            //Wpf.Ui.Controls.MessageBox.Show(
            //    "提示",
            //    "忘记密码功能正在开发中...",
            //    MessageBoxButton.OK,
            //    Wpf.Ui.Controls.MessageBoxImage.Information);
        }

        private void WeChatLogin_Click(object sender, RoutedEventArgs e)
        {
            //Wpf.Ui.Controls.MessageBox.Show(
            //    "提示",
            //    "微信登录功能正在开发中...",
            //    MessageBoxButton.OK,
            //    Wpf.Ui.Controls.MessageBoxImage.Information);
        }

        private void QQLogin_Click(object sender, RoutedEventArgs e)
        {
            //Wpf.Ui.Controls.MessageBox.Show(
            //    "提示",
            //    "QQ登录功能正在开发中...",
            //    MessageBoxButton.OK,
            //    Wpf.Ui.Controls.MessageBoxImage.Information);
        }

        private void PhoneLogin_Click(object sender, RoutedEventArgs e)
        {
            //Wpf.Ui.Controls.MessageBox.Show(
            //    "提示",
            //    "手机验证码登录功能正在开发中...",
            //    MessageBoxButton.OK,
            //    Wpf.Ui.Controls.MessageBoxImage.Information);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //Wpf.Ui.Controls.MessageBox.Show(
            //    "提示",
            //    "注册功能正在开发中...",
            //    //MessageBoxButton.OK,
            //    //Wpf.Ui.Controls.MessageBoxImage.Information);
        }
    }
}