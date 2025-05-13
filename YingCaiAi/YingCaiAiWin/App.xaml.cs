using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using YingCaiAiService;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Models;
using YingCaiAiWin.Services;
using YingCaiAiWin.Views;
using YingCaiAiWin.Views.Pages;
namespace YingCaiAiWin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        private static readonly IHost _host = Host.CreateDefaultBuilder()
     .ConfigureAppConfiguration(c =>
     {
         var basePath =
             Path.GetDirectoryName(AppContext.BaseDirectory)
             ?? throw new DirectoryNotFoundException(
                 "Unable to find the base directory of the application."
             );
         _ = c.SetBasePath(basePath);
     })
     .ConfigureServices(
         (context, services) =>
         {
         _ = services.AddNavigationViewPageProvider();

         //注册service层服务
         _ = services.AddApplicationServices();

         // App Host
         _ = services.AddHostedService<ApplicationHostService>();

         // Theme manipulation
         _ = services.AddSingleton<IThemeService, ThemeService>();

         // TaskBar manipulation
         _ = services.AddSingleton<ITaskBarService, TaskBarService>();

         // Service containing navigation, same as INavigationWindow... but without window
         _ = services.AddSingleton<INavigationService, NavigationService>();

         // Main window with navigation
         _ = services.AddSingleton<INavigationWindow, Views.MainWindow>();
         _ = services.AddSingleton<ViewModels.MainWindowViewModel>();

         // Views and ViewModels
         _ = services.AddSingleton<Views.Pages.DashboardPage>();
         _ = services.AddSingleton<ViewModels.DashboardViewModel>();
         _ = services.AddSingleton<ViewModels.AIWindowsViewModel>();
         _ = services.AddSingleton<Views.Pages.DataPage>();
         _ = services.AddSingleton<ViewModels.DataViewModel>();
         _ = services.AddSingleton<Views.Pages.SettingsPage>();
         _ = services.AddSingleton<ViewModels.SettingsViewModel>();
          _ = services.AddSingleton<IContentDialogService, ContentDialogService>();
             //统一继承依赖注入
             _ = services.AddSingleton<Views.Pages.UsersPage>();
         _ = services.AddSingleton<ViewModels.UsersPageViewModel>();
         _ = services.AddSingleton<Views.Pages.RolesPage>();
         _ = services.AddSingleton<ViewModels.RolePageViewModel>();
         _ = services.AddSingleton<Views.Pages.KnowledgeBase>();
         _ = services.AddSingleton<ViewModels.KnowledgeBaseViewModel>();
         _ = services.AddSingleton<Views.Pages.BrowserCrawlerPage>();


             _ = services.AddSingleton<Views.Login>();//注册登录窗口
         _ = services.AddSingleton<Views.Pages.AIWindows>();//注册登录窗口
                                                            // Configuration
         _ = services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));

             // 配置数据库帮助类
             _ = services.AddScoped<DapperHelper>();
             // new DapperHelper("Host=113.105.116.171;Port=5432;Database=yingcaiai;Username=yingcai;Password=123456zk"));
             //new DapperHelper("Server=113.105.116.171;Port=5432;Database=yingcaiai;User Id=yingcai;Password=123456zk;"));
             Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
         }

     )
     .Build();

        /// <summary>
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
           var loginWindow = _host.Services.GetRequiredService<Login>();
           //var loginWindows = _host.Services.GetRequiredService<AIWindows>();
            //loginWindows.Show();
            loginWindow.Show();
            var scheduler = new VectorizationScheduler();
            scheduler.Start(); // 启动定时任务
            
            await _host.StartAsync();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }

}
