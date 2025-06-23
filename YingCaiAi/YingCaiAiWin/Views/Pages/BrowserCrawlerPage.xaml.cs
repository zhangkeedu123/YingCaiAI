

using DocumentFormat.OpenXml.Wordprocessing;
using HandyControl.Controls;
using HtmlAgilityPack;
using Microsoft.Playwright;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiWin.Models;
namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// BrowserCrawlerPage.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserCrawlerPage : System.Windows.Controls.Page
    {
        private bool _isInitialized = false;
        private bool _isphone = false;
        private bool _issearch = false;
        private bool _isstop = false;

        private readonly ICustomerService _customerService;

        private List<Areas> areasList = new List<Areas>();
        private int dataC = 0;

        private List<string> CoName = new List<string>();

        private IPage page = null;
        public BrowserCrawlerPage(ICustomerService customerService)
        {
            DataContext = this;
            _customerService = customerService;
            if (!_isInitialized)
            {
                InitializeComponent();
                LoadData();
                loadWeb();
            }
            Loaded += BrowserCrawlerPage_Loaded;

        }

        public async void LoadData()
        {
            var area = JsonSerializer.Deserialize<List<Areas>>("[\r\n  {\"name\": \"全国\", \"code\": \"489\"},\r\n  {\"name\": \"安徽\", \"code\": \"541\"},\r\n  { \"name\": \"澳门\", \"code\": \"562\"},\r\n  { \"name\": \"北京\", \"code\": \"530\"},\r\n  { \"name\": \"重庆\", \"code\": \"551\"},\r\n  { \"name\": \"福建\", \"code\": \"542\"},\r\n  { \"name\": \"甘肃\", \"code\": \"557\"},\r\n  { \"name\": \"广东\", \"code\": \"548\"},\r\n  { \"name\": \"广西\", \"code\": \"549\"},\r\n  { \"name\": \"贵州\", \"code\": \"553\"},\r\n  { \"name\": \"海南\", \"code\": \"550\"},\r\n  { \"name\": \"河北\", \"code\": \"532\"},\r\n  { \"name\": \"黑龙江\", \"code\": \"537\"},\r\n  { \"name\": \"河南\", \"code\": \"545\"},\r\n  { \"name\": \"湖北\", \"code\": \"546\"},\r\n  { \"name\": \"湖南\", \"code\": \"547\"},\r\n  { \"name\": \"江苏\", \"code\": \"539\"},\r\n  { \"name\": \"江西\", \"code\": \"543\"},\r\n  { \"name\": \"吉林\", \"code\": \"536\"},\r\n  { \"name\": \"辽宁\", \"code\": \"535\"},\r\n  { \"name\": \"内蒙古\", \"code\": \"534\"},\r\n  { \"name\": \"宁夏\", \"code\": \"559\"},\r\n  { \"name\": \"青海\", \"code\": \"558\"},\r\n  { \"name\": \"山东\", \"code\": \"544\"},\r\n  { \"name\": \"上海\", \"code\": \"538\"},\r\n  { \"name\": \"陕西\", \"code\": \"556\"},\r\n  { \"name\": \"山西\", \"code\": \"533\"},\r\n  { \"name\": \"四川\", \"code\": \"552\"},\r\n  { \"name\": \"台湾\", \"code\": \"563\"},\r\n  { \"name\": \"天津\", \"code\": \"531\"},\r\n  { \"name\": \"香港\", \"code\": \"561\"},\r\n  { \"name\": \"新疆\", \"code\": \"560\"},\r\n  { \"name\": \"西藏\", \"code\": \"555\"},\r\n  { \"name\": \"云南\", \"code\": \"554\"},\r\n  { \"name\": \"浙江\", \"code\": \"540\"}\r\n]");
            areasList = area;
            Area.ItemsSource = area;
            Area.DisplayMemberPath = "name";
            Area.SelectedIndex = 0;
            var cus = await _customerService.GetAllNameAsync();
            if (cus != null && cus.Count > 0)
            {
                CoName = cus.Select(x => x.Name).Distinct().ToList();
            }

            _isInitialized = true;
        }

        private async void loadWeb()
        {
            try
            {
                if (page == null)
                {
                    var playwright = await Playwright.CreateAsync();
                    var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

                    // 创建一个临时上下文（无缓存、无 Cookie）
                    var context = await browser.NewContextAsync(new()
                    {
                        ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
                        //UserAgent = GetRandomUserAgent(),  // 🧠 可选：随机 UA
                    });
                    page = await context.NewPageAsync();
                    //var page = await browser.NewPageAsync();
                    await page.GotoAsync($"https://chat.baidu.com/search", new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle,
                        Timeout = 40000 // 延长等待时间
                    });
                }
            }
            catch (Exception)
            {


            }

        }
        private async void BrowserCrawlerPage_Loaded(object sender, RoutedEventArgs e)
        {
            await MyBrowser.EnsureCoreWebView2Async();
            UrlTextBox.SelectionChanged += UrlTextBox_SelectionChanged;
        }

        private void LoadUrl()
        {
            var url = UrlTextBox.SelectedIndex == 0 ? "https://www.zhaopin.com/" : "https://www.zhipin.com/";

            if (!string.IsNullOrWhiteSpace(url))
            {
                MyBrowser.Source = new Uri(url);
                AppendLog($"✅ 状态：已加载 {url}");
                SearchByKeyword(1);
            }
        }

        private void SearchByKeyword(int page)
        {
            var keyword = KeywordTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var encoded = Uri.EscapeDataString(keyword);
                var city = areasList[Area.SelectedIndex].code;
                var url =  $"https://www.zhaopin.com/sou/jl{city}/kw{encoded}/p{page}" ;
                if (UrlTextBox.SelectedIndex == 2)
                {
                    url = $"https://mh.job1001.com/SearchResult.php?jtzw={encoded}&page={page}";
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    YiLanExtractHtml();
                }
                else if(UrlTextBox.SelectedIndex == 3)
                {
                    url = $"https://chem.job1001.com/SearchResult.php?jtzw={encoded}&page={page}";
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    YiLanExtractHtml();
                }
                else if(UrlTextBox.SelectedIndex == 4)
                {
                    url = $"https://www.sjjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    YiLanExtractHtml();
                }
                else if(UrlTextBox.SelectedIndex == 5)
                {
                    url = $"https://sz.tmjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    YiLanExtractHtml();
                }
                else if(UrlTextBox.SelectedIndex == 6)
                {
                    url = $"https://www.tmjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    YiLanExtractHtml();
                }
                else if(UrlTextBox.SelectedIndex == 7)
                {
                    url = $"https://www.lqjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    YiLanExtractHtml();
                }
                else if(UrlTextBox.SelectedIndex == 8)
                {
                    url = $"https://www.jljob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    YiLanExtractHtml();
                }
                else
                {
                    MyBrowser.Source = new Uri(url);
                    AppendLog($"✅ 状态：搜索关键词 {keyword}");

                    ExtractHtml();
                }

               
            }
            else
            {
                AppendLog($"✅ 状态：请输入关键词！");
                _issearch = false;
            }
        }
        private void UrlTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var keyword = KeywordTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var encoded = Uri.EscapeDataString(keyword);
                var city = areasList[Area.SelectedIndex].code;
                var url = UrlTextBox.SelectedIndex == 0 ? $"https://www.zhaopin.com/sou/jl{city}/kw{encoded}/p{page}" : $"https://www.zhipin.com/web/geek/jobs?query={encoded}&city=100010000";
                AppendLog($"✅ 状态：搜索关键词 {keyword}");
                if (UrlTextBox.SelectedIndex == 2)
                {
                    url = $"https://mh.job1001.com/SearchResult.php?jtzw={encoded}&page={page}";
                  
                }
                else if (UrlTextBox.SelectedIndex == 3)
                {
                    url = $"https://chem.job1001.com/SearchResult.php?jtzw={encoded}&page={page}";
                  
                }
                else if(UrlTextBox.SelectedIndex == 4)
                {
                    url = $"https://www.sjjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                   
                }
                else if(UrlTextBox.SelectedIndex == 5)
                {
                    url = $"https://sz.tmjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                   
                }
                else if(UrlTextBox.SelectedIndex == 6)
                {
                    url = $"https://www.tmjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                  
                }
                else if(UrlTextBox.SelectedIndex == 7)
                {
                    url = $"https://www.lqjob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                   
                }
                else if(UrlTextBox.SelectedIndex == 8)
                {
                    url = $"https://www.jljob88.com/SearchResult.php?jtzw={encoded}&page={page}";
                   
                }
                MyBrowser.Source = new Uri(url);

            }
            else
            {
                AppendLog($"✅ 状态：请输入关键词！");
                _issearch = false;
            }
        }

        private async void YiLanExtractHtml()
        {
            try
            {
                AppendLog("⏳ 正在滚动页面...");

                await MyBrowser.ExecuteScriptAsync("window.scrollTo(0, document.body.scrollHeight);");
                await Task.Delay(5000);  // 等待页面渲染

                AppendLog("⏳ 正在提取 HTML...");
                string html = await MyBrowser.ExecuteScriptAsync("document.documentElement.outerHTML");
                string cleanHtml = JsonSerializer.Deserialize<string>(html);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(cleanHtml);

                var root = htmlDoc.DocumentNode;

                var jobBoxList = root.SelectNodes("//ul[@class='post_list']/li");
                AppendLog("⏳ 已提取 HTML，正在处理数据...");
                var customerList = new List<Customer>();
                if (jobBoxList?.Count > 0)
                {
                    foreach (var item in jobBoxList)
                    {
                        // 公司名称
                        var company = item.SelectSingleNode(".//div[@class='item_r_A']//a")?.InnerText?.Trim()
                         ?? item.SelectSingleNode(".//div[@class='item_r_pic']//a[@title]")?.GetAttributeValue("title", "")?.Trim();
                        if (CoName.Contains(company ?? "") || customerList.Count(m => m.Name.Equals(company)) > 0)
                        {
                            continue;
                        }

                        // 职位名称
                        var jobTitle = item.SelectSingleNode(".//div[@class='item_l_A']//a")?.InnerText.Trim();



                        // 联系人
                        var contact = item.SelectSingleNode(".//div[contains(@class, 'companyinfo__staff-name')]")?.InnerText.Trim();

                        // 薪资
                        var salary = item.SelectSingleNode(".//span[contains(@class, 'top_c_page')]")?.InnerText?.Trim();

                        // 工作地点
                        var location = item.SelectSingleNode(".//div[@class='item_r_B']")?.InnerText?.Trim();

                       
                        string CoProperty ="";
                        string CoSize = "";

                        var customer = new Customer()
                        {
                            Area = location,
                            Contacts = contact,
                            CoProperty = CoProperty,
                            CoSize = CoSize,
                            JobTitle = jobTitle,
                            Name = company,
                            Salary = salary,
                            Status = 0,
                            StatusName = "未联系",
                            CreatedAt = DateTime.Now
                        };
                        customerList.Add(customer);
                    }
                    var flag = await _customerService.AddListAsync(customerList);
                    if (flag.Status)
                    {
                        dataC += 20;
                        AppendLog("✅ HTML 提取成功，已入库20条数据");
                    }
                    else
                    {
                        AppendLog("✅ 入库失败，请联系管理员");
                    }
                }
                else
                {
                    AppendLog("✅ 没有找到数据。。。");
                }

                if (_isstop)
                {
                    return;
                }

                var numC = Convert.ToInt32(((ContentControl)DataCount.SelectedItem).Content);

                if (dataC < numC)
                {
                    SearchByKeyword((dataC / 20) + 1);
                }
                else
                {
                    dataC = 0;
                    _issearch = false;
                }
            }
            catch (Exception)
            {
                AppendLog("✅ 解析异常");
                _issearch = false;
            }
           

        }


        private async void ExtractHtml()
        {
            AppendLog("⏳ 正在滚动页面...");

            await MyBrowser.ExecuteScriptAsync("window.scrollTo(0, document.body.scrollHeight);");
            await Task.Delay(5000);  // 等待页面渲染

            AppendLog("⏳ 正在提取 HTML...");
            string html = await MyBrowser.ExecuteScriptAsync("document.documentElement.outerHTML");
            string cleanHtml = JsonSerializer.Deserialize<string>(html);

            LoadHtmls(cleanHtml);

        }

        private async void LoadHtmls(string htmlContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var root = htmlDoc.DocumentNode;

            var jobBoxList = root.SelectNodes("//div[contains(concat(' ', normalize-space(@class), ' '), ' joblist-box__item ')]");
            AppendLog("⏳ 已提取 HTML，正在处理数据...");
            var customerList = new List<Customer>();
            if (jobBoxList?.Count > 0)
            {
                foreach (var item in jobBoxList)
                {
                    // 公司名称
                    var company = item.SelectSingleNode(".//a[contains(@class, 'companyinfo__name')]")?.InnerText.Trim();
                    if (CoName.Contains(company ?? "") || customerList.Count(m => m.Name.Equals(company)) > 0)
                    {
                        continue;
                    }

                    // 职位名称
                    var jobTitle = item.SelectSingleNode(".//a[contains(@class, 'jobinfo__name')]")?.InnerText.Trim();



                    // 联系人
                    var contact = item.SelectSingleNode(".//div[contains(@class, 'companyinfo__staff-name')]")?.InnerText.Trim();

                    // 薪资
                    var salary = item.SelectSingleNode(".//p[contains(@class, 'jobinfo__salary')]")?.InnerText.Trim();

                    // 工作地点
                    var location = item.SelectSingleNode(".//div[contains(@class, 'jobinfo__other-info-item')]/span")?.InnerText.Trim();

                    // 公司性质，规模
                    var otherInfoItems = item.SelectNodes(".//div[contains(@class, 'companyinfo__tag')]/div");
                    string CoProperty = otherInfoItems?.ElementAtOrDefault(0)?.InnerText.Trim();
                    string CoSize = otherInfoItems?.ElementAtOrDefault(1)?.InnerText.Trim();

                    var customer = new Customer()
                    {
                        Area = location,
                        Contacts = contact,
                        CoProperty = CoProperty,
                        CoSize = CoSize,
                        JobTitle = jobTitle,
                        Name = company,
                        Salary = salary,
                        Status = 0,
                        StatusName = "未联系",
                        CreatedAt = DateTime.Now
                    };
                    customerList.Add(customer);
                }
                var flag = await _customerService.AddListAsync(customerList);
                if (flag.Status)
                {
                    dataC += 20;
                    AppendLog("✅ HTML 提取成功，已入库20条数据");
                }
                else
                {
                    AppendLog("✅ 入库失败，请联系管理员");
                }
            }
            else
            {
                AppendLog("✅ 没有找到数据。。。");
            }

            if (_isstop)
            {
                return;
            }

            var numC = Convert.ToInt32(((ContentControl)DataCount.SelectedItem).Content);

            if (dataC < numC)
            {
                SearchByKeyword((dataC / 20) + 1);
            }
            else
            {
                dataC = 0;
                _issearch = false;
            }




        }
        /// <summary>
        /// 点击下一页
        /// </summary>
        private async void ClickNextPageAsync()
        {

            await MyBrowser.ExecuteScriptAsync("window.scrollTo(0, document.body.scrollHeight);");
            await Task.Delay(5000);  // 等待页面渲染
            AppendLog("⏳ 尝试点击“下一页”...");

            // 检查“下一页”是否存在
            var exists = await MyBrowser.ExecuteScriptAsync(
                "document.querySelector('.soupager a.soupager__btn[href*=\"/p\"]') !== null");

            if (exists != "true")
            {
                AppendLog("⚠️ 未找到下一页按钮，可能已是最后一页。");

            }

            // 点击“下一页”
            await MyBrowser.ExecuteScriptAsync(
               "document.querySelector('.soupager a.soupager__btn[href*=\"/p\"]')?.click();");

            AppendLog("✅ 已点击“下一页”按钮，等待加载...");

            // 等待加载页面内容（页面跳转需要些时间）
            await Task.Delay(6000);  // 可改为更智能的监听

            // 返回 true 表示成功点击

        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Go_Click(object sender, RoutedEventArgs e)
        {
            if (AppUser.Instance.RoleName != "管理员")
            {
                Growl.Info(" 您没有权限操作 ！");
                return;
            }
            _isstop = false;

            if (_isphone)
            {
                AppendLog("⏳ 正在提取号码，请稍后在操作");
                return;
            }
            else
            {
                if (_issearch)
                {
                    AppendLog("⏳ 正在获取公司资料，请稍后再试！");
                }
                else
                {
                    _issearch = true;
                    AppendLog("⏳ 开始执行。。。");
                    LoadUrl();

                }

            }

        }

        /// <summary>
        /// 开始获取号码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Phone_Click(object sender, RoutedEventArgs e)
        {
            if (AppUser.Instance.RoleName != "管理员")
            {
                Growl.Info(" 您没有权限操作 ！");
                return;
            }

            _isstop = false;
            if (_isphone)
            {
                AppendLog("⏳ 正在提取号码，请稍后在操作");
                return;
            }
            else
            {
                if (_issearch)
                {
                    AppendLog("⏳ 正在获取公司资料，请稍后再试！");
                }
                else
                {
                    _isphone = true;
                    AppendLog("⏳ 开始执行。。。");

                    LoadPhone();

                }


            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (AppUser.Instance.RoleName != "管理员")
            {
                Growl.Info(" 您没有权限操作 ！");
                return;
            }
            _isstop = true;
            _isphone = false;
            _issearch = false;

            AppendLog("⏳ 已停止");

        }

        private async Task<string> GetPhone(string name, string name1 = "的电话")
        {
            AppendLog("⏳ 正在滚动页面...");
            if (page == null)
            {
                var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

                // 创建一个临时上下文（无缓存、无 Cookie）
                var context = await browser.NewContextAsync(new()
                {
                    ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
                    //UserAgent = GetRandomUserAgent(),  // 🧠 可选：随机 UA
                });
                var page = await context.NewPageAsync();
                //var page = await browser.NewPageAsync();
                await page.GotoAsync("https://chat.baidu.com/search", new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 60000 // 延长等待时间
                });
                await Task.Delay(10000);
            }

            AppendLog("⏳ 正在提取 HTML...");
            await Task.Delay(1000);

            await page.FillAsync("#chat-input-box", $"{name} {name1}");
            await Task.Delay(2000);
            await page.ClickAsync(".send-icon");
            // ✅ 等待搜索结果出现（你可以根据实际 class 名调整）
            await Task.Delay(10000);
            var div = "";
            int divC = 0;
            for (int i = 0; i < 15; i++)
            {
                // 获取当前页面完整 HTML
                string content = await page.ContentAsync(); // 或 await page.InnerHTMLAsync("body")
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);
                var root = htmlDoc.DocumentNode;
                var divs = page.Locator("div.cosd-markdown.cos-space-mt-lg");
                var divsQ = page.Locator("div.index_question-container__VKhsH");
                var copys = page.Locator("div.cs-answer-hover-menu");
                // 获取数量
                int count = await divs.CountAsync();
                int countcopy = await copys.CountAsync();
                int countQ = await divsQ.CountAsync();

                var likes = page.Locator("span", new() { HasTextString = "赞" });
                int likeCount = await likes.CountAsync();
                if (count*2 == likeCount&& countQ*2 == likeCount)
                {
                    div = await divs.Nth(count - 1).InnerTextAsync();
                    break;
                }
                await Task.Delay(3000);
                if (i == 14)
                {
                    div = await divs.Nth(count - 1).InnerTextAsync();
                }
              //  div = await divs.Nth(count - 1).InnerTextAsync();
            }
          
            string cleanedText = Regex.Replace(div, @"(\s\d+)(?=[。.\s]|$)", "");
            string cleanedText1 = Regex.Replace(cleanedText, @"\s*(\r?\n)+\s*", "\n").Replace("\n。", "。");
            AppendLog("✅ " + name + name1+":" + cleanedText1);

            return cleanedText1;


        }



        private async void LoadPhone()
        {

            var result = await _customerService.GetAllAsync();//获取手机号为空的，且未联系的数据
            if (result.Status)
            {
                var data = (result.Data as List<Customer>);
                if (data != null && data.Count > 0)
                {
                    foreach (var customer in data)
                    {
                        AppendLog($"⏳ 正在获取{customer.Name}的电话");
                        string text = await GetPhone(customer.Name);
                        await Task.Delay(5000);
                        string intro = await GetPhone(customer.Name, "的简介");
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            MatchCollection phoneMatches = Regex.Matches(text, @"(\d{3,4}-\d{7,8})|(1[3-9]\d{9})");
                            var phone = string.Join(",", phoneMatches.Select(m => m.Value).Distinct());
                            customer.Phone = phone;
                            customer.Remark = text;
                            customer.Intro = intro;
                            var db = await _customerService.UpdateAsync(customer);
                            AppendLog($"✅ 成功获取{customer.Name}的电话");
                            await Task.Delay(2000);

                        }
                        if (_isstop)
                        {
                            break;
                        }
                    }


                    AppendLog($"✅ 获取电话结束。");
                }
                else
                {
                    AppendLog("✅ 没有要获取号码的数据");
                }
            }
            _isphone = false;
        }




        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="message"></param>
        private void AppendLog(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var paragraph = new System.Windows.Documents.Paragraph();
            paragraph.Inlines.Add(new System.Windows.Documents.Run($"[{timestamp}] {message}\n"));
            StatusTextBlock.Document.Blocks.Add(paragraph);
            StatusTextBlock.ScrollToEnd();
        }

        string GetRandomUserAgent()
        {
            var agents = new[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/124.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 Chrome/122.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Linux; Android 12; Pixel 6) AppleWebKit/537.36 Chrome/121.0.0.0 Mobile Safari/537.36"
             };
            var rand = new Random();
            return agents[rand.Next(agents.Length)];
        }

      
    }

    public class Areas
    {
        public string name { get; set; }
        public string code { get; set; }
    }
}
