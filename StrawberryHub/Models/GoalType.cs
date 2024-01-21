using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class GoalType
{
    public int GoalTypeId { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Article> Article { get; set; } = new List<Article>();

    public virtual ICollection<Goal> Goal { get; set; } = new List<Goal>();
}
