// File: BUS/BUSDocGia.cs
using DAL;
using DAL.Models;
using DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using BUS.Utilities; // Sử dụng using cho namespace Utilities
using Microsoft.EntityFrameworkCore;

namespace BUS
{
    /// <summary>
    /// Business Logic Layer for Reader (Độc Giả).
    /// Implements IBUSDocGia.
    /// </summary>
    public class BUSDocGia : IBUSDocGia // Triển khai Interface đã cập nhật
    {
        private readonly IDALDocGia _dalDocGia;
        private readonly IDALNguoiDung _dalNguoiDung;
        private readonly IPasswordHasherService _passwordHasher; // Dùng trực tiếp nhờ using
        private readonly IBUSThongBao _busThongBao;
        private readonly IDALThamSo _dalThamSo;
        private readonly IDALPhieuThu _dalPhieuThu;

        // Constructor
        public BUSDocGia(
            IDALDocGia dalDocGia,
            IDALNguoiDung dalNguoiDung,
            IPasswordHasherService passwordHasher, // Dùng trực tiếp nhờ using
            IBUSThongBao busThongBao,
            IDALThamSo dalThamSo,
            IDALPhieuThu dalPhieuThu)
        {
            _dalDocGia = dalDocGia ?? throw new ArgumentNullException(nameof(dalDocGia));
            _dalNguoiDung = dalNguoiDung ?? throw new ArgumentNullException(nameof(dalNguoiDung));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _busThongBao = busThongBao ?? throw new ArgumentNullException(nameof(busThongBao));
            _dalThamSo = dalThamSo ?? throw new ArgumentNullException(nameof(dalThamSo));
            _dalPhieuThu = dalPhieuThu; // Assume IDALPhieuThu exists and is injected
        }

        // --- Get Methods ---
        public async Task<List<DocgiaDTO>> GetAllDocGiaAsync(bool includeDeleted = false, int? idLoaiDocGiaFilter = null)
        {
            try
            {
                var entities = await _dalDocGia.GetAllAsync(includeDeleted, idLoaiDocGiaFilter);
                // Sử dụng Select với MapToDocgiaDTO đã cập nhật
                return entities?.Select(dg => MapToDocgiaDTO(dg)).ToList() ?? new List<DocgiaDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"BUS Error GetAll: {ex.Message}"); return new List<DocgiaDTO>();
            }
        }
        public async Task<DocgiaDTO?> GetDocGiaByIdAsync(int id, bool includeDeleted = false)
        {
            try
            {
                var entity = await _dalDocGia.GetByIdAsync(id, includeDeleted);
                // Sử dụng MapToDocgiaDTO đã cập nhật
                return (entity != null) ? MapToDocgiaDTO(entity) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"BUS Error GetById {id}: {ex.Message}"); return null;
            }
        }
        public async Task<DocgiaDTO?> GetDocGiaByIdNguoiDungAsync(int idNguoiDung, bool includeDeleted = false)
        {
            try
            {
                var entity = await _dalDocGia.GetByIdNguoiDungAsync(idNguoiDung, includeDeleted);
                // Sử dụng MapToDocgiaDTO đã cập nhật
                return (entity != null) ? MapToDocgiaDTO(entity) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"BUS Error GetByIdNguoiDung {idNguoiDung}: {ex.Message}"); return null;
            }
        }
        public async Task<int> GetActiveReaderCountAsync()
        {
            try { return await _dalDocGia.GetActiveReaderCountAsync(); } catch (Exception ex) { Debug.WriteLine($"BUS Error GetActiveReaderCount: {ex.Message}"); return 0; }
        }

        // --- Search Method ---
        public async Task<List<DocgiaDTO>> SearchDocGiaAsync(string keyword, bool includeDeleted = false, int? idLoaiDocGiaFilter = null)
        {
            if (string.IsNullOrWhiteSpace(keyword)) { return await GetAllDocGiaAsync(includeDeleted, idLoaiDocGiaFilter); }
            try
            {
                var entities = await _dalDocGia.SearchAsync(keyword.Trim(), includeDeleted, idLoaiDocGiaFilter);
                // Sử dụng Select với MapToDocgiaDTO đã cập nhật
                return entities?.Select(dg => MapToDocgiaDTO(dg)).ToList() ?? new List<DocgiaDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"BUS Error Search: {ex.Message}"); return new List<DocgiaDTO>();
            }
        }

        // --- Add Method ---
        public async Task<DocgiaDTO?> AddDocGiaAsync(DocgiaDTO newDocGiaDTO)
        {
            if (newDocGiaDTO == null) throw new ArgumentNullException(nameof(newDocGiaDTO), "Thông tin độc giả không được rỗng.");
            var (minAge, maxAge) = await GetAgeRulesAsync();
            ValidateDocGia(newDocGiaDTO, isAdding: true, minAge, maxAge); // Đảm bảo NgaySinh không null ở đây
            if (!string.IsNullOrWhiteSpace(newDocGiaDTO.TenDangNhap)) { bool exists = await _dalNguoiDung.IsUsernameExistsAsync(newDocGiaDTO.TenDangNhap.Trim()); if (exists) throw new ArgumentException($"Tên đăng nhập '{newDocGiaDTO.TenDangNhap.Trim()}' đã tồn tại."); }
            if (!string.IsNullOrWhiteSpace(newDocGiaDTO.Email)) { bool exists = await _dalDocGia.IsEmailExistsAsync(newDocGiaDTO.Email.Trim()); if (exists) throw new ArgumentException($"Email '{newDocGiaDTO.Email.Trim()}' đã được sử dụng."); }

            Docgia? addedDocGiaEntity = null; int addedDocGiaId = 0; var cardValidityYears = await _dalThamSo.GetCardValidityDurationAsync();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var nguoiDungEntity = new Nguoidung { TenNguoiDung = newDocGiaDTO.TenDocGia.Trim(), NgaySinh = newDocGiaDTO.NgaySinh, TenDangNhap = newDocGiaDTO.TenDangNhap!.Trim(), MatKhau = _passwordHasher.HashPassword(newDocGiaDTO.MatKhauNguoiDung!), IdNhomNguoiDung = newDocGiaDTO.IdVaiTroNguoiDung ?? 15, DaAn = false };
                    var addedNguoiDung = await _dalNguoiDung.AddAsync(nguoiDungEntity);
                    if (addedNguoiDung == null || addedNguoiDung.Id <= 0) throw new InvalidOperationException("Không thể tạo tài khoản người dùng.");

                    Docgia docGiaEntity = MapToDocgiaEntity(newDocGiaDTO);
                    docGiaEntity.Id = 0; docGiaEntity.IdNguoiDung = addedNguoiDung.Id; docGiaEntity.NgayLapThe = DateTime.Today; docGiaEntity.NgayHetHan = DateTime.Today.AddYears(cardValidityYears); docGiaEntity.TongNoHienTai = 0; docGiaEntity.DaAn = false; docGiaEntity.MaDocGia = $"DG{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

                    addedDocGiaEntity = await _dalDocGia.AddAsync(docGiaEntity);
                    if (addedDocGiaEntity == null || addedDocGiaEntity.Id <= 0) throw new InvalidOperationException("Không thể thêm thông tin độc giả.");

                    addedDocGiaId = addedDocGiaEntity.Id; scope.Complete();
                }
                catch (Exception ex) { Debug.WriteLine($"Error adding reader TX: {ex}"); if (ex is ArgumentException || ex is InvalidOperationException) throw; throw new Exception($"Lỗi hệ thống khi thêm độc giả: {ex.Message}", ex); }
            }
            if (addedDocGiaId > 0) { try { var resultEntity = await _dalDocGia.GetByIdAsync(addedDocGiaId); if (resultEntity != null) { var dto = MapToDocgiaDTO(resultEntity); _ = _busThongBao.CreateActivityLogAsync($"Độc giả '{resultEntity.TenDocGia}' thêm mới."); return dto; } } catch (Exception ex) { Debug.WriteLine($"Error fetching after add TX: {ex.Message}"); } }
            // Sử dụng MapToDocgiaDTO đã cập nhật
            return (addedDocGiaEntity != null) ? MapToDocgiaDTO(addedDocGiaEntity) : null;
        }

        // --- Update Method ---
        public async Task<bool> UpdateDocGiaAsync(DocgiaDTO updatedDocGiaDTO)
        {
            if (updatedDocGiaDTO == null || updatedDocGiaDTO.Id <= 0) throw new ArgumentException("Thông tin cập nhật độc giả không hợp lệ.");
            var (minAge, maxAge) = await GetAgeRulesAsync();
            ValidateDocGia(updatedDocGiaDTO, isAdding: false, minAge, maxAge); // Đảm bảo NgaySinh không null
            if (!string.IsNullOrWhiteSpace(updatedDocGiaDTO.Email)) { bool exists = await _dalDocGia.IsEmailExistsAsync(updatedDocGiaDTO.Email.Trim(), updatedDocGiaDTO.Id); if (exists) throw new ArgumentException($"Email '{updatedDocGiaDTO.Email.Trim()}' đã được dùng."); }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var existingDocGiaEntity = await _dalDocGia.GetByIdAsync(updatedDocGiaDTO.Id, includeDeleted: false);
                    if (existingDocGiaEntity == null) throw new KeyNotFoundException($"Không tìm thấy độc giả ID {updatedDocGiaDTO.Id}.");
                    var existingNguoiDungEntity = await _dalNguoiDung.GetByIdAsync(existingDocGiaEntity.IdNguoiDung, includeDeleted: false);
                    if (existingNguoiDungEntity == null) throw new InvalidOperationException($"Không tìm thấy người dùng ID {existingDocGiaEntity.IdNguoiDung}.");

                    string oldTenDocGia = existingDocGiaEntity.TenDocGia; bool hasChanges = false; bool nguoiDungChanged = false;

                    // Cập nhật Docgia (NgaySinh là DateTime)
                    if (existingDocGiaEntity.TenDocGia != updatedDocGiaDTO.TenDocGia.Trim()) { existingDocGiaEntity.TenDocGia = updatedDocGiaDTO.TenDocGia.Trim(); hasChanges = true; }
                    if (existingDocGiaEntity.NgaySinh != updatedDocGiaDTO.NgaySinh.Value) { existingDocGiaEntity.NgaySinh = updatedDocGiaDTO.NgaySinh.Value; hasChanges = true; }
                    if (existingDocGiaEntity.DiaChi != updatedDocGiaDTO.DiaChi?.Trim()) { existingDocGiaEntity.DiaChi = updatedDocGiaDTO.DiaChi?.Trim(); hasChanges = true; }
                    if (existingDocGiaEntity.DienThoai != updatedDocGiaDTO.DienThoai?.Trim()) { existingDocGiaEntity.DienThoai = updatedDocGiaDTO.DienThoai?.Trim(); hasChanges = true; }
                    if (existingDocGiaEntity.Email != updatedDocGiaDTO.Email?.Trim()) { existingDocGiaEntity.Email = updatedDocGiaDTO.Email?.Trim(); hasChanges = true; }
                    if (existingDocGiaEntity.IdLoaiDocGia != updatedDocGiaDTO.IdLoaiDocGia) { existingDocGiaEntity.IdLoaiDocGia = updatedDocGiaDTO.IdLoaiDocGia; hasChanges = true; }
                    // Kiểm tra null cho NgayLapThe/NgayHetHan từ DTO trước khi gán .Value
                    if (updatedDocGiaDTO.NgayLapThe.HasValue && existingDocGiaEntity.NgayLapThe != updatedDocGiaDTO.NgayLapThe.Value) { existingDocGiaEntity.NgayLapThe = updatedDocGiaDTO.NgayLapThe.Value; hasChanges = true; }
                    if (updatedDocGiaDTO.NgayHetHan.HasValue && existingDocGiaEntity.NgayHetHan != updatedDocGiaDTO.NgayHetHan.Value) { existingDocGiaEntity.NgayHetHan = updatedDocGiaDTO.NgayHetHan.Value; hasChanges = true; }

                    // Cập nhật Nguoidung (NgaySinh là DateTime?)
                    if (existingNguoiDungEntity.TenNguoiDung != existingDocGiaEntity.TenDocGia) { existingNguoiDungEntity.TenNguoiDung = existingDocGiaEntity.TenDocGia; nguoiDungChanged = true; }
                    if (existingNguoiDungEntity.NgaySinh != existingDocGiaEntity.NgaySinh) { existingNguoiDungEntity.NgaySinh = existingDocGiaEntity.NgaySinh; nguoiDungChanged = true; }
                    if (!string.IsNullOrWhiteSpace(updatedDocGiaDTO.MatKhauNguoiDung)) { existingNguoiDungEntity.MatKhau = _passwordHasher.HashPassword(updatedDocGiaDTO.MatKhauNguoiDung); nguoiDungChanged = true; }

                    if (!hasChanges && !nguoiDungChanged) { scope.Complete(); return true; }

                    bool nguoiDungUpdated = true; if (nguoiDungChanged) { nguoiDungUpdated = await _dalNguoiDung.UpdateAsync(existingNguoiDungEntity); if (!nguoiDungUpdated) throw new InvalidOperationException("Lỗi cập nhật người dùng."); }
                    bool docGiaUpdated = await _dalDocGia.UpdateAsync(existingDocGiaEntity);

                    scope.Complete();
                    _ = _busThongBao.CreateActivityLogAsync($"Độc giả '{oldTenDocGia}' (ID: {updatedDocGiaDTO.Id}) đã cập nhật.");
                    return true;
                }
                catch (Exception ex) { Debug.WriteLine($"Error updating reader: {ex}"); if (ex is ArgumentException || ex is InvalidOperationException || ex is KeyNotFoundException) throw; throw new Exception($"Lỗi hệ thống khi cập nhật độc giả: {ex.Message}", ex); }
            }
        }

        // --- Soft Delete Method ---
        public async Task<bool> SoftDeleteDocGiaAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID không hợp lệ.", nameof(id));
            bool hasActiveLoans = await _dalDocGia.HasActiveLoansAsync(id); if (hasActiveLoans) throw new InvalidOperationException("Độc giả còn phiếu mượn chưa trả.");
            var docGia = await _dalDocGia.GetByIdAsync(id, includeDeleted: false); if (docGia == null) return false; int? userId = docGia.IdNguoiDung;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) { try { bool dgDeleted = await _dalDocGia.SoftDeleteAsync(id); if (!dgDeleted) return false; if (userId.HasValue) await _dalNguoiDung.SoftDeleteAsync(userId.Value); scope.Complete(); _ = _busThongBao.CreateActivityLogAsync($"Độc giả '{docGia.TenDocGia}' bị xóa."); return true; } catch (Exception ex) { Debug.WriteLine($"Error soft delete TX: {ex}"); throw new Exception($"Lỗi xóa mềm độc giả: {ex.Message}", ex); } }
        }

        // --- Restore Method ---
        public async Task<bool> RestoreDocGiaAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID không hợp lệ.", nameof(id));
            var docGia = await _dalDocGia.GetByIdAsync(id, includeDeleted: true); if (docGia == null) return false;
            if (!docGia.DaAn)
            {
                Debug.WriteLine($"Reader ID {id} is not deleted."); return true;
            }
            int? idNguoiDungToRestore = docGia.IdNguoiDung;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) { try { bool dgRestored = await _dalDocGia.RestoreAsync(id); if (!dgRestored) return false; if (idNguoiDungToRestore.HasValue) await _dalNguoiDung.RestoreAsync(idNguoiDungToRestore.Value); scope.Complete(); _ = _busThongBao.CreateActivityLogAsync($"Độc giả '{docGia.TenDocGia}' được khôi phục."); return true; } catch (Exception ex) { Debug.WriteLine($"Error restore TX: {ex}"); throw new Exception($"Lỗi khôi phục độc giả: {ex.Message}", ex); } }
        }

        // --- Triển khai các phương thức bổ sung từ Interface ---
        public async Task<bool> GiaHanTheDocGiaAsync(int idDocGia)
        {
            if (idDocGia <= 0) throw new ArgumentException("ID không hợp lệ.", nameof(idDocGia)); try { var docGia = await _dalDocGia.GetByIdAsync(idDocGia, false); if (docGia == null) throw new KeyNotFoundException($"Không tìm thấy độc giả ID {idDocGia}."); var years = await _dalThamSo.GetCardValidityDurationAsync(); DateTime newExp = DateTime.Today.AddYears(years); bool updated = await _dalDocGia.UpdateNgayHetHanAsync(idDocGia, newExp); if (updated) _ = _busThongBao.CreateActivityLogAsync($"Thẻ độc giả '{docGia.TenDocGia}' gia hạn đến {newExp:dd/MM/yyyy}."); return updated; } catch (Exception ex) { Debug.WriteLine($"Error renew card: {ex}"); if (ex is KeyNotFoundException) throw; throw new Exception($"Lỗi gia hạn thẻ: {ex.Message}", ex); }
        }
        public async Task<bool> LapPhieuThuPhatAsync(PhieuThuPhatDTO phieuThuPhatDTO)
        {
            // ... (code khác)
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // ... (code khác)
                    var docGia = await _dalDocGia.GetByIdAsync(phieuThuPhatDTO.IdDocGia, false);
                    if (docGia == null) throw new KeyNotFoundException($"Không tìm thấy độc giả ID {phieuThuPhatDTO.IdDocGia}.");

                    int currentDebt = docGia.TongNoHienTai;

                    if (currentDebt <= 0) throw new InvalidOperationException("Độc giả này không có nợ.");
                    if (phieuThuPhatDTO.SoTienThu > currentDebt) throw new ArgumentException($"Số tiền thu vượt quá nợ.");

                    // Sửa ở dòng dưới đây: NgayLapPhieu -> NgayLap
                    var phieuThu = new Phieuthu
                    {
                        IdDocGia = phieuThuPhatDTO.IdDocGia,
                        SoTienThu = phieuThuPhatDTO.SoTienThu,
                        NgayLap = DateTime.Now // <-- ĐÃ SỬA TÊN THUỘC TÍNH
                    };

                    var added = await _dalPhieuThu.AddAsync(phieuThu);
                    if (added == null) throw new InvalidOperationException("Lỗi tạo phiếu thu.");
                    int newDebt = currentDebt - phieuThuPhatDTO.SoTienThu;
                    bool updated = await _dalDocGia.UpdateTongNoAsync(phieuThuPhatDTO.IdDocGia, newDebt);
                    if (!updated) throw new InvalidOperationException("Lỗi cập nhật nợ.");
                    scope.Complete();
                    _ = _busThongBao.CreateActivityLogAsync($"Thu phạt {phieuThuPhatDTO.SoTienThu:N0}đ từ độc giả '{docGia.TenDocGia}'.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error fine receipt: {ex.Message}");
                    if (ex is ArgumentException || ex is InvalidOperationException || ex is KeyNotFoundException) throw;
                    throw new Exception($"Lỗi khi lập phiếu thu phạt: {ex.Message}", ex);
                }
            }
        }
        public async Task<string?> ResetPasswordAsync(int idNguoiDung)
        {
            if (idNguoiDung <= 0) throw new ArgumentException("ID không hợp lệ.", nameof(idNguoiDung)); try { var user = await _dalNguoiDung.GetByIdAsync(idNguoiDung, false); if (user == null) throw new KeyNotFoundException($"Không tìm thấy người dùng ID {idNguoiDung}."); string newPass = GenerateRandomPassword(8); string hashed = _passwordHasher.HashPassword(newPass); bool updated = await _dalNguoiDung.UpdatePasswordAsync(idNguoiDung, hashed); if (updated) { _ = _busThongBao.CreateActivityLogAsync($"Mật khẩu người dùng '{user.TenNguoiDung}' đã đặt lại."); return newPass; } else { return null; } } catch (Exception ex) { Debug.WriteLine($"Error reset password: {ex}"); if (ex is KeyNotFoundException) throw; throw new Exception($"Lỗi đặt lại mật khẩu: {ex.Message}", ex); }
        }

        // --- Mapping Methods ---

        // ===== BẮT ĐẦU THAY ĐỔI =====
        private DocgiaDTO MapToDocgiaDTO(Docgia entity)
        {
            if (entity == null) return null!;

            string trangThaiThe = "Không xác định";
            // Kiểm tra NgayHetHan có giá trị hợp lệ không (vì kiểu DateTime trong C# không thể null trực tiếp)
            // Giá trị mặc định của DateTime là DateTime.MinValue
            if (entity.NgayHetHan != default(DateTime) && entity.NgayHetHan != DateTime.MinValue)
            {
                // So sánh phần ngày (không tính giờ) của NgayHetHan với ngày hiện tại
                trangThaiThe = (entity.NgayHetHan.Date >= DateTime.Today) ? "Còn hạn" : "Hết hạn";
            }

            return new DocgiaDTO
            {
                Id = entity.Id,
                MaDocGia = entity.MaDocGia,
                TenDocGia = entity.TenDocGia,
                NgaySinh = entity.NgaySinh,
                DiaChi = entity.DiaChi,
                DienThoai = entity.DienThoai,
                Email = entity.Email,
                NgayLapThe = entity.NgayLapThe,
                NgayHetHan = entity.NgayHetHan, // Giữ nguyên giá trị gốc
                IdLoaiDocGia = entity.IdLoaiDocGia,
                TongNoHienTai = entity.TongNoHienTai, // Chú ý: Model là int, DTO là decimal? -> Đã map ở MapToDocgiaEntity
                IdNguoiDung = entity.IdNguoiDung,
                DaAn = entity.DaAn,
                TenLoaiDocGia = entity.IdLoaiDocGiaNavigation?.TenLoaiDocGia,
                TenDangNhap = entity.IdNguoiDungNavigation?.TenDangNhap,
                IdVaiTroNguoiDung = entity.IdNguoiDungNavigation?.IdNhomNguoiDung,
                MatKhauNguoiDung = null, // Không bao giờ map mật khẩu đã hash vào DTO
                TrangThaiThe = trangThaiThe, // << GÁN TRẠNG THÁI THẺ ĐÃ TÍNH
                // SoSachDangMuon = ... // Cần tính riêng nếu logic yêu cầu
            };
        }
        // ===== KẾT THÚC THAY ĐỔI =====

        private Docgia MapToDocgiaEntity(DocgiaDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            // NgaySinh trong Model là DateTime không null, nên dto.NgaySinh phải có giá trị
            if (!dto.NgaySinh.HasValue) throw new InvalidOperationException("Lỗi logic: NgaySinh DTO không được null khi map sang Entity Docgia.");
            // NgayLapThe và NgayHetHan trong Model là DateTime không null
            if (!dto.NgayLapThe.HasValue) throw new InvalidOperationException("Lỗi logic: NgayLapThe DTO không được null khi map sang Entity Docgia.");
            if (!dto.NgayHetHan.HasValue) throw new InvalidOperationException("Lỗi logic: NgayHetHan DTO không được null khi map sang Entity Docgia.");

            return new Docgia
            {
                Id = dto.Id,
                MaDocGia = dto.MaDocGia?.Trim(),
                TenDocGia = dto.TenDocGia.Trim(),
                NgaySinh = dto.NgaySinh.Value, // Gán trực tiếp vì Model không null
                DiaChi = dto.DiaChi?.Trim(),
                DienThoai = dto.DienThoai?.Trim(),
                Email = dto.Email?.Trim(),
                IdLoaiDocGia = dto.IdLoaiDocGia,
                NgayLapThe = dto.NgayLapThe.Value, // Gán trực tiếp
                NgayHetHan = dto.NgayHetHan.Value, // Gán trực tiếp
                // Chuyển đổi từ decimal? của DTO sang int của Model
                TongNoHienTai = dto.TongNoHienTai.HasValue ? Convert.ToInt32(dto.TongNoHienTai.Value) : 0,
                IdNguoiDung = dto.IdNguoiDung,
                DaAn = dto.DaAn
            };
        }

        // --- Validation Helper Method ---
        private async Task<(int MinAge, int MaxAge)> GetAgeRulesAsync() { try { int min = await _dalThamSo.GetMinimumReaderAgeAsync(); int max = await _dalThamSo.GetMaximumReaderAgeAsync(); return (min > 0 ? min : 18, max > 0 ? max : 55); } catch { return (18, 55); } }
        private void ValidateDocGia(DocgiaDTO dto, bool isAdding, int minAge, int maxAge)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto)); var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.TenDocGia)) errors.Add("Tên độc giả là bắt buộc.");
            if (!dto.NgaySinh.HasValue) errors.Add("Ngày sinh là bắt buộc."); else { /* Check age */ int age = DateTime.Today.Year - dto.NgaySinh.Value.Year; if (dto.NgaySinh.Value.Date > DateTime.Today.AddYears(-age)) age--; if (age < minAge || age > maxAge) errors.Add($"Tuổi phải từ {minAge}-{maxAge}."); }
            if (dto.IdLoaiDocGia <= 0) errors.Add("Loại độc giả là bắt buộc.");
            if (!dto.NgayLapThe.HasValue) errors.Add("Ngày lập thẻ là bắt buộc."); // Thêm validation
            if (!dto.NgayHetHan.HasValue) errors.Add("Ngày hết hạn là bắt buộc."); // Thêm validation
            if (dto.NgayLapThe.HasValue && dto.NgayHetHan.HasValue && dto.NgayLapThe.Value.Date > dto.NgayHetHan.Value.Date) errors.Add("Ngày hết hạn không được trước ngày lập thẻ.");
            if (!string.IsNullOrWhiteSpace(dto.Email) && !Regex.IsMatch(dto.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Email không hợp lệ.");
            if (!string.IsNullOrWhiteSpace(dto.DienThoai) && !Regex.IsMatch(dto.DienThoai.Trim(), @"^0[0-9]{9,10}$")) errors.Add("Số điện thoại không hợp lệ.");
            if (isAdding) { if (string.IsNullOrWhiteSpace(dto.TenDangNhap)) errors.Add("Tên đăng nhập là bắt buộc."); else if (dto.TenDangNhap.Trim().Length < 5) errors.Add("Tên đăng nhập >= 5 ký tự."); if (string.IsNullOrWhiteSpace(dto.MatKhauNguoiDung)) errors.Add("Mật khẩu là bắt buộc."); else if (dto.MatKhauNguoiDung.Length < 6) errors.Add("Mật khẩu >= 6 ký tự."); }
            else if (!string.IsNullOrWhiteSpace(dto.MatKhauNguoiDung) && dto.MatKhauNguoiDung.Length < 6) errors.Add("Mật khẩu mới >= 6 ký tự.");
            if (errors.Any()) throw new ArgumentException("Dữ liệu không hợp lệ:\n- " + string.Join("\n- ", errors));
        }
        private string GenerateRandomPassword(int length = 8) { const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"; var random = new Random(); return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()); }

    } // End class BUSDocGia
} // End namespace BUS