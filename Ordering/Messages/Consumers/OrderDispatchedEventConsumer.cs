using System;
using System.Threading.Tasks;
using MassTransit;
using Messaging.Interfaces.Events;
using Ordering.Models;
using Ordering.Persistence;

namespace Ordering.Messages.Consumers
{
    public class OrderDispatchedEventConsumer : IConsumer<IOrderDispatchedEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderDispatchedEventConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var message = context.Message;
            var orderId = message.OrderId;
            UpdateDatabase(orderId);
            return Task.CompletedTask;
        }

        private void UpdateDatabase(Guid orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            
            if (order != null)
            {
                order.Status = Status.Sent;
                _orderRepository.UpdateOrder(order);
            }
        }
    }
}