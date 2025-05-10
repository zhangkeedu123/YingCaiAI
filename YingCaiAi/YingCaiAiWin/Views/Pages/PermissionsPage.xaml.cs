using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Data;
using YingCaiAiModel;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// 权限管理页面
    /// </summary>
    public partial class PermissionsPage : Page
    {
        // ViewModel 实例
        public  PermissionsPageViewModel _viewModel { get; }

        public PermissionsPage(PermissionsPageViewModel permissionsPageViewModel)
        {
            
            _viewModel = permissionsPageViewModel;
            DataContext = _viewModel;
            InitializeComponent();
            _viewModel.InitializeData();

            // 创建并设置 ViewModel
           

            // 绑定数据到TreeView
            PermissionsTreeView.ItemsSource = _viewModel.Permissions;

            // 隐藏对话框
            PermissionDialog.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 新增权限按钮点击事件
        /// </summary>
        private void AddPermissionButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddPermissionCommand.Execute(null);
            
            // 设置对话框标题
            PermissionDialog.SetValue(HandyControl.Controls.TitleElement.TitleProperty, _viewModel.DialogTitle);
            
            // 显示对话框
            PermissionDialog.Visibility = _viewModel.DialogVisibility;
        }

        /// <summary>
        /// 添加子权限按钮点击事件
        /// </summary>
        private void AddChildPermissionButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取父级权限ID
            Button button = sender as Button;
            int parentId = Convert.ToInt32(button.Tag);
            
            // 查找父权限对象并执行命令
            var parentPermission = _viewModel.FindPermissionById(_viewModel.Permissions, parentId);
            if (parentPermission != null)
            {
                _viewModel.AddChildPermissionCommand.Execute(parentPermission);
                
                // 更新父权限显示
                ParentPermissionTextBox.Text = parentPermission.Name;
                ParentPermissionTextBox.Visibility = Visibility.Visible;
                
                // 设置对话框标题
                PermissionDialog.SetValue(HandyControl.Controls.TitleElement.TitleProperty, _viewModel.DialogTitle);
                
                // 显示对话框
                PermissionDialog.Visibility = _viewModel.DialogVisibility;
            }
        }

        /// <summary>
        /// 编辑权限按钮点击事件
        /// </summary>
        private void EditPermissionButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前权限ID
            Button button = sender as Button;
            int permissionId = Convert.ToInt32(button.Tag);
            
            // 查找权限对象并执行命令
            var permission = _viewModel.FindPermissionById(_viewModel.Permissions, permissionId);
            if (permission != null)
            {
                _viewModel.EditPermissionCommand.Execute(permission);
                
                // 填充表单
                FillPermissionForm(_viewModel.CurrentPermission);
                
                // 设置对话框标题
                PermissionDialog.SetValue(HandyControl.Controls.TitleElement.TitleProperty, _viewModel.DialogTitle);
                
                // 显示对话框
                PermissionDialog.Visibility = _viewModel.DialogVisibility;
            }
        }

        /// <summary>
        /// 删除权限按钮点击事件
        /// </summary>
        private void DeletePermissionButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前权限ID
            Button button = sender as Button;
            int permissionId = Convert.ToInt32(button.Tag);
            
            // 查找权限对象并执行命令
            var permission = _viewModel.FindPermissionById(_viewModel.Permissions, permissionId);
            if (permission != null)
            {
                _viewModel.DeletePermissionCommand.Execute(permission);
                
                // 刷新TreeView
                PermissionsTreeView.ItemsSource = null;
                PermissionsTreeView.ItemsSource = _viewModel.Permissions;
            }
        }

        /// <summary>
        /// 对话框保存按钮点击事件
        /// </summary>
        private void PermissionDialog_PrimaryButtonClick(object sender, RoutedEventArgs e)
        {
            // 验证表单
            if (string.IsNullOrWhiteSpace(PermissionNameTextBox.Text))
            {
                Growl.Warning("请输入权限名称！");
                return;
            }

            if (string.IsNullOrWhiteSpace(PermissionCodeTextBox.Text))
            {
                Growl.Warning("请输入权限代码！");
                return;
            }

            if (PermissionTypeComboBox.SelectedItem == null)
            {
                Growl.Warning("请选择权限类型！");
                return;
            }

            if (PermissionModuleComboBox.SelectedItem == null)
            {
                Growl.Warning("请选择所属模块！");
                return;
            }

            // 更新ViewModel中的权限对象
            UpdatePermissionToViewModel();
            
            // 保存权限
            _viewModel.SavePermissionCommand.Execute(null);
            
            // 刷新TreeView
            PermissionsTreeView.ItemsSource = null;
            PermissionsTreeView.ItemsSource = _viewModel.Permissions;
            
            // 隐藏对话框
            PermissionDialog.Visibility = Visibility.Collapsed;
            
            // 提示成功
            Growl.Success("保存成功！");
        }

        /// <summary>
        /// 对话框关闭按钮点击事件
        /// </summary>
        private void PermissionDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // 取消对话框
            _viewModel.CancelDialogCommand.Execute(null);
            
            // 隐藏对话框
            PermissionDialog.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 搜索框文本变化事件
        /// </summary>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 获取搜索关键字
            string keyword = (sender as SearchBar).Text.Trim().ToLower();

            // 如果关键字为空，显示所有数据
            if (string.IsNullOrWhiteSpace(keyword))
            {
                PermissionsTreeView.ItemsSource = _viewModel.Permissions;
                return;
            }

            // 搜索权限
            var result = _viewModel.SearchPermissions(_viewModel.Permissions, keyword);

            // 显示搜索结果
            PermissionsTreeView.ItemsSource = result;
        }

        /// <summary>
        /// 模块筛选变化事件
        /// </summary>
        private void ModuleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 获取选中的模块
            HandyControl.Controls.ComboBox comboBox = sender as HandyControl.Controls.ComboBox;
            string module = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();

            // 如果选择全部模块，显示所有数据
            if (module == "全部模块")
            {
               // PermissionsTreeView.ItemsSource = _viewModel.Permissions;
                return;
            }

            // 筛选权限
            var result = _viewModel.FilterPermissionsByModule(_viewModel.Permissions, module);

            // 显示筛选结果
            PermissionsTreeView.ItemsSource = result;
        }

        /// <summary>
        /// 分页变化事件
        /// </summary>
        private void Pagination_PageUpdated(object sender, FunctionEventArgs<int> e)
        {
            // 获取当前页码
            int pageIndex = e.Info;
            
            // 这里可以调用ViewModel中的分页方法
        }

        /// <summary>
        /// 刷新按钮点击事件
        /// </summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // 重新初始化ViewModel
            DataContext = new PermissionsPageViewModel();
            _viewModel.InitializeData();
            
            // 刷新TreeView
            PermissionsTreeView.ItemsSource = _viewModel.Permissions;
            
            // 提示成功
            Growl.Info("数据已刷新！");
        }

        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 显示设置面板
            Growl.Info("设置功能开发中...");
        }

        #region 辅助方法

        /// <summary>
        /// 填充权限表单
        /// </summary>
        private void FillPermissionForm(YingCaiAiModel.Permission permission)
        {
            if (permission == null) return;
            
            PermissionNameTextBox.Text = permission.Name;
            PermissionCodeTextBox.Text = permission.Code;

            // 设置权限类型
            for (int i = 0; i < PermissionTypeComboBox.Items.Count; i++)
            {
                if ((PermissionTypeComboBox.Items[i] as ComboBoxItem).Content.ToString() == permission.Type)
                {
                    PermissionTypeComboBox.SelectedIndex = i;
                    break;
                }
            }

            // 设置所属模块
            for (int i = 0; i < PermissionModuleComboBox.Items.Count; i++)
            {
                if ((PermissionModuleComboBox.Items[i] as ComboBoxItem).Content.ToString() == permission.Module)
                {
                    PermissionModuleComboBox.SelectedIndex = i;
                    break;
                }
            }

            // 设置状态
            PermissionStatusToggle.IsChecked = permission.IsEnabled;

            // 设置排序
            PermissionSortNumeric.Value = permission.Sort;

            // 设置描述
            PermissionDescriptionTextBox.Text = permission.Description;
        }

        /// <summary>
        /// 更新权限到ViewModel
        /// </summary>
        private void UpdatePermissionToViewModel()
        {
            if (_viewModel.CurrentPermission == null)
            {
                _viewModel.CurrentPermission = new YingCaiAiModel.Permission();
            }
            
            _viewModel.CurrentPermission.Name = PermissionNameTextBox.Text.Trim();
            _viewModel.CurrentPermission.Code = PermissionCodeTextBox.Text.Trim();
            _viewModel.CurrentPermission.Type = (PermissionTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            _viewModel.CurrentPermission.Module = (PermissionModuleComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            _viewModel.CurrentPermission.IsEnabled = PermissionStatusToggle.IsChecked ?? true;
            _viewModel.CurrentPermission.Sort = (int)PermissionSortNumeric.Value;
            _viewModel.CurrentPermission.Description = PermissionDescriptionTextBox.Text.Trim();
            _viewModel.CurrentPermission.UpdateTime = DateTime.Now;
        }

        #endregion
    }
}