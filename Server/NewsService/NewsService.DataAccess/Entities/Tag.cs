using System;

namespace NewsService.DataAccess.Entities;

public class Tag
{
    public Tag(){
        Id = new Guid().ToString();
    }
    public string Id { get; set; } 
    public string Name { get; set; } 
    public virtual ICollection<News> News { get; set; } = new List<News>();
}
