using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrawberryHub.Models;

public partial class StrawberryArticle
{
    public int ArticleId { get; set; }

    public int? GoalTypeId { get; set; }

    public string ArticleContent { get; set; } = null!;

    public DateTime PublishedDate { get; set; }

    [Required(ErrorMessage = "Please select a photo.")]
    [NotMapped]
    public IFormFile Photo { get; set; } = null!;

    public string Picture { get; set; } = null!;

    [ValidateNever]
    public virtual StrawberryGoalType GoalType { get; set; } = null!;

}
