using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Infrastructure.VectorDataBase.Services
{
    public interface IVectorDbService
    {
        Task InitializeAsync();
        Task InsertAsync(string id, float[] vector, object payload);
        Task InsertAsync(string id, float[] vector, object payload, string collectionName);

        Task<List<(string Id, float Score, JsonElement Payload)>> SearchAsync(float[] vector, int topK = 5);
        Task<List<(string Id, float[] Vector, JsonElement Payload)>> GetAllPointsAsync();
        Task DeletePointAsync(string id);
        int VectorSize { get; }

        Task<string> ReadTopicsAsync(string collectionName, int limit = 20, string sort = "Timestamp DESC");


    }
}
