using Microsoft.AspNetCore.Mvc;

namespace StrawberryHub.Models;

public class Leaderboard
{
    public List<StrawberryUser> TopUsers { get; set; } = null!;
    public List<int> MostLikedArticles { get; set; } = null!;
}
