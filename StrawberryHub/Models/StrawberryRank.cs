using System;
using System.Collections.Generic;

namespace StrawberryHub.Models;

public partial class StrawberryRank
{
    public int RankId { get; set; }

    public string RankName { get; set; } = null!;

    public int MinPoints { get; set; }

    public int MaxPoints { get; set; }


    [ValidateNever]
    public virtual ICollection<StrawberryUser> StrawberryUsers { get; set; } = new List<StrawberryUser>();
}
