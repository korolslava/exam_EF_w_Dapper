namespace exam_Ef_dapper_14_3.Validators;

using exam_Ef_dapper_14_3.DTOs;
using FluentValidation;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Customer email must be a valid email address.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Key)
                .GreaterThan(0).WithMessage("Book ID must be greater than zero.");

            item.RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
                .LessThanOrEqualTo(100).WithMessage("Cannot order more than 100 copies at once.");
        });
    }
}