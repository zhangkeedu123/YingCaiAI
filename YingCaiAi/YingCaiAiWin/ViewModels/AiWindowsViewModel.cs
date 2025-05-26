using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Data;
using Microsoft.Extensions.DependencyInjection;
using NPOI.Util;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
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
        private int coId ;
        public AIWindowsViewModel(INavigationService navigationService, ICustomerService customerService)
        {
            if (!_isInitialized)
            {
                 _customerService = customerService;
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
                        new ToolItem { Icon = "List24", Title = "优惠政策" },
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
            if (title == "客户资料"&&CardItems.Count(m => m.Title.Contains(title)) <1)
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
            else if(title=="招聘痛点")
            {
                var newList = CardItems.Copy();
                newList.Add(new CardItem
                {
                    Title = title,
                    Description = "‌企业端招聘流程痛点‌\r\n‌\r\n人才供需结构性矛盾‌\r\n\r\n硬科技等细分领域因行业窄、技术壁垒高，直接适配人才稀缺；\r\n\r\n顶尖人才仅10%处于求职状态，90%简历有效性低导致筛选成本激增。\r\n\r\n‌评估机制有效性不足‌\r\n90%面试存在漫谈化、主观化倾向，缺乏标准化胜任力模型；\r\n\r\n跨行业人才适配困难，易出现转型失败导致的“用不好”问题。\r\n‌‌\r\n‌隐性成本长期被忽视\r\n‌\r\n直接成本（渠道费、差旅费）与间接成本（工时损耗、业务停滞风险）叠加，企业平均招聘成本可达岗位年薪的1.5倍；‌‌\r\n\r\n人才流失后重置成本高昂，尤其是核心技术岗位员工离职可能引发项目延期。‌‌\r\n\r\n‌平台机制引发的双向困境‌\r\n\r\n‌匹配算法商业化异化‌\r\n付费推广机制导致未充值用户简历优先级降低，经济弱势群体错失机会；‌‌\r\n\r\n平台存在大量僵尸岗位，求职者投递反馈率不足5%。‌‌\r\n\r\n‌技术应用尚未破局‌\r\nAI解析工具虽能1秒处理千份简历，但过度依赖关键词匹配可能遗漏潜力型人才；‌‌\r\n\r\n云端面试系统节省70%差旅成本，却难以评估文化适配度等软性指标。‌‌\r\n\r\n‌新生代求职特征带来的挑战‌\r\n\r\nZ世代更关注职业发展空间与企业价值观，传统“低固定+高浮动”薪酬模式吸引力下降；‌‌\r\n\r\n简历优化需求催生付费服务产业链，加重经济弱势群体求职负担。‌‌\r\n",
                    IconPath = "#c8e1c4",
                    CommandParam = Guid.NewGuid().ToString()
                });
                CardItems = newList;
            }
         
        }

    }


    public class ToolItem
    {
        public string Icon { get; set; }       // Symbol 字符串
        public string Title { get; set; }      // 标题
    }
}


