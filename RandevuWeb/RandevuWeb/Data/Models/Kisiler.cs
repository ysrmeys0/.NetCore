using System;
using System.Collections.Generic;

namespace RandevuWeb.Data.Models;

public partial class Kisiler
{
    public string? Id { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int? Gender { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Randevular> RandevularDoctors { get; set; } = new List<Randevular>();

    public virtual ICollection<Randevular> RandevularPatients { get; set; } = new List<Randevular>();
}
