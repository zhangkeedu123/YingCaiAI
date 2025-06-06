using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using Wpf.Ui;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiWin.Models;
using YingCaiAiWin.Views.Pages;
using Wpf.Ui.Extensions;
using NPOI.Util;

namespace YingCaiAiWin.ViewModels
{
    public partial class UserCustomersViewModel : ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private List<Customer> _customersList = [];
        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageCount = 1;

        [ObservableProperty]
        private int _intId = 0;

        public ICustomerService _customerService { get; set; }

        [ObservableProperty]
        public Customer _customerSer = new Customer();

        [ObservableProperty]
        public Customer _customerSelect = new Customer();

        [ObservableProperty]
        public Customer _customerUser = new Customer();
        private readonly IContentDialogService _contentDialogService;
        [ObservableProperty]
        private string _dialogResultText = string.Empty;

        [ObservableProperty]
        private bool _isAllSelected;
        public UserCustomersViewModel(INavigationService navigationService, ICustomerService customerService, IContentDialogService contentDialogService)
        {

            if (!_isInitialized)
            {
                _contentDialogService = contentDialogService;
                _customerService = customerService;
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

            CustomersList.Clear();

            Task.Run(() =>
            {
                var roleName = AppUser.Instance.RoleName;
                if (roleName != null && roleName != "")
                {
                    
                   _customerSer.CreatedUser = AppUser.Instance.Username;
                    
                }
                var data = _customerService.GetUserPageAsync(_currentPage, _customerSer);
                CustomersList = data.Data as List<Customer>;
                PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToInt32(data.Message) / 20f));

            });

        }

        [RelayCommand]
        private void OnSerach(string parameter)
        {
            _currentPage = 1;
            LoadSampleData();
        }


        /// <summary>
        /// 打标记
        /// </summary>
        /// <param name="parameter"></param>
        [RelayCommand]
        private async void OnApproveStatus(int parameter)
        {

            CustomerUser = new Customer() { Id = CustomerSelect.Id, Name = "", Area = "", Status = null };
            var dialog = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "标记状态",
                Content = new Views.Pages.CustomerStatus
                {
                    DataContext = CustomerUser
                },
                PrimaryButtonText = "保存",
                //SecondaryButtonText = "取消",
                CloseButtonText = "关闭",
            });

            if (dialog == ContentDialogResult.Primary)
            {
                if (CustomerUser.Name != "")
                {
                    CustomerUser.Status = Convert.ToInt32(CustomerUser.Name);
                    CustomerUser.StatusName = CustomerUser.Status == 1 ? "已联系" : "联系不上";
                }


                var cus = (await _customerService.GetByIdAsync(CustomerUser.Id ?? 0)).Data as Customer;

                if (cus?.CreatedUser == AppUser.Instance.Username)
                {

                    if (CustomerUser.Area == "2")
                    {
                        CustomerUser.CreatedUser = "";
                    }
                }
                else if (string.IsNullOrWhiteSpace(cus?.CreatedUser))
                {
                    if (CustomerUser.Area == "1")
                    {
                        CustomerUser.CreatedUser = AppUser.Instance.Username;
                    }
                }
                else
                {
                    CustomerUser.Area = "";
                }



                var flag = await _customerService.UpdateUserAsync(CustomerUser);

                if (flag.Status)
                {
                    Growl.Success("操作成功");
                    LoadSampleData();
                }
                else
                {
                    Growl.Error("操作失败！");
                }
                await Task.Delay(2000);
                Growl.Clear();
            }
        }




        [RelayCommand]
        private void OnDelete()
        {

            Growl.Ask("是否确定删除", isConfirmed =>
            {
                if (isConfirmed)
                {
                    Growl.Clear();
                    Task.Run(() =>
                    {

                        var select = CustomersList.Where(m => m.IsSelected);
                        if (select.Count() == 0)
                        {
                            Task.Run(() =>
                            {
                                Growl.Info("请选择要删除的数据！");
                            });

                        }
                        else
                        {
                            var ids = select.Select(s => s.Id).ToArray();
                            var flag = _customerService.DeleteAsync(ids);
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
        public async void OnToContact(int parameter)
        {

            var cus = (await _customerService.GetByIdAsync(parameter)).Data as Customer;
            if (cus.CreatedUser != AppUser.Instance.Username)
            {
                Growl.Error("不能联系不属于你的客户！");
                await Task.Delay(2000);
                Growl.Clear();

            }
            else
            {
                var navigationService = App.Services.GetService<INavigationService>();

                AppUser.Instance.CoId = parameter;

                navigationService.Navigate(typeof(AIWindows));
            }



        }

        [RelayCommand]
        private async void OnAddCus()
        {

            var termsOfUseContentDialog = new AddCustomerDialog(_contentDialogService.GetDialogHost(), new Customer(), _customerService);
            var result = await termsOfUseContentDialog.ShowAsync();
            LoadSampleData();
            return;
        }

        [RelayCommand]
        private async void OnEditCus(int parameter)
        {

            var termsOfUseContentDialog = new AddCustomerDialog(_contentDialogService.GetDialogHost(), CustomerSelect, _customerService);
            var result = await termsOfUseContentDialog.ShowAsync();
            LoadSampleData();
            return;
        }


        // 添加与XAML中控件绑定的属性和命令
        [RelayCommand]
        public async Task GetPhone()
        {
            if (string.IsNullOrWhiteSpace(CustomerSelect.Remark))
            {
                CustomerSelect.Remark = CustomerSelect.Phone;
            }
            var dialog = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "电话说明",
                Content = new Views.Pages.ShowPhone
                {
                    DataContext = CustomerSelect
                },
                //PrimaryButtonText = "保存",
                //SecondaryButtonText = "取消",
                CloseButtonText = "关闭",
            });

            if (dialog == ContentDialogResult.Primary)
            {


            }

        }

        [RelayCommand]
        private async void OnALLCheckChanged()
        {
            var temp = CustomersList.Copy();
            //IsAllSelected = !IsAllSelected;
            foreach (var item in temp)
            {
                item.IsSelected = IsAllSelected;
            }
            CustomersList = temp;
        }

        [RelayCommand]
        private async void OnCheckChanged(int parameter)
        {
            var temp = CustomersList.Copy();
            temp.ForEach(item =>
            {
                if (item.Id == parameter)
                    item.IsSelected = !item.IsSelected;

                if (!item.IsSelected)
                    IsAllSelected = false;
            });

            CustomersList = temp;
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
