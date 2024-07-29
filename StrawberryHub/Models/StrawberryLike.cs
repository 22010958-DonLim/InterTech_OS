using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryLike
{
    public int LikeId { get; set; }

    public int? UserId { get; set; }

    public int? ArticleId { get; set; }

    public int? Likes { get; set; }

    public DateTime? LikeDateTime { get; set; }

    [ValidateNever]
    public virtual StrawberryArticle StrawberryArticle { get; set; } = null!;

    [ValidateNever]
    public virtual StrawberryUser StrawberryUser { get; set; } = null!;
}
