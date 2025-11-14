using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatElioraSystem.Core.Infrastructure.Models
{
    public class ChatMessage : IChatMessage, INotifyPropertyChanged
    {
        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }

        public Role Role { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string FileName { get; set; }

        public bool IsUser => Role == Role.user;
        public bool IsDbAction => Role == Role.dbAction;
        public bool IsTool => Role == Role.tool;
        public bool IsSystem => Role == Role.system;
        public bool IsAssistant => Role == Role.assistant;


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
