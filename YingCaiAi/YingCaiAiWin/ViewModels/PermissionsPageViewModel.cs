using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using YingCaiAiModel;


namespace YingCaiAiWin.ViewModels
{
    public class PermissionsPageViewModel : ObservableObject
    {
        // 权限数据集合
        private ObservableCollection<Permission> _permissions;
        
        // 当前编辑的权限
        private Permission _currentPermission;
        
        // 对话框可见性
        private Visibility _dialogVisibility = Visibility.Collapsed;
        
        // 对话框标题
        private string _dialogTitle;
        
        // 当前操作类型
        private OperationType _currentOperation;
        
        // 父级权限ID
        private int? _parentPermissionId;
        
        // 权限类型选项
        private ObservableCollection<string> _permissionTypes;
        
        // 模块选项
        private ObservableCollection<string> _modules;

        public ObservableCollection<Permission> Permissions
        {
            get => _permissions;
            set => SetProperty(ref _permissions, value);
        }

        public Permission CurrentPermission
        {
            get => _currentPermission;
            set => SetProperty(ref _currentPermission, value);
        }

        public Visibility DialogVisibility
        {
            get => _dialogVisibility;
            set => SetProperty(ref _dialogVisibility, value);
        }

        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        public OperationType CurrentOperation
        {
            get => _currentOperation;
            set => SetProperty(ref _currentOperation, value);
        }

        public int? ParentPermissionId
        {
            get => _parentPermissionId;
            set => SetProperty(ref _parentPermissionId, value);
        }

        public ObservableCollection<string> PermissionTypes
        {
            get => _permissionTypes;
            set => SetProperty(ref _permissionTypes, value);
        }

        public ObservableCollection<string> Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        // 命令
        public IRelayCommand AddPermissionCommand { get; }
        public IRelayCommand AddChildPermissionCommand { get; }
        public IRelayCommand EditPermissionCommand { get; }
        public IRelayCommand DeletePermissionCommand { get; }
        public IRelayCommand SavePermissionCommand { get; }
        public IRelayCommand CancelDialogCommand { get; }

        public PermissionsPageViewModel()
        {
            // 初始化命令
            AddPermissionCommand = new RelayCommand(AddPermission);
            AddChildPermissionCommand = new RelayCommand<Permission>(AddChildPermission);
            EditPermissionCommand = new RelayCommand<Permission>(EditPermission);
            DeletePermissionCommand = new RelayCommand<Permission>(DeletePermission);
            SavePermissionCommand = new RelayCommand(SavePermission);
            CancelDialogCommand = new RelayCommand(CancelDialog);

            // 初始化数据
            InitializeData();
        }

        /// <summary>
        /// 初始化权限数据
        /// </summary>
        public void InitializeData()
        {
            // 初始化权限类型选项
            PermissionTypes = new ObservableCollection<string>
            {
                "菜单",
                "按钮",
                "接口",
                "数据"
            };

            // 初始化模块选项
            Modules = new ObservableCollection<string>
            {
                "系统管理",
                "数据管理",
                "AI功能",
                "用户管理",
                "内容管理",
                "统计分析"
            };

            // 创建模拟数据
            Permissions = new ObservableCollection<Permission>
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
                                    Id = 111,
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
                                    Id = 112,
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
                                    Id = 113,
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
                                    Id = 114,
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
                            Id = 12,
                            Name = "角色管理",
                            Code = "system:role:manage",
                            Type = "菜单",
                            Module = "系统管理",
                            Sort = 2,
                            IsEnabled = true,
                            Description = "角色管理相关功能",
                            Children = new ObservableCollection<Permission>
                            {
                                new Permission
                                {
                                    Id = 121,
                                    Name = "查看角色",
                                    Code = "system:role:view",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 1,
                                    IsEnabled = true,
                                    Description = "查看角色信息权限"
                                },
                                new Permission
                                {
                                    Id = 122,
                                    Name = "新增角色",
                                    Code = "system:role:add",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 2,
                                    IsEnabled = true,
                                    Description = "新增角色权限"
                                },
                                new Permission
                                {
                                    Id = 123,
                                    Name = "编辑角色",
                                    Code = "system:role:edit",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 3,
                                    IsEnabled = true,
                                    Description = "编辑角色权限"
                                },
                                new Permission
                                {
                                    Id = 124,
                                    Name = "删除角色",
                                    Code = "system:role:delete",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 4,
                                    IsEnabled = true,
                                    Description = "删除角色权限"
                                }
                            }
                        },
                        new Permission
                        {
                            Id = 13,
                            Name = "权限管理",
                            Code = "system:permission:manage",
                            Type = "菜单",
                            Module = "系统管理",
                            Sort = 3,
                            IsEnabled = true,
                            Description = "权限管理相关功能",
                            Children = new ObservableCollection<Permission>
                            {
                                new Permission
                                {
                                    Id = 131,
                                    Name = "查看权限",
                                    Code = "system:permission:view",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 1,
                                    IsEnabled = true,
                                    Description = "查看权限信息"
                                },
                                new Permission
                                {
                                    Id = 132,
                                    Name = "新增权限",
                                    Code = "system:permission:add",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 2,
                                    IsEnabled = true,
                                    Description = "新增权限"
                                },
                                new Permission
                                {
                                    Id = 133,
                                    Name = "编辑权限",
                                    Code = "system:permission:edit",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 3,
                                    IsEnabled = true,
                                    Description = "编辑权限"
                                },
                                new Permission
                                {
                                    Id = 134,
                                    Name = "删除权限",
                                    Code = "system:permission:delete",
                                    Type = "按钮",
                                    Module = "系统管理",
                                    Sort = 4,
                                    IsEnabled = true,
                                    Description = "删除权限"
                                }
                            }
                        }
                    }
                },
                new Permission
                {
                    Id = 2,
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
                            Id = 21,
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
                            Id = 22,
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
                    Id = 3,
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
                            Id = 31,
                            Name = "AI助手",
                            Code = "ai:assistant",
                            Type = "菜单",
                            Module = "AI功能",
                            Sort = 1,
                            IsEnabled = true,
                            Description = "AI助手功能"
                        },
                        new Permission
                        {
                            Id = 32,
                            Name = "AI模型管理",
                            Code = "ai:model",
                            Type = "菜单",
                            Module = "AI功能",
                            Sort = 2,
                            IsEnabled = true,
                            Description = "AI模型管理"
                        },
                        new Permission
                        {
                            Id = 33,
                            Name = "AI训练",
                            Code = "ai:training",
                            Type = "菜单",
                            Module = "AI功能",
                            Sort = 3,
                            IsEnabled = true,
                            Description = "AI训练功能"
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 添加顶级权限
        /// </summary>
        private void AddPermission()
        {
            // 创建新权限
            CurrentPermission = new Permission
            {
                IsEnabled = true,
                Sort = 1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            // 设置对话框标题和操作类型
            DialogTitle = "添加权限";
            CurrentOperation = OperationType.Add;
            ParentPermissionId = null;

            // 显示对话框
            DialogVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 根据ID查找权限
        /// </summary>
        public Permission FindPermissionById(ObservableCollection<Permission> permissions, int id)
        {
            foreach (var permission in permissions)
            {
                if (permission.Id == id)
                {
                    return permission;
                }

                if (permission.Children != null && permission.Children.Count > 0)
                {
                    var childPermission = FindPermissionById(permission.Children, id);
                    if (childPermission != null)
                    {
                        return childPermission;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据搜索文本筛选权限
        /// </summary>
        public ObservableCollection<Permission> SearchPermissions(ObservableCollection<Permission> permissions, string keyword)
        {
            var result = new ObservableCollection<Permission>();

            foreach (var permission in permissions)
            {
                // 创建权限副本
                var permissionCopy = new Permission
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Code = permission.Code,
                    Type = permission.Type,
                    Module = permission.Module,
                    Sort = permission.Sort,
                    IsEnabled = permission.IsEnabled,
                    Description = permission.Description,
                    CreateTime = permission.CreateTime,
                    UpdateTime = permission.UpdateTime
                };

                // 检查当前权限是否匹配
                bool currentMatches = permission.Name.ToLower().Contains(keyword) ||
                                     permission.Code.ToLower().Contains(keyword);

                // 筛选子权限
                if (permission.Children != null && permission.Children.Count > 0)
                {
                    permissionCopy.Children = SearchPermissions(permission.Children, keyword);
                }

                // 如果当前权限匹配或有匹配的子权限，则添加到结果中
                if (currentMatches || (permissionCopy.Children != null && permissionCopy.Children.Count > 0))
                {
                    result.Add(permissionCopy);
                }
            }

            return result;
        }

        /// <summary>
        /// 根据模块筛选权限
        /// </summary>
        public ObservableCollection<Permission> FilterPermissionsByModule(ObservableCollection<Permission> permissions, string module)
        {
            var result = new ObservableCollection<Permission>();

            foreach (var permission in permissions)
            {
                // 创建权限副本
                var permissionCopy = new Permission
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Code = permission.Code,
                    Type = permission.Type,
                    Module = permission.Module,
                    Sort = permission.Sort,
                    IsEnabled = permission.IsEnabled,
                    Description = permission.Description,
                    CreateTime = permission.CreateTime,
                    UpdateTime = permission.UpdateTime
                };

                // 检查当前权限是否匹配
                bool currentMatches = permission.Module == module;

                // 筛选子权限
                if (permission.Children != null && permission.Children.Count > 0)
                {
                    permissionCopy.Children = FilterPermissionsByModule(permission.Children, module);
                }

                // 如果当前权限匹配或有匹配的子权限，则添加到结果中
                if (currentMatches || (permissionCopy.Children != null && permissionCopy.Children.Count > 0))
                {
                    result.Add(permissionCopy);
                }
            }

            return result;
        }


        /// <summary>
        /// 添加子权限
        /// </summary>
        private void AddChildPermission(Permission parentPermission)
        {
            if (parentPermission == null) return;

            // 创建新权限
            CurrentPermission = new Permission
            {
                Module = parentPermission.Module,
                IsEnabled = true,
                Sort = 1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            // 设置对话框标题和操作类型
            DialogTitle = $"添加 {parentPermission.Name} 的子权限";
            CurrentOperation = OperationType.AddChild;
            ParentPermissionId = parentPermission.Id;

            // 显示对话框
            DialogVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 编辑权限
        /// </summary>
        private void EditPermission(Permission permission)
        {
            if (permission == null) return;

            // 创建权限副本进行编辑
            CurrentPermission = new Permission
            {
                Id = permission.Id,
                Name = permission.Name,
                Code = permission.Code,
                Type = permission.Type,
                Module = permission.Module,
                Sort = permission.Sort,
                IsEnabled = permission.IsEnabled,
                Description = permission.Description,
                CreateTime = permission.CreateTime,
                UpdateTime = DateTime.Now
            };

            // 设置对话框标题和操作类型
            DialogTitle = "编辑权限";
            CurrentOperation = OperationType.Edit;

            // 显示对话框
            DialogVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        private void DeletePermission(Permission permission)
        {
            if (permission == null) return;

            // 确认删除
            var result = HandyControl.Controls.MessageBox.Show($"确定要删除权限 [{permission.Name}] 吗？\n删除后将无法恢复！", "删除确认", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.OK)
            {
                // 查找并删除权限
                RemovePermissionFromCollection(Permissions, permission.Id);
            }
        }

        /// <summary>
        /// 从集合中递归删除权限
        /// </summary>
        private bool RemovePermissionFromCollection(ObservableCollection<Permission> permissions, int permissionId)
        {
            for (int i = 0; i < permissions.Count; i++)
            {
                if (permissions[i].Id == permissionId)
                {
                    permissions.RemoveAt(i);
                    return true;
                }

                if (permissions[i].Children != null && permissions[i].Children.Count > 0)
                {
                    if (RemovePermissionFromCollection(permissions[i].Children, permissionId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        private void SavePermission()
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(CurrentPermission.Name))
            {
                HandyControl.Controls.MessageBox.Show("权限名称不能为空！", "验证错误");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentPermission.Code))
            {
                HandyControl.Controls.MessageBox.Show("权限编码不能为空！", "验证错误");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentPermission.Type))
            {
                HandyControl.Controls.MessageBox.Show("权限类型不能为空！", "验证错误");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentPermission.Module))
            {
                HandyControl.Controls.MessageBox.Show("所属模块不能为空！", "验证错误");
                return;
            }

            // 根据操作类型执行不同的保存逻辑
            switch (CurrentOperation)
            {
                case OperationType.Add:
                    // 添加顶级权限
                    AddPermissionToCollection(Permissions, CurrentPermission);
                    break;
                case OperationType.AddChild:
                    // 添加子权限
                    AddChildPermissionToCollection(Permissions, ParentPermissionId.Value, CurrentPermission);
                    break;
                case OperationType.Edit:
                    // 编辑权限
                    UpdatePermissionInCollection(Permissions, CurrentPermission);
                    break;
            }

            // 关闭对话框
            DialogVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 添加顶级权限到集合
        /// </summary>
        private void AddPermissionToCollection(ObservableCollection<Permission> permissions, Permission permission)
        {
            // 生成新ID
            int maxId = GetMaxPermissionId(permissions);
            permission.Id = maxId + 1;
            
            // 初始化子权限集合
            permission.Children = new ObservableCollection<Permission>();
            
            // 添加到集合
            permissions.Add(permission);
        }

        /// <summary>
        /// 添加子权限到集合
        /// </summary>
        private bool AddChildPermissionToCollection(ObservableCollection<Permission> permissions, int parentId, Permission childPermission)
        {
            foreach (var permission in permissions)
            {
                if (permission.Id == parentId)
                {
                    // 找到父权限，添加子权限
                    if (permission.Children == null)
                    {
                        permission.Children = new ObservableCollection<Permission>();
                    }

                    // 生成新ID
                    int maxId = GetMaxPermissionId(Permissions);
                    childPermission.Id = maxId + 1;
                    
                    // 添加到子权限集合
                    permission.Children.Add(childPermission);
                    return true;
                }

                if (permission.Children != null && permission.Children.Count > 0)
                {
                    if (AddChildPermissionToCollection(permission.Children, parentId, childPermission))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 更新权限信息
        /// </summary>
        private bool UpdatePermissionInCollection(ObservableCollection<Permission> permissions, Permission updatedPermission)
        {
            for (int i = 0; i < permissions.Count; i++)
            {
                if (permissions[i].Id == updatedPermission.Id)
                {
                    // 保留原有的子权限
                    updatedPermission.Children = permissions[i].Children;
                    
                    // 更新权限信息
                    permissions[i] = updatedPermission;
                    return true;
                }

                if (permissions[i].Children != null && permissions[i].Children.Count > 0)
                {
                    if (UpdatePermissionInCollection(permissions[i].Children, updatedPermission))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取权限集合中的最大ID
        /// </summary>
        private int GetMaxPermissionId(ObservableCollection<Permission> permissions)
        {
            int maxId = 0;

            foreach (var permission in permissions)
            {
                maxId = Math.Max(maxId, permission.Id);

                if (permission.Children != null && permission.Children.Count > 0)
                {
                    int childMaxId = GetMaxPermissionId(permission.Children);
                    maxId = Math.Max(maxId, childMaxId);
                }
            }

            return maxId;
        }

        /// <summary>
        /// 取消对话框
        /// </summary>
        private void CancelDialog()
        {
            DialogVisibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 操作类型枚举
    /// </summary>
    public enum OperationType
    {
        Add,        // 添加
        AddChild,   // 添加子项
        Edit        // 编辑
    }
}
