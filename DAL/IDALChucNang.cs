// DAL/DALChucNang.cs
// *** PHIÊN BẢN ĐÃ SỬA ĐỔI ĐỂ SỬ DỤNG IServiceScopeFactory ***
using DAL.Models; // Cần để sử dụng Entity 'Chucnang' và 'QLTVContext'
using Microsoft.EntityFrameworkCore; // Cần cho ToListAsync() và các phương thức EF Core khác
using Microsoft.Extensions.DependencyInjection; // *** THÊM using này ***
using System;
using System.Collections.Generic;
using System.Diagnostics; // Cho Debug.WriteLine
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Data Access Layer class for ChucNang (Function/Permission).
    /// Implements the IDALChucNang interface to provide data access methods.
    /// </summary>
    public class DALChucNang : IDALChucNang // Implement interface đã tạo
    {
        // *** THAY ĐỔI: Sử dụng IServiceScopeFactory thay vì QLTVContext trực tiếp ***
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Constructor that accepts the IServiceScopeFactory via Dependency Injection.
        /// </summary>
        /// <param name="scopeFactory">The service scope factory.</param>
        public DALChucNang(IServiceScopeFactory scopeFactory) // *** THAY ĐỔI Tham số constructor ***
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory)); // *** Gán scopeFactory ***
        }

        /// <summary>
        /// Retrieves all Chucnang entities from the database asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of all Chucnang entities.</returns>
        public async Task<List<Chucnang>> GetAllAsync()
        {
            // *** THAY ĐỔI: Tạo scope và lấy context trong scope ***
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Truy cập DbSet<Chucnang> trong context và lấy tất cả bản ghi
                    // ToListAsync() thực hiện truy vấn đến CSDL một cách bất đồng bộ
                    return await context.Chucnang.ToListAsync();
                }
                catch (Exception ex)
                {
                    // Ghi lại lỗi để debug (có thể sử dụng một logging framework thực tế)
                    Debug.WriteLine($"Error in DALChucNang.GetAllAsync: {ex.Message}");
                    // Ném lại lỗi hoặc trả về danh sách rỗng tùy theo cách xử lý lỗi mong muốn
                    // Ở đây, ném lại lỗi gốc để lớp BUS có thể xử lý
                    throw new Exception("An error occurred while fetching the list of ChucNang.", ex);
                    // Hoặc trả về danh sách rỗng: return new List<Chucnang>();
                }
            } // Scope và context sẽ tự động được dispose ở đây
        }

        // --- Implement các phương thức khác từ IDALChucNang nếu bạn đã thêm chúng ---
        // Ví dụ:
        // public async Task<Chucnang?> GetByIdAsync(int id)
        // {
        //     using (var scope = _scopeFactory.CreateScope()) // Tạo scope
        //     {
        //         var context = scope.ServiceProvider.GetRequiredService<QLTVContext>(); // Lấy context
        //         try
        //         {
        //             return await context.Chucnang.FindAsync(id);
        //         }
        //         catch (Exception ex)
        //         {
        //             Debug.WriteLine($"Error in DALChucNang.GetByIdAsync({id}): {ex.Message}");
        //             throw new Exception($"An error occurred while fetching ChucNang with ID {id}.", ex);
        //         }
        //     }
        // }

        // ... (implement các phương thức Add, Update, Delete nếu cần, sử dụng scope factory tương tự) ...
    }
}
