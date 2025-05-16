using NPOI.SS.Formula.Functions;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// DataDashboardPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataDashboardPage : INavigableView<ViewModels.DataDashboardViewModel>
    {
        public ViewModels.DataDashboardViewModel ViewModel { get; }
        private double height = SystemParameters.PrimaryScreenHeight;
        public DataDashboardPage(DataDashboardViewModel viewMode)
        {
            ViewModel = viewMode;
            DataContext =this;
            InitializeComponent();
         

        }
      
    }
}