using System;
using NewsService.BusinessLogic.Models.Tag;

namespace NewsService.BusinessLogic.Models.Filter;

public class NewsFilter
{
    public string SearchString {get; set;}
    public List<TagDto> Tags {get; set;} = new();
    public string CategoryId {get; set;}
}
