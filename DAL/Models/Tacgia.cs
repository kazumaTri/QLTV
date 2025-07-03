using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Tacgia
{
    public int Id { get; set; }

    public string? Matacgia { get; set; }

    public string TenTacGia { get; set; } = null!;
    public bool DaAn { get; set; }
    public virtual ICollection<Tuasach> IdTuaSach { get; set; } = new List<Tuasach>();
}
