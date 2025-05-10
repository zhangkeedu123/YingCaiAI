using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using System.IO;
using Wpf.Ui;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiWin.Helpers;

namespace YingCaiAiWin.ViewModels
{

    public partial class KnowledgeBaseViewModel : ViewModel
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

        [ObservableProperty]
        private  Documents _documents=new Documents();
        public KnowledgeBaseViewModel(INavigationService navigationService, IDocumentsService service)
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
        public void OnOpenFile()
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
                var data = _fileHelper.FileReadAll(openFileDialog.FileName);
                var docs = new List<Documents>();
                foreach (var document in data)
                {
                    if (document.Length > 10)
                    {
                        var doc = new Documents()
                        {
                            Content = document,
                            Filename = document.Substring(0, 10).Trim(),
                            CreatedAt = DateTime.Now,
                            Status = 0,
                            StatusName = "未审核"
                        };
                        docs.Add(doc);
                    }

                }
                Task.Run(() =>
                {
                    _service.AddListAsync(docs);
                });
                Growl.Success("上传成功！");

            }

            OpenedFilePath = openFileDialog.FileName;
            OpenedFilePathVisibility = Visibility.Visible;
        }


        private void LoadSampleData()
        {

            Docs.Clear();

            Task.Run(() =>
            {
                var data = _service.GetAllPageAsync(_currentPage, _documents);
                Docs = data.Data as List<Documents>;
                PageCount =Convert.ToInt32( Math.Ceiling( Convert.ToInt32(data.Message)/20f));
               
            });

        }
        [RelayCommand]
        private void OnSerach(string parameter)
        {

            LoadSampleData();
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
            
            Growl.Ask("是否确定删除",  isConfirmed =>
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
