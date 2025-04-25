using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Data;
using YingCaiAiModel;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// 权限管理页面
    /// </summary>
    public partial class PermissionsPage :Page
    {
        // 权限数据集合
        private ObservableCollection<Permission> _permissions;

        // 当前选中的权限ID
        private int _currentPermissionId;

        // 当前操作类型（新增/编辑）
        private OperationType _currentOperation;

        // 父级权限ID（用于添加子权限）
        private int? _parentPermissionId;

        public PermissionsPage()
        {
            InitializeComponent();

            // 初始化数据
            InitializeData();

            // 隐藏对话框
            PermissionDialog.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 初始化权限数据
        /// </summary>
        private void InitializeData()
        {
            // 创建模拟数据
            _permissions = new ObservableCollection<Permission>
            {
                new Permission
                {
                    Id = 1,
                    Name = "系统管理",
                    Code = "system:manage",
                    Type = "菜单",
                    Module = "系统管理",
                    Sort = 1,
                    IsEnabled = true,
                    Description = "系统管理相关功能",
                    Children = new ObservableCollection<Permission>
                    {
                        new Permission
                        {
                            Id = 11,
                            Name = "用户管理",
                            Code = "system:user:manage",
                            Type = "菜单",
                            Module = "系统管理",
                            Sort = 1,
                            IsEnabled = true,
                            Description = "用户管理相关功能",
                            Children = new ObservableCollection<Permission>
                            {
                                new Permission
                                {
                                    Id = 23,
                                    Name = "查看用户",
                                    Code = "system:user:view",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 1,
                                    IsEnabled = true,
                                    Description = "查看用户信息权限"
                                },
                                new Permission
                                {
                                    Id = 23,
                                    Name = "新增用户",
                                    Code = "system:user:add",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 2,
                                    IsEnabled = true,
                                    Description = "新增用户权限"
                                },
                                new Permission
                                {
                                    Id = 222,
                                    Name = "编辑用户",
                                    Code = "system:user:edit",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 3,
                                    IsEnabled = true,
                                    Description = "编辑用户权限"
                                },
                                new Permission
                                {
                                    Id = 333,
                                    Name = "删除用户",
                                    Code = "system:user:delete",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 4,
                                    IsEnabled = true,
                                    Description = "删除用户权限"
                                }
                            }
                        },
                        new Permission
                        {
                            Id = 222,
                            Name = "角色管理",
                            Code = "system:role:manage",
                            Type = "菜单",
                            Module = "系统管理",
                            Sort = 2,
                            IsEnabled = true,
                            Description = "角色管理相关功能"
                        },
                        new Permission
                        {
                            Id = 666,
                            Name = "权限管理",
                            Code = "system:permission:manage",
                            Type = "菜单",
                            Module = "系统管理",
                            Sort = 3,
                            IsEnabled = true,
                            Description = "权限管理相关功能"
                        }
                    }
                },
                new Permission
                {
                    Id = 77,
                    Name = "数据管理",
                    Code = "data:manage",
                    Type = "菜单",
                    Module = "数据管理",
                    Sort = 2,
                    IsEnabled = true,
                    Description = "数据管理相关功能",
                    Children = new ObservableCollection<Permission>
                    {
                        new Permission
                        {
                            Id = 888,
                            Name = "数据字典",
                            Code = "data:dict:manage",
                            Type = "菜单",
                            Module = "数据管理",
                            Sort = 1,
                            IsEnabled = true,
                            Description = "数据字典管理"
                        },
                        new Permission
                        {
                            Id = 44,
                            Name = "数据备份",
                            Code = "data:backup:manage",
                            Type = "菜单",
                            Module = "数据管理",
                            Sort = 2,
                            IsEnabled = true,
                            Description = "数据备份管理"
                        }
                    }
                },
                new Permission
                {
                    Id = 888,
                    Name = "AI功能",
                    Code = "ai:manage",
                    Type = "菜单",
                    Module = "AI功能",
                    Sort = 3,
                    IsEnabled = true,
                    Description = "AI相关功能",
                    Children = new ObservableCollection<Permission>
                    {
                        new Permission
                        {
                            Id = 88,
                            Name = "模型管理",
                            Code = "ai:model:manage",
                            Type = "菜单",
                            Module = "AI功能",
                            Sort = 1,
                            IsEnabled = true,
                            Description = "AI模型管理"
                        },
                        new Permission
                        {
                            Id = 88,
                            Name = "训练管理",
                            Code = "ai:training:manage",
                            Type = "菜单",
                            Module = "AI功能",
                            Sort = 2,
                            IsEnabled = true,
                            Description = "AI训练管理"
                        }
                    }
                }
            };

            // 绑定数据到TreeView
            PermissionsTreeView.ItemsSource = _permissions;
        }

        /// <summary>
        /// 新增权限按钮点击事件
        /// </summary>
        private void AddPermissionButton_Click(object sender, RoutedEventArgs e)
        {
            // 设置当前操作为新增
            _currentOperation = OperationType.Add;
            _parentPermissionId = null;

            // 清空表单
            ClearPermissionForm();

            // 隐藏父级权限输入框
            ParentPermissionTextBox.Visibility = Visibility.Collapsed;

            // 设置对话框标题
            PermissionDialog.SetValue(HandyControl.Controls.TitleElement.TitleProperty, "新增权限");

            // 显示对话框
            PermissionDialog.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 添加子权限按钮点击事件
        /// </summary>
        private void AddChildPermissionButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取父级权限ID
            Button button = sender as Button;
            _parentPermissionId = Convert.ToInt32(button.Tag);

            // 设置当前操作为新增子级
            _currentOperation = OperationType.AddChild;

            // 清空表单
            ClearPermissionForm();

            // 查找并显示父级权限名称
            Permission parentPermission = FindPermissionById(_parentPermissionId);
            if (parentPermission != null)
            {
                ParentPermissionTextBox.Text = parentPermission.Name;
                ParentPermissionTextBox.Visibility = Visibility.Visible;
            }

            // 设置对话框标题
            PermissionDialog.SetValue(HandyControl.Controls.TitleElement.TitleProperty, "新增子权限");

            // 显示对话框
            PermissionDialog.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 编辑权限按钮点击事件
        /// </summary>
        private void EditPermissionButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前权限ID
            Button button = sender as Button;
            _currentPermissionId =Convert.ToInt32(button.Tag);

            // 设置当前操作为编辑
            _currentOperation = OperationType.Edit;

            // 查找权限
            Permission permission = FindPermissionById(_currentPermissionId);
            if (permission != null)
            {
                // 填充表单
                FillPermissionForm(permission);

                // 查找并显示父级权限名称
                Permission parentPermission = FindParentPermission(permission);
                if (parentPermission != null)
                {
                    ParentPermissionTextBox.Text = parentPermission.Name;
                    ParentPermissionTextBox.Visibility = Visibility.Visible;
                }
                else
                {
                    ParentPermissionTextBox.Visibility = Visibility.Collapsed;
                }

                // 设置对话框标题
                PermissionDialog.SetValue(HandyControl.Controls.TitleElement.TitleProperty, "编辑权限");

                // 显示对话框
                PermissionDialog.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 删除权限按钮点击事件
        /// </summary>
        private void DeletePermissionButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前权限ID
            Button button = sender as Button;
            int permissionId =Convert.ToInt32(button.Tag);

            // 确认删除
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("确定要删除该权限吗？删除后将无法恢复，且会同时删除其下所有子权限。", "删除确认", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                // 执行删除
                DeletePermission(permissionId);

                // 刷新TreeView
                PermissionsTreeView.ItemsSource = null;
                PermissionsTreeView.ItemsSource = _permissions;

                // 提示成功
                Growl.Success("删除成功！");
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

            // 创建权限对象
            Permission permission = new Permission
            {
                Name = PermissionNameTextBox.Text.Trim(),
                Code = PermissionCodeTextBox.Text.Trim(),
                Type = (PermissionTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString(),
                Module = (PermissionModuleComboBox.SelectedItem as ComboBoxItem).Content.ToString(),
                IsEnabled = PermissionStatusToggle.IsChecked ?? true,
                Sort = (int)PermissionSortNumeric.Value,
                Description = PermissionDescriptionTextBox.Text.Trim(),
                Children = new ObservableCollection<Permission>()
            };

            // 根据操作类型执行不同的操作
            switch (_currentOperation)
            {
                case OperationType.Add:
                    // 生成新ID
                    permission.Id = (_permissions.Count + 1);

                    // 添加到集合
                    _permissions.Add(permission);
                    break;

                case OperationType.AddChild:
                    // 查找父级权限
                    Permission parentPermission = FindPermissionById(_parentPermissionId);
                    if (parentPermission != null)
                    {
                        // 生成新ID
                        //permission.Id = ((int)_parentPermissionId)-parentPermission.Children.Count + 1;

                        // 添加到父级的子集合
                        parentPermission.Children.Add(permission);
                    }
                    break;

                case OperationType.Edit:
                    // 查找当前权限
                    Permission currentPermission = FindPermissionById(_currentPermissionId);
                    if (currentPermission != null)
                    {
                        // 保留ID和子集合
                        permission.Id = currentPermission.Id;
                        permission.Children = currentPermission.Children;

                        // 更新权限
                        UpdatePermission(currentPermission, permission);
                    }
                    break;
            }

            // 刷新TreeView
            PermissionsTreeView.ItemsSource = null;
            PermissionsTreeView.ItemsSource = _permissions;

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
                PermissionsTreeView.ItemsSource = _permissions;
                return;
            }

            // 搜索权限
            var result = SearchPermissions(_permissions, keyword);

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
                //PermissionsTreeView.ItemsSource = _permissions;
                return;
            }

            // 筛选权限
            var result = FilterPermissionsByModule(_permissions, module);

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

            // TODO: 根据页码加载数据
        }

        /// <summary>
        /// 刷新按钮点击事件
        /// </summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // 重新加载数据
            InitializeData();

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
        /// 清空权限表单
        /// </summary>
        private void ClearPermissionForm()
        {
            ParentPermissionTextBox.Text = string.Empty;
            PermissionNameTextBox.Text = string.Empty;
            PermissionCodeTextBox.Text = string.Empty;
            PermissionTypeComboBox.SelectedIndex = 0;
            PermissionModuleComboBox.SelectedIndex = 0;
            PermissionStatusToggle.IsChecked = true;
            PermissionSortNumeric.Value = 0;
            PermissionDescriptionTextBox.Text = string.Empty;
        }

        /// <summary>
        /// 填充权限表单
        /// </summary>
        private void FillPermissionForm(Permission permission)
        {
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

            PermissionStatusToggle.IsChecked = permission.IsEnabled;
            PermissionSortNumeric.Value = permission.Sort;
            PermissionDescriptionTextBox.Text = permission.Description;
        }

        /// <summary>
        /// 根据ID查找权限
        /// </summary>
        private Permission FindPermissionById(int? id)
        {
            return FindPermissionById(_permissions, id);
        }

        /// <summary>
        /// 递归查找权限
        /// </summary>
        private Permission FindPermissionById(IEnumerable<Permission> permissions, int? id)
        {
            foreach (var permission in permissions)
            {
                if (permission.Id == id)
                {
                    return permission;
                }

                if (permission.Children != null && permission.Children.Count > 0)
                {
                    var result = FindPermissionById(permission.Children, id);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 查找父级权限
        /// </summary>
        private Permission FindParentPermission(Permission childPermission)
        {
            return FindParentPermission(_permissions, childPermission);
        }

        /// <summary>
        /// 递归查找父级权限
        /// </summary>
        private Permission FindParentPermission(IEnumerable<Permission> permissions, Permission childPermission)
        {
            foreach (var permission in permissions)
            {
                if (permission.Children != null && permission.Children.Contains(childPermission))
                {
                    return permission;
                }

                if (permission.Children != null && permission.Children.Count > 0)
                {
                    var result = FindParentPermission(permission.Children, childPermission);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        private void UpdatePermission(Permission oldPermission, Permission newPermission)
        {
            oldPermission.Name = newPermission.Name;
            oldPermission.Code = newPermission.Code;
            oldPermission.Type = newPermission.Type;
            oldPermission.Module = newPermission.Module;
            oldPermission.IsEnabled = newPermission.IsEnabled;
            oldPermission.Sort = newPermission.Sort;
            oldPermission.Description = newPermission.Description;
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        private void DeletePermission(int id)
        {
            // 尝试从顶级集合中删除
            var permission = _permissions.FirstOrDefault(p => p.Id == id);
            if (permission != null)
            {
                _permissions.Remove(permission);
                return;
            }

            // 递归查找并删除
            foreach (var p in _permissions)
            {
                if (DeletePermissionFromChildren(p.Children, id))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 从子集合中删除权限
        /// </summary>
        private bool DeletePermissionFromChildren(ObservableCollection<Permission> children, int id)
        {
            var permission = children.FirstOrDefault(p => p.Id == id);
            if (permission != null)
            {
                children.Remove(permission);
                return true;
            }

            foreach (var child in children)
            {
                if (child.Children != null && child.Children.Count > 0)
                {
                    if (DeletePermissionFromChildren(child.Children, id))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 搜索权限
        /// </summary>
        private ObservableCollection<Permission> SearchPermissions(IEnumerable<Permission> permissions, string keyword)
        {
            var result = new ObservableCollection<Permission>();

            foreach (var permission in permissions)
            {
                // 创建新的权限对象，避免修改原始数据
                var newPermission = new Permission
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Code = permission.Code,
                    Type = permission.Type,
                    Module = permission.Module,
                    IsEnabled = permission.IsEnabled,
                    Sort = permission.Sort,
                    Description = permission.Description,
                    Children = new ObservableCollection<Permission>()
                };

                // 搜索子权限
                if (permission.Children != null && permission.Children.Count > 0)
                {
                    var children = SearchPermissions(permission.Children, keyword);
                    foreach (var child in children)
                    {
                        newPermission.Children.Add(child);
                    }
                }

                // 如果当前权限匹配关键字或者有匹配的子权限，则添加到结果中
                if (permission.Name.ToLower().Contains(keyword) ||
                    permission.Code.ToLower().Contains(keyword) ||
                    newPermission.Children.Count > 0)
                {
                    result.Add(newPermission);
                }
            }

            return result;
        }

        /// <summary>
        /// 按模块筛选权限
        /// </summary>
        private ObservableCollection<Permission> FilterPermissionsByModule(IEnumerable<Permission> permissions, string module)
        {
            var result = new ObservableCollection<Permission>();

            foreach (var permission in permissions)
            {
                // 如果当前权限属于指定模块
                if (permission.Module == module)
                {
                    result.Add(permission);
                    continue;
                }

                // 创建新的权限对象，避免修改原始数据
                var newPermission = new Permission
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Code = permission.Code,
                    Type = permission.Type,
                    Module = permission.Module,
                    IsEnabled = permission.IsEnabled,
                    Sort = permission.Sort,
                    Description = permission.Description,
                    Children = new ObservableCollection<Permission>()
                };

                // 筛选子权限
                if (permission.Children != null && permission.Children.Count > 0)
                {
                    var children = FilterPermissionsByModule(permission.Children, module);
                    foreach (var child in children)
                    {
                        newPermission.Children.Add(child);
                    }
                }

                // 如果有匹配的子权限，则添加到结果中
                if (newPermission.Children.Count > 0)
                {
                    result.Add(newPermission);
                }
            }

            return result;
        }

        #endregion
    }  
}