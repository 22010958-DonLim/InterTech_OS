using System.ComponentModel.DataAnnotations;

namespace StrawberryHub.Models;

public class LoginOTP
{
    public int UserId { get; set; }

    public int OTP { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

}



