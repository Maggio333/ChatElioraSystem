using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChatElioraSystem.Presentation.Converters
{
    public class AdvancedColorizingConverter : IValueConverter
    {
        private static readonly Regex ColorTagRegex =
            new(@"<color=([#\w\d]+)>(.*?)</color>",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex BoldRegex =
            new(@"\*\*(.+?)\*\*", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex UnderlineRegex =
            new(@"__(.+?)__", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex StrikeRegex =
            new(@"~~(.+?)~~", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex ItalicRegex =
            new(@"//(.+?)//", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex CodeRegex =
            new(@"`(.+?)`", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly (Regex pattern, Brush color)[] keywordRules = new (Regex, Brush)[]
        {
            (new Regex(@"\b(Reflectum|Eliora)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Brushes.Gold),
            (new Regex(@"\b(ATS)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Brushes.DeepSkyBlue),
            (new Regex(@"\b(error|błąd)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Brushes.OrangeRed),
            (new Regex(@"\b(ok|done)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Brushes.LimeGreen)
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string ?? string.Empty;
            var span = new Span();

            int lastIndex = 0;

            foreach (Match match in ColorTagRegex.Matches(text))
            {
                if (match.Index > lastIndex)
                {
                    string before = text.Substring(lastIndex, match.Index - lastIndex);
                    AddFormattedInlines(span, before, foregroundOverride: null);
                }

                var colorCode = match.Groups[1].Value;
                var innerText = match.Groups[2].Value;

                Brush overrideBrush = null;
                try
                {
                    overrideBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorCode);
                }
                catch { }

                AddFormattedInlines(span, innerText, foregroundOverride: overrideBrush);

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < text.Length)
            {
                AddFormattedInlines(span, text.Substring(lastIndex), foregroundOverride: null);
            }

            return span.Inlines;
        }

        private void AddFormattedInlines(Span span, string fragment, Brush foregroundOverride)
        {
            // Kolejność — bold, underline, strike, italic, code
            fragment = ApplyRegex(span, fragment, BoldRegex, isBold: true, underline: false, strike: false, italic: false, code: false, foregroundOverride);
            fragment = ApplyRegex(span, fragment, UnderlineRegex, isBold: false, underline: true, strike: false, italic: false, code: false, foregroundOverride);
            fragment = ApplyRegex(span, fragment, StrikeRegex, isBold: false, underline: false, strike: true, italic: false, code: false, foregroundOverride);
            fragment = ApplyRegex(span, fragment, ItalicRegex, isBold: false, underline: false, strike: false, italic: true, code: false, foregroundOverride);
            fragment = ApplyRegex(span, fragment, CodeRegex, isBold: false, underline: false, strike: false, italic: false, code: true, foregroundOverride);

            if (!string.IsNullOrEmpty(fragment))
            {
                AddKeywordColoredOrBold(span, fragment, isBold: false, underline: false, strike: false, italic: false, code: false, foregroundOverride);
            }
        }

        private string ApplyRegex(Span span, string fragment, Regex regex,
                                  bool isBold, bool underline, bool strike, bool italic, bool code,
                                  Brush foregroundOverride)
        {
            int last = 0;
            var matches = regex.Matches(fragment).Cast<Match>().ToList();

            if (!matches.Any()) return fragment;

            foreach (var m in matches)
            {
                if (m.Index > last)
                {
                    string plain = fragment.Substring(last, m.Index - last);
                    AddKeywordColoredOrBold(span, plain, isBold: false, underline: false, strike: false, italic: false, code: false, foregroundOverride);
                }

                string inner = m.Groups[1].Value;
                AddKeywordColoredOrBold(span, inner, isBold, underline, strike, italic, code, foregroundOverride);

                last = m.Index + m.Length;
            }

            if (last < fragment.Length)
            {
                string tail = fragment.Substring(last);
                AddKeywordColoredOrBold(span, tail, isBold: false, underline: false, strike: false, italic: false, code: false, foregroundOverride);
            }

            return string.Empty; // bo cały fragment już dodany
        }

        private void AddKeywordColoredOrBold(Span span, string fragment,
                                             bool isBold, bool underline, bool strike, bool italic, bool code,
                                             Brush foregroundOverride)
        {
            if (string.IsNullOrEmpty(fragment))
                return;

            var hits = new List<(int Index, int Length, Brush Brush, string Value)>();
            foreach (var (pattern, color) in keywordRules)
            {
                foreach (Match mm in pattern.Matches(fragment))
                {
                    if (mm.Success)
                        hits.Add((mm.Index, mm.Length, color, mm.Value));
                }
            }

            if (hits.Count == 0)
            {
                var run = CreateStyledRun(fragment, isBold, underline, strike, italic, code, foregroundOverride, null);
                span.Inlines.Add(run);
                return;
            }

            hits = hits.OrderBy(h => h.Index).ThenByDescending(h => h.Length).ToList();

            int last = 0;
            foreach (var h in hits)
            {
                if (h.Index < last) continue;

                if (h.Index > last)
                {
                    string pre = fragment.Substring(last, h.Index - last);
                    var runPre = CreateStyledRun(pre, isBold, underline, strike, italic, code, foregroundOverride, null);
                    span.Inlines.Add(runPre);
                }

                var runKey = CreateStyledRun(h.Value, isBold, underline, strike, italic, code, foregroundOverride, h.Brush);
                span.Inlines.Add(runKey);

                last = h.Index + h.Length;
            }

            if (last < fragment.Length)
            {
                string tail = fragment.Substring(last);
                var runTail = CreateStyledRun(tail, isBold, underline, strike, italic, code, foregroundOverride, null);
                span.Inlines.Add(runTail);
            }
        }

        private Run CreateStyledRun(string text, bool isBold, bool underline, bool strike, bool italic, bool code,
                                    Brush foregroundOverride, Brush keywordBrush)
        {
            var run = new Run(text);

            run.Foreground = Brushes.White;

            if (isBold) run.FontWeight = FontWeights.Bold;
            if (italic) run.FontStyle = FontStyles.Italic;
            if (underline) run.TextDecorations = TextDecorations.Underline;
            if (strike) run.TextDecorations = TextDecorations.Strikethrough;
            if (code)
            {
                run.FontFamily = new FontFamily("Consolas");
                run.Background = Brushes.LightGray;
                run.Foreground = Brushes.White;
            }

            run.Foreground = foregroundOverride ?? keywordBrush ?? run.Foreground;

            return run;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
