using System;

namespace Messaging.Interfaces.Events
{
    public interface IOrderDispatchedEvent
    {
        Guid OrderId { get; }
        DateTime DispatchDateTime { get; }
    }
}