using System;
using System.Collections.Generic;

namespace basics.Models;

public partial class BölgeGiris
{
    public int Id { get; set; }

    public int? ArabaId { get; set; }

    public int? BölgeId { get; set; }

    public DateTime? GirisZamani { get; set; }

    public DateTime? CikisZamani { get; set; }

    public virtual Araba? Araba { get; set; }

    public virtual Bölge? Bölge { get; set; }
}
