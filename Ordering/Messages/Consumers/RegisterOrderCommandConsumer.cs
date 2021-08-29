using System;
using System.Threading.Tasks;
using MassTransit;
using Messaging.Interfaces.Commands;

namespace Ordering.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        public Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}