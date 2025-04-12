using FluentValidation;

namespace DapperSRP.Dto.Product.CreateProduct.Request
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required");
        }
    }
}
