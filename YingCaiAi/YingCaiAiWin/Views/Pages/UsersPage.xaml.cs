using DocumentFormat.OpenXml.Drawing.Charts;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// UsersPage.xaml 的交互逻辑
    /// </summary>
    public partial class UsersPage : INavigableView<ViewModels.UsersPageViewModel>
    {
        public ViewModels.UsersPageViewModel ViewModel { get; }

        private double height = SystemParameters.PrimaryScreenHeight;

        public UsersPage(UsersPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

            this.Loaded += (s, e) =>
            {
                var parentWindow = System.Windows.Window.GetWindow(this);
                if (parentWindow != null)
                {
                    parentWindow.StateChanged += Window_StateChanged;

                }
            };

            // 在窗口的构造函数中添加
        }

        private void Window_StateChanged(object sender, EventArgs e)
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
