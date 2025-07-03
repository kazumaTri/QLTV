using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Phieunhapsach
{
    public int SoPhieuNhap { get; set; }

    public int TongTien { get; set; }

    public DateTime NgayNhap { get; set; }

    public virtual ICollection<CtPhieunhap> CtPhieunhap { get; set; } = new List<CtPhieunhap>();
}
