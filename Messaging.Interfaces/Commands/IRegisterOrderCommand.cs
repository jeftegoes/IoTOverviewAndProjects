using System;

namespace Messaging.Interfaces.Commands
{
    public interface IRegisterOrderCommand
    {
        public Guid OrderId { get; set; }
        string PictureUrl { get; set; }
        string UserEmail { get; set; }
        byte[] ImageData { get; set; }
    }
}