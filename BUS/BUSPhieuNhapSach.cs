using DAL;
using DAL.Models;
using DTO;
using Microsoft.EntityFrameworkCore; // Cần cho Transaction
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Cho ValidationContext
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUSPhieuNhapSach : IBUSPhieuNhapSach
    {
        private readonly IDALPhieuNhapSach _dalPhieuNhapSach;
        private readonly IDALCtPhieunhap _dalCtPhieuNhap;
        private readonly IDALSach _dalSach; // Inject DAL Sách
        private readonly IDALCuonSach _dalCuonSach; // Inject DAL Cuốn Sách
        private readonly QLTVContext _context; // Inject DbContext để dùng Transaction
        private readonly ILogger<BUSPhieuNhapSach> _logger;
        private readonly IDALThamSo _dalThamSo; // Để lấy quy định về nhập sách


        public BUSPhieuNhapSach(IDALPhieuNhapSach dalPhieuNhapSach,
                                IDALCtPhieunhap dalCtPhieuNhap,
                                IDALSach dalSach,
                                IDALCuonSach dalCuonSach,
                                QLTVContext context, // Nhận DbContext
                                ILogger<BUSPhieuNhapSach> logger,
                                IDALThamSo dalThamSo)
        {
            _dalPhieuNhapSach = dalPhieuNhapSach;
            _dalCtPhieuNhap = dalCtPhieuNhap;
            _dalSach = dalSach;
            _dalCuonSach = dalCuonSach;
            _context = context; // Lưu DbContext
            _logger = logger;
            _dalThamSo = dalThamSo;
        }

        public async Task<List<PhieuNhapSachDTO>> GetAllPhieuNhapDTOAsync()
        {
            _logger.LogInformation("Getting all PhieuNhapSach as DTOs.");
            var phieuNhaps = await _dalPhieuNhapSach.GetAllAsync();
            // Map từ List<Phieunhapsach> sang List<PhieuNhapSachDTO>
            // Chỉ lấy thông tin cơ bản, không cần chi tiết ở đây
            return phieuNhaps.Select(pn => new PhieuNhapSachDTO
            {
                SoPhieuNhap = pn.SoPhieuNhap,
                NgayNhap = pn.NgayNhap,
                TongTien = pn.TongTien // Xử lý null nếu có
            }).ToList();
        }

        public async Task<PhieuNhapSachDTO?> GetPhieuNhapDTOByIdAsync(int soPhieuNhap)
        {
            _logger.LogInformation("Getting PhieuNhapSach DTO by Id: {SoPhieuNhap}", soPhieuNhap);
            var phieuNhap = await _dalPhieuNhapSach.GetByIdAsync(soPhieuNhap); // DAL đã Include chi tiết và Sách/Tựa sách

            if (phieuNhap == null)
            {
                _logger.LogWarning("PhieuNhapSach with Id: {SoPhieuNhap} not found.", soPhieuNhap);
                return null;
            }

            // Map từ Entity sang DTO
            var phieuNhapDTO = new PhieuNhapSachDTO
            {
                SoPhieuNhap = phieuNhap.SoPhieuNhap,
                NgayNhap = phieuNhap.NgayNhap,
                TongTien = phieuNhap.TongTien,
                ChiTietPhieuNhap = phieuNhap.CtPhieunhap.Select(ct => new CtPhieuNhapDTO
                {
                    IdSach = ct.IdSach,
                    MaSach = ct.IdSachNavigation?.MaSach, // Lấy mã sách từ IdSachNavigation
                    TenSach = ct.IdSachNavigation?.IdTuaSachNavigation?.TenTuaSach, // Lấy tên từ Tựa sách
                    SoLuongNhap = ct.SoLuongNhap,
                    DonGia = ct.DonGia
                    // ThanhTien được tính tự động trong DTO
                }).ToList()
            };
            return phieuNhapDTO;
        }

        public async Task<int> LapPhieuNhapAsync(PhieuNhapSachDTO phieuNhapSachDTO)
        {
            _logger.LogInformation("Attempting to create a new PhieuNhapSach.");
            if (phieuNhapSachDTO == null)
            {
                _logger.LogError("Input PhieuNhapSachDTO is null.");
                throw new ArgumentNullException(nameof(phieuNhapSachDTO));
            }

            // --- **1. Validate Input DTO (Basic Validation)** ---
            // TODO: Thêm validation chi tiết cho phieuNhapSachDTO và các chi tiết bên trong
            // Ví dụ: Kiểm tra Ngày nhập, Chi tiết phiếu nhập không rỗng, Số lượng > 0, Đơn giá >= 0
            if (phieuNhapSachDTO.ChiTietPhieuNhap == null || !phieuNhapSachDTO.ChiTietPhieuNhap.Any())
            {
                throw new ValidationException("Phiếu nhập phải có ít nhất một chi tiết sách.");
            }
            foreach (var ct in phieuNhapSachDTO.ChiTietPhieuNhap)
            {
                if (ct.IdSach <= 0 || ct.SoLuongNhap <= 0 || ct.DonGia < 0)
                {
                    throw new ValidationException($"Thông tin chi tiết sách không hợp lệ (ID Sách: {ct.IdSach}, SL: {ct.SoLuongNhap}, Đơn giá: {ct.DonGia}).");
                }
                // Có thể kiểm tra sự tồn tại của sách ở đây nếu muốn (nhưng sẽ kiểm tra lại trong transaction)
            }
            // --- HẾT VALIDATION ---


            // --- **2. LẤY VÀ KHAI BÁO THAM SỐ ĐÚNG CÁCH** ---

            // Lấy đối tượng Thamso bằng phương thức đúng
            var thamSo = await _dalThamSo.GetThamSoAsync(); // <<< SỬA LỖI: Gọi GetThamSoAsync
            if (thamSo == null)
            {
                _logger.LogError("Không thể tải tham số hệ thống."); // Sửa log message
                throw new InvalidOperationException("Không thể tải tham số hệ thống cần thiết.");
            }

            // Khai báo biến quyDinhNhapSach bằng cách truy cập thuộc tính đúng
            // !!! Chú ý: Đảm bảo lớp model 'Thamso' có thuộc tính tên là 'KhoangCachXuatBan'
            int quyDinhNhapSach = thamSo.KhoangCachXuatBan;

            // Xóa hoặc comment dòng khai báo sai trước đó:
            // var soNamXBToiThieu = thamSo.FirstOrDefault(...)?.GiaTri ?? 0; // BỎ ĐI
            // var quyDinhNhapSach = thamSo.FirstOrDefault(...)?.GiaTri ?? 9999; // BỎ ĐI

            // --- **HẾT PHẦN LẤY THAM SỐ** ---


            // --- **3. Start Transaction** ---
            using var transaction = await _context.Database.BeginTransactionAsync();
            _logger.LogInformation("Database transaction started for creating PhieuNhapSach.");

            try
            {
                // --- **4. Map DTO to Entity & Calculate TongTien** ---
                var phieuNhap = new Phieunhapsach
                {
                    NgayNhap = phieuNhapSachDTO.NgayNhap,
                    // Tính tổng tiền dựa trên chi tiết đã validate
                    TongTien = Convert.ToInt32(phieuNhapSachDTO.ChiTietPhieuNhap.Sum(ct => (long)ct.SoLuongNhap * ct.DonGia))
                };

                // --- **5. Save Phieunhapsach (Get SoPhieuNhap)** ---
                int soPhieuNhapMoi = await _dalPhieuNhapSach.AddAsync(phieuNhap);
                _logger.LogInformation("Phieunhapsach created with SoPhieuNhap: {SoPhieuNhap}", soPhieuNhapMoi);

                // --- **6. Prepare and Validate CtPhieunhap Entities** ---
                var ctPhieuNhaps = new List<CtPhieunhap>();
                var cuonSachsMoi = new List<Cuonsach>();
                var sachsToUpdate = new Dictionary<int, Sach>(); // Dùng Dictionary để tránh update trùng lặp

                foreach (var ctDTO in phieuNhapSachDTO.ChiTietPhieuNhap)
                {
                    // Lấy thông tin Sách từ DB để kiểm tra và cập nhật
                    var sach = await _dalSach.GetByIdAsync(ctDTO.IdSach);
                    if (sach == null)
                    {
                        // Nếu sách không tồn tại thì không thể nhập -> Lỗi logic hoặc dữ liệu đầu vào sai
                        await transaction.RollbackAsync(); // Rollback ngay lập tức
                        _logger.LogError("Sách với ID {IdSach} không tồn tại trong DB khi đang lập phiếu nhập.", ctDTO.IdSach);
                        throw new InvalidOperationException($"Sách với ID {ctDTO.IdSach} không tồn tại.");
                    }

                    // KIỂM TRA QUY ĐỊNH NHẬP SÁCH (SỬ DỤNG BIẾN ĐÃ KHAI BÁO ĐÚNG)
                    // !!! Chú ý: Đảm bảo lớp model 'Sach' có thuộc tính tên là 'NamXb'
                    if (DateTime.Now.Year - sach.NamXb > quyDinhNhapSach)
                    {
                        // Ném lỗi nếu sách quá cũ
                        await transaction.RollbackAsync(); // Rollback ngay lập tức
                        _logger.LogWarning("Attempted to import book '{TenTuaSach}' (Published: {NamXb}) older than rule ({QuyDinh} years).",
                            sach.IdTuaSachNavigation?.TenTuaSach, sach.NamXb, quyDinhNhapSach);
                        throw new InvalidOperationException($"Sách '{sach.IdTuaSachNavigation?.TenTuaSach}' (NXB {sach.NamXb}) quá cũ so với quy định (chỉ nhập sách trong vòng {quyDinhNhapSach} năm).");
                    }

                    // Tạo đối tượng CtPhieunhap
                    ctPhieuNhaps.Add(new CtPhieunhap
                    {
                        SoPhieuNhap = soPhieuNhapMoi,
                        IdSach = ctDTO.IdSach,
                        SoLuongNhap = ctDTO.SoLuongNhap,
                        DonGia = Convert.ToInt32(ctDTO.DonGia) // Đảm bảo kiểu dữ liệu khớp DB
                    });

                    // Chuẩn bị cập nhật số lượng cho Sách (dùng Dictionary để xử lý nếu cùng 1 sách nhập nhiều dòng)
                    if (!sachsToUpdate.ContainsKey(sach.Id))
                    {
                        sachsToUpdate.Add(sach.Id, sach);
                    }
                    sachsToUpdate[sach.Id].SoLuong += ctDTO.SoLuongNhap;
                    sachsToUpdate[sach.Id].SoLuongConLai += ctDTO.SoLuongNhap;


                    // Tạo các đối tượng Cuonsach mới
                    for (int i = 0; i < ctDTO.SoLuongNhap; i++)
                    {
                        // Tạo mã cuốn sách duy nhất (ví dụ) - Cần logic tạo mã phù hợp
                        string maCuonSach = $"{sach.MaSach}-{Guid.NewGuid().ToString().Substring(0, 4)}";
                        cuonSachsMoi.Add(new Cuonsach
                        {
                            MaCuonSach = maCuonSach,
                            IdSach = sach.Id,
                            TinhTrang = 0 // Mặc định là "Sẵn sàng"
                        });
                    }

                } // Kết thúc foreach

                // --- **7. Save CtPhieunhap Entities** ---
                bool ctAdded = await _dalCtPhieuNhap.AddRangeAsync(ctPhieuNhaps);
                if (!ctAdded)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Failed to add CtPhieuNhap entities for SoPhieuNhap: {SoPhieuNhap}.", soPhieuNhapMoi);
                    throw new DbUpdateException("Lỗi khi lưu chi tiết phiếu nhập.");
                }
                _logger.LogInformation("Successfully added {Count} CtPhieuNhap items for SoPhieuNhap: {SoPhieuNhap}.", ctPhieuNhaps.Count, soPhieuNhapMoi);


                // --- **8. Update Sach Entities** ---
                bool sachUpdated = await _dalSach.UpdateRangeAsync(sachsToUpdate.Values.ToList());
                if (!sachUpdated)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Failed to update Sach entities for SoPhieuNhap: {SoPhieuNhap}.", soPhieuNhapMoi);
                    throw new DbUpdateException("Lỗi khi cập nhật số lượng sách.");
                }
                _logger.LogInformation("Successfully updated {Count} Sach items for SoPhieuNhap: {SoPhieuNhap}.", sachsToUpdate.Count, soPhieuNhapMoi);


                // --- **9. Add new CuonSach Entities** ---
                bool cuonSachAdded = await _dalCuonSach.AddRangeAsync(cuonSachsMoi);
                if (!cuonSachAdded)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Failed to add CuonSach entities for SoPhieuNhap: {SoPhieuNhap}.", soPhieuNhapMoi);
                    throw new DbUpdateException("Lỗi khi thêm các cuốn sách mới.");
                }
                _logger.LogInformation("Successfully added {Count} CuonSach items for SoPhieuNhap: {SoPhieuNhap}.", cuonSachsMoi.Count, soPhieuNhapMoi);


                // --- **10. Commit Transaction** ---
                await transaction.CommitAsync();
                _logger.LogInformation("Transaction committed successfully for SoPhieuNhap: {SoPhieuNhap}", soPhieuNhapMoi);

                return soPhieuNhapMoi;
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                _logger.LogError(dbEx, "Database update error occurred during PhieuNhapSach creation. Transaction rolled back. Details: {ErrorMessage}", dbEx.InnerException?.Message ?? dbEx.Message);
                // Cung cấp thông điệp lỗi cụ thể hơn nếu có thể
                string errorMessage = "Lỗi khi cập nhật cơ sở dữ liệu. Vui lòng thử lại.";
                if (dbEx.InnerException != null)
                {
                    // Kiểm tra các lỗi phổ biến như khóa ngoại, unique constraint...
                    errorMessage += $" Chi tiết: {dbEx.InnerException.Message}";
                }
                throw new Exception(errorMessage, dbEx);
            }
            catch (ValidationException valEx) // Bắt lỗi validation từ DTO hoặc logic
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(valEx, "Validation error during PhieuNhapSach creation. Transaction rolled back.");
                throw; // Ném lại lỗi validation để hiển thị cho người dùng
            }
            catch (InvalidOperationException opEx) // Bắt lỗi logic nghiệp vụ (sách quá cũ, sách không tồn tại...)
            {
                await transaction.RollbackAsync();
                _logger.LogError(opEx, "Business logic error during PhieuNhapSach creation. Transaction rolled back.");
                throw; // Ném lại lỗi để hiển thị cho người dùng
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An unexpected error occurred during PhieuNhapSach creation. Transaction rolled back.");
                throw new Exception("Đã xảy ra lỗi không mong muốn trong quá trình lập phiếu nhập.", ex);
            }
        } // Kết thúc phương thức LapPhieuNhapAsync

        // Bạn cần đảm bảo IDALSach và IDALCuonSach có các phương thức UpdateRangeAsync và AddRangeAsync tương ứng
        // Nếu chưa có, bạn cần thêm chúng vào interface và implement trong DAL.
    }
}