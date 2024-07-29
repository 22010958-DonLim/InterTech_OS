using System.ComponentModel.DataAnnotations;

namespace StrawberryHub.Models;

public class UserProfileViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string RankName { get; set; } = null!; // Changed from Rank to RankName

    public int? Points { get; set; }

    public string Picture { get; set; } = null!;
    public List<StrawberryArticle> Posts { get; set; } = null!;
}




