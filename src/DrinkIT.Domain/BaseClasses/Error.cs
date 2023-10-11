namespace DrinkIT.Domain.BaseClasses
{
    public class Error
    {
        public Error(string message) => Message = message ?? string.Empty;
        public Error(string message, Exception exception)
        {
            Message = message ?? string.Empty;
            Exception = exception;
        }

        public string Message { get; }
        public Exception? Exception { get; }
    }
}
