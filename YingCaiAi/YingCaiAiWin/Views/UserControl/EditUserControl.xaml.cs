using System.Windows.Controls;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// EditUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class EditUserControl : UserControl
    {

        public event Action<string>? PasswordChanged; // 或者 Action<Users>，更灵活

        private readonly TaskCompletionSource<(bool confirmed, string username, string role)> _tcs =
            new TaskCompletionSource<(bool, string, string)>();

        public EditUserControl()
        {
            InitializeComponent();
        }
        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            var realPassword = PasswordBoxControl.Password;
            PasswordChanged?.Invoke(realPassword);
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
