using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandValidator: AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty()
                .WithMessage("{Id} is required.")
                .GreaterThan(0)
                .WithMessage("{Id} must be greater than 0.");
        }
    }
}
