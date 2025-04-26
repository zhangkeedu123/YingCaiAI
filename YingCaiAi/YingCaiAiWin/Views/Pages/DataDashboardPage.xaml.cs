using System.Windows.Controls;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// DataDashboardPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataDashboardPage : Page
    {
        public DataDashboardPage()
        {
            InitializeComponent();
            
            // 设置数据上下文为视图模型
            DataContext = new DataDashboardViewModel();
        }
    }
}