namespace backend.Dtos
{
    public class AuthenticateResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }

    public class AuthResult
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public AuthenticateResponse AuthenticateResponse { get; set; }
    }
}
