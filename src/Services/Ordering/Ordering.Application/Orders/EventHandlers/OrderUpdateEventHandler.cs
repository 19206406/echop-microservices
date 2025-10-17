namespace Ordering.Application.Orders.EventHandlers
{
    public class OrderUpdateEventHandler(ILogger<OrderUpdateEventHandler> logger)
        : INotificationHandler<OrderUpdatedEvent>
    {
        public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType());
            return Task.CompletedTask; 
        }
    }
}
