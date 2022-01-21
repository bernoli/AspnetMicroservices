using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;
using Ordering.Application.Exceptions;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler: IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;
        
        public DeleteOrderCommandHandler(IOrderRepository orderRepository, ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null)
            {
                _logger.LogError($"Order [{request.Id}] not found in database.");
                throw new NotFoundException(nameof(order), request.Id);
            }

            await _orderRepository.DeleteAsync(order);
            return Unit.Value;
        }
    }
}
