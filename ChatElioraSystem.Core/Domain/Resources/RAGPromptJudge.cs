using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Domain.Resources.Bases;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public class RAGPromptJudge : BaseRAGPrompt, IRAGPromptJudge
    {
        public new string Role { get => "Otrzymałaś dodatkowo rolę sędziego który ocenia jaki jest temat ogólny prowadzonej rozmowy na podstawie dyskusji. Nie możesz odpowiadać użytkownikowi na te pytania, masz tylko stwierdzić jaki to jest typ rozmowy. Musisz odpowiadać Kategoriami które masz sztywno ustawione. Aby odpowiedzieć mi, która to Kategoria, zastosuj format <Category:Nazwa_Kategorii>. Przykłady użycia: <Category:General>"; }

        public string GetTheme()
        {
            var test = new CategoryRegiester().Categories;
            string result = string.Empty;
            result = "Dostępne Kategorie to: { ";

            foreach (var c in test)
            {
                result += $"{c.Key}, ";
            }

            result += "}";

            return result;
        }

        public string Description => "Wytłumaczenie Kategorii\n\t<Category:General> - Generalna rozmowa, która nie jest techniczna ani refleksyjna\n\t<Category:Code> - Gdy rozmowa jest techniczna, gdy piszesz kod to też ta kategoria\n\t<Category:Reflection> - Gdy rozmowa jest refleksyjna\n\t<Category:CodeArchitecture> - Gdy jest rozmowa o architekturze kodu lub struktur. Gdy rozmawiasz o architekturze";
    
    }
}
