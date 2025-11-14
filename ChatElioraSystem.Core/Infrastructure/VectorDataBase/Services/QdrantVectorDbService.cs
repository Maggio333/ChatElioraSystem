using ChatElioraSystem.Core.Infrastructure.Resources;
using ChatElioraSystem.Core.Infrastructure.Services;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Collections;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ChatElioraSystem.Core.Infrastructure.VectorDataBase.Services
{
    public class QdrantVectorDbService : IVectorDbService
    {
        private readonly HttpClient _httpClient;
        private readonly IUriAdressService _uriAdressService;

        private readonly string _collectionName = "PierwszaKolekcjaOnline";
        public int VectorSize { get; } = 1024;

        private readonly QdrantClient _client;
        //private readonly int _vectorSize = 3584;

        public QdrantVectorDbService(IUriAdressService uriAdressService, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _uriAdressService = uriAdressService;

            _httpClient.BaseAddress = _uriAdressService.GetDbVecAdressRest();
            if (_httpClient.BaseAddress == null)
            {
                throw new Exception("Brak Uri do bazy wektorowej");
            }

            var dbVecUri = _uriAdressService.GetDbVecAdressgRPC();
            if (dbVecUri != null)
            {
                _client = new QdrantClient(dbVecUri);
            }
        }

        public async Task InitializeAsync()
        {
            var collectionsToCheck = new[] { _collectionName, CollectionsNames.TopicCollectionName };

            // Zakładamy, że Qdrant nasłuchuje na porcie 6333 (lub 6334 – zmień, jeśli inny)

            foreach (var collection in collectionsToCheck)
            {
                // 🔹 Sprawdź czy kolekcja istnieje
                var response = await _httpClient.GetAsync($"/collections/{collection}");
                if (!response.IsSuccessStatusCode)
                {
                    var collectionDefinition = new
                    {
                        vectors = new
                        {
                            size = VectorSize,
                            distance = "Cosine"
                        }
                    };

                    await _httpClient.PutAsJsonAsync($"/collections/{collection}", collectionDefinition);
                    Console.WriteLine($"✅ Utworzono kolekcję: {collection}");
                }
                else
                {
                    Console.WriteLine($"ℹ️ Kolekcja już istnieje: {collection}");
                }
            }
            

            var test = await _client.GetCollectionInfoAsync(CollectionsNames.TopicCollectionName);

            // 🔹 Tworzenie indeksu payloadu po Timestamp
            var field = "Akcja.Metadata.Timestamp";
            var result = await _client.CreatePayloadIndexAsync(
                collectionName: CollectionsNames.TopicCollectionName,
                fieldName: field,
                schemaType: PayloadSchemaType.Datetime
            );

            switch (result.Status)
            {
                case UpdateStatus.Completed:
                    Console.WriteLine($"🧭 Indeks '{field}' utworzony jako datetime");
                    break;
                case UpdateStatus.Acknowledged:
                    Console.WriteLine($"✅ Indeks '{field}' już istnieje");
                    break;
                default:
                    Console.WriteLine($"⚠️ Nieznany status: {result.Status}");
                    break;
            }

        }

        //public async Task InitializeAsync()
        //{
        //    //DeleteCollectionAsync();
        //    var response = await _httpClient.GetAsync($"/collections/{_collectionName}");
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var collectionDefinition = new
        //        {
        //            vectors = new
        //            {
        //                size = VectorSize,
        //                distance = "Cosine"
        //            }
        //        };

        //        await _httpClient.PutAsJsonAsync($"/collections/{_collectionName}", collectionDefinition);
        //    }

        //    var colletion2 = CollectionsNames.TopicCollectionName;
        //    var response2 = await _httpClient.GetAsync($"/collections/{colletion2}");
        //    if (!response2.IsSuccessStatusCode)
        //    {
        //        var collectionDefinition = new
        //        {
        //            vectors = new
        //            {
        //                size = VectorSize,
        //                distance = "Cosine"
        //            }
        //        };

        //        await _httpClient.PutAsJsonAsync($"/collections/{colletion2}", collectionDefinition);
        //    }

        //    await EnsureTimestampIndexAsync(colletion2);
        //}

        /// <summary>
        /// Tworzy indeks po Akcja.Metadata.Timestamp, jeśli nie istnieje.
        /// </summary>
        private async Task EnsureTimestampIndexAsync(string collectionName)
        {
            var indexRequest = new
            {
                field_name = "Payload.Metadata.Timestamp",
                field_schema = "datetime"
            };

            var response = await _httpClient.PostAsJsonAsync($"/collections/{collectionName}/index", indexRequest);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                Console.WriteLine($"🧭 Indeks Timestamp aktywny dla {collectionName}");
            else if (result.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                Console.WriteLine($"✅ Indeks Timestamp już istnieje dla {collectionName}");
            else
                Console.WriteLine($"⚠️ Błąd przy tworzeniu indeksu Timestamp dla {collectionName}: {result}");
        }

        public async Task DeletePointAsync(string id)
        {
            var request = new
            {
                points = new[] { Guid.Parse(id) }
            };

            var response = await _httpClient.PostAsJsonAsync($"/collections/{_collectionName}/points/delete", request);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Delete point failed: {response.StatusCode} - {err}");
            }
            else
            {
                Console.WriteLine($"✅ Deleted point {id}");
            }
        }

        public async Task UpdateAsync(string id, float[] newVector, object newPayload)
        {
            await InsertAsync(id, newVector, newPayload);
        }

        public async Task<List<(string Id, float[] Vector, JsonElement Payload)>> GetAllPointsAsync()
        {
            var response = await _httpClient.PostAsJsonAsync($"/collections/{_collectionName}/points/scroll", new
            {
                limit = 10000,
                with_vector = true,
                with_payload = true
            });

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ GetAllPoints failed: {response.StatusCode} - {json}");
                return new();
            }

            using var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("result").GetProperty("points");

            var list = new List<(string, float[], JsonElement)>();
            foreach (var item in results.EnumerateArray())
            {
                var id = item.GetProperty("id").ToString();
                var vector = item.GetProperty("vector").EnumerateArray().Select(v => v.GetSingle()).ToArray();
                var payload = item.GetProperty("payload").Clone();
                list.Add((id, vector, payload));
            }

            return list;
        }


        public async Task InsertAsync(string id, float[] vector, object payload, string collectionName)
        {
            if(collectionName == string.Empty)
            {
                collectionName = _collectionName;
            }

            var point = new
            {
                //id = Guid.Parse(id), // <-- lub Guid.NewGuid() bez parametru
                id, // <-- lub Guid.NewGuid() bez parametru
                vector,
                payload,
                size = VectorSize
            };

            var data = new { points = new[] { point } };

            var response = await _httpClient.PutAsJsonAsync($"/collections/{collectionName}/points", data);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                Console.WriteLine($"❌ Insert failed: {response.StatusCode} - {responseText}");
            else
                Console.WriteLine($"✅ Inserted point with ID {id}");
        }

        public async Task InsertAsync(string id, float[] vector, object payload) => await InsertAsync(id, vector, payload, string.Empty);


        public async Task DeleteCollectionAsync()
        {
            await _httpClient.DeleteAsync($"/collections/{_collectionName}");
        }


        //public async Task<string> ReadTopicsAsync(string collectionName, int limit = 20, string sort = "desc")
        //{
        //    var requestBody = new
        //    {
        //        with_vector = false,
        //        with_payload = true,
        //        limit = limit,
        //        order_by = new[]
        //        {
        //    new
        //    {
        //        path = new[] { "Akcja", "Metadata", "Timestamp" },
        //        order = sort.ToLower().Contains("desc") ? "desc" : "asc"
        //    }
        //}
        //    };

        //    var response = await _httpClient.PostAsJsonAsync($"/collections/{collectionName}/points/scroll", requestBody);
        //    var json = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"❌ ReadTopicsAsync failed: {response.StatusCode} - {json}");
        //        return string.Empty;
        //    }

        //    using var doc = JsonDocument.Parse(json);
        //    var results = doc.RootElement.GetProperty("points");
        //    var options = new JsonSerializerOptions
        //    {
        //        WriteIndented = true,
        //        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        //    };

        //    return JsonSerializer.Serialize(results, options);
        //}


        public async Task<string> ReadTopicsAsync(string collectionName, int limit = 20, string sort = "Timestamp DESC")
        {
            var requestBody = new
            {
                limit = limit,
                with_vector = false,
                with_payload = true,
                order_by = new
                {
                    key = "Akcja.Metadata.Timestamp",
                    direction = sort.Contains("DESC", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc"
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"/collections/{collectionName}/points/scroll", requestBody);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ ReadTopicsAsync failed: {response.StatusCode} - {json}");
                return string.Empty;
            }

            using var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("result");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var results2 = JsonSerializer.Serialize(results, options);

            return results2;
        }




        public async Task<List<(string Id, float Score, JsonElement Payload)>> SearchAsync(float[] vector, int topK = 5)
        {
            var requestBody = new
            {
                vector,
                limit = topK,              // ✅ poprawnie: "limit", nie "top"
                with_payload = true,        // ✅ opcjonalnie, jeśli chcesz odczytać dane
                size = VectorSize

            };

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync($"/collections/{_collectionName}/points/search", requestBody);
            }
            catch (Exception ex)
            {
                throw new Exception("sprawdź połączenie do bazy wektorowej");
            }

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Search failed: {response.StatusCode} - {json}");
                return new List<(string, float, JsonElement)>();
            }

            using var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("result");

            var list = new List<(string, float, JsonElement)>();
            foreach (var item in results.EnumerateArray())
            {
                var id = item.GetProperty("id").ToString();
                var score = item.GetProperty("score").GetSingle();
                var payload = item.GetProperty("payload").Clone();
                list.Add((id, score, payload));
            }

            return list;
        }
    }
}
