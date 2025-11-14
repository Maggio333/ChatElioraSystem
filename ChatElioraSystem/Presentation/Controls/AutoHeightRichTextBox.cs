using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace ChatElioraSystem.Presentation.Controls
{
    public class AutoSizingRichTextBox : RichTextBox
    {
        private ScrollViewer _host; // PART_ContentHost
        private const double MinWrapWidth = 120;
        public AutoSizingRichTextBox()
        {
            string text = GetDocumentText(Document);

            Loaded += (_, __) => HookHost();
            SizeChanged += (_, __) => InvalidateMeasure();
            TextChanged += (_, __) => InvalidateMeasure();
            LayoutUpdated += (_, __) => UpdateSize(); // po finalnym layoucie

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HookHost();
        }

        private void HookHost()
        {
            if (_host != null) return;
            _host = GetTemplateChild("PART_ContentHost") as ScrollViewer;
            if (_host == null) return;

            // kluczowe: reaguj na zmianę viewportu
            _host.SizeChanged += (_, __) => UpdateSize();

            // jeżeli masz paski – wyłącz pionowy, żeby rozszerzało wysokość
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

            UpdateSize();
        }

        private void UpdateSize()
        {
            if (_host == null || Document == null) return;

            // 1) Wyznacz szerokość zawijania z sensownymi fallbackami
            double viewport = _host.ViewportWidth;
            if (viewport <= 0) viewport = _host.ActualWidth;
            if (viewport <= 0) viewport = ActualWidth;

            if (viewport <= 1)
            {
                // layout jeszcze niegotowy – spróbuj po chwili
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(UpdateSize));
                return;
            }

            double wrap = Math.Max(MinWrapWidth, viewport); // CLAMP!
            Document.PagePadding = new Thickness(0);
            Document.PageWidth = wrap;
            Document.ColumnWidth = wrap; // wyłącz „wielokolumnowość”

            // 2) Wysokość z Extent + minimalny zapas (bez przycinania za mocno)
            double h = _host.ExtentHeight;
            if (h <= 0) h = MinHeight > 0 ? MinHeight : 20;

            // mały bufor na baseline/caret
            h += 2;

            if (Math.Abs(Height - h) > 0.5)
                Height = h;


            //var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            //int chars = FitChars(text, Width - Padding.Left - Padding.Right, typeface, 14);
            //Width = chars;
        }

        string GetDocumentText(FlowDocument doc)
        {
            if (doc == null)
                return string.Empty;

            TextRange range = new TextRange(
                doc.ContentStart,
                doc.ContentEnd
            );

            return range.Text?.TrimEnd('\r', '\n') ?? string.Empty;
        }


        static double MeasureWidth(string s, Typeface typeface, double fontSize)
        {
            var ft = new FormattedText(
                s,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                Brushes.Transparent,
                new NumberSubstitution(),
                TextFormattingMode.Display);
            return ft.WidthIncludingTrailingWhitespace;
        }

        // zwróci ile znaków z początku 'text' mieści się w 'width'
        static int FitChars(string text, double width, Typeface typeface, double fontSize)
        {
            int lo = 0, hi = text.Length;
            while (lo < hi)
            {
                int mid = (lo + hi + 1) / 2;
                double w = MeasureWidth(text.Substring(0, mid), typeface, fontSize);
                if (w <= width) lo = mid; else hi = mid - 1;
            }
            return lo;
        }
        //private void UpdateSize()
        //{
        //    if (_host == null || Document == null) return;

        //    // 1) Szerokość – używaj ViewportWidth ScrollViewer-a (nie ActualWidth RTB!)
        //    double viewport = _host.ViewportWidth;
        //    if (viewport <= 0)
        //    {
        //        // fallback na rzeczywisty obszar, gdy Viewport jeszcze 0
        //        viewport = Math.Max(0, _host.ActualWidth);
        //    }

        //    // uwzględnij padding dokumentu i samej kontrolki
        //    double pagePaddingLR = Document.PagePadding.Left + Document.PagePadding.Right;
        //    double ctrlPaddingLR = Padding.Left + Padding.Right;
        //    double pageWidth = Math.Max(0, viewport - ctrlPaddingLR); // ScrollViewer już liczy swoje paddingi
        //    if (Math.Abs(Document.PageWidth - pageWidth) > 0.5)
        //        Document.PageWidth = pageWidth;

        //    // 2) Wysokość – bierz ExtentHeight hosta (ile naprawdę zajmuje treść)
        //    // działa tylko gdy VerticalScrollBarVisibility=Disabled (inaczej extent równa się viewportowi)
        //    double h = _host.ExtentHeight + Padding.Top + Padding.Bottom;

        //    // Minimalna sensowna wysokość
        //    if (h <= 0) h = Math.Max(MinHeight, 20);

        //    // Ustaw raz, nie „pompować” przy drobnych zmianach pływających
        //    if (Math.Abs(Height - h) > 0.5)
        //        Height = h;
        //}
    }
}
