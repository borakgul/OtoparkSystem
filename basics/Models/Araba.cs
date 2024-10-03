using System;
using System.Collections.Generic;

namespace basics.Models;

public partial class Araba
{
    public int Id { get; set; }

    public string? ArabaPlaka { get; set; }

    public string? Boyut { get; set; }

    public virtual ICollection<BölgeGiris> BölgeGirises { get; set; } = new List<BölgeGiris>();
}
