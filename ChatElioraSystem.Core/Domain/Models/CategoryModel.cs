using ChatElioraSystem.Core.Application_.Enums;

namespace ChatElioraSystem.Core.Domain.Models
{
    public class CategoryModel : ICategoryModel
    {
        public CategoryModel(SesjaTematu sesjaTematu, string description, string categorySign)
        {
            SesjaTematu = sesjaTematu;
            Description = description;
            CategorySign = categorySign;
        }

        public string Description { get; set; }
        public string CategorySign { get; set; }
        public SesjaTematu SesjaTematu { get; set; }
    }
}
