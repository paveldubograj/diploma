using System;

namespace NewsService.BusinessLogic.Models.News;

public class NewsUpdateDto
{
    public string Id { get; set; } 
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishingDate { get; set; } 
    public string CategoryId { get; set; } 
    public string AuthorId { get; set; } 
}
