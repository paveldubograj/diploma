using System;

namespace NewsService.DataAccess.Entities;

public class NewsList
{
    public List<News> News { get; set; }
    public int Total { get; set; }
}
