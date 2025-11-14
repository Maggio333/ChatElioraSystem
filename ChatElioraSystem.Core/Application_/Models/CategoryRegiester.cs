using ChatElioraSystem.Core.Application_.Enums;

namespace ChatElioraSystem.Core.Application_.Models
{
    public class CategoryRegiester
    {
        public CategoryRegiester()
        {
            Categories.Add("<Category:General>", SesjaTematu.Ogólna);
            Categories.Add("<Category:Code>", SesjaTematu.Kod);
            Categories.Add("<Category:Reflection>", SesjaTematu.Refleksyjna);
            Categories.Add("<Category:CodeArchitecture>", SesjaTematu.ArchitekturaKodu);

        }

        public Dictionary<string, SesjaTematu> Categories { get; set; } = new Dictionary<string, SesjaTematu>();

    }
}
