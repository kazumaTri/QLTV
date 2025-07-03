// QuanLyThuVien/DAL/Models/NhaXuatBan.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("NhaXuatBan")]
    public partial class NhaXuatBan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Hoặc tuỳ theo cách bạn định nghĩa ID
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNXB { get; set; } = null!;

        [StringLength(200)]
        public string? DiaChi { get; set; }

        [StringLength(20)]
        public string? DienThoai { get; set; }

        [StringLength(100)] // Ví dụ giới hạn 100 ký tự
        public string? Email { get; set; }

        // Navigation property (nếu NXB có liên kết với Sách)
        // public virtual ICollection<Sach> Sachs { get; set; } = new List<Sach>();
    }
}