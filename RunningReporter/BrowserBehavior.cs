using System.Windows;
using System.Windows.Controls;

namespace RunningReporter
{
    internal sealed class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof(string), typeof(BrowserBehavior), new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lWebBrowser = d as WebBrowser;
            if (lWebBrowser == null) return;

            var lContentHtml = e.NewValue as string;
            if (lContentHtml == null) return;

            lWebBrowser.NavigateToString(lContentHtml);
        }
    }
}