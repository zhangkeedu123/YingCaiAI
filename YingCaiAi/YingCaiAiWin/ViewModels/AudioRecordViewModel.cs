
using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Playwright;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Models;
using YingCaiAiWin.Views.Pages;

namespace YingCaiAiWin.ViewModels
{
    public partial class AudioRecordViewModel : ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private List<AudioRecord> _docs = [];
        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageCount = 1;

        [ObservableProperty]
        private Visibility _openedFilePathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string _openedFilePath = string.Empty;

        private readonly FileHelper _fileHelper;

        private readonly IAudioRecordService _service;

        private readonly HttpClientHelper _httpClient;

        [ObservableProperty]
        private AudioRecord _audioRecordM = new AudioRecord();

        [ObservableProperty]
        private AudioRecord _audioRecordSelect = new AudioRecord();

        [ObservableProperty]
        private List<Users> _users = new List<Users>();

        [ObservableProperty]
        private int _userId = 0;

        private readonly IContentDialogService _contentDialogService;
        public IUsersService _usersService { get; set; }
        public AudioRecordViewModel(INavigationService navigationService, IAudioRecordService service, IUsersService usersService, IContentDialogService contentDialogService)
        {
            if (!_isInitialized)
            {
                _service = service;
                _usersService = usersService;
                _contentDialogService = contentDialogService;
                _fileHelper = new FileHelper();
                _httpClient = new HttpClientHelper();
                InitializeViewModel();

            }

        }
        private async void InitializeViewModel()
        {
            LoadSampleData();
            var users= new List<Users>() { new YingCaiAiModel.Users { UserName="全部",Id=0} };
            users.AddRange( await _usersService.GetAllUserAsync());
            Users = users;
            _isInitialized = true;


        }



        [RelayCommand]
        public async void OnOpenFile()
        {
            OpenedFilePathVisibility = Visibility.Collapsed;

            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "支持的文件 (*.wav;)|*.wav;",
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (File.Exists(openFileDialog.FileName))
            {
                Growl.Success("上传成功,识别录音可能需要几分钟，请稍后刷新！");
                var data = await _fileHelper.UploadAndRecognizeAudio(openFileDialog.FileName);
                if (data != null && data.Contains("id"))
                {
                    var json = JObject.Parse(data);
                    var id = Convert.ToInt32(json.GetValue("id"));
                
                    try
                    {
                        //if (text.Contains("</think>"))
                        //{
                        //    int n = text.IndexOf("</think>");
                        //    if (n > 0)
                        //    {
                        //        var retext = text.Substring(n + 10).Replace("```json", "").Replace("```", "").Trim();
                        //        var aiRes = JsonConvert.DeserializeObject<AudioSentiment>(retext);
                        //        var sentiment = string.Join(",", aiRes.sentiment.label);
                        //        var emotion = string.Join(",", aiRes.emotion.Select(s => s.label));
                        //        var intent = string.Join(",", aiRes.intent.Select(s => s.label));
                        //        var violation = string.Join(",", aiRes.violation.Select(s => s.label));
                        //        var ar = new AudioRecord()
                        //        {
                        //            Id = id,
                        //            Sentiment = sentiment,
                        //            Emotion = emotion,
                        //            Intent = intent,
                        //            ViolationTag = violation,

                        //        };
                        //        _service.UpdateAsync(ar);

                        //    }
                        //}
                    }
                    catch (Exception)
                    {

                    }

                }



            }

            OpenedFilePath = openFileDialog.FileName;
            OpenedFilePathVisibility = Visibility.Visible;
        }


        private void LoadSampleData()
        {

            Docs.Clear();
            var roleName = AppUser.Instance.RoleName;

            if (roleName == "管理员" && UserId == 0)
            {
                _audioRecordM.UserId = 0;
            }
            else if (roleName == "管理员" && UserId != 0)
            {
                _audioRecordM.UserId = Users[UserId].Id;
            }
            else if (roleName != "管理员")
            {
                _audioRecordM.UserId = AppUser.Instance.Id;
            }
            Task.Run(async () =>
            {
                var data = await _service.GetAllPageAsync(_currentPage, _audioRecordM);
                Docs = data.Data as List<AudioRecord> ?? new List<AudioRecord>(); ;
                PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToInt32(data.Message) / 20f));

            });


        }
        [RelayCommand]
        private void OnSerach(string parameter)
        {

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

        // 添加与XAML中控件绑定的属性和命令
        [RelayCommand]
        public async Task GetNLP(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            var data = new {text };
            var dialog = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "NLP分析报告",
                Content = new Views.Pages.ShowNLPControl
                {
                    DataContext = AudioRecordSelect,
                    
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
