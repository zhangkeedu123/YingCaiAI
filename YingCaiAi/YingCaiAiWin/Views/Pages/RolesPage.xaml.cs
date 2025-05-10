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
    public partial class RolesPage : INavigableView<ViewModels.RolePageViewModel>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RolePageViewModel ViewModel { get; }

        public RolesPage(RolePageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RoleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ViewModel.SelectedRole = e.AddedItems[0] as Role;
            }
        }

        private void PermissionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var permission = checkbox.DataContext as YingCaiAiModel.Permission;
            //ViewModel.UpdateChildrenSelection(permission, true);
            ViewModel.UpdateParentSelection(permission);
        }

        private void PermissionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var permission = checkbox.DataContext as YingCaiAiModel.Permission;
            //ViewModel.UpdateChildrenSelection(permission, false);
            ViewModel.UpdateParentSelection(permission);
        }

        private void AddRoleButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddRole();
            //RoleDialog.Show();
        }

        private void EditRoleButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var role = (Role)button.DataContext;
            ViewModel.EditRole(role);
            //RoleDialog.Show();
        }

        private void DeleteRoleButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var role = (Role)button.DataContext;

            var result = HandyControl.Controls.MessageBox.Show(
                $"确定要删除角色 {role.Name} 吗？此操作不可撤销。",
                "确认删除",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                ViewModel.DeleteRole(role);
                Growl.Success($"角色 {role.Name} 已被删除");
            }
        }

        private void RoleDialog_PrimaryButtonClick(object sender, RoutedEventArgs e)
        {
            // 更新角色信息
            //ViewModel.CurrentRole.Name = RoleNameTextBox.Text;
           // ViewModel.CurrentRole.Description = RoleDescriptionTextBox.Text;

            ViewModel.SaveRole();
            Growl.Success($"角色 {ViewModel.CurrentRole.Name} 已保存");
            //RoleDialog.Hide();
        }

        private void RoleDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelDialog();
            //RoleDialog.Hide();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as SearchBar;
            if (textBox != null)
            {
                ViewModel.SearchText = textBox.Text;
            }
        }
    }
}