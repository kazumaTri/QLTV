using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Loaidocgia
{
    public int Id { get; set; }

    public string? MaLoaiDocGia { get; set; }

    public string TenLoaiDocGia { get; set; } = null!;

    public virtual ICollection<Docgia> Docgia { get; set; } = new List<Docgia>();
}
