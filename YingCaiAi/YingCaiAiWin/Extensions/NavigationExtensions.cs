using System.Windows;
using Wpf.Ui.Controls;

namespace YingCaiAiWin.Extensions
{
    public static class NavigationExtensions
    {
        // 为NavigationViewItem添加权限代码依赖属性
        public static readonly DependencyProperty PermissionCodeProperty =
            DependencyProperty.RegisterAttached(
                "PermissionCode",
                typeof(string),
                typeof(NavigationExtensions),
                new PropertyMetadata(string.Empty));

        // Getter
        public static string GetPermissionCode(NavigationViewItem item)
        {
            return (string)item.GetValue(PermissionCodeProperty);
        }

        // Setter
        public static void SetPermissionCode(NavigationViewItem item, string value)
        {
            item.SetValue(PermissionCodeProperty, value);
        }
    }
}