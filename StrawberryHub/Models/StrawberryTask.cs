using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryTask
{
    public int TaskId { get; set; }

    public int? UserId { get; set; }

    public string TaskDescription { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public int PointsReward { get; set; }


    [ValidateNever]
    public virtual StrawberryUser User { get; set; } = null!;
}
