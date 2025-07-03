using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Thamso
{
    public int Id { get; set; }

    public int TuoiToiThieu { get; set; }

    public int TuoiToiDa { get; set; }

    public int ThoiHanThe { get; set; }

    public int KhoangCachXuatBan { get; set; }

    public int SoSachMuonToiDa { get; set; }

    public int SoNgayMuonToiDa { get; set; }

    public int DonGiaPhat { get; set; }

    public int AdQdkttienThu { get; set; }
}
