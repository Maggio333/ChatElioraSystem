using ChatElioraSystem.Core.Infrastructure.Models;
using System.Collections.ObjectModel;

namespace ChatElioraSystemShared.Models
{
    public interface IChatSession
    {
        Guid Id { get; set; }
        ObservableCollection<IChatMessage> Messages { get; }
        string Title { get; set; }

        string FileName { get; set; }
    }
}