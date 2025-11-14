using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChatElioraSystem.Presentation.Converters
{
    public class MarkdownToFlowDocumentConverter : IValueConverter
    {
        private static readonly Regex BoldRegex = new(@"\*\*(.+?)\*\*", RegexOptions.Compiled);


        private static readonly Regex ColorTagRegex = new(@"<color=([#\w\d]+)>(.*?)</color>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private static readonly Regex BoldRegex = new(@"\*\*(.+?)\*\*", RegexOptions.Compiled);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text) return null;

            var document = new FlowDocument();
            var paragraph = new Paragraph();

            int currentIndex = 0;

            // Najpierw kolory
            foreach (Match colorMatch in ColorTagRegex.Matches(text))
            {
                // Tekst przed tagiem koloru
                if (colorMatch.Index > currentIndex)
                {
                    string before = text.Substring(currentIndex, colorMatch.Index - currentIndex);
                    AddRunsWithBold(paragraph, before);
                }

                string color = colorMatch.Groups[1].Value;
                string coloredText = colorMatch.Groups[2].Value;

                var run = new Run(coloredText)
                {
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color))
                };
                
                paragraph.Inlines.Add(run);
                currentIndex = colorMatch.Index + colorMatch.Length;
            }

            // Tekst po ostatnim kolorze
            if (currentIndex < text.Length)
            {
                string remaining = text.Substring(currentIndex);
                AddRunsWithBold(paragraph, remaining);
            }
            
            document.Blocks.Add(paragraph);
            return document;
        }

        private void AddRunsWithBold(Paragraph paragraph, string text)
        {
            int index = 0;
            foreach (Match boldMatch in BoldRegex.Matches(text))
            {
                if (boldMatch.Index > index)
                {
                    paragraph.Inlines.Add(new Run(text.Substring(index, boldMatch.Index - index)));
                }

                string boldText = boldMatch.Groups[1].Value;
                paragraph.Inlines.Add(new Bold(new Run(boldText)));
                index = boldMatch.Index + boldMatch.Length;
            }

            if (index < text.Length)
            {
                paragraph.Inlines.Add(new Run(text.Substring(index)));
            }
        }
        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (value is not string text)
        //        return null;

        //    //var document = new FlowDocument();
        //    var paragraph = new Paragraph();

        //    int lastIndex = 0;
        //    foreach (Match match in BoldRegex.Matches(text))
        //    {
        //        // Zwykły tekst przed **
        //        if (match.Index > lastIndex)
        //        {
        //            string normalText = text.Substring(lastIndex, match.Index - lastIndex);
        //            paragraph.Inlines.Add(new Run(normalText));
        //        }

        //        // Tekst pogrubiony
        //        string boldText = match.Groups[1].Value;
        //        paragraph.Inlines.Add(new Bold(new Run(boldText)));

        //        lastIndex = match.Index + match.Length;
        //    }

        //    // Tekst po ostatnim **
        //    if (lastIndex < text.Length)
        //    {
        //        paragraph.Inlines.Add(new Run(text.Substring(lastIndex)));
        //    }

        //    //document.Blocks.Add(paragraph);
        //    return paragraph;
        //}

        public object ConvertToString(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not string text)
                return null;

            var document = new FlowDocument();
            var paragraph = new Paragraph();

            int lastIndex = 0;
            foreach (Match match in BoldRegex.Matches(text))
            {
                // Zwykły tekst przed **
                if (match.Index > lastIndex)
                {
                    string normalText = text.Substring(lastIndex, match.Index - lastIndex);
                    paragraph.Inlines.Add(new Run(normalText));
                }

                // Tekst pogrubiony
                string boldText = match.Groups[1].Value;
                paragraph.Inlines.Add(new Bold(new Run(boldText)));

                lastIndex = match.Index + match.Length;
            }

            // Tekst po ostatnim **
            if (lastIndex < text.Length)
            {
                paragraph.Inlines.Add(new Run(text.Substring(lastIndex)));
            }

            document.Blocks.Add(paragraph);
            return document;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}
