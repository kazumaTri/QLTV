// DAL/DALNhomNguoiDung.cs
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // Cần cho IServiceScopeFactory
using System;
using System.Collections.Generic;
using System.Diagnostics; // Cho Debug.WriteLine
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Lớp cài đặt truy cập dữ liệu cho Nhóm Người Dùng.
    /// </summary>
    public class DALNhomNguoiDung : IDALNhomNguoiDung // Implement interface
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // Constructor nhận IServiceScopeFactory qua DI
        public DALNhomNguoiDung(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        // === CÁC PHƯƠNG THỨC ĐÃ CÓ ===

        public async Task<List<Nhomnguoidung>> GetAllAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Có thể Include Nguoidung hoặc Chucnang nếu cần thiết ở tầng trên
                    return await context.Nhomnguoidung.ToListAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALNhomNguoiDung.GetAllAsync: {ex.Message}");
                    throw; // Ném lại lỗi để tầng BUS xử lý
                }
            }
        }

        public async Task<Nhomnguoidung?> GetByIdAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // FindAsync hiệu quả cho tìm theo khóa chính
                    return await context.Nhomnguoidung.FindAsync(id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALNhomNguoiDung.GetByIdAsync (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<Nhomnguoidung?> GetByMaAsync(string maNhom)
        {
            if (string.IsNullOrWhiteSpace(maNhom)) return null;

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Tìm theo mã, không phân biệt hoa thường
                    return await context.Nhomnguoidung
                                        .FirstOrDefaultAsync(n => n.MaNhomNguoiDung != null && n.MaNhomNguoiDung.ToLower() == maNhom.ToLower());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALNhomNguoiDung.GetByMaAsync (Ma: {maNhom}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<Nhomnguoidung?> AddAsync(Nhomnguoidung nhomNguoiDung)
        {
            if (nhomNguoiDung == null) throw new ArgumentNullException(nameof(nhomNguoiDung));

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Gỡ bỏ navigation properties trước khi thêm để tránh lỗi tracking
                    // Quan trọng: Không gán quyền khi thêm mới ở đây, việc gán quyền nên thực hiện sau khi nhóm đã được tạo ID
                    nhomNguoiDung.Nguoidung = new List<Nguoidung>(); // Khởi tạo list rỗng
                    nhomNguoiDung.IdChucNang = new List<Chucnang>(); // Khởi tạo list rỗng

                    await context.Nhomnguoidung.AddAsync(nhomNguoiDung);
                    int affectedRows = await context.SaveChangesAsync();
                    return (affectedRows > 0) ? nhomNguoiDung : null; // Trả về entity đã thêm với ID được gán
                }
                catch (DbUpdateException ex) // Bắt lỗi CSDL cụ thể (ví dụ: trùng mã nếu có unique constraint)
                {
                    Debug.WriteLine($"DB Error adding Nhomnguoidung: {ex.InnerException?.Message ?? ex.Message}");
                    // Có thể throw một exception cụ thể hơn thay vì DbUpdateException gốc
                    throw new InvalidOperationException("Lỗi cơ sở dữ liệu khi thêm nhóm người dùng. Mã nhóm có thể đã tồn tại.", ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error adding Nhomnguoidung: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<bool> UpdateAsync(Nhomnguoidung nhomNguoiDung)
        {
            if (nhomNguoiDung == null) throw new ArgumentNullException(nameof(nhomNguoiDung));
            if (nhomNguoiDung.Id <= 0) throw new ArgumentException("ID Nhóm người dùng không hợp lệ để cập nhật.", nameof(nhomNguoiDung.Id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Lấy entity đang được track trong context này
                    var existingNhom = await context.Nhomnguoidung.FindAsync(nhomNguoiDung.Id);
                    if (existingNhom == null)
                    {
                        Debug.WriteLine($"Nhomnguoidung ID {nhomNguoiDung.Id} not found for update.");
                        return false; // Không tìm thấy
                    }

                    // Cập nhật các thuộc tính cần thiết (trừ khóa chính và navigation properties)
                    // Việc cập nhật phân quyền sẽ được làm ở phương thức UpdatePhanQuyenAsync
                    existingNhom.MaNhomNguoiDung = nhomNguoiDung.MaNhomNguoiDung;
                    existingNhom.TenNhomNguoiDung = nhomNguoiDung.TenNhomNguoiDung;

                    // EF Core tự động theo dõi thay đổi của existingNhom
                    int affectedRows = await context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (DbUpdateException ex) // Bắt lỗi CSDL (ví dụ: trùng mã nếu sửa mã)
                {
                    Debug.WriteLine($"DB Error updating Nhomnguoidung (ID: {nhomNguoiDung.Id}): {ex.InnerException?.Message ?? ex.Message}");
                    throw new InvalidOperationException($"Lỗi cơ sở dữ liệu khi cập nhật nhóm người dùng ID {nhomNguoiDung.Id}. Mã nhóm có thể đã tồn tại.", ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error updating Nhomnguoidung (ID: {nhomNguoiDung.Id}): {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID không hợp lệ.", nameof(id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var nhomToDelete = await context.Nhomnguoidung
                        // Include cả collection ChucNang để EF Core biết cần xóa liên kết trong bảng Phanquyen
                        .Include(n => n.IdChucNang)
                        .FirstOrDefaultAsync(n => n.Id == id);

                    if (nhomToDelete == null)
                    {
                        Debug.WriteLine($"Nhomnguoidung ID {id} not found for delete.");
                        return false; // Không tìm thấy để xóa
                    }

                    // --- KIỂM TRA RÀNG BUỘC TRƯỚC KHI XÓA ---
                    bool hasUsers = await context.Nguoidung.AnyAsync(u => u.IdNhomNguoiDung == id /* && u.DaAn == false */); // Kiểm tra cả user đã ẩn nếu cần
                    if (hasUsers)
                    {
                        throw new InvalidOperationException($"Không thể xóa nhóm ID {id} vì còn người dùng thuộc nhóm này.");
                    }

                    // EF Core sẽ tự động xóa các liên kết trong bảng Phanquyen khi xóa Nhomnguoidung
                    // nhờ vào việc Include(n => n.IdChucNang) ở trên và cấu hình quan hệ trong OnModelCreating
                    context.Nhomnguoidung.Remove(nhomToDelete);
                    int affectedRows = await context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (DbUpdateException ex) // Bắt lỗi ràng buộc khóa ngoại từ CSDL nếu kiểm tra ở trên bỏ sót
                {
                    Debug.WriteLine($"DB Error deleting Nhomnguoidung (ID: {id}): {ex.InnerException?.Message ?? ex.Message}");
                    // Kiểm tra xem có phải lỗi FK không
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && (sqlEx.Number == 547)) // 547 là mã lỗi FK constraint
                    {
                        throw new InvalidOperationException($"Không thể xóa Nhóm người dùng ID {id} do còn tồn tại dữ liệu liên quan (ví dụ: Người dùng).", ex);
                    }
                    throw new InvalidOperationException($"Lỗi CSDL khi xóa Nhóm người dùng ID {id}.", ex);
                }
                catch (InvalidOperationException) // Bắt lỗi từ kiểm tra ràng buộc Nguoidung ở trên và ném lại
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error deleting Nhomnguoidung (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }


        // === PHƯƠNG THỨC MỚI ĐỂ QUẢN LÝ PHÂN QUYỀN ===

        /// <summary>
        /// Lấy danh sách ID các Chức năng đã được gán cho một Nhóm người dùng.
        /// </summary>
        public async Task<List<int>> GetChucNangIdsByNhomIdAsync(int nhomId)
        {
            if (nhomId <= 0) return new List<int>(); // Hoặc throw exception tùy logic

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var nhom = await context.Nhomnguoidung
                                        .AsNoTracking() // Chỉ đọc, không cần theo dõi thay đổi
                                        .Include(n => n.IdChucNang) // Quan trọng: Nạp danh sách chức năng liên kết
                                        .FirstOrDefaultAsync(n => n.Id == nhomId);

                    if (nhom == null || nhom.IdChucNang == null)
                    {
                        return new List<int>(); // Không tìm thấy nhóm hoặc nhóm chưa có quyền nào
                    }

                    // Trả về danh sách ID của các chức năng đã nạp
                    return nhom.IdChucNang.Select(cn => cn.Id).ToList();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALNhomNguoiDung.GetChucNangIdsByNhomIdAsync (ID: {nhomId}): {ex.Message}");
                    throw; // Ném lại lỗi
                }
            }
        }

        /// <summary>
        /// Cập nhật danh sách Chức năng được gán cho một Nhóm người dùng.
        /// </summary>
        public async Task<bool> UpdatePhanQuyenAsync(int nhomId, List<int> chucNangIds)
        {
            if (nhomId <= 0) throw new ArgumentException("ID nhóm không hợp lệ.", nameof(nhomId));
            // chucNangIds có thể null hoặc rỗng, nghĩa là xóa hết quyền

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Tìm nhóm người dùng và nạp danh sách quyền hiện tại của nó
                    var nhomToUpdate = await context.Nhomnguoidung
                                                .Include(n => n.IdChucNang) // Quan trọng: Phải Include để sửa collection
                                                .FirstOrDefaultAsync(n => n.Id == nhomId);

                    if (nhomToUpdate == null)
                    {
                        Debug.WriteLine($"Nhomnguoidung ID {nhomId} not found for permission update.");
                        return false; // Không tìm thấy nhóm để cập nhật
                    }

                    // Xóa tất cả các quyền hiện tại của nhóm
                    // EF Core sẽ xử lý việc xóa các bản ghi tương ứng trong bảng Phanquyen
                    nhomToUpdate.IdChucNang.Clear();

                    // Nếu danh sách ID quyền mới không rỗng, thêm các quyền mới vào
                    if (chucNangIds != null && chucNangIds.Any())
                    {
                        // Lấy các đối tượng Chucnang tương ứng với các ID mới
                        // Chỉ lấy những ID thực sự tồn tại trong bảng CHUCNANG
                        var chucNangMoi = await context.Chucnang
                                                      .Where(cn => chucNangIds.Contains(cn.Id))
                                                      .ToListAsync();

                        // Thêm các chức năng mới tìm được vào collection của nhóm
                        foreach (var cn in chucNangMoi)
                        {
                            nhomToUpdate.IdChucNang.Add(cn);
                        }
                    }

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    // EF Core sẽ tự động phát hiện việc xóa và thêm trong collection IdChucNang
                    // và cập nhật bảng Phanquyen tương ứng.
                    int affectedRows = await context.SaveChangesAsync();

                    // Trả về true nếu có ít nhất một thay đổi được lưu (hoặc không cần thay đổi gì)
                    // Có thể xem xét lại logic trả về tùy yêu cầu
                    return true; // Giả định thành công nếu không có exception
                                 // return affectedRows > 0; // Chỉ trả về true nếu thực sự có thay đổi được lưu

                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine($"DB Error updating permissions for Nhomnguoidung (ID: {nhomId}): {ex.InnerException?.Message ?? ex.Message}");
                    throw new InvalidOperationException($"Lỗi CSDL khi cập nhật quyền cho nhóm người dùng ID {nhomId}.", ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error updating permissions for Nhomnguoidung (ID: {nhomId}): {ex.Message}");
                    throw;
                }
            }
        }


    } // End class DALNhomNguoiDung
} // End namespace DAL