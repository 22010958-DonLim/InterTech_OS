using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryLikeComment
{
    public int CommentId { get; set; }

    public int? UserId { get; set; }

    public int? ArticleId { get; set; }

    public string? CommentText { get; set; }

    public int? Likes { get; set; }

    public DateTime? CommentTimestamp { get; set; }

    public DateTime? LikeTimestamp { get; set; }

    [ValidateNever]
    public virtual StrawberryArticle StrawberryArticle { get; set; } = null!;

    [ValidateNever]
    public virtual StrawberryUser StrawberryUser { get; set; } = null!;
}
