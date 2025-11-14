using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public interface IChatLogService
    {
        string Save(IEnumerable<IChatMessage> messages, IDictionary<string, object>? metadata = null);
        string Save(IEnumerable<IChatMessage> messages, string filename, IDictionary<string, object>? metadata = null);
        public void GenerateNewFileName();
        public IEnumerable<IChatMessage> Load(string fileName, out IDictionary<string, object>? metadata);
        public List<string> ListLogFiles();
        //public DateTime GetCreatedDate();

    }
}