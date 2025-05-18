using Wpf.Ui.Abstractions.Controls;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// AiRecordPage.xaml 的交互逻辑
    /// </summary>
    public partial class AiRecordPage : INavigableView<ViewModels.AiRecordViewModel>
    {
        public ViewModels.AiRecordViewModel ViewModel { get; }
        private double height = SystemParameters.PrimaryScreenHeight;
        public AiRecordPage(AiRecordViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                var parentWindow = System.Windows.Window.GetWindow(this);
                if (parentWindow != null)
                {
                    Window_StateChangedKB(s, e);
                    parentWindow.StateChanged += Window_StateChangedKB;

                }
            };
        }
        private void Window_StateChangedKB(object sender, EventArgs e)
        {

            var parentWindow = System.Windows.Window.GetWindow(this);
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Maximized)
                {

                    DocsDataGrid.Height = height - 230; ;  //全屏固定高度
                }

                else
                {
                    DocsDataGrid.Height = 620; // 非全屏固定高度
                }
            }

        }
    }
}
