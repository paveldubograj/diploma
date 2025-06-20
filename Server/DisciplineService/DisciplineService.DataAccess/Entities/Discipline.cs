using System;

namespace DisciplineService.DataAccess.Entities;

public class Discipline
{
    public Discipline()
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
    }
    public string Id {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public DateTime CreatedAt {get; set;}
}
