namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public interface ITextEmbeddingService
    {
        Task<float[]> GetCompletionAsync(string text)
;
    }
}