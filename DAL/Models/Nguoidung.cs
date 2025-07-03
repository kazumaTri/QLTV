using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Nguoidung
{
    public int Id { get; set; }

    public string? MaNguoiDung { get; set; }

    public string TenNguoiDung { get; set; } = null!;

    public DateTime? NgaySinh { get; set; }

    public string? ChucVu { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int IdNhomNguoiDung { get; set; }

    public virtual Docgia? Docgia { get; set; }
    public bool DaAn { get; set; }

    public virtual Nhomnguoidung IdNhomNguoiDungNavigation { get; set; } = null!;
}
