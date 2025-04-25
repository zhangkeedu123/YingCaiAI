using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YingCaiAiModel;

namespace YingCaiAiWin.Views.Pages
{
    public partial class RolesPage : INotifyPropertyChanged
    {
        private ObservableCollection<Role> _roles;
        private Role _selectedRole;
        private ObservableCollection<YingCaiAiModel.Permission> _rolePermissions;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                OnPropertyChanged(nameof(Roles));
            }
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
                LoadRolePermissions();
            }
        }

        public ObservableCollection<YingCaiAiModel.Permission> RolePermissions
        {
            get => _rolePermissions;
            set
            {
                _rolePermissions = value;
                OnPropertyChanged(nameof(RolePermissions));
            }
        }

        public RolesPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadMockData();
        }

        private void LoadMockData()
        {
            // 创建模拟权限数据
            var permissions = new List<YingCaiAiModel.Permission>
            {
                // 用户管理模块
                new YingCaiAiModel.Permission { Id = 1, Name = "用户管理", Code = "user:manage", Module = "用户管理", Description = "用户管理相关功能", ParentId = null },
                new YingCaiAiModel.Permission { Id = 2, Name = "查看用户", Code = "user:view", Module = "用户管理", Description = "查看用户列表", ParentId = 1 },
                new YingCaiAiModel.Permission { Id = 3, Name = "创建用户", Code = "user:create", Module = "用户管理", Description = "创建新用户", ParentId = 1 },
                new YingCaiAiModel.Permission { Id = 4, Name = "编辑用户", Code = "user:edit", Module = "用户管理", Description = "编辑用户信息", ParentId = 1 },
                
                // 系统设置模块
                new YingCaiAiModel.Permission { Id = 5, Name = "系统设置", Code = "system:manage", Module = "系统设置", Description = "系统设置相关功能", ParentId = null },
                new YingCaiAiModel.Permission { Id = 6, Name = "基础配置", Code = "system:basic", Module = "系统设置", Description = "系统基础配置", ParentId = 5 },
                new YingCaiAiModel.Permission { Id = 7, Name = "高级配置", Code = "system:advanced", Module = "系统设置", Description = "系统高级配置", ParentId = 5 },
                
                // 内容管理模块
                new YingCaiAiModel.Permission { Id = 8, Name = "内容管理", Code = "content:manage", Module = "内容管理", Description = "内容管理相关功能", ParentId = null },
                new YingCaiAiModel.Permission { Id = 9, Name = "文章管理", Code = "content:article", Module = "内容管理", Description = "文章管理", ParentId = 8 },
                new YingCaiAiModel.Permission { Id = 10, Name = "评论管理", Code = "content:comment", Module = "内容管理", Description = "评论管理", ParentId = 8 }
            };

            // 创建模拟角色数据
            _roles = new ObservableCollection<Role>
            {
                new Role 
                { 
                    Id = 1, 
                    Name = "超级管理员", 
                    Description = "拥有所有权限",
                    Permissions = permissions 
                },
                new Role 
                { 
                    Id = 2, 
                    Name = "内容编辑", 
                    Description = "管理内容相关功能",
                    Permissions = permissions.Where(p => p.Module == "内容管理").ToList()
                },
                new Role 
                { 
                    Id = 3, 
                    Name = "普通用户", 
                    Description = "基础功能权限",
                    Permissions = permissions.Where(p => p.Code.Contains("view")).ToList()
                }
            };

            // 初始化角色权限树
            RolePermissions = new ObservableCollection<YingCaiAiModel.Permission>();
            BuildPermissionTree(permissions);
        }

        private void BuildPermissionTree(List<YingCaiAiModel.Permission> allPermissions)
        {
            // 获取顶级权限
            var rootPermissions = allPermissions.Where(p => p.ParentId == null).ToList();
            
            foreach (var root in rootPermissions)
            {
                BuildPermissionTreeRecursive(root, allPermissions);
                RolePermissions.Add(root);
            }
        }

        private void BuildPermissionTreeRecursive(YingCaiAiModel.Permission parent, List<YingCaiAiModel.Permission> allPermissions)
        {
            var children = allPermissions.Where(p => p.ParentId == parent.Id).ToList();
            parent.Children = new ObservableCollection<YingCaiAiModel.Permission>(children);
            
            foreach (var child in children)
            {
                BuildPermissionTreeRecursive(child, allPermissions);
            }
        }

        private void LoadRolePermissions()
        {
            if (SelectedRole != null)
            {
                // 更新权限树的选中状态
                UpdatePermissionSelection(RolePermissions, SelectedRole.Permissions);
            }
        }

        private void UpdatePermissionSelection(IEnumerable<YingCaiAiModel.Permission> permissions, List<YingCaiAiModel.Permission> selectedPermissions)
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RoleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SelectedRole = e.AddedItems[0] as Role;
            }
        }

        private void PermissionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var permission = checkbox.DataContext as YingCaiAiModel.Permission;
            UpdateChildrenSelection(permission, true);
            UpdateParentSelection(permission);
        }

        private void PermissionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var permission = checkbox.DataContext as YingCaiAiModel.Permission;
            UpdateChildrenSelection(permission, false);
            UpdateParentSelection(permission);
        }

        private void UpdateChildrenSelection(YingCaiAiModel.Permission permission, bool isSelected)
        {
            permission.IsSelected = isSelected;
            foreach (var child in permission.Children)
            {
                UpdateChildrenSelection(child, isSelected);
            }
        }

        private void UpdateParentSelection(YingCaiAiModel.Permission permission)
        {
            // 获取父权限
            var parent = FindParentPermission(permission, RolePermissions);
            if (parent != null)
            {
                parent.IsSelected = parent.Children.All(p => p.IsSelected);
                UpdateParentSelection(parent);
            }
        }

        private YingCaiAiModel.Permission FindParentPermission(YingCaiAiModel.Permission child, IEnumerable<YingCaiAiModel.Permission> permissions)
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

        private void AddRoleButton_Click(object sender, RoutedEventArgs e)
        {
            // 这里可以实现添加角色的逻辑
            Growl.Info("添加角色功能待实现");
        }
    }
}