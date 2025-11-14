using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Application_.Models
{
    public interface IRAGChunk
    {
        List<IChatMessage>? ChatMessages { get; set; }
        string? TextChunk { get; set; }
    }
}