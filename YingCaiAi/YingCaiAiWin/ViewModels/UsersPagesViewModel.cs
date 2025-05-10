using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using YingCaiAiModel;


namespace YingCaiAiWin.ViewModels
{
    public partial class UsersPagesViewModel : ObservableObject
    {
        private ObservableCollection<User> _users;
        private CollectionViewSource _usersViewSource;
        private int _currentPage = 1;
        private string _currentStatus = "全部用户";
        private string _currentDepartment = "全部部门";
        private string _searchText = "";
        private int _totalRecords;
        private User _currentUser;
        private List<RoleSelectionItem> _roleSelectionItems;
        private bool _isDialogOpen;
        private string _dialogTitle;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        public string CurrentDepartment
        {
            get => _currentDepartment;
            set => SetProperty(ref _currentDepartment, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    RefreshDataView();
                }
            }
        }

        public int TotalRecords
        {
            get => _totalRecords;
            set => SetProperty(ref _totalRecords, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public List<RoleSelectionItem> RoleSelectionItems
        {
            get => _roleSelectionItems;
            set => SetProperty(ref _roleSelectionItems, value);
        }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => SetProperty(ref _isDialogOpen, value);
        }

        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        public ICollectionView UsersView => _usersViewSource?.View;

        public UsersPagesViewModel()
        {
            LoadSampleData();
            SetupDataBinding();
        }

        private void LoadSampleData()
        {
            // 创建一些模拟角色数据
            var adminRole = new Role { Id = 1, Name = "系统管理员", Description = "拥有所有权限" };
            var userManagerRole = new Role { Id = 2, Name = "用户管理员", Description = "管理用户账户" };
            var normalUserRole = new Role { Id = 3, Name = "普通用户", Description = "基本操作权限" };
            var guestRole = new Role { Id = 4, Name = "访客", Description = "只读权限" };

            // 创建模拟用户数据
            _users = new ObservableCollection<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    RealName = "系统管理员",
                    Email = "admin@example.com",
                    Department = "IT部门",
                    Position = "系统管理员",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-30),
                    LastLoginTime = DateTime.Now.AddHours(-2),
                    Roles = new List<Role> { adminRole }
                },
                new User
                {
                    Id = 2,
                    Username = "manager",
                    RealName = "张经理",
                    Email = "manager@example.com",
                    Department = "人事部",
                    Position = "部门经理",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-25),
                    LastLoginTime = DateTime.Now.AddDays(-1),
                    Roles = new List<Role> { userManagerRole }
                },
                new User
                {
                    Id = 3,
                    Username = "user1",
                    RealName = "李四",
                    Email = "user1@example.com",
                    Department = "市场部",
                    Position = "市场专员",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-20),
                    LastLoginTime = DateTime.Now.AddDays(-3),
                    Roles = new List<Role> { normalUserRole }
                },
                new User
                {
                    Id = 4,
                    Username = "user2",
                    RealName = "王五",
                    Email = "user2@example.com",
                    Department = "财务部",
                    Position = "会计",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-15),
                    LastLoginTime = DateTime.Now.AddDays(-2),
                    Roles = new List<Role> { normalUserRole }
                },
                new User
                {
                    Id = 5,
                    Username = "guest",
                    RealName = "访客用户",
                    Email = "guest@example.com",
                    Department = "外部",
                    Position = "访客",
                    IsActive = false,
                    CreateTime = DateTime.Now.AddDays(-10),
                    LastLoginTime = DateTime.Now.AddDays(-2),
                    Roles = new List<Role> { guestRole }
                },
                new User
                {
                    Id = 6,
                    Username = "developer",
                    RealName = "张开发",
                    Email = "dev@example.com",
                    Department = "IT部门",
                    Position = "开发工程师",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-18),
                    LastLoginTime = DateTime.Now.AddDays(-1),
                    Roles = new List<Role> { normalUserRole }
                },
                new User
                {
                    Id = 7,
                    Username = "tester",
                    RealName = "李测试",
                    Email = "test@example.com",
                    Department = "IT部门",
                    Position = "测试工程师",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-15),
                    LastLoginTime = DateTime.Now.AddHours(-5),
                    Roles = new List<Role> { normalUserRole }
                },
                new User
                {
                    Id = 8,
                    Username = "marketing",
                    RealName = "王营销",
                    Email = "marketing@example.com",
                    Department = "市场部",
                    Position = "营销经理",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-22),
                    LastLoginTime = DateTime.Now.AddDays(-2),
                    Roles = new List<Role> { normalUserRole, userManagerRole }
                },
                new User
                {
                    Id = 9,
                    Username = "finance",
                    RealName = "赵财务",
                    Email = "finance@example.com",
                    Department = "财务部",
                    Position = "财务主管",
                    IsActive = true,
                    CreateTime = DateTime.Now.AddDays(-28),
                    LastLoginTime = DateTime.Now.AddDays(-1),
                    Roles = new List<Role> { normalUserRole }
                },
                new User
                {
                    Id = 10,
                    Username = "disabled",
                    RealName = "禁用账户",
                    Email = "disabled@example.com",
                    Department = "人事部",
                    Position = "前员工",
                    IsActive = false,
                    CreateTime = DateTime.Now.AddDays(-60),
                    LastLoginTime = DateTime.Now.AddDays(-30),
                    Roles = new List<Role> { normalUserRole }
                }
            };

            TotalRecords = _users.Count;
        }

        private void SetupDataBinding()
        {
            _usersViewSource = new CollectionViewSource { Source = _users };
            _usersViewSource.Filter += UsersViewSource_Filter;
            OnPropertyChanged(nameof(UsersView));
        }

        private void UsersViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is User user)
            {
                // 状态筛选
                bool statusMatch = true;
                switch (_currentStatus)
                {
                    case "全部用户":
                        // 不需要筛选
                        break;
                    case "管理员":
                        statusMatch = user.Roles.Any(r => r.Name == "系统管理员");
                        break;
                    case "普通用户":
                        statusMatch = user.Roles.Any(r => r.Name == "普通用户");
                        break;
                    case "已禁用":
                        statusMatch = !user.IsActive;
                        break;
                }

                // 部门筛选
                bool departmentMatch = true;
                if (_currentDepartment != "全部部门")
                {
                    departmentMatch = user.Department == _currentDepartment;
                }

                // 搜索筛选
                bool searchMatch = string.IsNullOrWhiteSpace(_searchText) ||
                                  user.Username.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                                  user.RealName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                                  user.Email.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                                  user.Department.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                                  user.Position.Contains(_searchText, StringComparison.OrdinalIgnoreCase);

                e.Accepted = statusMatch && departmentMatch && searchMatch;
            }
            else
            {
                e.Accepted = false;
            }
        }

        public void RefreshDataView()
        {
            _usersViewSource.View.Refresh();
        }

        [RelayCommand]
        public void AddUser()
        {
            CurrentUser = new User();
            PrepareUserDialog("添加用户");
            IsDialogOpen = true;
        }

        [RelayCommand]
        public void EditUser(User user)
        {
            if (user == null) return;
            
            CurrentUser = user;
            PrepareUserDialog("编辑用户");
            IsDialogOpen = true;
        }

        [RelayCommand]
        public void DeleteUser(User user)
        {
            if (user == null) return;

            // 在实际应用中，这里应该有确认对话框
            _users.Remove(user);
            TotalRecords = _users.Count;
            RefreshDataView();
        }

        [RelayCommand]
        public void SaveUser()
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(CurrentUser.Username))
            {
                // 在实际应用中，这里应该显示错误消息
                return;
            }

            // 更新角色
            CurrentUser.Roles = RoleSelectionItems
                .Where(r => r.IsSelected)
                .Select(r => new Role { Id = r.Id, Name = r.Name })
                .ToList();

            // 保存用户
            if (CurrentUser.Id == 0)
            {
                // 新用户，分配ID
                CurrentUser.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
                CurrentUser.CreateTime = DateTime.Now;
                _users.Add(CurrentUser);
            }
            else
            {
                // 更新现有用户
                var index = _users.IndexOf(_users.FirstOrDefault(u => u.Id == CurrentUser.Id));
                if (index >= 0)
                {
                    _users[index] = CurrentUser;
                }
            }

            TotalRecords = _users.Count;
            RefreshDataView();
            IsDialogOpen = false;
        }

        [RelayCommand]
        public void CancelDialog()
        {
            IsDialogOpen = false;
        }

        [RelayCommand]
        public void ChangePage(int page)
        {
            CurrentPage = page;
            // 在实际应用中，这里应该根据页码加载对应的数据
        }

        [RelayCommand]
        public void SearchUsers(string searchText)
        {
            SearchText = searchText;
        }

        [RelayCommand]
        public void FilterByStatus(string status)
        {
            CurrentStatus = status;
            RefreshDataView();
        }

        [RelayCommand]
        public void FilterByDepartment(string department)
        {
            CurrentDepartment = department;
            RefreshDataView();
        }

        private void PrepareUserDialog(string title)
        {
            DialogTitle = title;

            // 准备角色选择列表
            RoleSelectionItems = new List<RoleSelectionItem>
            {
                new RoleSelectionItem { Id = 1, Name = "系统管理员", IsSelected = CurrentUser.Roles?.Any(r => r.Id == 1) ?? false },
                new RoleSelectionItem { Id = 2, Name = "用户管理员", IsSelected = CurrentUser.Roles?.Any(r => r.Id == 2) ?? false },
                new RoleSelectionItem { Id = 3, Name = "普通用户", IsSelected = CurrentUser.Roles?.Any(r => r.Id == 3) ?? false },
                new RoleSelectionItem { Id = 4, Name = "访客", IsSelected = CurrentUser.Roles?.Any(r => r.Id == 4) ?? false }
            };
        }
    }

    public class RoleSelectionItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
