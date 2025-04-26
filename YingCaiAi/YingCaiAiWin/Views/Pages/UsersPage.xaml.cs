using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// UsersPage.xaml 的交互逻辑
    /// </summary>
    public partial class UsersPage : INavigableView<ViewModels.UsersPagesViewModel>, INotifyPropertyChanged
    {

        private List<RoleSelectionItem> _roleSelectionItems;
        private ObservableCollection<UserInfo> _allUsers;
        private CollectionViewSource _usersViewSource;
        private int _currentPage = 1;
        //private string _currentStatus = "全部用户";
        //private string _currentDepartment = "全部部门";
        //private string _searchText = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public ObservableCollection<UserInfo> Users
        {
            get { return _allUsers; }
            set
            {
                _allUsers = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public UsersPagesViewModel ViewModel { get; set; }

        public UsersPage(UsersPagesViewModel viewModel)
        {
            ViewModel= viewModel;
            InitializeComponent();
            DataContext = this;
            
            LoadSampleData();
            SetupDataBinding();
            UserDialog.Visibility=Visibility.Hidden;
        }

        private void LoadSampleData()
        {
            // 创建一些假数据用于展示
            _allUsers = new ObservableCollection<UserInfo>
            {
                new UserInfo
                {
                    Id = 1,
                    Username = "admin",
                    FullName = "系统管理员",
                    Email = "admin@example.com",
                    Department = "IT部门",
                    Position = "系统管理员",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    LastLogin = DateTime.Now.AddHours(-2),
                   
                },
                new UserInfo
                {
                    Id = 2,
                    Username = "manager",
                    FullName = "张经理",
                    Email = "manager@example.com",
                    Department = "人事部",
                    Position = "部门经理",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-25),
                    LastLogin = DateTime.Now.AddDays(-1),
                   
                },
                new UserInfo
                {
                    Id = 3,
                    Username = "user1",
                    FullName = "李四",
                    Email = "user1@example.com",
                    Department = "市场部",
                    Position = "市场专员",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-20),
                    LastLogin = DateTime.Now.AddDays(-3),
                   
                },
                new UserInfo
                {
                    Id = 4,
                    Username = "user2",
                    FullName = "王五",
                    Email = "user2@example.com",
                    Department = "财务部",
                    Position = "会计",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    LastLogin = DateTime.Now.AddDays(-2),
                    
                },
                new UserInfo
                {
                    Id = 5,
                    Username = "guest",
                    FullName = "访客用户",
                    Email = "guest@example.com",
                    Department = "外部",
                    Position = "访客",
                    IsActive = false,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    LastLogin = DateTime.Now.AddDays(-2),

                },
                new UserInfo
                {
                    Id = 6,
                    Username = "developer",
                    FullName = "张开发",
                    Email = "dev@example.com",
                    Department = "IT部门",
                    Position = "开发工程师",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-18),
                    LastLogin = DateTime.Now.AddDays(-1),
                   
                },
                new UserInfo
                {
                    Id = 7,
                    Username = "tester",
                    FullName = "李测试",
                    Email = "test@example.com",
                    Department = "IT部门",
                    Position = "测试工程师",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    LastLogin = DateTime.Now.AddHours(-5),
                   
                },
                new UserInfo
                {
                    Id = 8,
                    Username = "marketing",
                    FullName = "王营销",
                    Email = "marketing@example.com",
                    Department = "市场部",
                    Position = "营销经理",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-22),
                    LastLogin = DateTime.Now.AddDays(-2),
                   
                },
                new UserInfo
                {
                    Id = 9,
                    Username = "finance",
                    FullName = "赵财务",
                    Email = "finance@example.com",
                    Department = "财务部",
                    Position = "财务主管",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-28),
                    LastLogin = DateTime.Now.AddDays(-1),
                    //Roles = new List<Role> { new Role { Id = 3, Name = "普通用户" } }
                },
                new UserInfo
                {
                    Id = 10,
                    Username = "disabled",
                    FullName = "禁用账户",
                    Email = "disabled@example.com",
                    Department = "人事部",
                    Position = "前员工",
                    IsActive = false,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    LastLogin = DateTime.Now.AddDays(-30),
                    //Roles = new List<Role> { new Role { Id = 3, Name = "普通用户" } }
                },
                new UserInfo
                {
                    Id = 11,
                    Username = "disabled",
                    FullName = "禁用账222户",
                    Email = "disabled@example.com",
                    Department = "人事部",
                    Position = "前员工",
                    IsActive = false,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    LastLogin = DateTime.Now.AddDays(-30),
                    //Roles = new List<Role> { new Role { Id = 3, Name = "普通用户" } }
                }
            };
        }

        private void SetupDataBinding()
        {
            _usersViewSource = new CollectionViewSource { Source = _allUsers };
            _usersViewSource.Filter += UsersViewSource_Filter;
            //.ItemsSource = _usersViewSource.View;
        }

        private void UsersViewSource_Filter(object sender, FilterEventArgs e)
        {
        //    if (e.Item is UserInfo user)
        //    {
        //        // 状态筛选
        //        bool statusMatch = true;
        //        switch (_currentStatus)
        //        {
        //            case "全部用户":
        //                // 不需要筛选
        //                break;
        //            case "管理员":
        //                statusMatch = user.Roles.Any(r => r.Name == "系统管理员");
        //                break;
        //            case "普通用户":
        //                statusMatch = user.Roles.Any(r => r.Name == "普通用户");
        //                break;
        //            case "已禁用":
        //                statusMatch = !user.IsActive;
        //                break;
        //        }

        //        // 部门筛选
        //        bool departmentMatch = true;
        //        if (_currentDepartment != "全部部门")
        //        {
        //            departmentMatch = user.Department == _currentDepartment;
        //        }

        //        // 搜索筛选
        //        bool searchMatch = string.IsNullOrWhiteSpace(_searchText) ||
        //                          user.Username.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
        //                          user.FullName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
        //                          user.Email.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
        //                          user.Department.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
        //                          user.Position.Contains(_searchText, StringComparison.OrdinalIgnoreCase);

        //        e.Accepted = statusMatch && departmentMatch && searchMatch;
        //    }
        //    else
        //    {
        //        e.Accepted = false;
        //    }
        }

        private void RefreshDataView()
        {
           // _usersViewSource.View.Refresh();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var textBox = sender as SearchBar;
            //if (textBox != null)
            //{
            //    _searchText = textBox.Text;
            //    RefreshDataView();
            //}
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var comboBox = sender as System.Windows.Controls.ComboBox;
            //if (comboBox != null && comboBox.SelectedItem != null)
            //{
            //    var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            //    if (selectedItem != null)
            //    {
            //        _currentStatus = selectedItem.Content.ToString();
            //        RefreshDataView();
            //    }
            //}
        }

        private void DepartmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var comboBox = sender as System.Windows.Controls.ComboBox;
            //if (comboBox != null && comboBox.SelectedItem != null)
            //{
            //    var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            //    if (selectedItem != null)
            //    {
            //        _currentDepartment = selectedItem.Content.ToString();
            //        RefreshDataView();
            //    }
            //}
        }

        private void AdvancedFilterButton_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //HandyControl.Controls.MessageBox.Show(
            //    "您可以自定义系统设置",
            //    "设置",
            //    MessageBoxButton.OK,
            //    MessageBoxImage.Information);
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 处理选择变更事件
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            //_currentUser = new User();
            //PrepareUserDialog("添加用户");
            //UserDialog.Show();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            //HandyControl.Controls.MessageBox.Show(
            //    "您可以通过Excel模板导入用户信息",
            //    "导入用户",
            //    MessageBoxButton.OK,
            //    MessageBoxImage.Information);
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            //var button = (Button)sender;
            //_currentUser = (User)button.DataContext;
            //PrepareUserDialog("编辑用户");
            //UserDialog.Show();
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            //var button = (Button)sender;
            //var user = (User)button.DataContext;

            //var result = HandyControl.Controls.MessageBox.Show(
            //    $"确定要删除用户 {user.Username} 吗？此操作不可撤销。",
            //    "确认删除",
            //    MessageBoxButton.YesNo,
            //    MessageBoxImage.Warning
            //);

            //if (result == MessageBoxResult.Yes)
            //{
            //    _dataService.DeleteUser(user.Id);
            //    _allUsers.Remove(user);
            //    Growl.Success($"用户 {user.Username} 已被删除");
            //}
        }

        private void PrepareUserDialog(string title)
        {
            //UserDialog.Title = title;

            //// 填充用户信息
            //UsernameTextBox.Text = _currentUser.Username;
            //FullNameTextBox.Text = _currentUser.FullName;
            //EmailTextBox.Text = _currentUser.Email;
            //DepartmentTextBox.Text = _currentUser.Department;
            //PositionTextBox.Text = _currentUser.Position;
            //IsActiveToggle.IsChecked = _currentUser.IsActive;

            //// 准备角色选择列表
            //_roleSelectionItems = new List<RoleSelectionItem>
            //{
            //    new RoleSelectionItem { Id = 1, Name = "系统管理员", IsSelected = _currentUser.Roles?.Any(r => r.Id == 1) ?? false },
            //    new RoleSelectionItem { Id = 2, Name = "用户管理员", IsSelected = _currentUser.Roles?.Any(r => r.Id == 2) ?? false },
            //    new RoleSelectionItem { Id = 3, Name = "普通用户", IsSelected = _currentUser.Roles?.Any(r => r.Id == 3) ?? false },
            //    new RoleSelectionItem { Id = 4, Name = "访客", IsSelected = _currentUser.Roles?.Any(r => r.Id == 4) ?? false }
            //};

            //RolesListBox.ItemsSource = _roleSelectionItems;
        }

        private void UserDialog_PrimaryButtonClick(object sender, RoutedEventArgs e)
        {
            //// 验证输入
            //if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            //{
            //    //HandyControl.Controls.MessageBox.Show(
            //    //    "用户名不能为空",
            //    //    "验证错误",
            //    //    MessageBoxButton.OK,
            //    //    MessageBoxImage.Error);
            //    return;
            //}

            //// 更新用户信息
            //_currentUser.Username = UsernameTextBox.Text;
            //_currentUser.FullName = FullNameTextBox.Text;
            //_currentUser.Email = EmailTextBox.Text;
            //_currentUser.Department = DepartmentTextBox.Text;
            //_currentUser.Position = PositionTextBox.Text;
            //_currentUser.IsActive = IsActiveToggle.IsChecked ?? false;

            //// 更新角色
            ////_currentUser.Roles = _roleSelectionItems
            ////    .Where(r => r.IsSelected)
            ////    .Select(r => new Role { Id = r.Id, Name = r.Name })
            ////    .ToList();

            //// 保存用户
            //if (_currentUser.Id == 0)
            //{
            //    // 新用户，分配ID
            //    _currentUser.Id = _allUsers.Count > 0 ? _allUsers.Max(u => u.Id) + 1 : 1;
            //    _currentUser.CreatedAt = DateTime.Now;
            //    _allUsers.Add(_currentUser);
            //    Growl.Success($"用户 {_currentUser.Username} 已添加");
            //}
            //else
            //{
            //    // 更新现有用户
            //    var index = _allUsers.IndexOf(_allUsers.FirstOrDefault(u => u.Id == _currentUser.Id));
            //    if (index >= 0)
            //    {
            //        _allUsers[index] = _currentUser;
            //    }
            //    RefreshDataView();
            //    Growl.Success($"用户 {_currentUser.Username} 已更新");
            //}

            //UserDialog.Hide();
        }

        private void UserDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            //UserDialog.Hide();
        }

        private void Pagination_PageUpdated(object sender, FunctionEventArgs<int> e)
        {
            CurrentPage = e.Info;
            // 在实际应用中，这里应该根据页码加载对应的数据
        }
    }

    // 用于角色选择的辅助类
    public class RoleSelectionItem : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private bool _isSelected;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 状态转换器
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? "启用" : "禁用";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // 背景色转换器
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6F4EA")) :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEE"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // 前景色转换器
    public class BoolToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34A853")) :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA4335"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
