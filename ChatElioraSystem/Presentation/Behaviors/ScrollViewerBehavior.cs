using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChatElioraSystem.Presentation.Behaviors
{
    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(ScrollViewerBehavior),
                new PropertyMetadata(false, OnAutoScrollChanged));

        public static bool GetAutoScroll(DependencyObject obj) => (bool)obj.GetValue(AutoScrollProperty);
        public static void SetAutoScroll(DependencyObject obj, bool value) => obj.SetValue(AutoScrollProperty, value);

        private static void OnAutoScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (d is ScrollViewer viewer && (bool)e.NewValue)
            //{
            //    viewer.ScrollChanged += (_, args) =>
            //    {
            //        if (args.ExtentHeightChange != 0)
            //            viewer.ScrollToEnd();
            //    };
            //}
        }
    }

}
