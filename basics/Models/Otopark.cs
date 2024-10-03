using System;
using System.Collections.Generic;

namespace basics.Models;

public partial class Otopark
{
    public int Id { get; set; }

    public int? KücükArabaPara { get; set; }

    public int? OrtaArabaPara { get; set; }

    public int? BüyükArabaPara { get; set; }

    public virtual ICollection<Bölge> Bölges { get; set; } = new List<Bölge>();
}
