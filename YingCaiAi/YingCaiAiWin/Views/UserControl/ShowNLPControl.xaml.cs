using Markdig;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
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
using YingCaiAiModel;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// ShowNLPControl.xaml 的交互逻辑
    /// </summary>
    public partial class ShowNLPControl : UserControl
    {
        private readonly TaskCompletionSource<(bool confirmed, string username, string role)> _tcs =
       new TaskCompletionSource<(bool, string, string)>();
        public ShowNLPControl()
        {
            
            InitializeComponent();
            this.Loaded += ShowNLPControl_Loaded;
        }
        private async void ShowNLPControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // 模拟获取 Markdown 内容
            string markdown = "";
            if (this.DataContext is AudioRecord model )
            {
                 markdown = model.Sentiment;
            }


            // 将 Markdown 转为 HTML
            string html = Markdown.ToHtml(markdown);
            string fullHtml = $@"
                    <html>
                    <head>
                    <meta charset='utf-8'>
                    <style>
                    body {{ font-family: 'Segoe UI'; padding: 1em; background-color: #f8f9fa; color: #212529; }}
                    h1 {{ color: #0d6efd; }}
                    </style>
                    </head>
                    <body>{html}</body>
                    </html>";

            // 确保 WebView2 初始化完成
            await MyBrowser.EnsureCoreWebView2Async();
            MyBrowser.NavigateToString(fullHtml);
        }
    

    public Task<(bool confirmed, string username, string role)> ShowAsync() => _tcs.Task;

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            _tcs.TrySetResult((false, null, null));
        }

        private void OnConfirmClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
