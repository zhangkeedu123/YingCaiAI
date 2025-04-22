using CommunityToolkit.Mvvm.ComponentModel;


namespace YingCaiAiWin.ViewModels;

public partial class AIWindowsViewModel : ViewModel
{
    [ObservableProperty]
    private string _currentPageTitle = "AI 智能助手";

    [ObservableProperty]
    private bool _isInitialized;

    public void InitializeViewModel()
    {
        if (!_isInitialized)
        {
            // 初始化数据逻辑
            _isInitialized = true;
        }
    }
    
    // 添加与XAML中控件绑定的属性和命令
    [RelayCommand]
    private void NavigateToDetail(string parameter)
    {
        // 导航逻辑
    }
}
