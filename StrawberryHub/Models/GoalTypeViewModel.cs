using StrawberryHub.Controllers;
using System.ComponentModel.DataAnnotations;

namespace StrawberryHub.Models;

public class GoalTypeViewModel
{
    public int GoalTypeId { get; set; }
    public string Types { get; set; } = null!;
    public bool IsSelected { get; set; }
}



