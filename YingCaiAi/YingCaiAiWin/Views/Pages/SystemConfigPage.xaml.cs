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
using Wpf.Ui.Abstractions.Controls;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// SystemConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class SystemConfigPage : INavigableView<ViewModels.SystemConfigViewModel>
    {
        public ViewModels.SystemConfigViewModel ViewModel { get; }
        private double height = SystemParameters.PrimaryScreenHeight;

        public SystemConfigPage(ViewModels.SystemConfigViewModel viewModel)
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

            // 在窗口的构造函数中添加

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
