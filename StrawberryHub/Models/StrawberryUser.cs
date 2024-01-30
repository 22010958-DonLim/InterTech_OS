using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryUser
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Username cannot be empty")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Empty password not allowed")]
    public byte[] Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public int? Points { get; set; }

    public int? RankId { get; set; }

    public int? GoalTypeId { get; set; }

    public string UserRole { get; set; } = null!;

    public virtual StrawberryGoalType StrawberryGoalType { get; set; } = null!;

    public virtual StrawberryRank StrawberryRank { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<StrawberryEmergencySupport> EmergencySupport { get; set; } = new List<StrawberryEmergencySupport>();

    [ValidateNever]
    public virtual ICollection<StrawberryGoal> Goal { get; set; } = new List<StrawberryGoal>();

    [ValidateNever]
    public virtual ICollection<StrawberryReflection> Reflection { get; set; } = new List<StrawberryReflection>();

    [ValidateNever]
    public virtual ICollection<StrawberryTask> Task { get; set; } = new List<StrawberryTask>();
}
