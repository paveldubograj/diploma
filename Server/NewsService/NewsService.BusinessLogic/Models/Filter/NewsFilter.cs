using System;
using NewsService.BusinessLogic.Models.Tag;
using NewsService.Shared.Enums;

namespace NewsService.BusinessLogic.Models.Filter;

public class NewsFilter
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SearchString { get; set; }
    public List<string>? Tags {get; set;} = new();
    public string? CategoryId {get; set;}
    public SortOptions? sortOptions {get; set;}
}
