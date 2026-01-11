using ChatElioraSystem.Core.Application_.Enums;

namespace ChatElioraSystem.Core.Domain.Models
{
    public interface ICategoryModel
    {
        string Description { get; set; }
        string CategorySign { get; set; }
    }
}