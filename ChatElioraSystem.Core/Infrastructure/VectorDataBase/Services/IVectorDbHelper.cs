using System.Text.Json;

namespace ChatElioraSystem.Core.Infrastructure.VectorDataBase.Services
{
    public interface IVectorDbHelper
    {
        float[] AverageVectors(List<float[]> vectors);

        public Task<bool> InsertTopic(string payloadToVec, JsonElement payloadText, string collectionName);
        Task<string> ReadTopics(string collectionName, int limit = 5, string sort = "Timestamp DESC");

        Task<string> GetBaseIdioms(int limit = 20);


        Task<JsonDocument> InsertIfNotDuplicateAsync(string payloadToVec, string payloadText, float similarityThreshold = 0.93f);
        //Task TestAsync();
        Task<List<(string Id, float Score, JsonElement Payload)>> GetValueFromVDB(string toSearch);

    }
}