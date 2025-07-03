using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Sach
{
    public int Id { get; set; }

    public string? MaSach { get; set; }

    public int IdTuaSach { get; set; }

    public int SoLuong { get; set; }

    public int SoLuongConLai { get; set; }

    public int DonGia { get; set; }

    public int NamXb { get; set; }

    public string NhaXb { get; set; } = null!;

    public int DaAn { get; set; }

    public virtual ICollection<CtPhieunhap> CtPhieunhap { get; set; } = new List<CtPhieunhap>();

    public virtual ICollection<Cuonsach> Cuonsach { get; set; } = new List<Cuonsach>();

    public virtual Tuasach IdTuaSachNavigation { get; set; } = null!;
}
