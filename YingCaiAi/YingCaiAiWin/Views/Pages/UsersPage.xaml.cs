using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using HandyControl.Data;
using Wpf.Ui.Abstractions.Controls;
using YingCaiAiModel;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    public partial class UsersPage : INavigableView<ViewModels.UsersPagesViewModel>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UsersPagesViewModel ViewModel { get; }

        public UsersPage(UsersPagesViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            
            // 绑定DataGrid数据源
            UsersDataGrid.ItemsSource = ViewModel.UsersView;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as SearchBar;
            if (textBox != null)
            {
                ViewModel.SearchText = textBox.Text;
            }
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    ViewModel.FilterByStatus(selectedItem.Content.ToString());
                }
            }
        }

        private void DepartmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    ViewModel.FilterByDepartment(selectedItem.Content.ToString());
                }
            }
        }

        private void AdvancedFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // 高级筛选功能
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.MessageBox.Show(
                "您可以自定义系统设置",
                "设置",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 处理选择变更事件
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddUser();
            //UserDialog.Show();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.MessageBox.Show(
                "您可以通过Excel模板导入用户信息",
                "导入用户",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var user = (User)button.DataContext;
            ViewModel.EditUser(user);
           // UserDialog.Show();
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var user = (User)button.DataContext;

            var result = HandyControl.Controls.MessageBox.Show(
                $"确定要删除用户 {user.Username} 吗？此操作不可撤销。",
                "确认删除",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                ViewModel.DeleteUser(user);
                Growl.Success($"用户 {user.Username} 已被删除");
            }
        }

        private void UserDialog_PrimaryButtonClick(object sender, RoutedEventArgs e)
        {
            // 更新用户信息
            ViewModel.CurrentUser.Username = UsernameTextBox.Text;
            ViewModel.CurrentUser.RealName = FullNameTextBox.Text;
            ViewModel.CurrentUser.Email = EmailTextBox.Text;
            ViewModel.CurrentUser.Department = DepartmentTextBox.Text;
            ViewModel.CurrentUser.Position = PositionTextBox.Text;
            ViewModel.CurrentUser.IsActive = IsActiveToggle.IsChecked ?? false;

            ViewModel.SaveUser();
            Growl.Success($"用户 {ViewModel.CurrentUser.Username} 已保存");
            //UserDialog.Hide();
        }

        private void UserDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelDialog();
           // UserDialog.Hide();
        }

        private void Pagination_PageUpdated(object sender, FunctionEventArgs<int> e)
        {
            ViewModel.ChangePage(e.Info);
        }
    }
}
