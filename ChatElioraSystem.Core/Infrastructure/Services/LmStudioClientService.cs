using ChatElioraSystem.Core.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public class LmStudioClientService : ILlmService
    {
        private readonly HttpClient _httpClient;
        private readonly IUriAdressService _uriAdressService;

        public LmStudioClientService(IUriAdressService uriAdressService, HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _uriAdressService = uriAdressService;

            _httpClient.Timeout = TimeSpan.FromSeconds(3000);

            //_ = InitializeAsync();
            _httpClient.BaseAddress = _uriAdressService.GetLlmAdress();
            if (_httpClient.BaseAddress == null)
            {
                throw new Exception("Brak Uri do LLM");
            }
        }
        public async Task InitializeAsync()
        {
            _httpClient.BaseAddress = _uriAdressService.GetLlmAdress();
            if (_httpClient.BaseAddress == null)
            {
                throw new Exception("Brak Uri do LLM");
            }
        }

        public async Task<string> GetCompletionAsync(IEnumerable<IChatMessage> chatMessages, string llmName, CancellationToken cancellationToken, double llmNo)
        {
            string modelName = llmNo switch
            {
                0 => $"{llmName}",
                33 => $"{llmName}:judge",
                _ => $"{llmName}:{llmNo}"
                
            };

            var requestBody = new
            {
                model = modelName,
                messages = chatMessages.Select(x => new
                {
                    role = x.Role.ToString().ToLower(),
                    content = x.Content
                }),
                temperature = 0.3,
                max_tokens = 10000,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return $"Błąd podczas odczytu LLM : {ex.Message}";
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return result;
        }

        // --- NOWOŚĆ: streaming przez SSE ---
        public async IAsyncEnumerable<string> StreamCompletionAsync(
            IEnumerable<IChatMessage> chatMessages,
            string llmName,
            CancellationToken ct,
            double llmNo = 1,
            double temperature = 0.3,
            int maxTokens = 16000)
        {


            string modelName = llmNo switch
            {
                0 => $"{llmName}",
                _ => $"{llmName}:{llmNo}",
            };

            var requestBody = new
            {
                model = modelName,
                messages = chatMessages.Select(x => new
                {
                    role = x.Role.ToString().ToLower(),
                    content = x.Content
                }),
                temperature,
                max_tokens = maxTokens,
                stream = true
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            // ważne dla SSE
            req.Headers.Accept.Clear();
            req.Headers.Accept.ParseAdd("text/event-stream");

            HttpResponseMessage resp;

            if(ct.IsCancellationRequested)
            {
                yield break;
            }

            resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            while (!reader.EndOfStream && !ct.IsCancellationRequested)
            {
                string line;

                line = await reader.ReadLineAsync();

                if (line is null) break;
                if (line.Length == 0) continue;     // keep-alive/puste
                if (line.StartsWith(":")) continue; // komentarze SSE
                if (!line.StartsWith("data:")) continue;

                var payload = line["data:".Length..].Trim();

                if (payload == "[DONE]")
                    yield break;


                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;

                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var choice = choices[0];

                    // standard: delta.content
                    if (choice.TryGetProperty("delta", out var delta) &&
                        delta.TryGetProperty("content", out var contentEl) &&
                        contentEl.ValueKind == JsonValueKind.String)
                    {
                        var chunk = contentEl.GetString();
                        if (!string.IsNullOrEmpty(chunk))
                            yield return chunk!;
                        continue;
                    }

                    // niektóre serwery nadają pełne message.content
                    if (choice.TryGetProperty("message", out var msg) &&
                        msg.TryGetProperty("content", out var msgContent) &&
                        msgContent.ValueKind == JsonValueKind.String)
                    {
                        var chunk = msgContent.GetString();
                        if (!string.IsNullOrEmpty(chunk))
                            yield return chunk!;
                        continue;
                    }
                }

                continue;

            }
        }

        IEnumerable<string> HandleError(Exception ex)
        {
            yield return "Błąd: " + ex.Message;
        }

        // --- Dodatkowy Fallback: symulacja streamu, gdy serwer nie wspiera SSE ---
        public async IAsyncEnumerable<string> StreamCompletionSimulatedAsync(
            IEnumerable<IChatMessage> chatMessages,
            string llmName,
            double llmNo = 0,
            int chunkSize = 32,
            int delayMs = 30,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            var full = await GetCompletionAsync(chatMessages, llmName, ct, llmNo);
            for (int i = 0; i < full.Length && !ct.IsCancellationRequested; i += chunkSize)
            {
                var part = full.Substring(i, Math.Min(chunkSize, full.Length - i));
                yield return part;
                await Task.Delay(delayMs, ct);
            }
        }
    }
}
