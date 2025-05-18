using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions;
using YingCaiAiWin.Views.Pages;

namespace YingCaiAiWin.Services
{
    public class PartialCachedPageProvider : INavigationViewPageProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, object> _cachedPages = new();

        public PartialCachedPageProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? GetPage(Type pageType)
        {
            // 👇 只缓存 AIWindows 页面
            if (pageType == typeof(AIWindows))
            {
                if (!_cachedPages.ContainsKey(pageType))
                {
                    var page = _serviceProvider.GetService(pageType);
                    if (page != null)
                        _cachedPages[pageType] = page;
                }

                return _cachedPages[pageType];
            }

            // 默认行为：每次创建新页面
            return _serviceProvider.GetService(pageType);
        }
    }

}
