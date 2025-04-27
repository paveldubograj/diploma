using System;

namespace NewsService.BusinessLogic.Models.News;

public class NewsPagedResponse
{
    public List<NewsCleanDto> News {get; set;}
    public int Total {get; set;}
}
