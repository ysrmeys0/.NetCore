using System;
using System.Collections.Generic;

namespace RandevuWeb.Data.Models;

public partial class RandevuTipleri
{
    public byte Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string TypeName { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Randevular> Randevulars { get; set; } = new List<Randevular>();
}
