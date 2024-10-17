namespace QRCodeGenerator.App.Interfaces
{
    public interface IMessageService
    {
        Task<byte[]> SendMessage(string message);
    }
}
