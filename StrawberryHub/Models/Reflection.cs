using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class Reflection
{
    public int ReflectionId { get; set; }

    public int? UserId { get; set; }

    public DateTime Date { get; set; }

    public string Content { get; set; } = null!;


    [ValidateNever]
    public virtual User User { get; set; } = null!;
}
