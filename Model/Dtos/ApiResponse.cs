namespace Remitplus_Authentication.Model.Dtos
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static ApiResponse Success(string message, object? data = null) =>
            new ApiResponse { IsSuccess = true, Message = message, Data = data };

        public static ApiResponse Failed(string message) =>
            new ApiResponse { IsSuccess = false, Message = message };
    }

}
