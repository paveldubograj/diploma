using System;

namespace NewsService.BusinessLogic.Models.News;

public class NewsCleanDto
{
    public string Id { get; set; } 
    public string Title { get; set; }
    public DateTime PublishingDate { get; set; } 
    public string AuthorId { get; set; } 
}
