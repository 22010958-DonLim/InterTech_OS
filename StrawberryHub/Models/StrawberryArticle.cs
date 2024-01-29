using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryArticle
{
    public int ArticleId { get; set; }

    public int? GoalTypeId { get; set; }

    public string ArticleContent { get; set; } = null!;

    public DateTime PublishedDate { get; set; }

    [ValidateNever]
    public virtual StrawberryGoalType GoalType { get; set; } = null!;

}
