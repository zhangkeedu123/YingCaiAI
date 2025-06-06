using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiWin.ViewModels
{
    public partial class AiRecordViewModel : ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private List<AiRecord> _docs = [];
        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageCount = 1;

        [ObservableProperty]
        private Visibility _openedFilePathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string _openedFilePath = string.Empty;


        private readonly IAiRecordService _service;

        [ObservableProperty]
        private AiRecord _aiRecords = new AiRecord();
        private readonly IContentDialogService _contentDialogService;
        public AiRecordViewModel(INavigationService navigationService, IAiRecordService service, IContentDialogService contentDialogService)
        {
            if (!_isInitialized)
            {
                _service = service;
                _contentDialogService = contentDialogService;
                InitializeViewModel();

            }

        }
        private async void InitializeViewModel()
        {
             await LoadSampleData();
            _isInitialized = true;


        }



        private async Task LoadSampleData()
        {

                Docs.Clear();
                var data =await _service.GetAllPageAsync(_currentPage, _aiRecords);
                Docs = data.Data as List<AiRecord> ?? new List<AiRecord>(); 
                PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToInt32(data.Message) / 20f));

        }
        [RelayCommand]
        private async Task OnSerach(string parameter)
        {

            await LoadSampleData();

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
                        var flag = _service.DeleteAsync(parameter);
                        LoadSampleData();
                        if (flag.Status)
                        {
                            Growl.Success("删除成功！");
                        }

                        else
                        {
                            
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
        public async Task GetNLP(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            var data = new { text };
            var AudioRecord=new AudioRecord() {  Sentiment=text};
            var dialog = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "答案",
                Content = new Views.Pages.ShowNLPControl
                {
                    DataContext = AudioRecord,

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
