using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions;

namespace YingCaiAiWin.Services
{
    public class DependencyInjectionNavigationViewPageProvider(IServiceProvider serviceProvider)
        : INavigationViewPageProvider
    {
        /// <inheritdoc />
        public object? GetPage(Type pageType)
        {
            return serviceProvider.GetService(pageType);
        }
    }
}
