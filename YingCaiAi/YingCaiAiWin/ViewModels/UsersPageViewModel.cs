using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using EnumsNET;
using HandyControl.Controls;
using HandyControl.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.Views.Pages;

namespace YingCaiAiWin.ViewModels
{
    public partial class UsersPageViewModel : ViewModel
    {


        private bool _isInitialized = false;

        [ObservableProperty]
        private List<Users> _usersList = [];
        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageCount = 1;

        [ObservableProperty]
        private int _intId = 0;

        public IUsersService _usersService { get; set; }

        [ObservableProperty]
        public Users _userSer = new Users();

        [ObservableProperty]
        public Users _userEdit = new Users();

        private readonly IContentDialogService _contentDialogService;
        [ObservableProperty]
        private string _dialogResultText = string.Empty;

        [ObservableProperty]
        private List<Role> _roleSerList = new List<Role>();


        private readonly IRolesService _rolesService;

        public  UsersPageViewModel(INavigationService navigationService,  IUsersService usersService,IRolesService rolesService, IContentDialogService contentDialogService) {

            if (!_isInitialized)
            {
                _contentDialogService= contentDialogService;
                  _usersService = usersService;
                _rolesService= rolesService;
                InitializeViewModel();

            }

        }
        public async void InitializeViewModel()
        {
          
                
                // 初始化数据逻辑
             LoadSampleData();

            var flag = await _rolesService.GetRoleAsync();
            var  roles = new List<Role>() { new Role() { Name = "全部", Id = 0 } };
            roles.AddRange(flag.Data as List<Role> ?? new List<Role>());

            RoleSerList = roles;
            _isInitialized = true;
        }


        private  void LoadSampleData()
        {

            UsersList.Clear();

            Task.Run(() =>
            {
                
                var data = _usersService.GetAllPageAsync(_currentPage, _userSer);
                UsersList = data.Data as List<Users>;
                PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToInt32(data.Message) / 20f));

            });

            

        }

     

        [RelayCommand]
        private void OnSerach(string parameter)
        {
            _currentPage = 1;
            LoadSampleData();
        }


        [RelayCommand]
        private void OnDelete(int parameter)
        {

            Growl.Ask("是否确定删除", isConfirmed =>
            {
                if (isConfirmed)
                {
                    Growl.Clear();
                    Task.Run(() => {
                        var flag = _usersService.DeleteUserAsync(parameter);
                        LoadSampleData();
                        if (flag.Status)
                        {
                            Growl.Success("删除成功！");
                        }

                        else
                        {
                            Growl.Error("删除失败！");
                            Thread.Sleep(2500);
                            Growl.Clear();
                        }

                    });
                }
                else
                {

                    Growl.Clear();
                }
                return true;
            });


        }

        [RelayCommand]
        public void OnEditUser(int parameter)
        {

        }


        [RelayCommand]
        public async Task OnShowUserDialog(int parameter)
        {
            

            if (parameter != 0)
            {
                UserEdit = await _usersService.GetUserByIdAsync(parameter) ?? new Users();
            }
            else
            {
                UserEdit = new Users();
            }
            var termsOfUseContentDialog = new EditUserControl(_contentDialogService.GetDialogHost(),UserEdit, _rolesService,_usersService);
            var result= await termsOfUseContentDialog.ShowAsync();
            LoadSampleData();
            return;
        }

        /// <summary>
        ///     页码改变命令
        /// </summary>
        public RelayCommand<FunctionEventArgs<int>> PageUpdatedCmd => new(PageUpdated);

        /// <summary>
        ///     页码改变
        /// </summary>
        private void PageUpdated(FunctionEventArgs<int> info)
        {
            _currentPage = info.Info;
            LoadSampleData();
        }
    }
}
