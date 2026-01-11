using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Domain.Models;
using System.Text;

namespace ChatElioraSystem.Core.Application_.Models
{
    public class CategoryRegiester : ICategoryRegiester
    {
        public CategoryRegiester()
        {
            Categories.Add(new CategoryModel(SesjaTematu.Ogólna, "Generalna rozmowa, która nie jest techniczna ani refleksyjna", "<Category:General>"));
            Categories.Add(new CategoryModel(SesjaTematu.Kod, "Gdy rozmowa jest techniczna, gdy piszesz kod to też ta kategoria", "<Category:Code>"));
            Categories.Add(new CategoryModel(SesjaTematu.Refleksyjna, "Gdy rozmowa jest refleksyjna", "<Category:Reflection>"));
            Categories.Add(new CategoryModel(SesjaTematu.ArchitekturaKodu, "Gdy jest rozmowa o architekturze kodu lub struktur. Gdy rozmawiasz o architekturze", "<Category:CodeArchitecture>"));
        }
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

        public string Description => GetDescription();

        public List<string> GetCategoriesList()
        {
            List<string> result = new List<string>();

            foreach (var category in Categories)
            {
                result.Add(category.CategorySign);
            }
            
            return result;
        }

        public string GetDescription()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Wytłumaczenie Kategorii:");
            foreach (var category in Categories)
            {
                stringBuilder.AppendLine($"{category.CategorySign} - {category.Description}");
            }

            return stringBuilder.ToString();

        }
    }
}
