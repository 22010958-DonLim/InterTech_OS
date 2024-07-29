using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryTask
{
    public int TaskId { get; set; }

    public string TaskDescription { get; set; } = null!;

    public int PointsReward { get; set; }

	[ValidateNever]
	public virtual ICollection<StrawberryUserTask> StrawberryUserTask { get; set; } = new List<StrawberryUserTask>();

}
