namespace StandardArticture.Common;

public class DataRequest
{
    public List<FilterRequest> Filters { get; set; } = new();
    public List<SortRequest> Sort { get; set; } = new();
    public PaginationRequest Pagination { get; set; } = new();
    public List<string> Columns { get; set; } = new();
}