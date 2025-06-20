using System;
using NewsService.BusinessLogic.Models.Tag;

namespace NewsService.BusinessLogic.Models.News;

public class NewsDto
{
    public string Id { get; set; } 
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishingDate { get; set; } 
    public string CategoryId { get; set; } 
    public string AuthorId { get; set; } 
    public string AuthorName { get; set; }
    public bool Visibility { get; set; }
    public string ImagePath { get; set; }

    public List<TagDto> tags { get; set; } = new(); 
}
