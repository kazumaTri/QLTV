// DAL/Models/ThongBao.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("ThongBao")] // Đảm bảo tên bảng khớp
    public partial class ThongBao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; } = null!;

        [Required]
        public string NoiDung { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime NgayBatDau { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayKetThuc { get; set; } // Nullable

        [Required]
        [StringLength(50)]
        public string MucDo { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }
    }
}