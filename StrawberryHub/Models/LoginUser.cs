using System.ComponentModel.DataAnnotations;

namespace StrawberryHub.Models;

public class LoginUser
{
    [Required(ErrorMessage = "Username cannot be empty")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Empty password not allowed")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public int UserId { get; set; }

    public bool RememberMe { get; set; }

}



