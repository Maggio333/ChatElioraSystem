using ChatElioraSystem.Core.Infrastructure.Models;
using System.Collections.ObjectModel;

namespace ChatElioraSystem.Presentation.Models
{
    public class ChatSession : BindableBase, IChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _title = "test";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ObservableCollection<IChatMessage> Messages { get; set; } = new();

        public string FileName { get; set; } = string.Empty;
        //public DateTime LastUpdated => Messages.LastOrDefault()?.Timestamp ?? DateTime.Now;
    }

}
