// File: DAL/DALPhieuThu.cs
using DAL.Models;        // Namespace chứa Entity Phieuthu và QLTVContext
using Microsoft.EntityFrameworkCore; // Namespace cho Entity Framework Core (DbSet, AddAsync, SaveChangesAsync, FindAsync, ToListAsync, Where)
using System;
using System.Collections.Generic;
using System.Diagnostics;   // Namespace cho Debug.WriteLine
using System.Linq;          // Namespace cho LINQ (ví dụ: Where)
using System.Threading.Tasks; // Namespace cho lập trình bất đồng bộ (Task, async, await)

namespace DAL
{
    /// <summary>
    /// Lớp triển khai truy cập dữ liệu cho Phiếu Thu Tiền Phạt (Phieuthu).
    /// Thực thi interface IDALPhieuThu.
    /// </summary>
    public class DALPhieuThu : IDALPhieuThu // <<< Sửa thành public và triển khai IDALPhieuThu
    {
        private readonly QLTVContext _context; // <<< Thêm DbContext

        /// <summary>
        /// Hàm khởi tạo, nhận QLTVContext thông qua Dependency Injection.
        /// </summary>
        /// <param name="context">QLTVContext được inject.</param>
        public DALPhieuThu(QLTVContext context) // <<< Constructor nhận DbContext
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Thêm mới một phiếu thu vào cơ sở dữ liệu một cách bất đồng bộ.
        /// </summary>
        /// <param name="phieuThu">Entity Phieuthu cần thêm.</param>
        /// <returns>Entity Phieuthu đã được thêm (với ID được cập nhật) nếu thành công, null nếu thất bại.</returns>
        public async Task<Phieuthu?> AddAsync(Phieuthu phieuThu)
        {
            if (phieuThu == null)
            {
                Debug.WriteLine("DALPhieuThu - AddAsync: Input Phieuthu is null.");
                return null;
            }

            try
            {
                _context.Phieuthu.Add(phieuThu); // <<< Sử dụng DbSet Phieuthus
                await _context.SaveChangesAsync();
                Debug.WriteLine($"DALPhieuThu - AddAsync: Added Phieuthu with SoPhieuThu {phieuThu.SoPhieuThu}"); // <<< CHANGE Id to SoPhieuThu
                return phieuThu;
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine($"DALPhieuThu - AddAsync: DbUpdateException occurred: {ex.Message}");
                // Ghi log chi tiết hơn nếu cần
                // Log.Error(ex, "Error adding Phieuthu");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DALPhieuThu - AddAsync: General exception occurred: {ex.Message}");
                // Ghi log chi tiết hơn nếu cần
                // Log.Error(ex, "Unexpected error adding Phieuthu");
                return null;
            }
        }

        /// <summary>
        /// Lấy thông tin phiếu thu theo ID một cách bất đồng bộ.
        /// </summary>
        /// <param name="id">ID phiếu thu cần lấy.</param>
        /// <returns>Entity Phieuthu nếu tìm thấy, null nếu không.</returns>
        public async Task<Phieuthu?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                Debug.WriteLine("DALPhieuThu - GetByIdAsync: Invalid ID.");
                return null;
            }
            try
            {
                // FindAsync là cách hiệu quả để tìm theo khóa chính
                return await _context.Phieuthu.FindAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DALPhieuThu - GetByIdAsync: Exception occurred for ID {id}: {ex.Message}");
                // Ghi log chi tiết hơn nếu cần
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả phiếu thu một cách bất đồng bộ.
        /// </summary>
        /// <returns>Danh sách Entity Phieuthu.</returns>
        public async Task<List<Phieuthu>> GetAllAsync()
        {
            try
            {
                // Lấy tất cả các phiếu thu
                return await _context.Phieuthu.ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DALPhieuThu - GetAllAsync: Exception occurred: {ex.Message}");
                // Ghi log chi tiết hơn nếu cần
                return new List<Phieuthu>(); // Trả về danh sách rỗng khi có lỗi
            }
        }

        /// <summary>
        /// Lấy danh sách phiếu thu của một độc giả cụ thể một cách bất đồng bộ.
        /// </summary>
        /// <param name="docGiaId">ID của độc giả.</param>
        /// <returns>Danh sách Entity Phieuthu của độc giả đó.</returns>
        public async Task<List<Phieuthu>> GetByDocGiaIdAsync(int docGiaId)
        {
            if (docGiaId <= 0)
            {
                Debug.WriteLine("DALPhieuThu - GetByDocGiaIdAsync: Invalid DocGia ID.");
                return new List<Phieuthu>();
            }
            try
            {
                // Lọc danh sách phiếu thu theo IdDocGia
                return await _context.Phieuthu
                                     .Where(pt => pt.IdDocGia == docGiaId)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DALPhieuThu - GetByDocGiaIdAsync: Exception occurred for DocGia ID {docGiaId}: {ex.Message}");
                // Ghi log chi tiết hơn nếu cần
                return new List<Phieuthu>(); // Trả về danh sách rỗng khi có lỗi
            }
        }

        // Có thể thêm các phương thức UpdateAsync, DeleteAsync nếu cần thiết theo interface
        // Ví dụ:
        // public async Task<bool> UpdateAsync(Phieuthu phieuThu) { ... }
        // public async Task<bool> DeleteAsync(int id) { ... }
    }
}