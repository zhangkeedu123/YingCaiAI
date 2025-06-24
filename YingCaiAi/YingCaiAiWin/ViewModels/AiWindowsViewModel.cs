using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Data;
using Markdig;
using Microsoft.Extensions.DependencyInjection;
using NPOI.Util;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Models;
using YingCaiAiWin.Views.Pages;


namespace YingCaiAiWin.ViewModels
{
    public partial class AIWindowsViewModel : ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _currentPageTitle = "AI 智能助手";

        [ObservableProperty]
        public List<ToolItem> _toolItems = [];

        [ObservableProperty]
        private List<CardItem> _cardItems = [];

        private string[] Color = new string[] { "#febcd5", "#c8e1c4", "#aee8f5", "#fce48c" };

        [ObservableProperty]
        private Customer _customers=new Customer();

        [ObservableProperty]
        private string _cusName = "请先选择要联系的客户";

        private ICustomerService _customerService;

        private readonly IDocumentsService _service;

        private List<Documents> _documentList = [];

        private List<AudioRecord> AudioRecordList = [];
        private readonly IAudioRecordService _audioRecordService;
        private readonly IAiRecordService _aiRecordService;
        public int coId=0 ;

        private string remand = "| 场景 | 原话 | 改进话术 | |------|------|----------| | 开场 | \"我想看看招聘可以支持你\" | \"我们服务过XX家建筑企业，平均帮助客户节省30%招聘成本，看到贵司正在招聘工程类岗位，想和您聊聊...\" | | 价值传递 | \"我们有PC端、公众号、APP\" | \"我们的PC端实时更新建筑行业人才库，覆盖XX万专业简历，您可随时筛选，同时支持微信端一键触达候选人\" | | 行动引导 | \"方便的话加一下我的微信\" | \"我现在把建筑英才网的行业解决方案发到微信，您看看是否需要我们针对贵司的XX岗位做一次免费简历匹配测试？\" ";
        public Action? LoadMemoAction { get; set; }

        public ICommand LoadCommand => new RelayCommand(async () =>
        {
            // 调用 View 中的方法（如 LoadQuestions）
            LoadMemoAction?.Invoke();
        });
        public AIWindowsViewModel(INavigationService navigationService, ICustomerService customerService, IDocumentsService documentsService , IAudioRecordService audioRecordService , IAiRecordService aiRecordService)
        {
            if (!_isInitialized)
            {
                 _customerService = customerService;
                _audioRecordService = audioRecordService;
                _aiRecordService = aiRecordService;
                _service = documentsService;
                InitializeViewModel();

            }
          
        }
        public  override  void OnNavigatedTo()
        {
            var coid = AppUser.Instance.CoId;
            if (coId!=coid)
            {
                coId = coid;
              
                InitializeViewModel(); // 每次进入页面重新加载数据
                
            }
         
            
            base.OnNavigatedTo();
        }

     

        public async void InitializeViewModel()
        {
            ToolItems = new List<ToolItem>()
                    {
                        new ToolItem { Icon = "People24", Title = "客户资料" },
                        new ToolItem { Icon = "Key24", Title = "招聘痛点" },
                        new ToolItem { Icon = "CheckmarkCircle24", Title = "推荐话术" },
                        new ToolItem { Icon = "ClipboardTextLtr20", Title = "企业套餐" },
                        new ToolItem { Icon = "List24", Title = "行业新闻" },
                        new ToolItem { Icon = "Warning24", Title = "平台数据" },
                        new ToolItem { Icon = "Document24", Title = "售后服务" },
                        new ToolItem { Icon = "Shield24", Title = "心灵鸡汤" },
                    };
            if (coId!=0)
            {
                var cus =await _customerService.GetByIdAsync(Convert.ToInt32(coId));
                if (cus.Status && cus.Data != null)
                {
                    Customers = cus.Data as Customer;

                    CusName = Customers?.Name;
                }
            }
            LoadCommand.Execute(LoadMemoAction);

            CardItems = new()
                {
                 
                        new CardItem
                        {
                            Title = "客户资料",
                            Description =Customers?.Intro?? "建筑英才网拥有超过1400多万份建筑行业人才简历，涵盖中高级人才库及紧缺专业人才库，日均新增注册用户约5000人，远超综合类平台。建筑英才网与60余所建筑类高校合作，搭建校园招聘体系。针对中高端岗位需求，提供一对一猎头服务，依托五年以上从业经验的专业人才库，满足企业高管、设计院专家等稀缺岗位招聘需求。90%以上的企业客户选择续费合作，日均保持3万个有效职位，访问量达120万次，反映用户对平台专业性的高度依赖。建筑英才网自2000年成立以来，多次被评为行业标杆，如“中国十大网络招聘机构”，并与多家建筑企业、高校建立战略合作，形成品牌壁垒。",
                            IconPath = "#febcd5",
                            CommandParam = Guid.NewGuid().ToString()
                        }
                  };

            _documentList =await _service.GetAllSystemAsync();
            AudioRecordList= await _audioRecordService.GetAllAsync();
            _isInitialized = true;
        }

        // 添加与XAML中控件绑定的属性和命令
        [RelayCommand]
        private void NavigateToDetail(string parameter)
        {
            // 导航逻辑
        }

        [RelayCommand]
        private async void OnCloseCard(string parameter)
        {
            var cardToRemove = CardItems.FindAll(c => c.CommandParam?.ToString() != parameter?.ToString());
            if (cardToRemove != null)
            {
                CardItems = cardToRemove;
            }
        }

        /// <summary>
        /// 展示卡片
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="qText"></param>
        public void ShowPopupCommand(string parameter,string qText="")
        {
            var newList = CardItems.Copy();
            var flag = CardItems.Count(m => m.Description.Equals(parameter)) > 0;
            if (flag)
            {

            }
            else
            {
                var i = new Random();
                var d = new CardItem
                {
                    Title = qText.Length<11?qText: qText.Substring(0,10),
                    Description = parameter,
                    IconPath = Color[i.Next(0, 4)],
                    CommandParam = Guid.NewGuid().ToString(),
                };
                newList.Add(d);
                //CardItems.Clear();
                CardItems = newList;
            }
            
        }

        /// <summary>
        /// 展示客户资料卡片
        /// </summary>
        /// <param name="title"></param>
        public void ShowCustomer(string title)
        {
            if (CardItems.Count(m => m.Title.Equals(title)) > 0)
                return;
            if (title == "客户资料")
            {
                var newList = CardItems.Copy();
                newList.Add(new CardItem
                {
                    Title = title,
                    Description = Customers?.Intro ?? "建筑英才网拥有超过1400多万份建筑行业人才简历，涵盖中高级人才库及紧缺专业人才库，日均新增注册用户约5000人，远超综合类平台。建筑英才网与60余所建筑类高校合作，搭建校园招聘体系。针对中高端岗位需求，提供一对一猎头服务，依托五年以上从业经验的专业人才库，满足企业高管、设计院专家等稀缺岗位招聘需求。90%以上的企业客户选择续费合作，日均保持3万个有效职位，访问量达120万次，反映用户对平台专业性的高度依赖。建筑英才网自2000年成立以来，多次被评为行业标杆，如“中国十大网络招聘机构”，并与多家建筑企业、高校建立战略合作，形成品牌壁垒。",
                    IconPath = "#febcd5",
                    CommandParam = Guid.NewGuid().ToString()
                });
                CardItems = newList;
            }
            else if(title=="智能方案")
            {
                var newList = CardItems.Copy();
                var i = new Random();
                var des = _documentList.FirstOrDefault(m => m.Filename.Equals("企业套餐"))?.Content;
                newList.Add(new CardItem
                {
                    Title = title,
                    Description = "正在努力加载中...",
                    IconPath = Color[i.Next(0, 4)],
                    CommandParam = Guid.NewGuid().ToString()
                });
                CardItems = newList;

                string text = $"请结合客户资料，和我公司的企业套餐总结之后给我推荐新的销售专业话术和为客户推荐合适套餐，内容要准确无误。\n\n 客户名称：{Customers.Name} ,介绍： {Customers?.Intro ?? "建筑英才网拥有超过1400多万份建筑行业人才简历，涵盖中高级人才库及紧缺专业人才库，日均新增注册用户约5000人，远超综合类平台。建筑英才网与60余所建筑类高校合作，搭建校园招聘体系。针对中高端岗位需求，提供一对一猎头服务，依托五年以上从业经验的专业人才库，满足企业高管、设计院专家等稀缺岗位招聘需求。90%以上的企业客户选择续费合作，日均保持3万个有效职位，访问量达120万次，反映用户对平台专业性的高度依赖。建筑英才网自2000年成立以来，多次被评为行业标杆，如“中国十大网络招聘机构”，并与多家建筑企业、高校建立战略合作，形成品牌壁垒。" } 企业正在招聘的职位：{Customers?.JobTitle},薪资：{Customers?.Salary} \n\n 企业套餐：{des}";
                Task.Run(async () => {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var response = await new HttpClientHelper().PostDataAsync("milvus/ask", new { text, top_k = 10 });
                    if (response != null)
                    {
                        stopwatch.Stop();
                        if (response.Contains("</think>"))
                        {
                            var aimodel = JsonSerializer.Deserialize<AiModelRes>(response);
                            var aitext = aimodel.answer;
                            int n = aitext.IndexOf("</think>");
                            if (n > 0)
                            {
                                var newList = CardItems.Copy();
                                var retext = aitext.Substring(n + 10);
                              
                                remand = retext;
                                
                                newList.ForEach(item =>
                                {
                                    if (item.Title.Equals(title))
                                    {
                                        item.Description = retext;
                                    }
                                });
                                CardItems = newList;
                                TimeSpan elapsedTime = stopwatch.Elapsed;
                                Task.Run( () =>
                                {
                                    // 获取执行时间
                                   
                                    var ai = new AiRecord()
                                    {
                                        Question = title,
                                        Answer = retext,
                                        CreatedAt = DateTime.Now,
                                        CreatedUser = AppUser.Instance.Username,
                                        Times = (int)elapsedTime.TotalSeconds,
                                    };
                                    _aiRecordService.AddAsync(ai);

                                });
                            }
                        }

                    }
                });

            }
            else if (title == "模拟对话"||title == "推荐话术")
            {
                var newList = CardItems.Copy();
                var i = new Random();

                newList.Add(new CardItem
                {
                    Title = title,
                    Description = "正在努力加载中...",
                    IconPath = Color[i.Next(0, 4)],
                    CommandParam = Guid.NewGuid().ToString()
                });
                CardItems = newList;

                Task.Run(async () => {


                    var nlp = AudioRecordList.Select(m => m.Sentiment).ToArray();
                    var audios = string.Join("\n\n", nlp);
                    string text = "";
                    if(title == "模拟对话")
                    {
                        text = $"请结合我的客户的客户资料和模型参考客户资料、企业套餐生成的话术，给我模拟一下销售人员和客户的对话，从开场白、询问过程、找话题、有可能出现的情况、到最后是否成交等尽量真实有条理，有助于帮助销售人员去实际沟通。\n\n 客户资料：{Customers?.Intro ?? "建筑英才网拥有超过1400多万份建筑行业人才简历，涵盖中高级人才库及紧缺专业人才库，日均新增注册用户约5000人，远超综合类平台。建筑英才网与60余所建筑类高校合作，搭建校园招聘体系。针对中高端岗位需求，提供一对一猎头服务，依托五年以上从业经验的专业人才库，满足企业高管、设计院专家等稀缺岗位招聘需求。90%以上的企业客户选择续费合作，日均保持3万个有效职位，访问量达120万次，反映用户对平台专业性的高度依赖。建筑英才网自2000年成立以来，多次被评为行业标杆，如“中国十大网络招聘机构”，并与多家建筑企业、高校建立战略合作，形成品牌壁垒。"} \n\n  推荐话术：{remand} ";
                    }
                    else
                    {
                         text = $" 请结合我之前录音内容NLP分析的结果,总结经验后给我推荐新的销售话术。\n NLP结果如下：\n\n{audios}";
                    }

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var response =await new HttpClientHelper().PostDataAsync("milvus/ask", new { text, top_k = 10 });
                    if (response != null)
                    {
                        stopwatch.Stop();
                        if (response.Contains("</think>"))
                        {
                            var aimodel = JsonSerializer.Deserialize<AiModelRes>(response);
                            var aitext = aimodel.answer;
                            int n = aitext.IndexOf("</think>");
                            if (n > 0)
                            {
                                var newList = CardItems.Copy();
                                var retext = aitext.Substring(n + 10);
                                if (text == "推荐话术")
                                {
                                    remand += retext;
                                }
                                newList.ForEach(item =>
                                {
                                    if (item.Title.Equals(title))
                                    {
                                       item.Description = retext;
                                    }
                                });
                                CardItems = newList;
                                TimeSpan elapsedTime = stopwatch.Elapsed;
                                Task.Run(() =>
                                {
                                    // 获取执行时间
                               
                                    var ai = new AiRecord()
                                    {
                                        Question = title,
                                        Answer = retext,
                                        CreatedAt = DateTime.Now,
                                        CreatedUser = AppUser.Instance.Username,
                                        Times = (int)elapsedTime.TotalSeconds,
                                    };
                                    _aiRecordService.AddAsync(ai);

                                });
                            }
                        }

                    }
                });

            }
            else
            {
               
            }
            //else if ( title == "行业新闻")
            //{
            //    var newList = CardItems.Copy();
            //    var i = new Random();

            //    newList.Add(new CardItem
            //    {
            //        Title = title,
            //        Description = "正在努力加载中...",
            //        IconPath = Color[i.Next(0, 4)],
            //        CommandParam = Guid.NewGuid().ToString()
            //    });
            //    CardItems = newList;


            //}

        }


        public void ShowCard(string title,string content)
        {
            var newList = CardItems.Copy();
            var i = new Random();

            newList.Add(new CardItem
            {
                Title = title,
                Description = content,
                IconPath = Color[i.Next(0, 4)],
                CommandParam = Guid.NewGuid().ToString()
            });
            CardItems = newList;
        }
    }


    public class ToolItem
    {
        public string Icon { get; set; }       // Symbol 字符串
        public string Title { get; set; }      // 标题
    }
}


