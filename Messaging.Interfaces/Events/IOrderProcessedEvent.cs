using System;
using System.Collections.Generic;

namespace Messaging.Interfaces.Events
{
    public interface IOrderProcessedEvent
    {
        Guid OrderId { get; }
        string PirctureUrl { get; }
        List<byte[]> Faces { get; }
        String UserEmail { get; }
    }
}