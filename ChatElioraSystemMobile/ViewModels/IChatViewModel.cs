using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystemMobile.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ChatElioraSystemMobile.ViewModels
{
    public interface IChatViewModel
    {
        bool CanEditContent { get; set; }
        IRelayCommand CreateNewSessionCommand { get; }
        string InputText { get; set; }
        bool IsLogOnView { get; set; }
        bool IsSaveToDbVec { get; set; }
        ObservableCollection<IChatMessage> Messages { get; }
        //ObservableCollection<IChatMessage> MessagesAll { get; }
        IRelayCommand OpenSessionCommand { get; }
        IChatMessage SelectedMessage { get; set; }
        IRelayCommand SendCommand { get; }
        ObservableCollection<ChatSession> TopicsConversation { get; }
        SesjaTematu WybranyTemat { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}