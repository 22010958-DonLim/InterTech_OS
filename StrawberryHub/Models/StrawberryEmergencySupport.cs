using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryEmergencySupport
{
    public int EmergencySupportId { get; set; }

    public int? UserId { get; set; }

    public DateTime Timestamp { get; set; }

    public string Message { get; set; } = null!;


    [ValidateNever]
    public virtual StrawberryUser User { get; set; } = null!;
}
