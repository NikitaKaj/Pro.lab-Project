namespace ProLab.Api.Infrastructure.Configurations
{
    public class MapboxOptions
    {
        public const string SectionName = "Mapbox";

        public string AccessToken { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.mapbox.com";
        public int CacheDurationMinutes { get; set; } = 60;
    }
}
