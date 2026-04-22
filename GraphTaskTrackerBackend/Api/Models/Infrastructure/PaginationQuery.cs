namespace GraphTaskTrackerBackend.Api.Models;

public class PaginationQuery
{
    public int PageNumber {get; set;}
    public int PageSize {get; set;}
    public string? KeyWordForSearch {get; set;}
}