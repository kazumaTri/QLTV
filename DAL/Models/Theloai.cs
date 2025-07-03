using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Theloai
{
    public int Id { get; set; }

    public string? MaTheLoai { get; set; }

    public string TenTheLoai { get; set; } = null!;

    public virtual ICollection<CtBcluotmuontheotheloai> CtBcluotmuontheotheloai { get; set; } = new List<CtBcluotmuontheotheloai>();

    public virtual ICollection<Tuasach> Tuasach { get; set; } = new List<Tuasach>();
}
