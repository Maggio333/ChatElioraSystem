using Newtonsoft.Json;

namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public class JsonReaderService
    {
        /// <summary>
        /// Odczytuje plik JSON i zwraca jego zawartość jako obiekt.
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku JSON.</param>
        /// <returns>Obiekt reprezentujący dane z pliku JSON, lub null w przypadku błędu.</returns>
        public static object ReadJsonFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Plik nie istnieje: {filePath}"); // Możesz użyć loggera zamiast Console.WriteLine
                    return null;
                }

                string jsonString = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas odczytu pliku JSON: {ex.Message}"); // Możesz użyć loggera zamiast Console.WriteLine
                return null;  // Zwróć null w przypadku błędu. Rozważ rzutowanie wyjątku, jeśli to bardziej odpowiednie.
            }
        }

        /// <summary>
        /// Odczytuje plik JSON i zwraca jego zawartość jako obiekt.
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku JSON.</param>
        /// <returns>Obiekt reprezentujący dane z pliku JSON, lub null w przypadku błędu.</returns>
        public static T? ReadJsonFile<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Plik nie istnieje: {filePath}"); // Możesz użyć loggera zamiast Console.WriteLine
                    return default;
                }

                string jsonString = File.ReadAllText(filePath);

                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas odczytu pliku JSON: {ex.Message}"); // Możesz użyć loggera zamiast Console.WriteLine
                return default;  // Zwróć null w przypadku błędu. Rozważ rzutowanie wyjątku, jeśli to bardziej odpowiednie.
            }
        }

        public static string? ReadJsonAsString(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Plik nie istnieje: {filePath}"); // Możesz użyć loggera zamiast Console.WriteLine
                    return default;
                }

                return File.ReadAllText(filePath);               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas odczytu pliku JSON: {ex.Message}"); // Możesz użyć loggera zamiast Console.WriteLine
                return default;  // Zwróć null w przypadku błędu. Rozważ rzutowanie wyjątku, jeśli to bardziej odpowiednie.
            }
        }
    }
}
