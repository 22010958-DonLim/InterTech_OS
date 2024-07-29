using System.ComponentModel.DataAnnotations;

namespace StrawberryHub.Models;

public class EditAdmin
{

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public int? Points { get; set; }

    public int? RankId { get; set; }

    [ValidateNever]
    public IFormFile Photo { get; set; } = null!;

    public string Picture { get; set; } = null!;

    public string? TelegramId { get; set; } // Telegram user ID

    public string UserRole { get; set; } = null!;
}



