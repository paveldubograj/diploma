using System;

namespace NewsService.DataAccess.Entities;

public class News
{
    public News(){ Id = Guid.NewGuid().ToString(); }
    public string Id { get; set; } 
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishingDate { get; set; }
    public string CategoryId { get; set; } 
    public string AuthorId { get; set; } 
    public string AuthorName { get; set; }
    public bool Visibility { get; set; } = true;
    public string ImagePath { get; set; } = string.Empty;
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
