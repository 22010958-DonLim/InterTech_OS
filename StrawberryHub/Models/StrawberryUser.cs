using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

	[DataType(DataType.EmailAddress)]
	public string? Email { get; set; }

    public int? Points { get; set; }

    public int? RankId { get; set; }

    //public int? GoalTypeId { get; set; }

    public string UserRole { get; set; } = null!;

    [ValidateNever]
    public IFormFile Photo { get; set; } = null!;

    public string Picture { get; set; } = null!;

    public int? Otp { get; set; } // One-time password
    public int? OtpCount { get; set; } // OTP attempt count
    public string? TelegramId { get; set; } // Telegram user ID

    public virtual ICollection<StrawberryGoalType> StrawberryGoalType { get; set; } = new List<StrawberryGoalType>();

    [ValidateNever]
    public virtual ICollection<StrawberryComment> StrawberryComment { get; set; } = new List<StrawberryComment>();

    public virtual StrawberryRank StrawberryRank { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<StrawberryFeedback> StrawberryFeedback { get; set; } = new List<StrawberryFeedback>();

    [ValidateNever]
    public virtual ICollection<StrawberryGoal> Goal { get; set; } = new List<StrawberryGoal>();

    //[ValidateNever]
    //public virtual ICollection<StrawberryTask> Task { get; set; } = new List<StrawberryTask>();

    [ValidateNever]
    public virtual ICollection<StrawberryArticle> StrawberryArticle { get; set; } = new List<StrawberryArticle>();

    [ValidateNever]
    public virtual ICollection<StrawberryLike> StrawberryLike { get; set; } = new List<StrawberryLike>();

	[ValidateNever]
	public virtual ICollection<StrawberryUserTask> StrawberryUserTask { get; set; } = new List<StrawberryUserTask>();
}
