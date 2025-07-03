using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Phieuthu
{
    public int SoPhieuThu { get; set; }

    public int IdDocGia { get; set; }

    public int SoTienThu { get; set; }

    public DateTime NgayLap { get; set; }

    public virtual Docgia IdDocGiaNavigation { get; set; } = null!;
}
