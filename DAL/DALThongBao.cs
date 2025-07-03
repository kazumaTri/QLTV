// DAL/DALThongBao.cs
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics; // Thêm using cho Debug
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class DALThongBao : IDALThongBao // Đảm bảo class này implement IDALThongBao
    {
        private readonly QLTVContext _context;

        public DALThongBao(QLTVContext context)
        {
            _context = context;
        }

        // --- Phương thức hiện có ---
        public async Task<List<ThongBao>> GetAllActiveAsync()
        {
            DateTime now = DateTime.Now;
            try
            {
                var results = await _context.ThongBaos
                    .Where(tb => tb.TrangThai == "Active" &&
                                 tb.NgayBatDau <= now &&
                                 (tb.NgayKetThuc == null || tb.NgayKetThuc >= now))
                    .OrderByDescending(tb => tb.NgayTao) // Hiển thị thông báo mới nhất trước
                    .AsNoTracking() // Thêm AsNoTracking nếu chỉ đọc dữ liệu
                    .ToListAsync();
                // System.Diagnostics.Debug.WriteLine($"[DALThongBao] Found {results.Count} active notifications in DB."); // Log nếu cần
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DALThongBao.GetAllActiveAsync] Error: {ex.Message}");
                return new List<ThongBao>(); // Trả về danh sách rỗng khi lỗi
            }
        }

        // --- Triển khai các phương thức CRUD mới ---

        // Lấy tất cả thông báo (cho màn hình quản lý)
        public async Task<List<ThongBao>> GetAllAsync()
        {
            try
            {
                return await _context.ThongBaos
                                     .OrderByDescending(tb => tb.NgayTao) // Sắp xếp theo ngày tạo mới nhất
                                     .AsNoTracking() // Tối ưu nếu chỉ đọc
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DALThongBao.GetAllAsync] Error: {ex.Message}");
                return new List<ThongBao>();
            }
        }

        // Lấy thông báo theo ID
        public async Task<ThongBao?> GetByIdAsync(int id)
        {
            if (id <= 0) return null;
            try
            {
                // Dùng FindAsync vì nó tối ưu cho tìm kiếm theo khóa chính
                return await _context.ThongBaos.FindAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DALThongBao.GetByIdAsync] Error finding Id {id}: {ex.Message}");
                return null;
            }
        }

        // Thêm thông báo mới
        public async Task<bool> AddAsync(ThongBao thongBao)
        {
            if (thongBao == null) return false;

            // Có thể thêm các kiểm tra khác nếu cần ở DAL
            thongBao.NgayTao = DateTime.Now; // Đảm bảo Ngày tạo được gán

            try
            {
                _context.ThongBaos.Add(thongBao);
                int result = await _context.SaveChangesAsync();
                return result > 0; // Trả về true nếu có ít nhất 1 dòng bị ảnh hưởng (thêm thành công)
            }
            catch (DbUpdateException ex) // Bắt lỗi cụ thể của EF Core
            {
                Debug.WriteLine($"[DALThongBao.AddAsync] DbUpdateException Error: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DALThongBao.AddAsync] General Error: {ex.Message}");
                return false;
            }
        }

        // Cập nhật thông báo
        public async Task<bool> UpdateAsync(ThongBao thongBao)
        {
            if (thongBao == null || thongBao.Id <= 0) return false;

            try
            {
                // Kiểm tra xem entity có tồn tại không trước khi cập nhật
                var existingThongBao = await _context.ThongBaos.FindAsync(thongBao.Id);
                if (existingThongBao == null)
                {
                    Debug.WriteLine($"[DALThongBao.UpdateAsync] Error: ThongBao with Id {thongBao.Id} not found.");
                    return false; // Không tìm thấy để cập nhật
                }

                // Cách 2: Cập nhật từng thuộc tính (Kiểm soát tốt hơn những gì được cập nhật)
                existingThongBao.TieuDe = thongBao.TieuDe;
                existingThongBao.NoiDung = thongBao.NoiDung;
                existingThongBao.NgayBatDau = thongBao.NgayBatDau;
                existingThongBao.NgayKetThuc = thongBao.NgayKetThuc;
                existingThongBao.MucDo = thongBao.MucDo;
                existingThongBao.TrangThai = thongBao.TrangThai;
                // Không cập nhật NgayTao

                _context.Entry(existingThongBao).State = EntityState.Modified; // Đánh dấu là đã sửa đổi

                int result = await _context.SaveChangesAsync();
                return result > 0; // Trả về true nếu có ít nhất 1 dòng bị ảnh hưởng
            }
            catch (DbUpdateConcurrencyException ex) // Bắt lỗi xung đột khi cập nhật (nếu có ai đó đã sửa đổi hoặc xóa)
            {
                Debug.WriteLine($"[DALThongBao.UpdateAsync] DbUpdateConcurrencyException Error for Id {thongBao.Id}: {ex.Message}");
                // Xử lý lỗi xung đột ở đây nếu cần (ví dụ: load lại dữ liệu và thử lại)
                return false;
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine($"[DALThongBao.UpdateAsync] DbUpdateException Error for Id {thongBao.Id}: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DALThongBao.UpdateAsync] General Error for Id {thongBao.Id}: {ex.Message}");
                return false;
            }
        }

        // Xóa thông báo
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) return false;
            try
            {
                var thongBaoToDelete = await _context.ThongBaos.FindAsync(id);
                if (thongBaoToDelete != null)
                {
                    _context.ThongBaos.Remove(thongBaoToDelete);
                    int result = await _context.SaveChangesAsync();
                    return result > 0; // Trả về true nếu xóa thành công
                }
                else
                {
                    Debug.WriteLine($"[DALThongBao.DeleteAsync] Warning: ThongBao with Id {id} not found.");
                    return false; // Không tìm thấy để xóa
                }
            }
            catch (DbUpdateException ex) // Có thể xảy ra nếu có ràng buộc khóa ngoại
            {
                Debug.WriteLine($"[DALThongBao.DeleteAsync] DbUpdateException Error for Id {id}: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DALThongBao.DeleteAsync] General Error for Id {id}: {ex.Message}");
                return false;
            }
        }

        // *** START: THÊM CHO HOẠT ĐỘNG GẦN ĐÂY ***
        /// <summary>
        /// Lấy một số lượng giới hạn các thông báo gần đây nhất, sắp xếp theo ngày tạo giảm dần.
        /// </summary>
        /// <param name="count">Số lượng thông báo cần lấy.</param>
        /// <returns>Danh sách các thông báo gần đây.</returns>
        public async Task<List<ThongBao>> GetRecentAsync(int count)
        {
            if (count <= 0)
            {
                return new List<ThongBao>(); // Trả về danh sách rỗng nếu count không hợp lệ
            }

            try
            {
                // Lấy tất cả thông báo, sắp xếp theo NgayTao giảm dần, lấy số lượng 'count'
                return await _context.ThongBaos
                                     .OrderByDescending(tb => tb.NgayTao)
                                     .Take(count)
                                     .AsNoTracking() // Tối ưu vì chỉ đọc
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DALThongBao.GetRecentAsync] Error getting recent {count} items: {ex.Message}");
                return new List<ThongBao>(); // Trả về danh sách rỗng khi lỗi
            }
        }
        // *** END: THÊM CHO HOẠT ĐỘNG GẦN ĐÂY ***
    }
}