namespace ProLab.Data;


public class AppSettings
{
	public string Secret { get; set; }
	public List<string> CorsWhitelist { get; set; }
	public string FrontendBaseUrl { get; set; }
	public string BackendBaseUrl { get; set; }
}
