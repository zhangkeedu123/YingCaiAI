using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiService.IService;
using DocumentFormat.OpenXml.ExtendedProperties;
using YingCaiAiWin.Models;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// AddCustomerDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddCustomerDialog :ContentDialog
    {
        public Customer CustomerEdit { get; set; }

        private ICustomerService _service;
        private bool obj = true;
        public AddCustomerDialog(ContentPresenter? contentPresenter, Customer customer, ICustomerService service)
            : base(contentPresenter)
        {
            _service = service;
            InitializeComponent();
            Title = "编辑/添加信息";
            this.PrimaryButtonText = "保存";
            CloseButtonText = "关闭";
            CustomerEdit = customer;
            DataContext = this;
            Load();
        }

        private async void Load()
        {
             Name.Text = CustomerEdit.Name;
            Area.Text= CustomerEdit.Area;
            Contacts.Text = CustomerEdit.Contacts;
            CoProperty.Text = CustomerEdit.CoProperty;
            CoSize.Text = CustomerEdit.CoSize;
            Intro.Text = CustomerEdit.Intro;
            Phone.Text = CustomerEdit.Phone;
        }

        protected async override void OnButtonClick(ContentDialogButton button)
        {


            if (button == ContentDialogButton.Close)
            {
                base.OnButtonClick(button);
                return;
            }
            if (obj == false)
            {
                return;
            }
            obj = false;
            if (string.IsNullOrWhiteSpace(Name.Text.Trim()))
            {
                ErrorInfoBar.Message = "公司名称不能为空！请重新输入!";
                ErrorInfoBar.IsOpen = true;
                obj = true;
                return;
            }
           
            if (CustomerEdit.Id == null || CustomerEdit.Id < 1)
            {
                CustomerEdit.Status = 0;
                CustomerEdit.StatusName = "未联系";
                CustomerEdit.CreatedAt = DateTime.Now;
                CustomerEdit.CreatedUser = AppUser.Instance.Username;
            }

            CustomerEdit.Name = Name.Text.Trim();
            CustomerEdit.Area = Area.Text.Trim();
            CustomerEdit.Contacts = Contacts.Text.Trim();
            CustomerEdit.CoProperty = CoProperty.Text.Trim();
            CustomerEdit.CoSize = CoSize.Text.Trim();
            CustomerEdit.Intro = Intro.Text.Trim();
            CustomerEdit.Phone = Phone.Text.Trim();

            if (CustomerEdit.Id != null && CustomerEdit.Id > 0)
            {
                var flag = await _service.UpdateAsync(CustomerEdit);
                if (flag.Status)
                {
                    Growl.Success("操作成功");
                }
                else
                {
                    Growl.Error("操作失败！");
                }
            }
            else
            {

                
                var flag = await _service.AddAsync(CustomerEdit);
                if (flag.Status)
                {
                    Growl.Success("操作成功");
                }
                else
                {
                    Growl.Error("操作失败！");
                }

            }

            await Task.Delay(2000);
            Growl.Clear();

            obj = true;
            base.OnButtonClick(button);
            return;


        }
    }
}
