using System.ComponentModel.DataAnnotations;

namespace StrawberryHub.Models;

public class RegisterUser
{

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public int? Points { get; set; }

    public int? RankId { get; set; }

    public int? GoalTypeId { get; set; }



}



