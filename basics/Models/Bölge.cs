using System;
using System.Collections.Generic;

namespace basics.Models;

public partial class Bölge
{
    public int Id { get; set; }

    public int? KücükBos { get; set; }

    public int? OrtaBos { get; set; }

    public int? BüyükBos { get; set; }

    public int? OtoparkId { get; set; }

    public virtual ICollection<BölgeGiris> BölgeGirises { get; set; } = new List<BölgeGiris>();

    public virtual Otopark? Otopark { get; set; }
}
