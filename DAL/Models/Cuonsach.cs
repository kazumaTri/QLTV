using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Cuonsach
{
    public int Id { get; set; }

    public string? MaCuonSach { get; set; }

    public int IdSach { get; set; }

    public int TinhTrang { get; set; }

    public int DaAn { get; set; }

    public virtual ICollection<Bcsachtratre> Bcsachtratre { get; set; } = new List<Bcsachtratre>();

    public virtual Sach IdSachNavigation { get; set; } = null!;

    public virtual ICollection<Phieumuontra> Phieumuontra { get; set; } = new List<Phieumuontra>();
}
