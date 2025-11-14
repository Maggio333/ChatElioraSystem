using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChatElioraSystem.Presentation.Controls
{
    public class AutoScrollRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty BindableContentProperty =
            DependencyProperty.Register("BindableContent", typeof(string), typeof(AutoScrollRichTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBindableContentChanged));

        public string BindableContent
        {
            get => (string)GetValue(BindableContentProperty);
            set => SetValue(BindableContentProperty, value);
        }

        private static void OnBindableContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not AutoScrollRichTextBox rtb) return;

            rtb.ApplyContent((string)e.NewValue);
        }

        private void ApplyContent(string content)
        {
            var para = new Paragraph(new Run(content));
            var flow = new FlowDocument(para)
            {
                PagePadding = new Thickness(0),
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight
            };

            this.Document = flow;

            this.UpdateLayout();              // <– ważne!
            this.InvalidateMeasure();         // <– wymusza przeliczenie wysokości
            this.ScrollToEnd();               // <– przewija na dół
        }
    }
}
