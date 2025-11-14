using System.Net.Http.Json;
using System.Text.Json;

namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public class LMStudioEmbeddingService : ITextEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly IUriAdressService _uriAdressService;

        //private readonly string _endpointUrl = "http://localhost:1234/v1/embeddings";
        //private readonly string _endpointUrl = "http://10.0.2.2:1234/v1/chat/completions"; //dla maui




        public LMStudioEmbeddingService(IUriAdressService uriAdressService ,HttpClient httpClient)
        {
            _httpClient = httpClient;
            _uriAdressService = uriAdressService;
            //_httpClient.BaseAddress = new Uri("http://localhost:1234"); // domyślny port LM Studio
            //_httpClient.BaseAddress = new Uri($"http://{UriAdress.Ip}:1234"); // //dla maui
            //_httpClient.BaseAddress = _uriAdressService.GetLlmEmbeddingAdress();
            //_ = InitializeAsync();
            _httpClient.BaseAddress = _uriAdressService.GetLlmEmbeddingAdress();
            if (_httpClient.BaseAddress == null)
            {
                throw new Exception("Brak Uri do Embedded LLM");
            }
        }

        public async Task InitializeAsync()
        {
            _httpClient.BaseAddress = _uriAdressService.GetLlmEmbeddingAdress();
            if (_httpClient.BaseAddress == null)
            {
                throw new Exception("Brak Uri do Embedded LLM");
            }
        }

        public async Task<float[]> GetCompletionAsync(string text)
        {
            var requestBody = new
            {
                model = "model:10", // <- dokładna nazwa modelu z LM Studio
                input = text
            };

            var response = await _httpClient.PostAsJsonAsync("/v1/embeddings", requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Błąd podczas odczytu embeddingu: {responseBody}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var vector = doc
                .RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            return vector;
        }
    }
}
