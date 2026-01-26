namespace StandardArticture.Common;

public class SortRequest
{
    public string SortBy { get; set; } = string.Empty;
    public string SortDirection { get; set; } = "asc"; // asc, desc
}