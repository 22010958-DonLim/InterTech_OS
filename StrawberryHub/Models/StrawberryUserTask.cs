using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryUserTask
{
    public int CompletedId { get; set; }

    public int? TaskId { get; set; }

    public int? UserId { get; set; }

    public int? Points { get; set; }

    public DateTime? CompletedDate { get; set; }

    [ValidateNever]
    public virtual StrawberryTask StrawberryTask { get; set; } = null!;

	[ValidateNever]
	public virtual StrawberryUser StrawberryUser { get; set; } = null!;
}
