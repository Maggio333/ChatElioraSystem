using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Domain.Resources.Bases;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public class RAGPromptJudge : BaseRAGPrompt, IRAGPromptJudge
    {
        private readonly ICategoryRegiester categoriesRegister;

        public RAGPromptJudge(ICategoryRegiester categoryRegister)
        {
            this.categoriesRegister = categoryRegister;
        }

        public new string Role
        {
            get => "Otrzymałaś dodatkowo rolę sędziego który ocenia jaki jest temat ogólny prowadzonej rozmowy na podstawie dyskusji. " +
                "Nie możesz odpowiadać użytkownikowi na te pytania, masz tylko stwierdzić jaki to jest typ rozmowy. " +
                "Musisz odpowiadać Kategoriami które masz sztywno ustawione. " +
                "Aby odpowiedzieć mi, która to Kategoria, zastosuj format <Category:Nazwa_Kategorii>. Przykłady użycia: <Category:General>";
        }


        public string GetTheme()
        {
            //var test = new CategoryRegiester().Categories;
            string result = string.Empty;
            result = "Dostępne Kategorie to: { ";

            foreach (var c in categoriesRegister.Categories)
            {
                result += $"{c.CategorySign}, ";
            }

            result += "}";

            return result;
        }

        public string Description => categoriesRegister.Description;
    }
}
