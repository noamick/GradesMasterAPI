namespace backend.Dtos
{
    public class AppSettings
    {
        public string JwtSecretKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public int JwtExpireMinutes { get; set; }
        public double JwtRefreshExpireDays { get; internal set; }
    }
}
