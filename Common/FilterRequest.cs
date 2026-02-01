namespace HelpEmpowermentApi.Common;

public class FilterRequest
{
    public string PropertyName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public FilterOperation Operation { get; set; } = FilterOperation.Equal;
}

public enum FilterOperation
{
    Equal = 0,
    NotEqual = 1,
    Contains = 2,
    StartsWith = 3,
    EndsWith = 4,
    GreaterThan = 5,
    LessThan = 6,
    GreaterThanOrEqual = 7,
    LessThanOrEqual = 8,
    In = 9,
    NotIn = 10,
    IsNull = 11,
    IsNotNull = 12
}