using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Nhomnguoidung
{
    public int Id { get; set; }

    public string? MaNhomNguoiDung { get; set; }

    public string TenNhomNguoiDung { get; set; } = null!;

    public virtual ICollection<Nguoidung> Nguoidung { get; set; } = new List<Nguoidung>();

    public virtual ICollection<Chucnang> IdChucNang { get; set; } = new List<Chucnang>();
}
