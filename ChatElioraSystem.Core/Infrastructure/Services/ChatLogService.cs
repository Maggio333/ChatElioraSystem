using ChatElioraSystem.Core.Domain.Services;
using ChatElioraSystem.Core.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public class ChatLogService : IChatLogService
    {
        //private readonly string _logDirectory;
        private readonly string _filenamePrefix = "chat";
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly DateTime _createdDate;
        private string _fileName;
        private readonly Encoding _encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        private readonly string _basePath;

        public ChatLogService(IStoragePathProvider pathProvider)
        {
            _basePath = pathProvider.GetMemoryDirectory();
            GenerateNewFileName();
        }


        public void GenerateNewFileName() => _fileName = GenerateFilename();


        public string Save(IEnumerable<IChatMessage> messages, IDictionary<string, object>? metadata = null) => Save(messages, string.Empty, metadata);
        public string Save(IEnumerable<IChatMessage> messages, string filename, IDictionary<string, object>? metadata = null)
        {
            if (filename != null && filename != string.Empty)
            {
                _fileName = filename;
            }

            string path = Path.Combine(_basePath, _fileName);

            var logEntry = new
            {
                metadata = metadata ?? new Dictionary<string, object>
                {
                    { "system", "Eliora Reflectum" },
                    { "generatedAt", DateTime.Now }
                },
                messages
            };

            string json = JsonSerializer.Serialize(logEntry, _serializerOptions);
            File.WriteAllText(path, json, _encoding);

            return path;
        }

        private string GenerateFilename()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            _fileName = $"{_filenamePrefix}_{timestamp}.json";
            return _fileName;
        }

        public IEnumerable<IChatMessage> Load(string fileName, out IDictionary<string, object>? metadata)
        {
            string path = Path.Combine(_basePath, fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException("Nie znaleziono pliku logu.", path);

            string json = File.ReadAllText(path, Encoding.UTF8); // obsługa BOM
            using var doc = JsonDocument.Parse(json);

            metadata = new Dictionary<string, object>();
            var root = doc.RootElement;

            if (root.TryGetProperty("metadata", out JsonElement metaElement))
            {
                foreach (var kvp in metaElement.EnumerateObject())
                {
                    metadata[kvp.Name] = GetDynamicValue(kvp.Value);
                }
            }

            if (!root.TryGetProperty("messages", out JsonElement messagesElement))
                throw new InvalidDataException("Brakuje sekcji 'messages' w logu.");

            var messages = new List<IChatMessage>();

            foreach (var msg in messagesElement.EnumerateArray())
            {
                var message = new ChatMessage();

                message.Content = GetPropertyIgnoreCase(msg, "content") ?? "";

                var roleStr = GetPropertyIgnoreCase(msg, "role") ?? "Assistant";
                message.Role = Enum.Parse<Role>(roleStr, ignoreCase: true);

                var timestamp = GetDateTimePropertyIgnoreCase(msg, "timestamp");
                message.Timestamp = timestamp ?? DateTime.MinValue;

                messages.Add(message);
            }

            return messages;
        }

        public List<string> ListLogFiles()
        {
            if (!Directory.Exists(_basePath))
                return new List<string>();

            var files = Directory.GetFiles(_basePath, "*.json", SearchOption.TopDirectoryOnly);

            return files.Select(Path.GetFileName).OrderByDescending(name => name).ToList();
        }

        string? GetPropertyIgnoreCase(JsonElement element, string propertyName)
        {
            foreach (var prop in element.EnumerateObject())
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    return prop.Value.ToString();
            }
            return null;
        }

        DateTime? GetDateTimePropertyIgnoreCase(JsonElement element, string propertyName)
        {
            foreach (var prop in element.EnumerateObject())
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    return prop.Value.GetDateTime();
            }
            return null;
        }


        // Pomocnicze: dynamiczne odczytywanie wartości JSON
        private object GetDynamicValue(JsonElement el)
        {
            return el.ValueKind switch
            {
                JsonValueKind.Number when el.TryGetInt64(out var i) => i,
                JsonValueKind.Number => el.GetDouble(),
                JsonValueKind.String => el.GetString() ?? "",
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => el.ToString() ?? ""
            };
        }
    }
}
