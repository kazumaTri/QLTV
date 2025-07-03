// File: DAL/DALTheLoai.cs
using DAL.Models;
using DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class DALTheLoai : IDALTheLoai
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DALTheLoai(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task<List<Theloai>> GetAllAsync()
        {
            Debug.WriteLine(">>> DAL: Entering GetAllAsync for TheLoai");
            using (var scope = _scopeFactory.CreateScope())
            {
                // Lấy DbContext và kiểm tra null
                QLTVContext? _context = null; // Khởi tạo là null
                try
                {
                    _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                    Debug.WriteLine(">>> DAL: DbContext obtained.");
                }
                catch (Exception exResolve)
                {
                    Debug.WriteLine($"*** CRITICAL ERROR resolving DbContext in DALTheLoai.GetAllAsync: {exResolve.ToString()}");
                    throw new InvalidOperationException("Không thể khởi tạo kết nối cơ sở dữ liệu.", exResolve);
                }

                // Kiểm tra context null (dù GetRequiredService thường ném lỗi nếu không resolve được)
                if (_context == null)
                {
                    Debug.WriteLine("*** CRITICAL ERROR in DALTheLoai.GetAllAsync: _context is NULL after GetRequiredService! ***");
                    throw new InvalidOperationException("Database context is not available.");
                }

                try
                {
                    // Kiểm tra DbSet Theloai có null không
                    if (_context.Theloai == null)
                    {
                        Debug.WriteLine("*** CRITICAL ERROR in DALTheLoai.GetAllAsync: _context.Theloai DbSet is NULL! Check QLTVContext model setup. ***");
                        throw new InvalidOperationException("TheLoai DbSet is not available in the context.");
                    }
                    Debug.WriteLine(">>> DAL: DbSet _context.Theloai is not null. Calling ToListAsync...");

                    // Thực hiện truy vấn
                    var result = await _context.Theloai.AsNoTracking().ToListAsync();
                    Debug.WriteLine($">>> DAL: ToListAsync finished. Returning {(result == null ? "NULL list" : $"{result.Count} items")}.");
                    return result;
                }
                catch (Exception ex)
                {
                    // Log lỗi chi tiết hơn
                    Debug.WriteLine($"*** ERROR in DALTheLoai.GetAllAsync during query execution: {ex.ToString()}");
                    throw; // Ném lại lỗi để tầng trên xử lý
                }
            }
        }

        // ... (Các phương thức khác giữ nguyên như trong file bạn đã cung cấp)
        public async Task<Theloai?> GetByIdAsync(int id) { if (id <= 0) return null; using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { return await _context.Theloai.FindAsync(id); } catch (Exception ex) { Debug.WriteLine($"Error in DALTheLoai.GetByIdAsync (ID: {id}): {ex.Message}"); throw; } } }
        public async Task<Theloai?> AddAsync(Theloai theLoai) { if (theLoai == null) throw new ArgumentNullException(nameof(theLoai)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { theLoai.Id = 0; theLoai.CtBcluotmuontheotheloai?.Clear(); theLoai.Tuasach?.Clear(); await _context.Theloai.AddAsync(theLoai); int affectedRows = await _context.SaveChangesAsync(); return affectedRows > 0 ? theLoai : null; } catch (DbUpdateException ex) { Debug.WriteLine($"DbUpdateException adding Theloai: {ex.InnerException?.Message ?? ex.Message}"); throw; } catch (Exception ex) { Debug.WriteLine($"Unexpected error adding Theloai: {ex.Message}"); throw; } } }
        public async Task<bool> UpdateAsync(Theloai theLoai) { if (theLoai == null) throw new ArgumentNullException(nameof(theLoai)); if (theLoai.Id <= 0) throw new ArgumentException("ID thể loại không hợp lệ để cập nhật.", nameof(theLoai.Id)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var existingTheLoai = await _context.Theloai.FindAsync(theLoai.Id); if (existingTheLoai == null) return false; existingTheLoai.TenTheLoai = theLoai.TenTheLoai; int affectedRows = await _context.SaveChangesAsync(); return affectedRows > 0; } catch (DbUpdateConcurrencyException ex) { Debug.WriteLine($"Concurrency error updating Theloai (ID: {theLoai.Id}): {ex.Message}"); throw; } catch (DbUpdateException ex) { Debug.WriteLine($"DbUpdateException updating Theloai (ID: {theLoai.Id}): {ex.InnerException?.Message ?? ex.Message}"); throw; } catch (Exception ex) { Debug.WriteLine($"Unexpected error updating Theloai (ID: {theLoai.Id}): {ex.Message}"); throw; } } }
        public async Task<bool> DeleteAsync(int id) { if (id <= 0) throw new ArgumentException("ID thể loại không hợp lệ để xóa.", nameof(id)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var theLoaiToRemove = await _context.Theloai.FindAsync(id); if (theLoaiToRemove == null) return false; _context.Theloai.Remove(theLoaiToRemove); int affectedRows = await _context.SaveChangesAsync(); return affectedRows > 0; } catch (DbUpdateException ex) { Debug.WriteLine($"DbUpdateException deleting Theloai (ID: {id}): {ex.InnerException?.Message ?? ex.Message}"); throw new InvalidOperationException($"Không thể xóa thể loại (ID: {id}) do có ràng buộc dữ liệu.", ex); } catch (Exception ex) { Debug.WriteLine($"Unexpected error deleting Theloai (ID: {id}): {ex.Message}"); throw; } } }
        public async Task<int> GetMaxMaTheLoaiNumberAsync() { using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var allMaTL = await _context.Theloai.Where(tl => tl.MaTheLoai != null && tl.MaTheLoai.StartsWith("TL")).Select(tl => tl.MaTheLoai).ToListAsync(); int maxNum = 0; foreach (var ma in allMaTL) { if (ma != null && ma.Length > 2 && int.TryParse(ma.AsSpan(2), out int num)) { if (num > maxNum) maxNum = num; } } return maxNum; } catch (Exception ex) { Debug.WriteLine($"Error in DALTheLoai.GetMaxMaTheLoaiNumberAsync: {ex.Message}"); throw; } } }
        public async Task<bool> MaTheLoaiExistsAsync(string maTheLoai) { if (string.IsNullOrWhiteSpace(maTheLoai)) return false; using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { string maToCheck = maTheLoai.Trim(); return await _context.Theloai.AnyAsync(t => t.MaTheLoai == maToCheck); } catch (Exception ex) { Debug.WriteLine($"Error in DALTheLoai.MaTheLoaiExistsAsync (Ma: {maTheLoai}): {ex.Message}"); throw; } } }
        public async Task<bool> IsTenTheLoaiExistsAsync(string tenTheLoai) { if (string.IsNullOrWhiteSpace(tenTheLoai)) return false; using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { string tenToCheck = tenTheLoai.Trim(); return await _context.Theloai.AnyAsync(t => t.TenTheLoai == tenToCheck); } catch (Exception ex) { Debug.WriteLine($"Error in DALTheLoai.IsTenTheLoaiExistsAsync (Ten: {tenTheLoai}): {ex.Message}"); throw; } } }
        public async Task<bool> IsTenTheLoaiExistsExcludingIdAsync(string tenTheLoai, int excludeId) { if (string.IsNullOrWhiteSpace(tenTheLoai)) return false; if (excludeId <= 0) return await IsTenTheLoaiExistsAsync(tenTheLoai); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { string tenToCheck = tenTheLoai.Trim(); return await _context.Theloai.AnyAsync(t => t.Id != excludeId && t.TenTheLoai == tenToCheck); } catch (Exception ex) { Debug.WriteLine($"Error in DALTheLoai.IsTenTheLoaiExistsExcludingIdAsync (Ten: {tenTheLoai}, ExcludeId: {excludeId}): {ex.Message}"); throw; } } }
        public async Task<bool> CanDeleteAsync(int id) { if (id <= 0) return false; using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { bool hasRelatedTuasach = await _context.Tuasach.AnyAsync(ts => ts.IdTheLoai == id); if (hasRelatedTuasach) return false; bool hasRelatedReports = await _context.CtBcluotmuontheotheloai.AnyAsync(ct => ct.IdTheLoai == id); if (hasRelatedReports) return false; return true; } catch (Exception ex) { Debug.WriteLine($"Error in DALTheLoai.CanDeleteAsync (ID: {id}): {ex.Message}"); throw; } } }

    }
}