using HandyControl.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiWin.ViewModels
{
    public partial class RolePageViewModel : ViewModel
    {
        [ObservableProperty]
        private string _currentPageTitle = "角色管理";

        [ObservableProperty]
        private List<Role> _roles = [];

        [ObservableProperty]
        private List<Permission> _rolePermissions = [];

        [ObservableProperty]
         private Role _selectedRole  = new Role() ;

  

        [ObservableProperty]
        private Role _addRole = new Role();

        [ObservableProperty]
        private bool _isInitialized;

        private readonly IRolesService _rolesService;

        private readonly IContentDialogService _contentDialogService;

    
        public RolePageViewModel(INavigationService navigationService, IRolesService rolesService, IContentDialogService contentDialogService)
        {

            if (!_isInitialized)
            {
                _rolesService = rolesService;
                _contentDialogService = contentDialogService;
                InitializeViewModel();
            }

        }
        public void InitializeViewModel()
        {
            // 初始化数据逻辑
            LoadSampleData();
            _isInitialized = true;

        }

        private void LoadSampleData()
        {

            Roles.Clear();
            RolePermissions.Clear();
            Task.Run(() =>
            {
                Roles = _rolesService.GetRoleAsync().Data as List<Role> ?? new List<Role>();
                //SelectedRole = Roles.FirstOrDefault();

                var data = _rolesService.GetPermAsync(0).Data as List<Permission> ?? new List<Permission>();
                var temp = new List<Permission>();
                foreach (var item in data.FindAll(m => m.ParentId == 0))
                {
                    item.Children = data.FindAll(x => x.ParentId == item.Id);
                    temp.Add(item);
                }
                RolePermissions = temp;
            });

        }


        // 添加与XAML中控件绑定的属性和命令
        [RelayCommand]
        private  async Task OnAddRole()
        {
            var dialog = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "新增角色",
                Content = new Views.Pages.AddRole
                {

                    DataContext = AddRole
                },
                PrimaryButtonText = "保存",
                //SecondaryButtonText = "取消",
                CloseButtonText = "关闭",
            });

            if (dialog == ContentDialogResult.Primary)
            {

                await Task.Run(() =>
                {
                    var flag = _rolesService.AddRoleAsync(AddRole).Status;
                    if (flag)
                    {
                        Growl.Success("操作成功");
                        LoadSampleData();
                    }
                    else
                    {
                        Growl.Error("操作失败！");
                    }
                });

                await Task.Delay(2000);
                Growl.Clear();
            }

        }

        /// <summary>
        /// 保存权限配置
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task OnAddPer()
        {
            var per = RolePermissions;

            await Task.Run(() =>
            {
                var pids = string.Join(",", per.Where(m => m.IsSelected == true).Select(x => x.Id));
                per.ForEach((item) => {
                    pids += string.Join(",", item.Children.Where(m => m.IsSelected == true).Select(x => x.Id));
                });
                SelectedRole.PerIds = pids;
                var flag = _rolesService.AddRoleAsync(SelectedRole).Status;
                if (flag)
                {
                    Growl.Success("操作成功");
                }
                else
                {
                    Growl.Error("操作失败！");
                }
            });

            await Task.Delay(2000);
            Growl.Clear();

        }


        /// <summary>
        /// 选择角色
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OnSelectedRoleChanged(int  parameter)
        {
                SelectedRole = Roles.Find(m => m.Id == parameter);
                var ids = SelectedRole.PerIds;
                var temp = new List<Permission>();
                foreach (var item in RolePermissions)
                {
                    if (ids != null&& ids.Contains(item.Id.ToString()))
                    {
                        item.IsSelected = true;
                    }
                    else
                    {
                        item.IsSelected = false;
                    }

                    item.Children.ForEach((m) => { if (ids != null && ids.Contains(m.Id.ToString())) { m.IsSelected = true; } else { m.IsSelected = false; } });
                    temp.Add(item);
                }
                RolePermissions.Clear();
                RolePermissions = temp;
           
            
        }

        [RelayCommand]
        private void OnShuaXin()
        {
            LoadSampleData();
        }
    }
}
