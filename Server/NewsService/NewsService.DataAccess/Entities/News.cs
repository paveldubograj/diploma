using System;

namespace NewsService.DataAccess.Entities;

public class News
{
    public News(){
        Id = new Guid().ToString();
    }
    public string Id { get; set; } 
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishingDate { get; set; } 
    public string CategoryId { get; set; } 
    public string AuthorId { get; set; } 

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
