using System;

namespace NewsService.BusinessLogic.Models.News;

public class NewsUpdateDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string CategoryId { get; set; } 
    public string ImagePath { get; set; }
}
