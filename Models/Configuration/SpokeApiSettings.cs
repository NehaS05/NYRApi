namespace NYR.API.Models.Configuration
{
    public class SpokeApiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.spoke.com/v1";
        public int TimeoutSeconds { get; set; } = 30;
    }
}
