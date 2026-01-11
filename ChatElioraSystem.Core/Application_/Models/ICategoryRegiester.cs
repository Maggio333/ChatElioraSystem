using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Domain.Models;

namespace ChatElioraSystem.Core.Application_.Models
{
    public interface ICategoryRegiester
    {
        List<CategoryModel> Categories { get; set; }
        string Description { get; }
        List<string> GetCategoriesList();
    }
}