using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class EmergencySupport
{
    public int EmergencySupportId { get; set; }

    public int? UserId { get; set; }

    public DateTime Timestamp { get; set; }

    public string Message { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
