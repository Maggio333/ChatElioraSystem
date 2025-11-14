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
    public class ColorizingConverter : IValueConverter
    {
        // <color=#RRGGBB>...</color> albo <color=Red>...</color>
        private static readonly Regex ColorTagRegex =
            new(@"<color=([#\w\d]+)>(.*?)</color>",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // **bold** (pozwalamy na wielolinijkowe)
        private static readonly Regex BoldRegex =
            new(@"\*\*(.+?)\*\*", RegexOptions.Compiled | RegexOptions.Singleline);

        // Słowa-klucze -> kolor
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

            // 1) Najpierw pociąć po tagach <color=...>...</color>
            foreach (Match match in ColorTagRegex.Matches(text))
            {
                // Tekst przed tagiem - bez nadpisanego koloru
                if (match.Index > lastIndex)
                {
                    string before = text.Substring(lastIndex, match.Index - lastIndex);
                    AddFormattedInlines(span, before, foregroundOverride: null);
                }

                // Wnętrze tagu z nadpisanym Foreground
                var colorCode = match.Groups[1].Value;
                var innerText = match.Groups[2].Value;

                Brush overrideBrush = null;
                try
                {
                    overrideBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorCode);
                }
                catch
                {
                    // Jak kolor niepoprawny – po prostu brak override
                }

                AddFormattedInlines(span, innerText, foregroundOverride: overrideBrush);

                lastIndex = match.Index + match.Length;
            }

            // Ogon po ostatnim tagu
            if (lastIndex < text.Length)
            {
                AddFormattedInlines(span, text.Substring(lastIndex), foregroundOverride: null);
            }

            return span.Inlines;
        }

        /// <summary>
        /// Rozbija fragment na zwykłe oraz **pogrubione** i dokleja do spana.
        /// </summary>
        private void AddFormattedInlines(Span span, string fragment, Brush foregroundOverride)
        {
            int last = 0;
            foreach (Match m in BoldRegex.Matches(fragment))
            {
                // Zwykły tekst przed **...**
                if (m.Index > last)
                {
                    string plain = fragment.Substring(last, m.Index - last);
                    AddKeywordColoredOrBold(span, plain, isBold: false, foregroundOverride);
                }

                // Wnętrze pogrubienia
                string boldInner = m.Groups[1].Value;
                AddKeywordColoredOrBold(span, boldInner, isBold: true, foregroundOverride);

                last = m.Index + m.Length;
            }

            // Ogon po ostatnim boldzie
            if (last < fragment.Length)
            {
                AddKeywordColoredOrBold(span, fragment.Substring(last), isBold: false, foregroundOverride);
            }
        }

        /// <summary>
        /// Koloruje słowa-klucze z zachowaniem kolejności i dokładaniem Bold/Foreground.
        /// </summary>
        private void AddKeywordColoredOrBold(Span span, string fragment, bool isBold, Brush foregroundOverride)
        {
            if (string.IsNullOrEmpty(fragment))
                return;

            // Zbierz WSZYSTKIE dopasowania z reguł, posortuj po Index
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
                // Bez słów-kluczy – zwykły run
                var runPlain = new Run(fragment);
                if (isBold) runPlain.FontWeight = FontWeights.Bold;
                if (foregroundOverride != null) runPlain.Foreground = foregroundOverride;
                span.Inlines.Add(runPlain);
                return;
            }

            // Sortuj i iteruj unikając nakładających się powtórek
            hits = hits.OrderBy(h => h.Index).ThenByDescending(h => h.Length).ToList();

            int last = 0;
            foreach (var h in hits)
            {
                if (h.Index < last) continue; // pomiń nakładki

                // Tekst przed słowem-kluczem
                if (h.Index > last)
                {
                    string pre = fragment.Substring(last, h.Index - last);
                    var runPre = new Run(pre);
                    if (isBold) runPre.FontWeight = FontWeights.Bold;
                    if (foregroundOverride != null) runPre.Foreground = foregroundOverride;
                    span.Inlines.Add(runPre);
                }

                // Samo słowo-klucz – foreground z reguły chyba że jest override z <color=...>
                var runKey = new Run(h.Value);
                if (isBold) runKey.FontWeight = FontWeights.Bold;
                runKey.Foreground = foregroundOverride ?? h.Brush;
                span.Inlines.Add(runKey);

                last = h.Index + h.Length;
            }

            // Ogon po ostatnim trafieniu
            if (last < fragment.Length)
            {
                string tail = fragment.Substring(last);
                var runTail = new Run(tail);
                if (isBold) runTail.FontWeight = FontWeights.Bold;
                if (foregroundOverride != null) runTail.Foreground = foregroundOverride;
                span.Inlines.Add(runTail);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
