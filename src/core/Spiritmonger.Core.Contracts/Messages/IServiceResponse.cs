namespace Spiritmonger.Core.Contracts.Messages
{
    public interface IServiceResponse
    {
        bool Succeeded { get; }
        string Error { get; }
    }
}
