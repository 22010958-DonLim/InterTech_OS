using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public int? GoalTypeId { get; set; }

    public string ArticleContent { get; set; } = null!;

    public DateTime PublishedDate { get; set; }

    public virtual GoalType GoalType { get; set; } = null!;
}
