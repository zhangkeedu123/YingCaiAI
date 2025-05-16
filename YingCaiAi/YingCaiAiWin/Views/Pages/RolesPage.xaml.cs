using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;
using YingCaiAiModel;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    public partial class RolesPage : INavigableView<ViewModels.RolePageViewModel>
    {
        public ViewModels.RolePageViewModel ViewModel { get; }

        public RolesPage(RolePageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }

   

        private void AddRoleButton_Click(object sender, RoutedEventArgs e)
        {
            // 这里可以实现添加角色的逻辑
            Growl.Info("添加角色功能待实现");
        }
    }
}