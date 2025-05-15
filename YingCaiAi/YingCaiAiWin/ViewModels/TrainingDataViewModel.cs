using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiWin.Helpers;

namespace YingCaiAiWin.ViewModels
{
    public partial class TrainingDataViewModel : ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private List<TrainingData> _docs = [];

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageCount = 1;

        [ObservableProperty]
        private Visibility _openedFilePathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string _openedFilePath = string.Empty;

        private readonly FileHelper _fileHelper;

        private readonly ITrainingDataService _service;

        [ObservableProperty]
        private TrainingData _trainingDataSearch = new TrainingData();
        public TrainingDataViewModel(INavigationService navigationService, ITrainingDataService service)
        {
            if (!_isInitialized)
            {
                _service = service;
                _fileHelper = new FileHelper();
                InitializeViewModel();

            }

        }
        private void InitializeViewModel()
        {
            LoadSampleData();
            _isInitialized = true;

        }



        [RelayCommand]
        public async void OnOpenFile()
        {
            OpenedFilePathVisibility = Visibility.Collapsed;

            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "支持的文件 (*.doc;*.docx;*.xls;*.xlsx;*.txt)|*.doc;*.docx;*.xls;*.xlsx;*.txt",
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (File.Exists(openFileDialog.FileName))
            {
                var data = _fileHelper.FileReadAll(openFileDialog.FileName, 2);
                var docs = new List<TrainingData>();
                foreach (var document in data)
                {
                    if (!string.IsNullOrWhiteSpace(document.Question))
                    {
                        var doc = new TrainingData()
                        {
                            Question = document.Question,
                            Answer = document.Answer,
                            CreatedAt = DateTime.Now,
                            ForLora = false,
                            Status =1,
                            StatusName = "未审核"
                        };
                        docs.Add(doc);
                    }

                }
              
                  await  _service.AddListAsync(docs);
            
                await LoadSampleData();
                Growl.Success("上传成功！");

            }

            OpenedFilePath = openFileDialog.FileName;
            OpenedFilePathVisibility = Visibility.Visible;
        }


        private async Task LoadSampleData()
        {

            
            var data = await _service.GetAllPageAsync(_currentPage, _trainingDataSearch);
            Docs = data.Data as List<TrainingData>??new List<TrainingData>();
            PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToInt32(data.Message) / 20f));




        }
        [RelayCommand]
        private async Task OnSerach(string parameter)
        {

            await LoadSampleData();

        }


        [RelayCommand]
        private void OnApproveStatus(int parameter)
        {


            Growl.Ask("请先确认资料准确无误，再点击确定按钮入库!", isConfirmed =>
            {
                if (isConfirmed)
                {
                    Growl.Clear();
                    Task.Run(() =>
                    {
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
                    Task.Run(() =>
                    {
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
