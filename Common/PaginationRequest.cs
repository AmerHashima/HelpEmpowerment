namespace HelpEmpowermentApi.Common;

public class PaginationRequest
{
    public bool GetAll { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}