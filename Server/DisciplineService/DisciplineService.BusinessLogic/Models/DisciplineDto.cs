using System;

namespace DisciplineService.BusinessLogic.Models;

public class DisciplineDto
{
    public string Id {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public DateTime CreatedAt {get; set;}
}
