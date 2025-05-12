using HandyControl.Controls;
using HandyControl.Data;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiWin.ViewModels
{
    public partial class UsersPageViewModel: ViewModel
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
        public Users _userSer=new Users ();

        [ObservableProperty]
        public Users _userEdit = new Users();

        private readonly IContentDialogService _contentDialogService;
        [ObservableProperty]
        private string _dialogResultText = string.Empty;
        public  UsersPageViewModel(INavigationService navigationService,  IUsersService usersService, IContentDialogService contentDialogService) {

            if (!_isInitialized)
            {
                _contentDialogService= contentDialogService;
                  _usersService = usersService;
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
            
            UserEdit.PasswordHash = "";
            UserEdit.IsActiveInt = UserEdit.IsActive ? 0 : 1;
            UserEdit.Role = (UserEdit.Role - 1)??0;
            var dialog = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "编辑用户信息",
                Content = new Views.Pages.EditUserControl
                {
                   
                    DataContext = UserEdit
                },
                PrimaryButtonText = "保存",
                //SecondaryButtonText = "取消",
                CloseButtonText = "关闭",
            });

            if (dialog == ContentDialogResult.Primary)
            {
                var editVM = UserEdit;
                if (editVM != null) {
                    var flag = false;
                    var userControl = new Views.Pages.EditUserControl();
                    userControl.PasswordChanged += (pwd) =>
                    {
                        UserEdit.PasswordHash = pwd;
                    };
                    await Task.Run(() =>
                    {
                        
                        editVM.Role = editVM.Role + 1;
                        editVM.RoleName = editVM.Role == 1 ? "管理员" : editVM.Role == 2 ? "员工" : "演示";
                        editVM.IsActive = editVM.IsActiveInt == 0;
                       
                        editVM.PasswordHash =new  Helpers.FileHelper().ToMD5(UserEdit.PasswordHash);
                        if (editVM.Id > 0)
                        {

                            flag = _usersService.UpdateUserAsync(editVM).Status;
                        }
                        else
                        {
                            editVM.CreatedAt = DateTime.Now;
                            flag = _usersService.AddUserAsync(editVM).Status;
                        }

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
