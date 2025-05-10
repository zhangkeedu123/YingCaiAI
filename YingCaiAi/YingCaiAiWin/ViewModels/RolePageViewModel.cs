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
    public partial class RolePageViewModel : ViewModel
    {
        private ObservableCollection<Role> _roles;
        private Role _selectedRole;
        private ObservableCollection<Permission> _permissions;
        private CollectionViewSource _rolesViewSource;
        private string _searchText = "";
        private bool _isDialogOpen;
        private string _dialogTitle;
        private Role _currentRole;

        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    LoadRolePermissions();
                }
            }
        }

        public ObservableCollection<Permission> Permissions
        {
            get => _permissions;
            set => SetProperty(ref _permissions, value);
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

        public Role CurrentRole
        {
            get => _currentRole;
            set => SetProperty(ref _currentRole, value);
        }

        public ICollectionView RolesView => _rolesViewSource?.View;

        public RolePageViewModel()
        {
            LoadMockData();
            SetupDataBinding();
        }

        private void LoadMockData()
        {
            // 创建模拟权限数据
            var allPermissions = new List<Permission>
            {
                // 用户管理模块
                new Permission { Id = 1, Name = "用户管理", Code = "user:manage", Module = "用户管理", Description = "用户管理相关功能", ParentId = null },
                new Permission { Id = 2, Name = "查看用户", Code = "user:view", Module = "用户管理", Description = "查看用户列表", ParentId = 1 },
                new Permission { Id = 3, Name = "创建用户", Code = "user:create", Module = "用户管理", Description = "创建新用户", ParentId = 1 },
                new Permission { Id = 4, Name = "编辑用户", Code = "user:edit", Module = "用户管理", Description = "编辑用户信息", ParentId = 1 },
                new Permission { Id = 5, Name = "删除用户", Code = "user:delete", Module = "用户管理", Description = "删除用户", ParentId = 1 },
                
                // 角色管理模块
                new Permission { Id = 6, Name = "角色管理", Code = "role:manage", Module = "角色管理", Description = "角色管理相关功能", ParentId = null },
                new Permission { Id = 7, Name = "查看角色", Code = "role:view", Module = "角色管理", Description = "查看角色列表", ParentId = 6 },
                new Permission { Id = 8, Name = "创建角色", Code = "role:create", Module = "角色管理", Description = "创建新角色", ParentId = 6 },
                new Permission { Id = 9, Name = "编辑角色", Code = "role:edit", Module = "角色管理", Description = "编辑角色信息", ParentId = 6 },
                new Permission { Id = 10, Name = "删除角色", Code = "role:delete", Module = "角色管理", Description = "删除角色", ParentId = 6 },
                
                // 系统设置模块
                new Permission { Id = 11, Name = "系统设置", Code = "system:manage", Module = "系统设置", Description = "系统设置相关功能", ParentId = null },
                new Permission { Id = 12, Name = "基础配置", Code = "system:basic", Module = "系统设置", Description = "系统基础配置", ParentId = 11 },
                new Permission { Id = 13, Name = "高级配置", Code = "system:advanced", Module = "系统设置", Description = "系统高级配置", ParentId = 11 },
                
                // AI管理模块
                new Permission { Id = 14, Name = "AI管理", Code = "ai:manage", Module = "AI管理", Description = "AI管理相关功能", ParentId = null },
                new Permission { Id = 15, Name = "模型管理", Code = "ai:model", Module = "AI管理", Description = "AI模型管理", ParentId = 14 },
                new Permission { Id = 16, Name = "训练管理", Code = "ai:training", Module = "AI管理", Description = "AI训练管理", ParentId = 14 },
                
                // 数据分析模块
                new Permission { Id = 17, Name = "数据分析", Code = "data:manage", Module = "数据分析", Description = "数据分析相关功能", ParentId = null },
                new Permission { Id = 18, Name = "用户分析", Code = "data:user", Module = "数据分析", Description = "用户数据分析", ParentId = 17 },
                new Permission { Id = 19, Name = "内容分析", Code = "data:content", Module = "数据分析", Description = "内容数据分析", ParentId = 17 },
                new Permission { Id = 20, Name = "系统分析", Code = "data:system", Module = "数据分析", Description = "系统数据分析", ParentId = 17 }
            };

            // 创建模拟角色数据
            _roles = new ObservableCollection<Role>
            {
                new Role 
                { 
                    Id = 1, 
                    Name = "超级管理员", 
                    Description = "拥有所有权限",
                    CreateTime = DateTime.Now.AddDays(-30),
                    UpdateTime = DateTime.Now.AddDays(-5)
                },
                new Role 
                { 
                    Id = 2, 
                    Name = "用户管理员", 
                    Description = "管理用户相关功能",
                    CreateTime = DateTime.Now.AddDays(-25),
                    UpdateTime = DateTime.Now.AddDays(-10)
                },
                new Role 
                { 
                    Id = 3, 
                    Name = "AI管理员", 
                    Description = "管理AI相关功能",
                    CreateTime = DateTime.Now.AddDays(-20),
                    UpdateTime = DateTime.Now.AddDays(-8)
                },
                new Role 
                { 
                    Id = 4, 
                    Name = "数据分析师", 
                    Description = "查看和分析数据",
                    CreateTime = DateTime.Now.AddDays(-15),
                    UpdateTime = DateTime.Now.AddDays(-3)
                },
                new Role 
                { 
                    Id = 5, 
                    Name = "普通用户", 
                    Description = "基础功能权限",
                    CreateTime = DateTime.Now.AddDays(-10),
                    UpdateTime = DateTime.Now.AddDays(-1)
                }
            };

            // 创建角色权限关联
            var rolePermissions = new List<RolePermission>
            {
                // 超级管理员拥有所有权限
                new RolePermission { RoleId = 1, PermissionId = 1 },
                new RolePermission { RoleId = 1, PermissionId = 2 },
                new RolePermission { RoleId = 1, PermissionId = 3 },
                new RolePermission { RoleId = 1, PermissionId = 4 },
                new RolePermission { RoleId = 1, PermissionId = 5 },
                new RolePermission { RoleId = 1, PermissionId = 6 },
                new RolePermission { RoleId = 1, PermissionId = 7 },
                new RolePermission { RoleId = 1, PermissionId = 8 },
                new RolePermission { RoleId = 1, PermissionId = 9 },
                new RolePermission { RoleId = 1, PermissionId = 10 },
                new RolePermission { RoleId = 1, PermissionId = 11 },
                new RolePermission { RoleId = 1, PermissionId = 12 },
                new RolePermission { RoleId = 1, PermissionId = 13 },
                new RolePermission { RoleId = 1, PermissionId = 14 },
                new RolePermission { RoleId = 1, PermissionId = 15 },
                new RolePermission { RoleId = 1, PermissionId = 16 },
                new RolePermission { RoleId = 1, PermissionId = 17 },
                new RolePermission { RoleId = 1, PermissionId = 18 },
                new RolePermission { RoleId = 1, PermissionId = 19 },
                new RolePermission { RoleId = 1, PermissionId = 20 },
                
                // 用户管理员拥有用户管理权限
                new RolePermission { RoleId = 2, PermissionId = 1 },
                new RolePermission { RoleId = 2, PermissionId = 2 },
                new RolePermission { RoleId = 2, PermissionId = 3 },
                new RolePermission { RoleId = 2, PermissionId = 4 },
                new RolePermission { RoleId = 2, PermissionId = 5 },
                
                // AI管理员拥有AI管理权限
                new RolePermission { RoleId = 3, PermissionId = 14 },
                new RolePermission { RoleId = 3, PermissionId = 15 },
                new RolePermission { RoleId = 3, PermissionId = 16 },
                
                // 数据分析师拥有数据分析权限
                new RolePermission { RoleId = 4, PermissionId = 17 },
                new RolePermission { RoleId = 4, PermissionId = 18 },
                new RolePermission { RoleId = 4, PermissionId = 19 },
                new RolePermission { RoleId = 4, PermissionId = 20 },
                
                // 普通用户拥有查看权限
                new RolePermission { RoleId = 5, PermissionId = 2 },
                new RolePermission { RoleId = 5, PermissionId = 7 }
            };

            // 初始化权限树
            _permissions = new ObservableCollection<Permission>();
            
            // 获取顶级权限
            var rootPermissions = allPermissions.Where(p => p.ParentId == null).ToList();
            
            foreach (var root in rootPermissions)
            {
                BuildPermissionTreeRecursive(root, allPermissions);
                _permissions.Add(root);
            }

            // 为每个角色设置其拥有的权限
            foreach (var role in _roles)
            {
                var permissionIds = rolePermissions
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => rp.PermissionId)
                    .ToList();

                role.Permissions = allPermissions
                    .Where(p => permissionIds.Contains(p.Id))
                    .ToList();
            }
        }

        private void BuildPermissionTreeRecursive(Permission parent, List<Permission> allPermissions)
        {
            var children = allPermissions.Where(p => p.ParentId == parent.Id).ToList();
            parent.Children = new ObservableCollection<Permission>(children);
            
            foreach (var child in children)
            {
                BuildPermissionTreeRecursive(child, allPermissions);
            }
        }

        private void SetupDataBinding()
        {
            _rolesViewSource = new CollectionViewSource { Source = _roles };
            _rolesViewSource.Filter += RolesViewSource_Filter;
            OnPropertyChanged(nameof(RolesView));
        }

        private void RolesViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Role role)
            {
                // 搜索筛选
                bool searchMatch = string.IsNullOrWhiteSpace(_searchText) ||
                                  role.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                                  role.Description.Contains(_searchText, StringComparison.OrdinalIgnoreCase);

                e.Accepted = searchMatch;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void LoadRolePermissions()
        {
            if (SelectedRole != null)
            {
                // 更新权限树的选中状态
                UpdatePermissionSelection(Permissions, SelectedRole.Permissions);
            }
        }

        private void UpdatePermissionSelection(IEnumerable<Permission> permissions, List<Permission> selectedPermissions)
        {
            foreach (var permission in permissions)
            {
                permission.IsSelected = selectedPermissions.Any(p => p.Id == permission.Id);
                if (permission.Children.Any())
                {
                    UpdatePermissionSelection(permission.Children, selectedPermissions);
                }
            }
        }

        public void RefreshDataView()
        {
            _rolesViewSource.View.Refresh();
        }

        [RelayCommand]
        public void AddRole()
        {
            CurrentRole = new Role
            {
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            DialogTitle = "添加角色";
            IsDialogOpen = true;
        }

        [RelayCommand]
        public void EditRole(Role role)
        {
            if (role == null) return;
            
            CurrentRole = new Role
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreateTime = role.CreateTime,
                UpdateTime = DateTime.Now,
                Permissions = role.Permissions?.ToList() ?? new List<Permission>()
            };
            
            DialogTitle = "编辑角色";
            IsDialogOpen = true;
        }

        [RelayCommand]
        public void DeleteRole(Role role)
        {
            if (role == null) return;

            _roles.Remove(role);
            RefreshDataView();
        }

        [RelayCommand]
        public void SaveRole()
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(CurrentRole.Name))
            {
                return;
            }

            // 更新角色权限
            CurrentRole.Permissions = Permissions
                .SelectMany(GetSelectedPermissions)
                .ToList();

            // 保存角色
            if (CurrentRole.Id == 0)
            {
                // 新角色，分配ID
                CurrentRole.Id = _roles.Count > 0 ? _roles.Max(r => r.Id) + 1 : 1;
                _roles.Add(CurrentRole);
            }
            else
            {
                // 更新现有角色
                var index = _roles.IndexOf(_roles.FirstOrDefault(r => r.Id == CurrentRole.Id));
                if (index >= 0)
                {
                    _roles[index] = CurrentRole;
                }
            }

            RefreshDataView();
            IsDialogOpen = false;
        }

        private IEnumerable<Permission> GetSelectedPermissions(Permission permission)
        {
            var result = new List<Permission>();
            
            if (permission.IsSelected)
            {
                result.Add(permission);
            }
            
            foreach (var child in permission.Children)
            {
                result.AddRange(GetSelectedPermissions(child));
            }
            
            return result;
        }

        [RelayCommand]
        public void CancelDialog()
        {
            IsDialogOpen = false;
        }

        [RelayCommand]
        public void SearchRoles(string searchText)
        {
            SearchText = searchText;
        }

        //[RelayCommand]
        //public void UpdateChildrenSelection(Permission permission, bool isSelected)
        //{
        //    permission.IsSelected = isSelected;
        //    foreach (var child in permission.Children)
        //    {
        //        UpdateChildrenSelection(child, isSelected);
        //    }
        //}

        [RelayCommand]
        public void UpdateParentSelection(Permission permission)
        {
            // 获取父权限
            var parent = FindParentPermission(permission, Permissions);
            if (parent != null)
            {
                parent.IsSelected = parent.Children.All(p => p.IsSelected);
                UpdateParentSelection(parent);
            }
        }

        private Permission FindParentPermission(Permission child, IEnumerable<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                if (permission.Children.Contains(child))
                    return permission;

                var parent = FindParentPermission(child, permission.Children);
                if (parent != null)
                    return parent;
            }
            return null;
        }

        // 判断角色是否拥有指定权限
        public bool HasPermission(Role role, string permissionCode)
        {
            if (role == null || string.IsNullOrEmpty(permissionCode))
                return false;

            return role.Permissions.Any(p => p.Code == permissionCode);
        }

        // 判断当前选中角色是否拥有指定权限
        public bool CurrentRoleHasPermission(string permissionCode)
        {
            return HasPermission(SelectedRole, permissionCode);
        }
    }
}
