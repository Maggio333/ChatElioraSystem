using ChatElioraSystem.Presentation.ViewModels;
using System.Windows;

namespace ChatElioraSystem.Presentation.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Test.xaml
    /// </summary>
    public partial class ChatWindow2 : Window
    {
        public ChatWindow2(IChatViewModel chatViewModel)
        {
            InitializeComponent();
            DataContext = chatViewModel;
        }

        private void RichTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //MessagesScrollViewer?.ScrollToEnd();
        }
    }
}
