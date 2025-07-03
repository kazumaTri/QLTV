using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Chucnang
{
    public int Id { get; set; }

    public string? MaChucNang { get; set; }

    public string TenChucNang { get; set; } = null!;

    public string TenManHinh { get; set; } = null!;

    public virtual ICollection<Nhomnguoidung> IdNhomNguoiDung { get; set; } = new List<Nhomnguoidung>();
}
