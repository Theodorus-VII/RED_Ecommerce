namespace Ecommerce.Utilities
{
    public class ServiceResult<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T Data { get; }

        public ServiceResult(bool success, string message, T data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

}
