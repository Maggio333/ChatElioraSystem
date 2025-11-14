using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;


namespace ChatElioraSystemMobile.Converters
{
        public class MauiColorizingConverter : IValueConverter
        {
            private static readonly Regex ColorTagRegex =
                new(@"<color=([#\w\d]+)>(.*?)</color>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            private static readonly Regex BoldRegex =
                new(@"\*\*(.+?)\*\*", RegexOptions.Compiled | RegexOptions.Singleline);

            private static readonly (Regex pattern, Color color)[] keywordRules = new (Regex, Color)[]
            {
            (new Regex(@"\b(Reflectum|Eliora)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Colors.Gold),
            (new Regex(@"\b(ATS)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Colors.DeepSkyBlue),
            (new Regex(@"\b(error|błąd)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Colors.OrangeRed),
            (new Regex(@"\b(ok|done)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled), Colors.LimeGreen)
            };

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var text = value as string ?? string.Empty;
                var formattedString = new FormattedString();

                int lastIndex = 0;
                foreach (Match match in ColorTagRegex.Matches(text))
                {
                    if (match.Index > lastIndex)
                    {
                        string before = text.Substring(lastIndex, match.Index - lastIndex);
                        AddFormattedSpans(formattedString, before, null);
                    }

                    var colorCode = match.Groups[1].Value;
                    var innerText = match.Groups[2].Value;

                    Color? overrideColor = null;
                    try
                    {
                        overrideColor = Color.FromArgb(colorCode);
                    }
                    catch
                    {
                        // Invalid color — ignore
                    }

                    AddFormattedSpans(formattedString, innerText, overrideColor);
                    lastIndex = match.Index + match.Length;
                }

                if (lastIndex < text.Length)
                {
                    AddFormattedSpans(formattedString, text.Substring(lastIndex), null);
                }

                return formattedString;
            }

            private void AddFormattedSpans(FormattedString formattedString, string fragment, Color? overrideColor)
            {
                int last = 0;
                foreach (Match match in BoldRegex.Matches(fragment))
                {
                    if (match.Index > last)
                    {
                        string plain = fragment.Substring(last, match.Index - last);
                        AddKeywordColoredOrBold(formattedString, plain, false, overrideColor);
                    }

                    string boldInner = match.Groups[1].Value;
                    AddKeywordColoredOrBold(formattedString, boldInner, true, overrideColor);
                    last = match.Index + match.Length;
                }

                if (last < fragment.Length)
                {
                    AddKeywordColoredOrBold(formattedString, fragment.Substring(last), false, overrideColor);
                }
            }

            private void AddKeywordColoredOrBold(FormattedString formattedString, string fragment, bool isBold, Color? overrideColor)
            {
                if (string.IsNullOrEmpty(fragment))
                    return;

                var hits = new List<(int Index, int Length, Color Color, string Value)>();
                foreach (var (pattern, color) in keywordRules)
                {
                    foreach (Match match in pattern.Matches(fragment))
                    {
                        if (match.Success)
                            hits.Add((match.Index, match.Length, color, match.Value));
                    }
                }

                if (hits.Count == 0)
                {
                    var span = new Span { Text = fragment };
                    if (isBold) span.FontAttributes = FontAttributes.Bold;
                    if (overrideColor != null) span.TextColor = overrideColor;
                    formattedString.Spans.Add(span);
                    return;
                }

                hits.Sort((a, b) => a.Index.CompareTo(b.Index));
                int last = 0;
                foreach (var hit in hits)
                {
                    if (hit.Index < last) continue;

                    if (hit.Index > last)
                    {
                        string pre = fragment.Substring(last, hit.Index - last);
                        var spanPre = new Span { Text = pre };
                        if (isBold) spanPre.FontAttributes = FontAttributes.Bold;
                        if (overrideColor != null) spanPre.TextColor = overrideColor;
                        formattedString.Spans.Add(spanPre);
                    }

                    var spanKey = new Span { Text = hit.Value };
                    if (isBold) spanKey.FontAttributes = FontAttributes.Bold;
                    spanKey.TextColor = overrideColor ?? hit.Color;
                    formattedString.Spans.Add(spanKey);

                    last = hit.Index + hit.Length;
                }

                if (last < fragment.Length)
                {
                    string tail = fragment.Substring(last);
                    var spanTail = new Span { Text = tail };
                    if (isBold) spanTail.FontAttributes = FontAttributes.Bold;
                    if (overrideColor != null) spanTail.TextColor = overrideColor;
                    formattedString.Spans.Add(spanTail);
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
    

}
