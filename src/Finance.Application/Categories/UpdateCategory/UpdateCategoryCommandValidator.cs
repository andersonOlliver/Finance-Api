using FluentValidation;

namespace Finance.Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Name).NotEmpty().MaximumLength(200).WithMessage("Informe um nome válido para a categoria");
        RuleFor(p => p.Type).IsInEnum().WithMessage("Informe um tipo de categoria válido");
        RuleFor(p => p.Color).NotEmpty().MaximumLength(8).WithMessage("Informe uma cor válida");
        RuleFor(p => p.Icon).NotEmpty().MaximumLength(20).WithMessage("Informe um ícone válido");
    }
}
