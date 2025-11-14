using ChatElioraSystem.Core.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Application_.Models
{
    public class RAGChunk : IRAGChunk
    {
        public string? TextChunk { get; set; }
        public List<IChatMessage>? ChatMessages { get; set; }
    }

}
