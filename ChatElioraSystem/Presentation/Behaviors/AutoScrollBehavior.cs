using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ChatElioraSystem.Presentation.Behaviors
{
    public class AutoScrollBehavior : Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.ScrollChanged += OnScrollChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.ScrollChanged -= OnScrollChanged;
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange > 0)
            {
                AssociatedObject?.ScrollToEnd();
            }
        }
    }
}
