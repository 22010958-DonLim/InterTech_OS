using System.ComponentModel.DataAnnotations;

namespace StrawberryHub.Models;

public class UserProfileViewModel
{
    public string Username { get; set; } = null!;
    public string RankName { get; set; } = null!; // Changed from Rank to RankName
    public List<StrawberryArticle> Posts { get; set; } = null!;
}




