using Finance.Domain.Abstracts;

namespace Finance.Domain.Categories;

public static class CategoryErrors
{
    public static Error NotFound = new(
        "Category.NotFound",
        "A categoria com o identificador informado não foi encontrada");
}
