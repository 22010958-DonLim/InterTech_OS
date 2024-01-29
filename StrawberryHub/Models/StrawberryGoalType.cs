using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryGoalType
{
    public int GoalTypeId { get; set; }

    public string Type { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<StrawberryArticle> StrawberryArticles { get; set; } = new List<StrawberryArticle>();

    [ValidateNever]
    public virtual ICollection<StrawberryGoal> Goal { get; set; } = new List<StrawberryGoal>();

    [ValidateNever]
    public virtual ICollection<StrawberryUser> StrawberryUsers { get; set; } = new List<StrawberryUser>();
}
