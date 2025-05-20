using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// TermsOfUseContentDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditUserControl : ContentDialog
    {
        
        public Users UserEdit { get; set; }
        private List<Role> Roles { get; set; }

        private IRolesService _rolesService;
        private IUsersService _usersService;
        private static readonly Regex _validRegex = new Regex("^[a-zA-Z0-9]+$");
        public EditUserControl(ContentPresenter? contentPresenter , Users users,IRolesService rolesService,IUsersService usersService)
            : base(contentPresenter)
        {
            InitializeComponent();
            _rolesService=rolesService;
            _usersService=usersService;
            Title = "编辑用户信息";
            this.PrimaryButtonText= "保存";
            CloseButtonText = "关闭";
            UserEdit = users;
            DataContext = this;
            Load();
        }

        private async void Load()
        {
            UserName.Text= UserEdit.UserName;
            Roles = (await _rolesService.GetRoleAsync()).Data as List<Role> ?? new List<Role>();
            if (Roles.Count > 0)
            {
                RoleCB.ItemsSource = Roles;
                RoleCB.SelectedItem = Roles.FirstOrDefault(m=>m.Id==(UserEdit.Role??m.Id));
            }
            
            IsActiveCB.SelectedIndex=UserEdit.IsActive?0: 1;
        }

        protected async override void OnButtonClick(ContentDialogButton button)
        {
            if(button== ContentDialogButton.Close)
            {
                base.OnButtonClick(button);
                return;
            }

            if (string.IsNullOrWhiteSpace(UserName.Text.Trim()))
            {
                ErrorInfoBar.Message = "用户名不能为空！请重新输入!";
                ErrorInfoBar.IsOpen = true;
               
                return;
            }
            var f = !_validRegex.IsMatch(UserName.Text.Trim());
            if (f)
            {
                ErrorInfoBar.Message = "只能输入数字和英文!";
                ErrorInfoBar.IsOpen = true;
                return;
            }
            if (UserEdit.Id == null || UserEdit.Id < 1)
            {
                var  temp=await _usersService.GetUserByNameAsync(UserName.Text.Trim());
                if (temp != null&&temp.Id>0) {
                    ErrorInfoBar.Message = "已存在相同用户名！";
                    ErrorInfoBar.IsOpen = true;
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(PasswordBoxControl.Password))
            {
                UserEdit.PasswordHash = new Helpers.FileHelper().ToMD5(PasswordBoxControl.Password);
            }
            UserEdit.UserName = UserName.Text.Trim();
            UserEdit.Role = Roles[RoleCB.SelectedIndex].Id;
            UserEdit.RoleName= Roles[RoleCB.SelectedIndex].Name;
            UserEdit.IsActive=IsActiveCB.SelectedIndex==0;
            await Task.Run(() =>
            {
                var flag = false;
                if (UserEdit.Id > 0)
                {

                    flag = _usersService.UpdateUserAsync(UserEdit).Status;
                }
                else
                {
                    UserEdit.CreatedAt = DateTime.Now;
                    flag = _usersService.AddUserAsync(UserEdit).Status;
                }

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


            base.OnButtonClick(button);
            return;
        }

   

        private void PasswordBoxControl_PasswordChanged(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
