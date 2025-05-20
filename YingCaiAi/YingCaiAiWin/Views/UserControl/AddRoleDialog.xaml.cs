using HandyControl.Controls;
using System;
using System.Collections.Generic;
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

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// AddRoleDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddRoleDialog : ContentDialog
    {

        private IRolesService _rolesService;
        private static readonly Regex _validRegex = new Regex("^[a-zA-Z0-9]+$");
        public AddRoleDialog(ContentPresenter? contentPresenter,IRolesService rolesService)
            : base(contentPresenter)
        {
            InitializeComponent();
            Title = "新增角色";
            PrimaryButtonText = "保存";
            CloseButtonText = "关闭";
            _rolesService = rolesService;
            DataContext = this;
        }


        protected async override void OnButtonClick(ContentDialogButton button)
        {
            if (button == ContentDialogButton.Close)
            {
                base.OnButtonClick(button);
                return;
            }

            if (string.IsNullOrWhiteSpace(RoleName.Text.Trim()))
            {
                ErrorInfoBar.Message = "角色不能为空！请重新输入!";
                ErrorInfoBar.IsOpen = true;

                return;
            }
            var rolesList =await _rolesService.GetRoleByNameAsync(RoleName.Text.Trim());
            if (rolesList!=null&& rolesList.Id>0)
            {
                ErrorInfoBar.Message = "已存在相同角色名，请重新输入!";
                ErrorInfoBar.IsOpen = true;

                return;
            }

            var role = new Role() { Name= RoleName.Text.Trim() ,Description= RoleDes .Text.Trim()};
            await Task.Run(() =>
            {
                var flag = _rolesService.AddRoleAsync(role).Status;
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


    }
}
