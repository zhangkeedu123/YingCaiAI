using DocumentFormat.OpenXml.Drawing.Charts;
using HandyControl.Controls;
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
using Wpf.Ui.Controls;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// KnowledgeBase.xaml 的交互逻辑
    /// </summary>
    public partial class KnowledgeBase : INavigableView<ViewModels.KnowledgeBaseViewModel>
    {
        public ViewModels.KnowledgeBaseViewModel ViewModel { get; }
        private  double height =  SystemParameters.PrimaryScreenHeight;

        public KnowledgeBase(ViewModels.KnowledgeBaseViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
           
            InitializeComponent();
            UserDialog.Visibility = Visibility.Hidden;
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

                    DocsDataGrid.Height = height - 280; ;  //全屏固定高度
                }

                else
                {
                    DocsDataGrid.Height = 570; // 非全屏固定高度
                }
            }
           
        }

    }
}
