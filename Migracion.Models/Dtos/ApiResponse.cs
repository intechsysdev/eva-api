namespace Migracion.Models.Dtos
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public T? ResultData { get; set; }

        public static ApiResponse<T> Success(T data, string message = "OK")
        {
            return new ApiResponse<T>
            {
                Message = message,
                StatusCode = 200,
                ResultData = data
            };
        }

        public static ApiResponse<T> Error(string message, int statusCode = 500)
        {
            return new ApiResponse<T>
            {
                Message = message,
                StatusCode = statusCode,
                ResultData = default
            };
        }
    }
}
