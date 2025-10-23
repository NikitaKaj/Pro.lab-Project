namespace ProLab.Data.Entities;

public class LogEntry
{
	public int Id { get; set; }
	public string Category { get; set; }
	public string LogLevel { get; set; }
	public string Message { get; set; }
	public string? StackTrace { get; set; }
	public DateTimeOffset Timestamp { get; set; }
}
