using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryFeedback
{
    public int FeedbackId { get; set; }

    public int? UserId { get; set; }

    public int Stars { get; set; }

    public string Message { get; set; } = null!;

    public virtual StrawberryUser? User { get; set; }
}
