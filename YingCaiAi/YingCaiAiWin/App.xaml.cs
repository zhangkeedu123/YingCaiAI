using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows.Threading;
using Wpf.Ui;
using YingCaiAiService;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Models;
using YingCaiAiWin.Services;
using YingCaiAiWin.Views;
using YingCaiAiWin.Views.Pages;
using LiveChartsCore.SkiaSharpView.WPF;
namespace YingCaiAiWin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            // 主线程未处理异常
            this.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            // 后台线程未处理异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 异步任务未处理异常
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            
            //在发布文件内powershell执行
            //powershell -ExecutionPolicy Bypass -File .\playwright.ps1 install


        }

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
     
         //_ = services.AddSingleton<ViewModels.MainWindowViewModel>();
        _ = services.AddSingleton<IContentDialogService, ContentDialogService>();
           
             //单独注册需要缓存的页面单例模式
             _ = services.AddSingleton<AIWindows>();
             _ = services.AddSingleton<BrowserCrawlerPage>();
             _ = services.AddSingleton<DataDashboardPage>();
             // All other pages and view models
             _ = services.AddTransientFromNamespace("YingCaiAiWin.Views", GalleryAssembly.Asssembly);
             _ = services.AddTransientFromNamespace(
                 "YingCaiAiWin.ViewModels",
                 GalleryAssembly.Asssembly
             );

             _ = services.AddSingleton<INavigationWindow, Views.MainWindow>();
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
            try
            {
                

                UserStorageHelper.LoadUser(); // 启动自动恢复用户状态
                var loginWindow = _host.Services.GetRequiredService<Login>();
                //var loginWindows = _host.Services.GetRequiredService<AIWindows>();
                //loginWindows.Show();
                loginWindow.Show();
                var scheduler = new VectorizationScheduler();
                scheduler.Start(); // 启动定时任务

                await _host.StartAsync();
               
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            new PaddleOcrService().Stop();
            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogError("UI线程异常", e.Exception);
            e.Handled = true; // 避免程序崩溃
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError("非UI线程异常", e.ExceptionObject as Exception);
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogError("Task未观察异常", e.Exception);
            e.SetObserved(); // 避免进程崩溃
        }

        private void LogError(string type, Exception? ex)
        {
            string logPath = "Logs"; // 日志文件夹
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            string filePath = Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd}.log");
            File.AppendAllText(filePath,
                $"[{DateTime.Now:HH:mm:ss}] [{type}] {ex?.Message}\n{ex?.StackTrace}\n\n");
        }
    }

}
