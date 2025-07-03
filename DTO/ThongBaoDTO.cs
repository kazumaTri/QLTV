// DTO/ThongBaoDTO.cs
namespace DTO
{
    public class ThongBaoDTO
    {
        public int Id { get; set; }
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MucDo { get; set; } = null!;
        public string TrangThai { get; set; } = null!;
        public DateTime NgayTao { get; set; }
    }
}