// File: DAL/DALDocGia.cs
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public class DALDocGia : IDALDocGia
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DALDocGia(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        // --- Read Operations ---

        public async Task<List<Docgia>> GetAllAsync(bool includeDeleted = false, int? idLoaiDocGiaFilter = null) // <<< CẬP NHẬT CHỮ KÝ
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Docgia
                        .Include(dg => dg.IdLoaiDocGiaNavigation)
                        .Include(dg => dg.IdNguoiDungNavigation)
                        .AsQueryable();

                    if (!includeDeleted)
                    {
                        query = query.Where(dg => dg.DaAn == false);
                    }

                    // <<< THÊM LOGIC LỌC >>>
                    if (idLoaiDocGiaFilter.HasValue && idLoaiDocGiaFilter.Value > 0)
                    {
                        query = query.Where(dg => dg.IdLoaiDocGia == idLoaiDocGiaFilter.Value);
                    }
                    // <<< KẾT THÚC LOGIC LỌC >>>

                    return await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALDocGia.GetAllAsync (filter={idLoaiDocGiaFilter}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<Docgia?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var query = _context.Docgia.Include(dg => dg.IdLoaiDocGiaNavigation).Include(dg => dg.IdNguoiDungNavigation).Where(dg => dg.Id == id).AsQueryable(); if (!includeDeleted) { query = query.Where(dg => dg.DaAn == false); } return await query.FirstOrDefaultAsync(); } catch (Exception ex) { Debug.WriteLine($"Error GetByIdAsync ID {id}: {ex.Message}"); throw; } }
        }

        public async Task<Docgia?> GetByIdNguoiDungAsync(int idNguoiDung, bool includeDeleted = false)
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var query = _context.Docgia.Include(dg => dg.IdLoaiDocGiaNavigation).Include(dg => dg.IdNguoiDungNavigation).Where(dg => dg.IdNguoiDung == idNguoiDung).AsQueryable(); if (!includeDeleted) { query = query.Where(dg => dg.DaAn == false); } return await query.FirstOrDefaultAsync(); } catch (Exception ex) { Debug.WriteLine($"Error GetByIdNguoiDungAsync ID {idNguoiDung}: {ex.Message}"); throw; } }
        }

        public async Task<int> GetActiveReaderCountAsync()
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { return await _context.Docgia.Where(dg => dg.DaAn == false && dg.NgayHetHan >= DateTime.Today).CountAsync(); } catch (Exception ex) { Debug.WriteLine($"Error GetActiveReaderCountAsync: {ex.Message}"); throw; } }
        }

        public async Task<List<Docgia>> SearchAsync(string keyword, bool includeDeleted = false, int? idLoaiDocGiaFilter = null) // <<< CẬP NHẬT CHỮ KÝ
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var lowerKeyword = keyword.ToLower().Trim();
                    var query = _context.Docgia
                        .Include(dg => dg.IdLoaiDocGiaNavigation)
                        .Include(dg => dg.IdNguoiDungNavigation)
                        .Where(dg =>
                            (dg.MaDocGia != null && dg.MaDocGia.ToLower().Contains(lowerKeyword)) ||
                            (dg.TenDocGia != null && dg.TenDocGia.ToLower().Contains(lowerKeyword)) ||
                            (dg.Email != null && dg.Email.ToLower().Contains(lowerKeyword)) ||
                            (dg.DienThoai != null && dg.DienThoai.Contains(keyword))
                        )
                        .AsQueryable();

                    if (!includeDeleted)
                    {
                        query = query.Where(dg => dg.DaAn == false);
                    }

                    // <<< THÊM LOGIC LỌC >>>
                    if (idLoaiDocGiaFilter.HasValue && idLoaiDocGiaFilter.Value > 0)
                    {
                        query = query.Where(dg => dg.IdLoaiDocGia == idLoaiDocGiaFilter.Value);
                    }
                    // <<< KẾT THÚC LOGIC LỌC >>>

                    return await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALDocGia.SearchAsync (Keyword: {keyword}, filter={idLoaiDocGiaFilter}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<bool> HasActiveLoansAsync(int docGiaId)
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { return await _context.Phieumuontra.AnyAsync(p => p.IdDocGia == docGiaId && p.NgayTra == null); } catch (Exception ex) { Debug.WriteLine($"Error HasActiveLoansAsync ID {docGiaId}: {ex.Message}"); throw; } }
        }

        // --- CUD Operations ---
        public async Task<Docgia?> AddAsync(Docgia docGia)
        {
            if (docGia == null) throw new ArgumentNullException(nameof(docGia)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); if (docGia.IdLoaiDocGiaNavigation != null) docGia.IdLoaiDocGiaNavigation = null!; if (docGia.IdNguoiDungNavigation != null) docGia.IdNguoiDungNavigation = null!; docGia.DaAn = false; try { await _context.Docgia.AddAsync(docGia); int rows = await _context.SaveChangesAsync(); return (rows > 0) ? docGia : null; } catch (Exception ex) { Debug.WriteLine($"Error AddAsync: {ex.InnerException?.Message ?? ex.Message}"); throw; } }
        }

        public async Task<bool> UpdateAsync(Docgia docGia)
        {
            if (docGia == null || docGia.Id <= 0) throw new ArgumentException("Invalid Docgia for update."); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var existing = await _context.Docgia.FindAsync(docGia.Id); if (existing == null) return false; _context.Entry(existing).CurrentValues.SetValues(docGia); return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error UpdateAsync ID {docGia.Id}: {ex.InnerException?.Message ?? ex.Message}"); throw; } }
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID.", nameof(id)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var entity = await _context.Docgia.FindAsync(id); if (entity == null || entity.DaAn == true) return false; entity.DaAn = true; return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error SoftDeleteAsync ID {id}: {ex.Message}"); throw; } }
        }

        public async Task<bool> RestoreAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID.", nameof(id)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var entity = await _context.Docgia.FindAsync(id); if (entity == null || entity.DaAn == false) return false; entity.DaAn = false; return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error RestoreAsync ID {id}: {ex.Message}"); throw; } }
        }

        // --- Các phương thức bổ sung ---
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { return await _context.Nguoidung.AnyAsync(u => u.TenDangNhap.ToLower() == username.ToLower()); } catch (Exception ex) { Debug.WriteLine($"Error IsUsernameExistsAsync ({username}): {ex.Message}"); throw; } }
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeDocGiaId = null)
        {
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var query = _context.Docgia.Where(d => d.Email != null && d.Email.ToLower() == email.ToLower()); if (excludeDocGiaId.HasValue) { query = query.Where(d => d.Id != excludeDocGiaId.Value); } return await query.AnyAsync(); } catch (Exception ex) { Debug.WriteLine($"Error IsEmailExistsAsync ({email}, exclude ID: {excludeDocGiaId}): {ex.Message}"); throw; } }
        }

        public async Task<bool> UpdateTongNoAsync(int idDocGia, int newTongNo)
        {
            if (idDocGia <= 0) throw new ArgumentException("ID độc giả không hợp lệ.", nameof(idDocGia));
            if (newTongNo < 0) newTongNo = 0;
            using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var docGia = await _context.Docgia.FindAsync(idDocGia); if (docGia == null) return false; docGia.TongNoHienTai = newTongNo; return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error UpdateTongNoAsync ID {idDocGia}: {ex.Message}"); throw; } }
        }

        public async Task<bool> UpdateNgayHetHanAsync(int idDocGia, DateTime ngayHetHanMoi)
        {
            if (idDocGia <= 0) throw new ArgumentException("ID độc giả không hợp lệ.", nameof(idDocGia)); using (var scope = _scopeFactory.CreateScope()) { var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); try { var docGia = await _context.Docgia.FindAsync(idDocGia); if (docGia == null) return false; docGia.NgayHetHan = ngayHetHanMoi; return await _context.SaveChangesAsync() > 0; } catch (Exception ex) { Debug.WriteLine($"Error UpdateNgayHetHanAsync ID {idDocGia}: {ex.Message}"); throw; } }
        }

    } // End class DALDocGia
} // End namespace DAL