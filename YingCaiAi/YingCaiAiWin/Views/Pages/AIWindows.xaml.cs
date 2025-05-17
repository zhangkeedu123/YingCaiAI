using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
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

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// AIWindows.xaml 的交互逻辑
    /// </summary>
    public partial class AIWindows : INavigableView<ViewModels.AIWindowsViewModel>
    {
        private ObservableCollection<string> questions = new ObservableCollection<string>
        {
            "当前客户的开场白?",
            "当前客户是否正在其他平台招聘?",
            "输入客户预算，为你推荐合适套餐?",
            "入驻审核失败的原因是什么?",
            "想知道最近的平台活跃度吗?",
            "企业相关专业知识?",
            "还会想好?"
        };

        private ObservableCollection<string> alternateQuestions = new ObservableCollection<string>
        {
            "如何提高招聘效率?",
            "企业如何设置更有吸引力的职位?",
            "如何查看应聘者简历?",
            "如何联系客服人员?",
            "平台使用有哪些注意事项?",
            "如何提高职位曝光率?",
            "如何评估招聘效果?"
        };

        private bool isFullscreen = false;
        private bool isQuestionsAlternate = false;
        private string defaultHomePage = "https://www.baidu.com";
        private double height = SystemParameters.PrimaryScreenHeight;

        private readonly HttpClientHelper _httpClient;

        public AIWindowsViewModel ViewModel { get; set; }

        private readonly IAiRecordService _aiRecordService;

        public AIWindows(AIWindowsViewModel viewModel, IAiRecordService aiRecordService)
        {

            ViewModel = viewModel;
            _aiRecordService = aiRecordService;
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
                    parentWindow.StateChanged += Window_StateChangedAI;

                }
            };



            //ChatBox.AddMessage("今天天气如何？", true);
            //ChatBox.AddMessage("您好，请问有什么可以帮您？", false);

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
                if (parentWindow.WindowState == WindowState.Maximized)
                {
                    Gridwin.Height = height - 150;
                    newsPanel.Height = height - 380;   //全屏固定高度
                    newScroll.Height = height - 450;
                    kefuH.Height = height - 200;
                }

                else
                {
                    Gridwin.Height = 700;
                    newsPanel.Height = 470; // 非全屏固定高度
                    newScroll.Height = 400;
                    kefuH.Height = 660;
                }
            }

        }


        private void LoadQuestions()
        {
            QuestionsList.ItemsSource = questions;
        }



        #region 浏览器功能相关

        private void InitializeBrowser()
        {
            try
            {
                // 禁用脚本错误
                DisableScriptErrors();

                // 设置浏览器初始页面
                EmbeddedBrowser.Navigate(new Uri(defaultHomePage));
                AddressBar.Text = defaultHomePage;

                // 处理浏览器导航完成事件
                EmbeddedBrowser.Navigated += EmbeddedBrowser_Navigated;

            }
            catch (Exception)
            {
                // 静默处理异常，不显示错误消息
            }
        }

        private void EmbeddedBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {

                // 更新地址栏
                AddressBar.Text = e.Uri.ToString();

                // 隐藏脚本错误
                DisableScriptErrors();
            }
            catch
            {
                // 静默处理异常
            }
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // 获取WebBrowser的底层ActiveX对象
            var webBrowserActiveX = EmbeddedBrowser.GetType().InvokeMember("ActiveXInstance",
                System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, EmbeddedBrowser, null) as SHDocVw.WebBrowser;

            if (webBrowserActiveX != null)
            {
                // 订阅NewWindow2事件
                //webBrowserActiveX.NewWindow2 += (ref object ppDisp, ref bool Cancel) =>
                //{
                //    Cancel = true  ; // 取消新窗口
                //};
                webBrowserActiveX.NewWindow3 += (ref object ppDisp, ref bool Cancel, uint dwFlags,
         string bstrUrlContext, string bstrUrl) =>
                {
                    Cancel = true; // 取消新窗口
                    EmbeddedBrowser.Navigate(bstrUrl); // 在当前窗口打开
                };

            }
        }

        private void DisableScriptErrors()
        {
            try
            {
                // 获取浏览器COM对象
                dynamic activeX = EmbeddedBrowser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, EmbeddedBrowser, new object[] { });

                // 禁用脚本错误
                activeX.Silent = true;

                // 禁用IE错误对话框
                if (EmbeddedBrowser.Document != null)
                {
                    //((HTMLDocument)EmbeddedBrowser.Document).parentWindow.onerror =
                    //    new Func<string, string, int, bool>((message, url, line) => true);
                }
            }
            catch
            {
                // 静默处理异常
            }
        }
        private void NavigateToUrl(string url)
        {
            try
            {
                // 确保URL格式正确
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                // 导航到指定URL
                EmbeddedBrowser.Navigate(new Uri(url));
                AddressBar.Text = url;
            }
            catch (Exception ex)
            {
                // 显示导航错误提示
                //Wpf.Ui.Controls.Snackbar.Show(
                //    "导航错误",
                //    $"无法访问该网址: {ex.Message}",
                //    Wpf.Ui.Common.SymbolRegular.ErrorCircle24);
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
                EmbeddedBrowser.Refresh();
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
                EmbeddedBrowser.Navigate(new Uri(defaultHomePage));
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

        private async void Tool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ToolItem tool)
            {
                string text = tool.Title;

                ChatBox.AddLoadingBubble();
                // 滚动到底部
                Scroll();
                // 关闭展开框
                expander.IsExpanded = false;
                await Task.Delay(2000);

                ChatBox.ReplaceLoadingBubble(text);


            }

        }
        private async void Question_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CardAction card && card.DataContext is string question)
            {

                var cards = sender as CardAction;
                string text = cards?.Tag?.ToString();
                ChatBox.AddLoadingBubble();
                // 滚动到底部
                Scroll();
                await Task.Delay(2000);

                ChatBox.ReplaceLoadingBubble(text);
                Scroll(); // 最后再滚动一次，确保展示完整
            }
        }

        private void RefreshQuestions_Click(object sender, MouseButtonEventArgs e)
        {
            // 切换问题集合
            isQuestionsAlternate = !isQuestionsAlternate;

            if (isQuestionsAlternate)
            {
                QuestionsList.ItemsSource = alternateQuestions;
            }
            else
            {
                QuestionsList.ItemsSource = questions;
            }


        }
        /// <summary>
        /// 搜索框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
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

            }
        }
        private async Task CallVectorizeApiAsync(string text)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                // 开始计时
                stopwatch.Start();
                var response = await _httpClient.PostDataAsync("milvus/ask", new { text, top_k = 10 });
                if (response != null)
                {
                    // 停止计时
                    stopwatch.Stop();
                    if (response.Contains("</think>"))
                    {
                        int n = response.IndexOf("</think>");
                        if (n > 0)
                        {

                            var retext = Regex.Replace(response.Substring(n + 12), @"[\n""}]", ""); ;
                            retext = retext.Replace("\\n", "\n");
                            ChatBox.ReplaceLoadingBubble(retext);
                            Scroll(); // 最后再滚动一次，确保展示完整

                            Task.Run( () =>
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

                }
                else
                {
                    ChatBox.ReplaceLoadingBubble("检索超时。。。");
                    Scroll(); // 最后再滚动一次，确保展示完整
                }
            }
            catch (Exception ex)
            {
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


    }
}
