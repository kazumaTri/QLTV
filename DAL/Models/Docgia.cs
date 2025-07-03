using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Docgia
{
    public int Id { get; set; }

    public string? MaDocGia { get; set; }

    public string TenDocGia { get; set; } = null!;

    public DateTime NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public DateTime NgayLapThe { get; set; }

    public DateTime NgayHetHan { get; set; }

    public int IdLoaiDocGia { get; set; }

    public int TongNoHienTai { get; set; }

    public int IdNguoiDung { get; set; }
    public bool DaAn { get; set; }
    public string? DienThoai { get; set; }

    public virtual Loaidocgia IdLoaiDocGiaNavigation { get; set; } = null!;

    public virtual Nguoidung IdNguoiDungNavigation { get; set; } = null!;

    public virtual ICollection<Phieumuontra> Phieumuontra { get; set; } = new List<Phieumuontra>();

    public virtual ICollection<Phieuthu> Phieuthu { get; set; } = new List<Phieuthu>();
}
