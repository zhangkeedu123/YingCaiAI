using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Views.Pages;

namespace YingCaiAiWin.ViewModels
{
     public  partial class SystemConfigViewModel:ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private List<Documents> _docs = [];
        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageCount = 1;

        [ObservableProperty]
        private Visibility _openedFilePathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string _openedFilePath = string.Empty;

        private readonly FileHelper _fileHelper;

        private readonly IDocumentsService _service;

        private readonly HttpClientHelper _httpClient;

        [ObservableProperty]
        private Documents _documents = new Documents();

        [ObservableProperty]
        private Documents _documentEdit = new Documents();

        [ObservableProperty]
        private int _intId = 0;

        private readonly IContentDialogService _contentDialogService;
        public SystemConfigViewModel(INavigationService navigationService, IDocumentsService service, IContentDialogService contentDialogService)
        {
            if (!_isInitialized)
            {
                _service = service;
                _contentDialogService = contentDialogService;
                _fileHelper = new FileHelper();
                _httpClient = new HttpClientHelper();
                InitializeViewModel();

            }

        }
        private void InitializeViewModel()
        {
            LoadSampleData();
            _isInitialized = true;


        }



      
        private void LoadSampleData()
        {

            Docs.Clear();

            Task.Run(() =>
            {
                var data = _service.GetSystemPageAsync(_currentPage, _documents);
                Docs = data.Data as List<Documents> ?? new List<Documents>(); ;
                PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToInt32(data.Message) / 20f));

            });


        }
        [RelayCommand]
        private void OnSerach(string parameter)
        {

            LoadSampleData();

        }

        [RelayCommand]
        public async Task OnAddSystem(int parameter)
        {


            if (parameter != 0)
            {
                DocumentEdit = (await _service.GetByIdAsync(parameter))?? new Documents();
            }
            else
            {
                DocumentEdit = new Documents();
            }
            var termsOfUseContentDialog = new AddSystemConfig(_contentDialogService.GetDialogHost(), DocumentEdit, _service);
            var result = await termsOfUseContentDialog.ShowAsync();
            LoadSampleData();
            return;
        }

        [RelayCommand]
        private void OnApproveStatus(int parameter)
        {


            Growl.Ask("请先确认资料准确无误，再点击确定按钮入库!", isConfirmed =>
            {
                if (isConfirmed)
                {
                    Growl.Clear();
                    Task.Run(() => {
                        var flag = _service.UpdateAsync(parameter).Result;
                        LoadSampleData();
                        if (flag.Status)
                        {
                            Growl.Success("审核成功！");
                        }

                        else
                        {
                            Growl.Error("审核失败！");
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
        private void OnDelete(int parameter)
        {

            Growl.Ask("是否确定删除", isConfirmed =>
            {
                if (isConfirmed)
                {
                    Growl.Clear();
                    Task.Run(() => {
                        var response = _httpClient.PostDataAsync($"milvus/delete_by_doc_id?doc_id={parameter}", null).Result;

                        var flag = _service.DeleteAsync(parameter);
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
