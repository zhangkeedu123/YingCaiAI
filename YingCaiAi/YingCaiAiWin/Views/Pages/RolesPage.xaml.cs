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

        //private void UpdatePermissionSelection(IEnumerable<YingCaiAiModel.Permission> permissions, List<YingCaiAiModel.Permission> selectedPermissions)
        //{
        //    foreach (var permission in permissions)
        //    {
        //        permission.IsSelected = selectedPermissions.Any(p => p.Id == permission.Id);
        //        if (permission.Children.Any())
        //        {
        //            UpdatePermissionSelection(permission.Children, selectedPermissions);
        //        }
        //    }
        //}

       

        //private void PermissionCheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    var checkbox = sender as CheckBox;
        //    var permission = checkbox.DataContext as YingCaiAiModel.Permission;
        //    UpdateChildrenSelection(permission, true);
        //    UpdateParentSelection(permission);
        //}

        //private void PermissionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    var checkbox = sender as CheckBox;
        //    var permission = checkbox.DataContext as YingCaiAiModel.Permission;
        //    UpdateChildrenSelection(permission, false);
        //    UpdateParentSelection(permission);
        //}

        //private void UpdateChildrenSelection(YingCaiAiModel.Permission permission, bool isSelected)
        //{
        //    permission.IsSelected = isSelected;
        //    foreach (var child in permission.Children)
        //    {
        //        UpdateChildrenSelection(child, isSelected);
        //    }
        //}

        //private void UpdateParentSelection(YingCaiAiModel.Permission permission)
        //{
        //    // 获取父权限
        //    var parent = FindParentPermission(permission, RolePermissions);
        //    if (parent != null)
        //    {
        //        parent.IsSelected = parent.Children.All(p => p.IsSelected);
        //        UpdateParentSelection(parent);
        //    }
        //}

        //private YingCaiAiModel.Permission FindParentPermission(YingCaiAiModel.Permission child, IEnumerable<YingCaiAiModel.Permission> permissions)
        //{
        //    foreach (var permission in permissions)
        //    {
        //        if (permission.Children.Contains(child))
        //            return permission;

        //        var parent = FindParentPermission(child, permission.Children);
        //        if (parent != null)
        //            return parent;
        //    }
        //    return null;
        //}

        private void AddRoleButton_Click(object sender, RoutedEventArgs e)
        {
            // 这里可以实现添加角色的逻辑
            Growl.Info("添加角色功能待实现");
        }
    }
}