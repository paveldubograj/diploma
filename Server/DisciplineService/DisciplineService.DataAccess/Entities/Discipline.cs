using System;

namespace DisciplineService.DataAccess.Entities;

public class Discipline
{
    public Discipline(){
        Id = new Guid().ToString();
    }
    public string Id {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public DateTime CreatedAt {get; set;}
}
