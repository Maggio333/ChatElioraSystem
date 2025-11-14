using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public class RAGPromptCode : IRAGPromptCode
    {
        public RAGPromptCode()
        {
            WzorceProjektowe = GetWzorceProjektowe();
        }

        public string WzorceProjektowe { get; private set; }

        private static string GetWzorceProjektowe()
        {
            return """
                {
                  "name": "SeedPack_ReflectumCoding",
                  "version": "1.0",
                  "description": "Złożony seedpack architektoniczny dla systemów ewolucyjnych opartych o C# GeneticService",
                  "layers": [
                    {
                      "layer": "ServiceLayer",
                      "id": "Seed_Genetic_ServiceRuntime",
                      "intent": "Zdefiniuj usługę ewolucyjną jako serwis implementujący interfejs.",
                      "implementation": "IGeneticService<T> + GeneticService<T>",
                      "benefit": "Izolacja logiki, testowalność, wstrzykiwanie zależności",
                      "risk": "Możliwy narzut na strukturę w małych projektach"
                    },
                    {
                      "layer": "ConfigLayer",
                      "id": "Seed_Genetic_Config",
                      "intent": "Centralizuj wszystkie parametry w jednym obiekcie konfiguracyjnym.",
                      "implementation": "GeneticConfig<T>",
                      "benefit": "Przejrzystość, skalowalność, łatwiejsze API",
                      "risk": "Zbyt sztywny config bez rozszerzalności może ograniczać"
                    },
                    {
                      "layer": "StrategyLayer",
                      "id": "Seed_Genetic_Strategies",
                      "intent": "Zastosuj strategię jako interfejsy do oceny, mutacji i krzyżowania.",
                      "implementation": "IFitnessEvaluator<T>, IMutationStrategy<T>, ICrossoverStrategy<T>",
                      "benefit": "Dynamiczna podmiana algorytmów bez modyfikacji serwisu",
                      "risk": "Potrzeba dodatkowego kodu dla każdej strategii"
                    },
                    {
                      "layer": "CallbackLayer",
                      "id": "Seed_Genetic_CallbackIntent",
                      "intent": "Pozwól użytkownikowi wstrzykiwać zachowania przez callbacki.",
                      "implementation": "Func<T, int, Task> OnEvaluateCallback",
                      "benefit": "Integracja z UI, logowaniem, telemetrią, LLM",
                      "risk": "Callbacki mogą zaburzyć deterministykę"
                    },
                    {
                      "layer": "ObserverLayer",
                      "id": "Seed_Genetic_Observer",
                      "intent": "Obserwuj przebieg generacji za pomocą observera.",
                      "implementation": "IGeneticObserver<T>",
                      "benefit": "Separacja logowania i telemetrii od logiki",
                      "risk": "Dodatkowa zależność runtime"
                    },
                    {
                      "layer": "ReflectionLayer",
                      "id": "Seed_ReflectionLayer",
                      "intent": "Automatyczne generowanie i utrzymanie dokumentacji oraz metadanych kodu, aby wspierać zrozumienie i rozwój.",
                      "implementation": "Narzędzia do analizy kodu, atrybuty, XML comments, generatory dokumentacji.",
                      "benefit": "Lepsza czytelność, łatwiejsza współpraca, trwała pamięć intencji.",
                      "risk": "Nadmiar dokumentacji może stać się ciężarem, wymaga dyscypliny."
                    },
                    {
                      "layer": "TestingLayer",
                      "id": "Seed_TestingLayer",
                      "intent": "Integracja testów jednostkowych i integracyjnych jako integralnej części rozwoju.",
                      "implementation": "Frameworki testowe (np. xUnit, NUnit), mockowanie, CI/CD.",
                      "benefit": "Wysoka jakość, szybkie wykrywanie błędów, pewność zmian.",
                      "risk": "Koszt utrzymania testów, ryzyko fałszywego poczucia bezpieczeństwa."
                    },
                    {
                      "layer": "CleanArchitectureLayer",
                      "id": "Seed_Clean_Architecture",
                      "intent": "Zorganizuj projekt zgodnie z zasadami Clean Architecture, oddzielając domenę, aplikację, infrastrukturę i interfejsy.",
                      "implementation": {
                        "Domain": "Encje, agregaty, interfejsy repozytoriów, logika biznesowa",
                        "Application": "Use case'y, serwisy aplikacyjne, DTO, interfejsy serwisów",
                        "Infrastructure": "Implementacje repozytoriów, integracje z zewnętrznymi systemami, parsery plików .ifc",
                        "Presentation": "UI, API, kontrolery, warstwa interakcji z użytkownikiem"
                      },
                      "benefit": "Modularność, testowalność, łatwość utrzymania i rozwoju",
                      "risk": "Większa złożoność na początku, wymaga dyscypliny w przestrzeganiu zasad"
                    }
                  ]
                }
                """;
        }
         
        public string CodeLanguage => "C#, .Net, WPF";

        public string Role => "Masz dodatkową rolę, jesteś ekspertem w programowaniu. Prowadzisz użytkownika po wzorcach projektowych, pomagasz z implementacją kodu oraz sugerujesz rozwiązania. Stawiasz duży nacisk na architekturę";
    }
}
