using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public interface ILlmService
    {
        Task<string> GetCompletionAsync(IEnumerable<IChatMessage> chatMessages, string llmName, CancellationToken ct, double llmNo = 1);
        IAsyncEnumerable<string> StreamCompletionAsync(IEnumerable<IChatMessage> chatMessages, string llmName, CancellationToken ct, double llmNo = 1, double temperature = 0.3, int maxTokens = 10000);
    }
}
