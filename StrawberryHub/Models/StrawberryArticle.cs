using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrawberryHub.Models;

public partial class StrawberryArticle
{
    public int ArticleId { get; set; }

    public int? GoalTypeId { get; set; }

    public string Title { get; set; } = null!;

    public string ArticleContent { get; set; } = null!;

    public DateTime PublishedDate { get; set; }

    [ValidateNever]
    public IFormFile Photo { get; set; } = null!;

    public string Picture { get; set; } = null!;

    public int UserId { get; set; }

    [ValidateNever]
    public virtual StrawberryGoalType GoalType { get; set; } = null!;

    [ValidateNever]
    public virtual StrawberryUser StrawberryUser { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<StrawberryComment> StrawberryComment { get; set; } = new List<StrawberryComment>();

    [ValidateNever]
    public virtual ICollection<StrawberryLike> StrawberryLike { get; set; } = new List<StrawberryLike>();

}
