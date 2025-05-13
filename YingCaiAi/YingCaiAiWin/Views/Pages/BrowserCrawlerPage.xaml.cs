using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
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
using YingCaiAiWin.ViewModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// BrowserCrawlerPage.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserCrawlerPage : Page
    {
        private bool _isInitialized = false;
      
      
        public BrowserCrawlerPage()
        {
            DataContext = this;
            if (!_isInitialized)
            {
                InitializeComponent();
                LoadData();
            }
            Loaded += BrowserCrawlerPage_Loaded;
        }

        public void LoadData()
        {
            _isInitialized = true;
        }
        private async void BrowserCrawlerPage_Loaded(object sender, RoutedEventArgs e)
        {
            await MyBrowser.EnsureCoreWebView2Async();
        }

        private void LoadUrl()
        {
            var url = UrlTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(url))
            {
                MyBrowser.Source = new Uri(url);
                AppendLog($"✅ 状态：已加载 {url}");
            }
        }

        private void SearchByKeyword()
        {
            var keyword = KeywordTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var encoded = Uri.EscapeDataString(keyword);
                var searchUrl = $"{UrlTextBox.Text.Trim()}?kw={encoded}";
                MyBrowser.Source = new Uri(searchUrl);
                AppendLog($"✅ 状态：搜索关键词 {keyword}");
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



            File.WriteAllText("zhaopin_page.html", cleanHtml);
            AppendLog("✅ HTML 提取成功，已保存到 zhaopin_page.html");
        }

     

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            //加载页面
            LoadUrl();
            SearchByKeyword();

            //ExtractHtml();
            ClickNextPageAsync();
        }
        private async void ClickNextPageAsync()
        {
         
                await MyBrowser.ExecuteScriptAsync("window.scrollTo(0, document.body.scrollHeight);");
                await Task.Delay(5000);  // 等待页面渲染
                AppendLog("⏳ 尝试点击“下一页”...");

                // 检查“下一页”是否存在
                var exists =await MyBrowser.ExecuteScriptAsync(
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

        private void AppendLog(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run($"[{timestamp}] {message}\n"));
            StatusTextBlock.Document.Blocks.Add(paragraph);
            StatusTextBlock.ScrollToEnd();
        }
    }
}
