using System.ComponentModel;

namespace ChatElioraSystem.Core.Infrastructure.Models
{
    public interface IChatMessage : INotifyPropertyChanged
    {
        string Content { get; set; }
        bool IsUser { get; }
        bool IsDbAction { get; }
        bool IsTool { get; }
        bool IsSystem { get; }
        bool IsAssistant { get; }


        Role Role { get; set; }
        DateTime Timestamp { get; set; }
        string FileName { get; set; }
    }
}