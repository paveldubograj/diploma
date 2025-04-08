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
    public override bool Equals(object? obj)
    {
        if(obj is null) return false;
        if(obj.GetType() !=  this.GetType()) return false;
        if(!(obj as Tag).Id.Equals(Id)) return false;
        if(!(obj as Tag).Name.Equals(Name)) return false;
        return true;
    }
}
