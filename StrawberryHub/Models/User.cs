using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public int? Points { get; set; }

    public int? RankId { get; set; }
    public DateTime? LastLogin { get; set; }

    [ValidateNever]
    public virtual ICollection<EmergencySupport> EmergencySupport { get; set; } = new List<EmergencySupport>();

    [ValidateNever]
    public virtual ICollection<Goal> Goal { get; set; } = new List<Goal>();

    [ValidateNever]
    public virtual Rank Rank { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<Reflection> Reflection { get; set; } = new List<Reflection>();

    [ValidateNever]
    public virtual ICollection<Task> Task { get; set; } = new List<Task>();
}
