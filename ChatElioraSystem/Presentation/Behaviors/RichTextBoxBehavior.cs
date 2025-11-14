using ChatElioraSystem.Presentation.Converters;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChatElioraSystem.Presentation.Behaviors
{
    public static class RichTextBoxBehavior
    {
        public static readonly DependencyProperty BindableContentProperty =
            DependencyProperty.RegisterAttached(
                "BindableContent",
                typeof(string),
                typeof(RichTextBoxBehavior),
                new PropertyMetadata(null, OnBindableContentChanged));

        public static string GetBindableContent(DependencyObject obj)
            => (string)obj.GetValue(BindableContentProperty);

        public static void SetBindableContent(DependencyObject obj, string value)
            => obj.SetValue(BindableContentProperty, value);

        private static void OnBindableContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox rtb)
            {
                var content = e.NewValue as string;
                UpdateDocument(rtb, content);
            }
        }



        private static void UpdateDocument(RichTextBox rtb, string? content)
        {
            var converter = new AdvancedColorizingConverter();
            var inlines = converter.Convert(content, null, null, CultureInfo.CurrentCulture) as InlineCollection;

            // Upewnij się, że dokument jest nowy
            var doc = new FlowDocument(new Paragraph());
            doc.PageHeight = 10000;
            rtb.UpdateLayout();
            //rtb.ScrollToEnd();

            if (inlines != null)
            {
                var inlineList = new List<Inline>(inlines.Cast<Inline>());
                foreach (var inline in inlineList)
                {
                    (doc.Blocks.FirstBlock as Paragraph)?.Inlines.Add(inline);
                }
            }

            rtb.Document = doc;

            // 👇 Dopiero teraz przewiń
            //rtb.Dispatcher.InvokeAsync(() =>
            //{
            //    rtb.ScrollToEnd();
            //}, System.Windows.Threading.DispatcherPriority.Background);


        }
    }
}
