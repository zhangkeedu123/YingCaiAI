using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using Wpf.Ui;
using YingCaiAiService.IService;
using YingCaiAiWin.Views.Pages;
using YingCaiAiModel;
using Wpf.Ui.Extensions;
using YingCaiAiService.Service;

namespace YingCaiAiWin.ViewModels
{
    public partial class CustomersViewModel : ViewModel
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

        private readonly IContentDialogService _contentDialogService;
        [ObservableProperty]
        private string _dialogResultText = string.Empty;
        public CustomersViewModel(INavigationService navigationService, ICustomerService customerService, IContentDialogService contentDialogService)
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

                var data = _customerService.GetAllPageAsync(_currentPage, _customerSer);
                CustomersList = data.Data as List<Customer> ;
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
            var dialog = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "标记状态",
                Content = new Views.Pages.CustomerStatus
                {
                    DataContext = CustomerSelect
                },
                PrimaryButtonText = "保存",
                //SecondaryButtonText = "取消",
                CloseButtonText = "关闭",
            });

            if (dialog == ContentDialogResult.Primary)
            {
                CustomerSelect.StatusName = CustomerSelect.Status == 1 ? "已联系" : "联系不上";
               var flag=await _customerService.UpdateAsync(CustomerSelect);

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
        private void OnDelete(int parameter)
        {

            Growl.Ask("是否确定删除", isConfirmed =>
            {
                if (isConfirmed)
                {
                    Growl.Clear();
                    Task.Run(() => {
                        var flag = _customerService.DeleteAsync(parameter);
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
        // 添加与XAML中控件绑定的属性和命令
        [RelayCommand]
        public async Task GetPhone()
        {
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
