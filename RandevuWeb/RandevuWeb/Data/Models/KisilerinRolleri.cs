using System;
using System.Collections.Generic;

namespace RandevuWeb.Data.Models;

public partial class KisilerinRolleri
{
    public string UserId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public virtual Roller Role { get; set; } = null!;

    public virtual Kisiler User { get; set; } = null!;
}
