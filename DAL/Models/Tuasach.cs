using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Tuasach
{
    public int Id { get; set; }

    public string? MaTuaSach { get; set; }

    public string TenTuaSach { get; set; } = null!;

    public int IdTheLoai { get; set; }

    public int? DaAn { get; set; }

    public virtual Theloai IdTheLoaiNavigation { get; set; } = null!;

    public virtual ICollection<Sach> Sach { get; set; } = new List<Sach>();

    public virtual ICollection<Tacgia> IdTacGia { get; set; } = new List<Tacgia>();
}
