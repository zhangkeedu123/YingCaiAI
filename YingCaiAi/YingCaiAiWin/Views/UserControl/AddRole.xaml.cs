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

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// AddRole.xaml 的交互逻辑
    /// </summary>
    public partial class AddRole : UserControl
    {
        private readonly TaskCompletionSource<(bool confirmed, string username, string role)> _tcs =
            new TaskCompletionSource<(bool, string, string)>();
        public AddRole()
        {
            InitializeComponent();
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
