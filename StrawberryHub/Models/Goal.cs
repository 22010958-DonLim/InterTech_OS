using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class Goal
{
    public int GoalId { get; set; }

    public int? UserId { get; set; }

    public int? GoalTypeId { get; set; }


    [ValidateNever]
    public virtual GoalType GoalType { get; set; } = null!;

    [ValidateNever]
    public virtual User User { get; set; } = null!;
}
