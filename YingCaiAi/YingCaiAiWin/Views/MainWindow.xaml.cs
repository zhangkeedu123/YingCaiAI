// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;
using Wpf.Ui;
using Microsoft.Extensions.DependencyInjection;
using YingCaiAiWin.Services;
using NHotkey.Wpf;
using System.Windows.Input;
using Newtonsoft.Json;
using NHotkey;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace YingCaiAiWin.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INavigationWindow
{
    public ViewModels.MainWindowViewModel ViewModel { get; }
    private PaddleOcrService _ocrService = new();

    public MainWindow(ViewModels.MainWindowViewModel viewModel, INavigationService navigationService, IContentDialogService contentDialogService ,IServiceProvider serviceProvider)
    {
        ViewModel = viewModel;
        DataContext = this;

        Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);

        InitializeComponent();

        navigationService.SetNavigationControl(RootNavigation);
        contentDialogService.SetDialogHost(RootContentDialog);
        // 注册热键，只保留 OCR 和退出
        HotkeyManager.Current.AddOrReplace("OcrClipboardImage", Key.X, ModifierKeys.Control | ModifierKeys.Shift, OnOcrClipboardImage);
        // ✅ 设置“只缓存 AIWindows 页”的服务
        SetPageService(new PartialCachedPageProvider(serviceProvider));
    }

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) =>
        RootNavigation.SetPageProviderService(navigationViewPageProvider);

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    INavigationView INavigationWindow.GetNavigation()
    {
        throw new NotImplementedException();
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    private async void OnOcrClipboardImage(object sender, HotkeyEventArgs e)
    {
        e.Handled = true;
        await OcrClipboardImageAsync();
    }
    public async Task OcrClipboardImageAsync()
    {
        if (Clipboard.ContainsImage())
        {
            var image = Clipboard.GetImage();
            if (image == null)
            {
                
                return;
            }

            string tempFile = Path.Combine(Path.GetTempPath(), $"ocr_clipboard_{DateTime.Now.Ticks}.png");

            try
            {
                using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fileStream);
                }

                await RunPaddleOcrAsync(tempFile);


            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
        else
        {
            
        }
    }

    public async Task RunPaddleOcrAsync(string imagePath)
    {

        string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "ocr", "PaddleOCR_json.exe");
        bool success = await _ocrService.StartAsync(exePath);
        if (!success)
        {
           

        }

        string resultJson = await _ocrService.RunClipboardAsync();
        dynamic json = JsonConvert.DeserializeObject(resultJson);

        if (json.code == 100)
        {
            var sb = new StringBuilder();
            foreach (var item in json.data)
            {
                sb.AppendLine((string)item.text);
            }

            string finalText = sb.ToString();
            Clipboard.SetText(finalText); // 自动复制到剪贴板
        }
        else
        {
            //MessageBox.Show("识别失败：" + json.data.ToString());
        }
    }
}
