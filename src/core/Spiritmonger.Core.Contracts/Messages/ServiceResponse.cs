namespace Spiritmonger.Core.Contracts.Messages
{
    public class ServiceResponse : IServiceResponse
    {
        public ServiceResponse()
        {
            Succeeded = true;
        }

        public ServiceResponse(string errorMessage)
        {
            Succeeded = false;
            Error = errorMessage;
        }

        public string Error { get; set; }
        public bool Succeeded { get; set; }
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public ServiceResponse() : base()
        {
        }

        public ServiceResponse(string errorMessage): base(errorMessage)
        {
        }

        public ServiceResponse(T data) : base()
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
