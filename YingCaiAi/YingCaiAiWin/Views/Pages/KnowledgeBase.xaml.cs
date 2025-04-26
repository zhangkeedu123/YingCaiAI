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
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// KnowledgeBase.xaml 的交互逻辑
    /// </summary>
    public partial class KnowledgeBase : INavigableView<ViewModels.KnowledgeBaseViewModel>
    {
        public ViewModels.KnowledgeBaseViewModel ViewModel { get; }
       
        public KnowledgeBase(ViewModels.KnowledgeBaseViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
           
            InitializeComponent();
            UserDialog.Visibility = Visibility.Hidden;
        }

    }
}
