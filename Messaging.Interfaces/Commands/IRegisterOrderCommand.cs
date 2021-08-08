namespace Messaging.Interfaces.Commands
{
    public interface IRegisterOrderCommand
    {
        string PictureUrl { get; set; }
        string UserEmail { get; set; }
        byte[] ImageData { get; set; }
    }
}