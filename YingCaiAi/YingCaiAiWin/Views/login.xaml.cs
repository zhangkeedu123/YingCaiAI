using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Windows;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Collections.Generic;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiWin.Common;
using YingCaiAiWin.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;

namespace YingCaiAiWin.Views
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : FluentWindow
    {
        private INavigationWindow? _navigationWindow;
        // Token 服务
        private readonly TokenService _tokenService;

        public Login(INavigationWindow navigationWindow)
        {
            _navigationWindow = navigationWindow;
            InitializeComponent();
            _tokenService = new TokenService();
        }

        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // 验证用户名和密码不能为空
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                //MessageBox.Show("用户名和密码不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 验证用户名和密码
                User user = ValidateUser(username, password);
                if (user != null)
                {

                    #region jwt  暂时注销

                    //// 生成 JWT Token
                    //string token = _tokenService.GenerateJwtToken(user);
                    //user.Token = token;

                    //// 将 Token 存储到 Redis
                    //_tokenService.StoreTokenInRedis(username, token);

                    //// 保存用户信息到全局变量
                    //GlobalVariables.CurrentUser = user;



                    //// 关闭登录窗口
                    //this.Close();
                    if (Application.Current.Windows.OfType<MainWindow>().Any())
                    {

                        _navigationWindow!.ShowWindow();

                        _ = _navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));
                        // 关闭登录窗口
                        Application.Current.Windows.OfType<Login>().FirstOrDefault()?.Close();
                    }
                    // 关闭登录窗口
                    Application.Current.Windows.OfType<Login>().FirstOrDefault()?.Close();

                    #endregion

                }
                else
                {
                   // MessageBox.Show("用户名或密码错误！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"登录失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      
/// <summary>
/// 验证用户名和密码
/// </summary>
private User ValidateUser(string username, string password)
        {
            // 创建模拟角色数据
            var _roles = new ObservableCollection<YingCaiAiModel.Role>
            {
                new YingCaiAiModel.Role
                {
                    Id = 1,
                    Name = "超级管理员",
                    Description = "拥有所有权限",
                    CreateTime = DateTime.Now.AddDays(-30),
                    UpdateTime = DateTime.Now.AddDays(-5)
                },
                new YingCaiAiModel.Role
                {
                    Id = 2,
                    Name = "用户管理员",
                    Description = "管理用户相关功能",
                    CreateTime = DateTime.Now.AddDays(-25),
                    UpdateTime = DateTime.Now.AddDays(-10)
                },
                new YingCaiAiModel.Role
                {
                    Id = 3,
                    Name = "AI管理员",
                    Description = "管理AI相关功能",
                    CreateTime = DateTime.Now.AddDays(-20),
                    UpdateTime = DateTime.Now.AddDays(-8)
                },
                new YingCaiAiModel.Role
                {
                    Id = 4,
                    Name = "数据分析师",
                    Description = "查看和分析数据",
                    CreateTime = DateTime.Now.AddDays(-15),
                    UpdateTime = DateTime.Now.AddDays(-3)
                },
                new YingCaiAiModel.Role
                {
                    Id = 5,
                    Name = "普通用户",
                    Description = "基础功能权限",
                    CreateTime = DateTime.Now.AddDays(-10),
                    UpdateTime = DateTime.Now.AddDays(-1)
                }
            };

            // TODO: 实现实际的用户验证逻辑，连接数据库验证用户名和密码
            // 这里使用模拟数据进行演示
            if (username == "admin" && password == "admin")
            {
                return new User
                {
                    Id = 1,
                    Username = "admin",
                    RealName = "管理员",
                    IsActive = true,
                    Roles = _roles.ToList(),
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now
                };
            }
            else if (username == "user" && password == "user")
            {
                return new User
                {
                    Id = 2,
                    Username = "user",
                    RealName = "普通用户",
                    IsActive = true,
                    Roles = _roles.ToList(),
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now
                };
            }

            return null;
        }

        /// <summary>
        /// 忘记密码按钮点击事件
        /// </summary>
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("请联系管理员重置密码！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 注册按钮点击事件
        /// </summary>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("请联系管理员创建账号！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}