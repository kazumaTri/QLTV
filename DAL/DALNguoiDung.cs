// File: DAL/DALNguoiDung.cs
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics; // Thêm using này nếu chưa có
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    /// <summary>
    /// Data Access Layer cho Người dùng.
    /// Triển khai IDALNguoiDung.
    /// Đã cập nhật để hỗ trợ Soft Delete, Restore và các phương thức mới.
    /// </summary>
    public class DALNguoiDung : IDALNguoiDung
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DALNguoiDung(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        // --- Triển khai các phương thức từ IDALNguoiDung ---

        public async Task<List<Nguoidung>> GetAllAsync(bool includeDeleted = false) // <<< CẬP NHẬT CHỮ KÝ
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Nguoidung
                                        .Include(nd => nd.IdNhomNguoiDungNavigation)
                                        .AsQueryable();

                    // <<< THÊM LOGIC LỌC >>>
                    if (!includeDeleted)
                    {
                        query = query.Where(nd => nd.DaAn == false);
                    }
                    // <<< KẾT THÚC LOGIC LỌC >>>

                    return await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALNguoiDung.GetAllAsync (includeDeleted={includeDeleted}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<Nguoidung?> GetByIdAsync(int id, bool includeDeleted = false) // <<< CẬP NHẬT CHỮ KÝ
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Nguoidung
                                        .Include(nd => nd.IdNhomNguoiDungNavigation)
                                        .Where(nd => nd.Id == id)
                                        .AsQueryable();

                    // <<< THÊM LOGIC LỌC >>>
                    if (!includeDeleted)
                    {
                        query = query.Where(nd => nd.DaAn == false);
                    }
                    // <<< KẾT THÚC LOGIC LỌC >>>

                    return await query.FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALNguoiDung.GetByIdAsync (ID: {id}, includeDeleted={includeDeleted}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<Nguoidung?> GetByUsernameAsync(string username)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // *** DÒNG QUAN TRỌNG: Truy vấn DB, tìm user theo username, không bị xóa mềm (DaAn == false) ***
                    // *** và Include thông tin Nhóm người dùng (vai trò) ***
                    return await _context.Nguoidung
                                          .Include(nd => nd.IdNhomNguoiDungNavigation) // Include Nhomnguoidung
                                          .FirstOrDefaultAsync(u => u.TenDangNhap == username && u.DaAn == false); // 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error GetByUsernameAsync ({username}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<Nguoidung?> GetByUsernameIncludingDeletedAsync(string username)
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { return await _context.Nguoidung.Include(nd => nd.IdNhomNguoiDungNavigation).FirstOrDefaultAsync(u => u.TenDangNhap == username); } catch (Exception ex) { Debug.WriteLine($"Error GetByUsernameIncludingDeletedAsync ({username}): {ex.Message}"); throw; } }
        }

        public async Task<Nguoidung?> CheckLoginAsync(string username, string passwordHash)
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { return await _context.Nguoidung.Include(nd => nd.IdNhomNguoiDungNavigation).FirstOrDefaultAsync(u => u.TenDangNhap == username && u.DaAn == false); } catch (Exception ex) { Debug.WriteLine($"Error CheckLoginAsync ({username}): {ex.Message}"); throw; } }
            // Lưu ý: Password hash sẽ được kiểm tra ở tầng BUS
        }

        public async Task<Nguoidung?> AddAsync(Nguoidung nguoidung)
        {
            if (nguoidung == null) throw new ArgumentNullException(nameof(nguoidung)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); nguoidung.DaAn = false; if (nguoidung.IdNhomNguoiDungNavigation != null) nguoidung.IdNhomNguoiDungNavigation = null!; try { await _context.Nguoidung.AddAsync(nguoidung); int rows = await _context.SaveChangesAsync(); return (rows > 0) ? nguoidung : null; } catch (Exception ex) { Debug.WriteLine($"Error AddAsync NguoiDung: {ex.InnerException?.Message ?? ex.Message}"); throw; } }
        }

        public async Task<bool> UpdateAsync(Nguoidung nguoidung)
        {
            if (nguoidung == null || nguoidung.Id <= 0) throw new ArgumentException("Invalid Nguoidung for update."); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var existing = await _context.Nguoidung.FindAsync(nguoidung.Id); if (existing == null) return false; /* Cập nhật thuộc tính cần thiết */ existing.TenNguoiDung = nguoidung.TenNguoiDung; existing.NgaySinh = nguoidung.NgaySinh; existing.IdNhomNguoiDung = nguoidung.IdNhomNguoiDung; /* Mật khẩu thường cập nhật riêng */ if (!string.IsNullOrWhiteSpace(nguoidung.MatKhau) && existing.MatKhau != nguoidung.MatKhau) existing.MatKhau = nguoidung.MatKhau; return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error UpdateAsync NguoiDung ID {nguoidung.Id}: {ex.InnerException?.Message ?? ex.Message}"); throw; } }
        }

        public async Task<bool> UpdatePasswordAsync(int idNguoiDung, string hashedPassword) // <<< TRIỂN KHAI PHƯƠNG THỨC MỚI
        {
            if (idNguoiDung <= 0) throw new ArgumentException("ID người dùng không hợp lệ.", nameof(idNguoiDung));
            if (string.IsNullOrWhiteSpace(hashedPassword)) throw new ArgumentNullException(nameof(hashedPassword), "Mật khẩu đã hash không được rỗng.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var user = await _context.Nguoidung.FindAsync(idNguoiDung);
                    if (user == null || user.DaAn == true) // Không cập nhật mật khẩu cho user không tồn tại hoặc đã xóa
                    {
                        Debug.WriteLine($"User ID {idNguoiDung} not found or is deleted. Cannot update password.");
                        return false;
                    }
                    user.MatKhau = hashedPassword; // Cập nhật mật khẩu đã hash
                    return await _context.SaveChangesAsync() > 0;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating password for Nguoidung ID {idNguoiDung}: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID.", nameof(id)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var entity = await _context.Nguoidung.FindAsync(id); if (entity == null || entity.DaAn == true) return false; entity.DaAn = true; return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error SoftDeleteAsync NguoiDung ID {id}: {ex.Message}"); throw; } }
        }

        public async Task<bool> RestoreAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID.", nameof(id)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var entity = await _context.Nguoidung.FindAsync(id); if (entity == null || entity.DaAn == false) return false; entity.DaAn = false; return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error RestoreAsync NguoiDung ID {id}: {ex.Message}"); throw; } }
        }

        public async Task<bool> IsUsernameExistsAsync(string username) // <<< TRIỂN KHAI PHƯƠNG THỨC MỚI
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Bao gồm cả đã xóa để đảm bảo tên đăng nhập là duy nhất
                    return await _context.Nguoidung.AnyAsync(u => u.TenDangNhap.ToLower() == username.ToLower());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error checking username existence ({username}): {ex.Message}");
                    throw; // Ném lỗi để tầng trên xử lý
                }
            }
        }

    } // End class DALNguoiDung
} // End namespace DAL
