using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryGoal
{
    public int GoalId { get; set; }

    public int? UserId { get; set; }

    public int? GoalTypeId { get; set; }


    [ValidateNever]
    public virtual StrawberryGoalType GoalType { get; set; } = null!;

    [ValidateNever]
    public virtual StrawberryUser User { get; set; } = null!;
}
