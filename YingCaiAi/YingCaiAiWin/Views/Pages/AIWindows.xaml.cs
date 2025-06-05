using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using Microsoft.Playwright;
using Microsoft.Web.WebView2.Core;
using NPOI.SS.Formula.Functions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Models;
using YingCaiAiWin.ViewModels;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// AIWindows.xaml 的交互逻辑
    /// </summary>
    public partial class AIWindows : INavigableView<ViewModels.AIWindowsViewModel>
    {
        private List<Documents> questions = new List<Documents>();

        private bool isFullscreen = false;
        private bool isQuestionsAlternate = false;
        private string defaultHomePage = "https://www.baidu.com";
        private double height = SystemParameters.PrimaryScreenHeight;

        private readonly HttpClientHelper _httpClient;

        public AIWindowsViewModel ViewModel { get; set; }

        private readonly IAiRecordService _aiRecordService;
        private readonly IDocumentsService _service;

        private bool isNet = false;
        private bool isSearch = true;
        private IPage page;

        public AIWindows(AIWindowsViewModel viewModel, IAiRecordService aiRecordService, IDocumentsService service)
        {

            ViewModel = viewModel;
            _aiRecordService = aiRecordService;
            _service = service;
            _httpClient = new HttpClientHelper();
            DataContext = this;
            InitializeComponent();
            InitializeBrowser();
            LoadQuestions();
            this.Loaded += (s, e) =>
            {

                var parentWindow = System.Windows.Window.GetWindow(this);
                if (parentWindow != null)
                {
                    if (parentWindow.WindowState != WindowState.Minimized)
                    {
                        parentWindow.WindowState = WindowState.Maximized;
                    }
                    parentWindow.StateChanged += Window_StateChangedAI;

                }
            };

            //ChatBox.AddMessage("今天天气如何？", true);
            //ChatBox.AddMessage("您好，请问有什么可以帮您？", false);
            loadWeb();

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

        /// <summary>
        /// 设置窗口大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChangedAI(object sender, EventArgs e)
        {
            if (!this.IsVisible)
                return;
            var parentWindow = System.Windows.Window.GetWindow(this);
            if (parentWindow != null)
            {
                if (parentWindow.WindowState != WindowState.Minimized)
                {
                    parentWindow.WindowState = WindowState.Maximized;
                }

                //if (parentWindow.WindowState == WindowState.Maximized)
                //{
                //    Gridwin.Height = height - 130;
                //    newsPanel.Height = height - 380;   //全屏固定高度
                //    newScroll.Height = height - 450;
                //    kefuH.Height = height - 200;
                //}

                //else
                //{
                //    Gridwin.Height = 700;
                //    newsPanel.Height = 470; // 非全屏固定高度
                //    newScroll.Height = 400;
                //    kefuH.Height = 660;
                //}
            }

        }


        private async void LoadQuestions()
        {
            questions = await _service.GetAllSystemAsync("猜你想问") ?? new List<Documents>();
            Random rand = new Random();
            QuestionsList.ItemsSource = questions.OrderBy(x => rand.Next()).Take(questions.Count / 2).ToList();
        }



        #region 浏览器功能相关

        private async void InitializeBrowser()
        {

            try
            {
                await EmbeddedBrowser.EnsureCoreWebView2Async();

                EmbeddedBrowser.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                EmbeddedBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
                EmbeddedBrowser.CoreWebView2.Settings.AreDevToolsEnabled = true;

                EmbeddedBrowser.Source = new Uri(defaultHomePage);
                AddressBar.Text = defaultHomePage;

                EmbeddedBrowser.NavigationCompleted += EmbeddedBrowser_NavigationCompleted;
                EmbeddedBrowser.ZoomFactor = 0.8; // 缩放至80%（根据内容调整比例）
                EmbeddedBrowser.CoreWebView2.Settings.IsPinchZoomEnabled = false; // 禁用用户缩放干扰
                // 拦截新窗口（弹窗）
                EmbeddedBrowser.CoreWebView2.NewWindowRequested += (s, e) =>
                {
                    e.Handled = true;
                    EmbeddedBrowser.CoreWebView2.Navigate(e.Uri);

                    if (EmbeddedBrowser.CoreWebView2 != null)
                    {
                        // EmbeddedBrowser.CoreWebView2.ZoomFactor = 0.9;

                        EmbeddedBrowser.CoreWebView2.ExecuteScriptAsync(@"
                                        document.body.style.zoom = '80%';
                                        document.body.style.overflowX = 'hidden';
                                        let style = document.createElement('style');
                                        style.innerHTML = `
                                            * {
                                                max-width: 100vw !important;
                                                box-sizing: border-box !important;
                                            }
                                            body {
                                                overflow-x: hidden !important;
                                            }
                                        `;
                                        document.head.appendChild(style);
                                    ");
                    }
                };

            }
            catch (Exception ex)
            {
                // 可以写日志
            }


        }
        private void EmbeddedBrowser_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (EmbeddedBrowser != null && EmbeddedBrowser.Source != null)
                {
                    AddressBar.Text = EmbeddedBrowser.Source.ToString();
                }
            }
            catch
            {
                // 忽略异常
            }
        }
        private void NavigateToUrl(string url)
        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                EmbeddedBrowser.Source = new Uri(url);
                AddressBar.Text = url;
            }
            catch (Exception)
            {
                // 可弹提示
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmbeddedBrowser.CanGoBack)
                {
                    EmbeddedBrowser.GoBack();
                }
            }
            catch
            {
                // 静默处理异常
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmbeddedBrowser.CanGoForward)
                {
                    EmbeddedBrowser.GoForward();
                }
            }
            catch
            {
                // 静默处理异常
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmbeddedBrowser.Reload();
            }
            catch
            {
                // 静默处理异常
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmbeddedBrowser.Source = new Uri(defaultHomePage);
                AddressBar.Text = defaultHomePage;
            }
            catch
            {
                // 静默处理异常
            }
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrl(AddressBar.Text);
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrl(AddressBar.Text);
        }
        #endregion



        #region ai对话相关

        //工具栏
        private async void Tool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ToolItem tool)
            {
                string text = tool.Title;

                if (isSearch)
                {
                    isSearch = false;
                    if (text == "行业新闻")
                    {
                        text = "今日建筑化工行业相关新闻热点";
                        ChatBox.AddMessage(text, true);

                        ChatBox.AddLoadingBubble();
                        // 滚动到底部
                        Scroll();

                        string retext = await GetNetSearch(text);
                        ChatBox.ReplaceLoadingBubble(retext, text);
                        Scroll(); // 最后再滚动一次，确保展示完整
                        isSearch = true;
                        return;
                    }

                    ViewModel.ShowCustomer(text);
                    //ChatBox.AddLoadingBubble();
                    // 滚动到底部
                    // Scroll();
                    // 关闭展开框
                    expander.IsExpanded = false;
                    isSearch = true;
                    await Task.Delay(2000);
                }



                //ChatBox.ReplaceLoadingBubble(text,text);


            }

        }

        /// <summary>
        /// 猜你想问
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Question_Click(object sender, RoutedEventArgs e)
        {

            if (sender is CardAction card && card.DataContext is Documents question)
            {
                if (isSearch)
                {
                    isSearch = false;
                    var cards = sender as CardAction;
                    string text = cards?.Tag?.ToString();
                    ChatBox.AddMessage(text, true);
                    ChatBox.AddLoadingBubble();
                    // 滚动到底部
                    Scroll();

                    await CallVectorizeApiAsync(text);
                    Scroll(); // 最后再滚动一次，确保展示完整
                    isSearch = true;
                }

            }
        }

        private void RefreshQuestions_Click(object sender, MouseButtonEventArgs e)
        {
            // 切换问题集合
            isQuestionsAlternate = !isQuestionsAlternate;
            Random rand = new Random();
            if (isQuestionsAlternate)
            {
                QuestionsList.ItemsSource = questions.OrderBy(x => rand.Next()).Take(questions.Count / 2).ToList();
            }
            else
            {
                QuestionsList.ItemsSource = questions.OrderBy(x => rand.Next()).Take(questions.Count / 2).ToList();
            }


        }

        /// <summary>
        /// 搜索框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            // 用户按下 Enter，并且没有按住 Shift
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = true; // 阻止默认回车换行
                if (isSearch)
                {
                    isSearch = false;
                    string text = SearchBox.Text?.Trim();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        ChatBox.AddMessage(text, true);
                        SearchBox.Text = "";
                        ChatBox.AddLoadingBubble();
                        // 滚动到底部
                        Scroll();

                        await CallVectorizeApiAsync(text);
                    }
                    isSearch = true;
                }
            }


        }
        private async Task CallVectorizeApiAsync(string text)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                // 开始计时
                stopwatch.Start();
                if (IsNet.IsChecked == false)
                {
                    var response = await _httpClient.PostDataAsync("milvus/ask", new { text, top_k = 10 });
                    if (response != null)
                    {
                        // 停止计时
                        stopwatch.Stop();
                        if (response.Contains("</think>"))
                        {
                            var aimodel = JsonSerializer.Deserialize<AiModelRes>(response);
                            var aitext = aimodel.answer;
                            int n = aitext.IndexOf("</think>");
                            if (n > 0)
                            {
                                var retext = aitext.Substring(n + 10);
                                ChatBox.ReplaceLoadingBubble(retext, text);
                                Scroll(); // 最后再滚动一次，确保展示完整

                                Task.Run(() =>
                                {
                                    // 获取执行时间
                                    TimeSpan elapsedTime = stopwatch.Elapsed;
                                    var ai = new AiRecord()
                                    {
                                        Question = text,
                                        Answer = retext,
                                        CreatedAt = DateTime.Now,
                                        CreatedUser = AppUser.Instance.Username,
                                        Times = (int)elapsedTime.TotalSeconds,
                                    };
                                    _aiRecordService.AddAsync(ai);

                                });
                            }
                        }
                        isSearch = true;

                    }
                    else
                    {
                        ChatBox.ReplaceLoadingBubble("检索超时。。。");
                        Scroll(); // 最后再滚动一次，确保展示完整
                        isSearch = true;
                    }
                }
                else
                {
                    string retext = await GetNetSearch(text);
                    ChatBox.ReplaceLoadingBubble(retext, text);
                    Scroll(); // 最后再滚动一次，确保展示完整
                    Task.Run(() =>
                    {
                        // 获取执行时间
                        TimeSpan elapsedTime = stopwatch.Elapsed;
                        var ai = new AiRecord()
                        {
                            Question = text,
                            Answer = retext,
                            CreatedAt = DateTime.Now,
                            CreatedUser = AppUser.Instance.Username,
                            Times = (int)elapsedTime.TotalSeconds,
                        };
                        _aiRecordService.AddAsync(ai);

                    });
                    isSearch = true;
                }

            }
            catch (Exception ex)
            {
                isSearch = true;
                ChatBox.ReplaceLoadingBubble("检索超时了。。。");
                Scroll(); // 最后再滚动一次，确保展示完整
            }
        }


        /// <summary>
        /// 滚动到最底部
        /// </summary>
        private void Scroll()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var scrollViewer = FindParent<ScrollViewer>(newStackPanel);
                scrollViewer?.ScrollToEnd();
            }), DispatcherPriority.Background);
        }
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent) return parent;

            return FindParent<T>(parentObject);
        }

        #endregion


        private async Task<string> GetNetSearch(string name)
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


            await page.FillAsync("#chat-input-box", $"{name}");
            await Task.Delay(1000);
            await page.ClickAsync(".send-icon");
            await Task.Delay(10000);
            var div = "";
            for (int i = 0; i < 8; i++)
            {
                // 获取当前页面完整 HTML
                string content = await page.ContentAsync(); // 或 await page.InnerHTMLAsync("body")
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);
                var root = htmlDoc.DocumentNode;
                var divs = page.Locator("div.cosd-markdown.cos-space-mt-lg");

                // 获取数量
                int count = await divs.CountAsync();

                if (count > 0)
                {
                    // 获取最后一个元素的文本
                    div = await divs.Nth(count - 1).InnerTextAsync();
                    await Task.Delay(2000);
                }
                else
                {
                    await Task.Delay(5000);
                }


            }

            string cleanedText = Regex.Replace(div, @"(\s\d+)(?=[。.\s]|$)", "");
            string cleanedText1 = Regex.Replace(cleanedText, @"\s*(\r?\n)+\s*", "\n").Replace("\n。", "。");
            isSearch = true;
            //await browser.CloseAsync();
            return cleanedText;

        }


    }

    public class AiModelRes
    {
        public string answer { get; set; }
    }
}
