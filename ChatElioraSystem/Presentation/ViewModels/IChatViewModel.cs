using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Presentation.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChatElioraSystem.Presentation.ViewModels
{
    public interface IChatViewModel
    {
        public IChatMessage? SelectedMessage { get; set; }
        bool CanEditContent { get; set; }
        ICommand CreateNewSessionCommand { get; }
        string InputText { get; set; }
        public ObservableCollection<IChatMessage> Messages { get; }
        ICommand SendCommand { get; }
        public ObservableCollection<ChatSession> TopicsConversation { get; set; }

        SesjaTematu WybranyTemat { get; set; }

        event Action ScrollToEndRequested;
    }
}