using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
// using DAL.Models; // Dư thừa nếu namespace chính là DAL.Models

namespace DAL.Models // Hoặc namespace DAL.Models; (file-scoped)
{
    public partial class QLTVContext : DbContext
    {
        // Constructor mặc định (cho design-time tools của EF)
        public QLTVContext()
        {
        }

        // Constructor nhận options - Được sử dụng bởi DI Container khi cấu hình DbContext
        public QLTVContext(DbContextOptions<QLTVContext> options)
            : base(options)
        {
        }

        // Định nghĩa các DbSet (đã lấy từ code bạn cung cấp)
        public virtual DbSet<Bcluotmuontheotheloai> Bcluotmuontheotheloai { get; set; }
        public virtual DbSet<Bcsachtratre> Bcsachtratre { get; set; }
        public virtual DbSet<Chucnang> Chucnang { get; set; }
        public virtual DbSet<CtBcluotmuontheotheloai> CtBcluotmuontheotheloai { get; set; }
        public virtual DbSet<CtPhieunhap> CtPhieunhap { get; set; }
        public virtual DbSet<Cuonsach> Cuonsach { get; set; }
        public virtual DbSet<Docgia> Docgia { get; set; }
        public virtual DbSet<Loaidocgia> Loaidocgia { get; set; }
        public virtual DbSet<Nguoidung> Nguoidung { get; set; }
        public virtual DbSet<Nhomnguoidung> Nhomnguoidung { get; set; }
        public virtual DbSet<Phieumuontra> Phieumuontra { get; set; }
        public virtual DbSet<Phieunhapsach> Phieunhapsach { get; set; }
        public virtual DbSet<Phieuthu> Phieuthu { get; set; }
        public virtual DbSet<Sach> Sach { get; set; }
        public virtual DbSet<Tacgia> Tacgia { get; set; }
        public virtual DbSet<Thamso> Thamso { get; set; }
        public virtual DbSet<Theloai> Theloai { get; set; }
        public virtual DbSet<Tuasach> Tuasach { get; set; }
        public virtual DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public virtual DbSet<ThongBao> ThongBaos { get; set; }

        // *** ĐÃ XÓA HOẶC COMMENT PHƯƠNG THỨC OnConfiguring CÓ CHỨA CHUỖI KẾT NỐI CỨNG ***
        // Việc cấu hình Connection String sẽ được thực hiện ở Program.cs thông qua DI.
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
             #warning To protect potentially sensitive information in your connection string...
             // Bỏ comment hoặc xóa hoàn toàn phần này khi dùng DI để cấu hình Connection String
            // => optionsBuilder.UseSqlServer("Server=DESKTOP-R5VEEIQ;Database=QLTV;Integrated Security=True;Encrypt=False;");
        }
        */


        // Cấu hình Model (đã lấy từ code bạn cung cấp)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bcluotmuontheotheloai>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__BCLUOTMU__3213E83FE24D00BC");
                entity.ToTable("BCLUOTMUONTHEOTHELOAI");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MaBaoCao)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](10),('BCLM'+right('0'+CONVERT([char](2),[THANG]),(2)))+CONVERT([char](4),[NAM])))", true)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Bcsachtratre>(entity =>
            {
                entity.HasKey(e => new { e.Ngay, e.IdCuonSach }).HasName("PK__BCSACHTR__8F93DB29A175628C");
                entity.ToTable("BCSACHTRATRE");
                entity.Property(e => e.Ngay).HasColumnType("datetime");
                entity.Property(e => e.IdCuonSach).HasColumnName("idCuonSach");
                entity.Property(e => e.NgayMuon).HasColumnType("datetime");

                entity.HasOne(d => d.IdCuonSachNavigation).WithMany(p => p.Bcsachtratre)
                    .HasForeignKey(d => d.IdCuonSach)
                    .HasConstraintName("FK__BCSACHTRA__idCuo__76969D2E");
            });

            modelBuilder.Entity<Chucnang>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CHUCNANG__3213E83F74C8BD9B");
                entity.ToTable("CHUCNANG");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MaChucNang)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](5),'CN'+right('000'+CONVERT([varchar](3),[id]),(3))))", true)
                    .IsFixedLength();
            });

            modelBuilder.Entity<CtBcluotmuontheotheloai>(entity =>
            {
                entity.HasKey(e => new { e.IdBaoCao, e.IdTheLoai }).HasName("PK__CT_BCLUO__816BEA6EF937F0E9");
                entity.ToTable("CT_BCLUOTMUONTHEOTHELOAI");
                entity.Property(e => e.IdBaoCao).HasColumnName("idBaoCao");
                entity.Property(e => e.IdTheLoai).HasColumnName("idTheLoai");
                entity.Property(e => e.TiLe)
                    .HasDefaultValue(0m)
                    .HasColumnType("numeric(4, 2)");

                entity.HasOne(d => d.IdBaoCaoNavigation).WithMany(p => p.CtBcluotmuontheotheloai)
                    .HasForeignKey(d => d.IdBaoCao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CT_BCLUOT__idBao__70DDC3D8");

                entity.HasOne(d => d.IdTheLoaiNavigation).WithMany(p => p.CtBcluotmuontheotheloai)
                    .HasForeignKey(d => d.IdTheLoai)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CT_BCLUOT__idThe__71D1E811");
            });

            modelBuilder.Entity<CtPhieunhap>(entity =>
            {
                entity.HasKey(e => new { e.SoPhieuNhap, e.IdSach }).HasName("PK__CT_PHIEU__56D67F6A105DEA68");
                entity.ToTable("CT_PHIEUNHAP");
                entity.Property(e => e.IdSach).HasColumnName("idSach");

                entity.HasOne(d => d.IdSachNavigation).WithMany(p => p.CtPhieunhap)
                    .HasForeignKey(d => d.IdSach)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CT_PHIEUN__idSac__5DCAEF64");

                entity.HasOne(d => d.SoPhieuNhapNavigation).WithMany(p => p.CtPhieunhap)
                    .HasForeignKey(d => d.SoPhieuNhap)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CT_PHIEUN__SoPhi__5CD6CB2B");
            });

            modelBuilder.Entity<Cuonsach>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CUONSACH__3213E83F6D89F2A8");
                entity.ToTable("CUONSACH");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdSach).HasColumnName("idSach");
                entity.Property(e => e.MaCuonSach)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'CS'+right('0000'+CONVERT([varchar](10),[id]),(4))))", true)
                    .IsFixedLength();
                entity.Property(e => e.TinhTrang).HasDefaultValue(1);

                entity.HasOne(d => d.IdSachNavigation).WithMany(p => p.Cuonsach)
                    .HasForeignKey(d => d.IdSach)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CUONSACH__idSach__60A75C0F");
            });

            modelBuilder.Entity<Docgia>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__DOCGIA__3214EC2746716D3A");
                entity.ToTable("DOCGIA");
                entity.HasIndex(e => e.IdNguoiDung, "UQ__DOCGIA__BE010FC8379F3B32").IsUnique();
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Email).IsUnicode(false);
                entity.Property(e => e.IdLoaiDocGia).HasColumnName("idLoaiDocGia");
                entity.Property(e => e.IdNguoiDung).HasColumnName("idNguoiDung");
                entity.Property(e => e.MaDocGia)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'DG'+right('0000'+CONVERT([varchar](10),[ID]),(4))))", true)
                    .IsFixedLength();
                entity.Property(e => e.NgayHetHan).HasColumnType("datetime");
                entity.Property(e => e.NgayLapThe).HasColumnType("datetime");
                entity.Property(e => e.NgaySinh).HasColumnType("datetime");

                entity.HasOne(d => d.IdLoaiDocGiaNavigation).WithMany(p => p.Docgia)
                    .HasForeignKey(d => d.IdLoaiDocGia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DOCGIA__idLoaiDo__5165187F");

                entity.HasOne(d => d.IdNguoiDungNavigation).WithOne(p => p.Docgia)
                    .HasForeignKey<Docgia>(d => d.IdNguoiDung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DOCGIA__idNguoiD__534D60F1");
            });

            modelBuilder.Entity<Loaidocgia>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__LOAIDOCG__3213E83F12628C52");
                entity.ToTable("LOAIDOCGIA");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MaLoaiDocGia)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'LDG'+right('000'+CONVERT([varchar](10),[ID]),(3))))", true)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Nguoidung>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__NGUOIDUN__3213E83F18F0E3C1");
                entity.ToTable("NGUOIDUNG");
                entity.HasIndex(e => e.TenDangNhap, "UQ__NGUOIDUN__55F68FC041F20D99").IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdNhomNguoiDung).HasColumnName("idNhomNguoiDung"); // << Cột này có thể NULL trong DB -> thuộc tính int?
                entity.Property(e => e.MaNguoiDung)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'ND'+right('000000'+CONVERT([varchar](4),[id]),(4))))", true)
                    .IsFixedLength();
                entity.Property(e => e.MatKhau).IsUnicode(false);
                entity.Property(e => e.NgaySinh).HasColumnType("datetime");
                entity.Property(e => e.TenDangNhap)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                // Cấu hình khóa ngoại, nếu IdNhomNguoiDung cho phép NULL, EF sẽ tự tạo FK nullable
                entity.HasOne(d => d.IdNhomNguoiDungNavigation).WithMany(p => p.Nguoidung)
                    .HasForeignKey(d => d.IdNhomNguoiDung) // <<< Thuộc tính IdNhomNguoiDung trong Entity là int?
                    .HasConstraintName("FK__NGUOIDUNG__idNho__403A8C7D");
            });

            modelBuilder.Entity<Nhomnguoidung>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__NHOMNGUO__3213E83FC12C1FAC");
                entity.ToTable("NHOMNGUOIDUNG");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MaNhomNguoiDung)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'NND'+right('000'+CONVERT([varchar](5),[id]),(3))))", true)
                    .IsFixedLength();

                entity.HasMany(d => d.IdChucNang).WithMany(p => p.IdNhomNguoiDung)
                    .UsingEntity<Dictionary<string, object>>(
                        "Phanquyen",
                        r => r.HasOne<Chucnang>().WithMany()
                            .HasForeignKey("IdChucNang")
                            .HasConstraintName("FK__PHANQUYEN__idChu__3C69FB99"),
                        l => l.HasOne<Nhomnguoidung>().WithMany()
                            .HasForeignKey("IdNhomNguoiDung")
                            .HasConstraintName("FK__PHANQUYEN__idNho__3B75D760"),
                        j =>
                        {
                            j.HasKey("IdNhomNguoiDung", "IdChucNang").HasName("PK__PHANQUYE__421733D85D0AA18B");
                            j.ToTable("PHANQUYEN");
                            j.IndexerProperty<int>("IdNhomNguoiDung").HasColumnName("idNhomNguoiDung");
                            j.IndexerProperty<int>("IdChucNang").HasColumnName("idChucNang");
                        });
            });

            modelBuilder.Entity<Phieumuontra>(entity =>
            {
                entity.HasKey(e => e.SoPhieuMuonTra).HasName("PK__PHIEUMUO__EF67FB6FFDC215AB");
                entity.ToTable("PHIEUMUONTRA");
                entity.Property(e => e.HanTra).HasColumnType("datetime");
                entity.Property(e => e.IdCuonSach).HasColumnName("idCuonSach");
                entity.Property(e => e.IdDocGia).HasColumnName("idDocGia");
                entity.Property(e => e.NgayMuon).HasColumnType("datetime");
                entity.Property(e => e.NgayTra).HasColumnType("datetime");
                entity.Property(e => e.SoTienPhat).HasDefaultValue(0);

                entity.HasOne(d => d.IdCuonSachNavigation).WithMany(p => p.Phieumuontra)
                    .HasForeignKey(d => d.IdCuonSach)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PHIEUMUON__idCuo__66603565");

                entity.HasOne(d => d.IdDocGiaNavigation).WithMany(p => p.Phieumuontra)
                    .HasForeignKey(d => d.IdDocGia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PHIEUMUON__idDoc__656C112C");
            });

            modelBuilder.Entity<Phieunhapsach>(entity =>
            {
                entity.HasKey(e => e.SoPhieuNhap).HasName("PK__PHIEUNHA__4A9C88C19B86535C");
                entity.ToTable("PHIEUNHAPSACH");
                entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            });

            modelBuilder.Entity<Phieuthu>(entity =>
            {
                entity.HasKey(e => e.SoPhieuThu).HasName("PK__PHIEUTHU__6D06AEAB0DEA2A8C");
                entity.ToTable("PHIEUTHU");
                entity.Property(e => e.IdDocGia).HasColumnName("idDocGia");
                entity.Property(e => e.NgayLap).HasColumnType("datetime");

                entity.HasOne(d => d.IdDocGiaNavigation).WithMany(p => p.Phieuthu)
                    .HasForeignKey(d => d.IdDocGia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PHIEUTHU__idDocG__6A30C649");
            });

            modelBuilder.Entity<Sach>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__SACH__3213E83FE96316AC");
                entity.ToTable("SACH");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdTuaSach).HasColumnName("idTuaSach");
                entity.Property(e => e.MaSach)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'S'+right('00000'+CONVERT([varchar](10),[id]),(5))))", true)
                    .IsFixedLength();
                entity.Property(e => e.NamXb).HasColumnName("NamXB");
                entity.Property(e => e.NhaXb).HasColumnName("NhaXB");

                entity.HasOne(d => d.IdTuaSachNavigation).WithMany(p => p.Sach)
                    .HasForeignKey(d => d.IdTuaSach)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SACH__idTuaSach__5629CD9C");
            });

            modelBuilder.Entity<Tacgia>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TACGIA__3213E83F564979BD");
                entity.ToTable("TACGIA");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Matacgia)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'TG'+right('0000'+CONVERT([varchar](10),[ID]),(4))))", true)
                    .IsFixedLength()
                    .HasColumnName("MATACGIA");

                entity.HasMany(d => d.IdTuaSach).WithMany(p => p.IdTacGia)
                    .UsingEntity<Dictionary<string, object>>(
                        "CtTacgia",
                        r => r.HasOne<Tuasach>().WithMany()
                            .HasForeignKey("IdTuaSach")
                            .HasConstraintName("FK__CT_TACGIA__idTua__4BAC3F29"),
                        l => l.HasOne<Tacgia>().WithMany()
                            .HasForeignKey("IdTacGia")
                            .HasConstraintName("FK__CT_TACGIA__idTac__4AB81AF0"),
                        j =>
                        {
                            j.HasKey("IdTacGia", "IdTuaSach").HasName("PK__CT_TACGI__0CEEA1FCD0051CCE");
                            j.ToTable("CT_TACGIA");
                            j.IndexerProperty<int>("IdTacGia").HasColumnName("idTacGia");
                            j.IndexerProperty<int>("IdTuaSach").HasColumnName("idTuaSach");
                        });
            });

            modelBuilder.Entity<Thamso>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__THAMSO__3213E83FC911CB09");
                entity.ToTable("THAMSO");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AdQdkttienThu).HasColumnName("AD_QDKTTienThu");
            });

            modelBuilder.Entity<Theloai>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__THELOAI__3213E83F8E3B1459");
                entity.ToTable("THELOAI");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MaTheLoai)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'TL'+right('0000'+CONVERT([varchar](4),[id]),(4))))", true)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Tuasach>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TUASACH__3213E83FD0D840EA");
                entity.ToTable("TUASACH");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DaAn).HasDefaultValue(0);
                entity.Property(e => e.IdTheLoai).HasColumnName("idTheLoai");
                entity.Property(e => e.MaTuaSach)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([char](6),'TS'+right('0000'+CONVERT([varchar](10),[ID]),(4))))", true)
                    .IsFixedLength();

                entity.HasOne(d => d.IdTheLoaiNavigation).WithMany(p => p.Tuasach)
                    .HasForeignKey(d => d.IdTheLoai)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TUASACH__idTheLo__44FF419A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}