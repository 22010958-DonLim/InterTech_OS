using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryComment
{
    public int CommentId { get; set; }

    public int? UserId { get; set; }

    public int? ArticleId { get; set; }

    public string? CommentText { get; set; }

    public DateTime? CommentDateTime { get; set; }

    [ValidateNever]
    public virtual StrawberryArticle StrawberryArticle { get; set; } = null!;

    [ValidateNever]
    public virtual StrawberryUser StrawberryUser { get; set; } = null!;
}
