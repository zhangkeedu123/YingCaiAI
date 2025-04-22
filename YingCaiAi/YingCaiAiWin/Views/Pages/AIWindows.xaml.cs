using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using YingCaiAiWin.ViewModels;
using SHDocVw;

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
        private string defaultHomePage = "https://www.bing.com";

        public AIWindowsViewModel ViewModel { get; set; }

        public AIWindows(AIWindowsViewModel viewModel)
        {
            ViewModel= viewModel;
            InitializeComponent();
            InitializeBrowser();
            LoadQuestions();
           
        }
        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            //string url = ((WebBrowser)sender).StatusText;
            //EmbeddedBrowser.Navigate(url);
            e.Cancel = true;
        }
        //private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    //将所有的链接的目标，指向本窗体
        //    foreach (HtmlElement archor in this.webBrowser1.Document.Links)
        //    {
        //        archor.SetAttribute("target", "_self");
        //    }

        //    //将所有的FORM的提交目标，指向本窗体
        //    foreach (HtmlElement form in this.webBrowser1.Document.Forms)
        //    {
        //        form.SetAttribute("target", "_self");
        //    }
        //}

        private void LoadQuestions()
        {
            QuestionsList.ItemsSource = questions;
        }

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
                webBrowserActiveX.NewWindow2 += (ref object ppDisp, ref bool Cancel) =>
                {
                    Cancel = true; // 取消新窗口
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

        #region 浏览器导航功能
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

        #region 全屏和最小化控制
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Minimized;
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }

        private void ToggleFullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }

        private void ToggleFullscreen()
        {
            if (isFullscreen)
            {
                // 退出全屏
                ///this.WindowState = WindowState.Normal;
                //FullscreenIcon.Symbol = Wpf.Ui.Common.SymbolRegular.FullScreenMaximize24;
            }
            else
            {
                // 进入全屏
                //this.WindowState = WindowState.Maximized;
                //this.WindowState = WindowState.Maximized;
                //FullscreenIcon.Symbol = Wpf.Ui.Common.SymbolRegular.FullScreenMinimize24;
            }

            isFullscreen = !isFullscreen;
        }
        #endregion

        #region 自助工具点击事件
        private void CustomerInfo_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("客户资料", "查看和管理客户的详细信息，包括联系方式、历史记录等。");
        }

        private void Recruiting_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("正在招聘", "查看当前客户正在招聘的职位信息和申请状态。");
        }

        private void RecommendedScripts_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("推荐话术", "根据客户需求和历史数据，提供个性化的沟通话术建议。");
        }

        private void EnterprisePackage_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("企业套餐", "查看和推荐适合客户的企业服务套餐方案。");
        }

        private void Promotions_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("优惠政策", "了解平台最新的优惠活动和政策信息。");
        }

        private void PlatformData_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("平台数据", "查看平台运营数据和客户使用统计信息。");
        }

        private void AfterSales_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("售后服务", "提供售后问题解决方案和服务支持。");
        }

        private void ChickenSoup_Click(object sender, RoutedEventArgs e)
        {
            ShowToolInfo("心灵鸡汤", "提供职场励志和企业管理相关的心灵鸡汤内容。");
        }

        private void ShowToolInfo(string title, string content)
        {
            //Wpf.Ui.Controls.MessageBox.Show(
            //    title,
            //    content,
            //    Wpf.Ui.Controls.MessageBoxButton.OK);
        }
        #endregion

        #region 猜你想问相关事件
        private void Question_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CardAction card && card.DataContext is string question)
            {
                // 处理问题点击事件
                //Wpf.Ui.Controls.MessageBox.Show(
                //    "问题详情",
                //    $"您的问题：{question}\n\n我们正在为您准备答案...",
                //    Wpf.Ui.Controls.MessageBoxButton.OK);
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

            // 显示提示
            //Wpf.Ui.Controls.Snackbar.Show("已更新", "问题列表已刷新", Wpf.Ui.Common.SymbolRegular.CheckmarkCircle24);
        }
        #endregion

        #region 搜索框事件
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
        }

        private void PerformSearch()
        {
            string searchText = SearchBox.Text?.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                // 执行搜索操作
                //Wpf.Ui.Controls.Snackbar.Show(
                //    "搜索",
                //    $"正在搜索：{searchText}",
                //    Wpf.Ui.Common.SymbolRegular.Search24);

                // 可以在这里添加实际的搜索逻辑
                // 例如：在浏览器中搜索
                try
                {
                    string searchUrl = $"https://www.bing.com/search?q={Uri.EscapeDataString(searchText)}";
                    EmbeddedBrowser.Navigate(new Uri(searchUrl));
                    AddressBar.Text = searchUrl;
                }
                catch
                {
                    // 静默处理异常
                }

                // 清空搜索框
                SearchBox.Text = string.Empty;
            }
        }
        #endregion
    }
}
