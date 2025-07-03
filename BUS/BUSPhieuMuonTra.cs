// File: BUS/BUSPhieuMuonTra.cs
// --- USING ---
using DAL; // Namespace chứa các lớp DAL (IDALPhieuMuonTra)
using DAL.Models; // Namespace chứa các Entity models (Phieumuontra)
using DTO; // Namespace chứa các DTO (PhieuMuonTraDTO)
using System; // Cần cho Exception, ArgumentNullException, DateTime, Convert
using System.Collections.Generic; // Cần cho List<T>
using System.Linq; // Cần cho LINQ
using System.Threading.Tasks; // Cần cho async/await Task
using System.Transactions; // Cần cho TransactionScope nếu dùng
using System.Diagnostics; // Cho Debug.WriteLine

namespace BUS
{
    /// <summary>
    /// Business Logic Layer cho Phiếu Mượn Trả.
    /// Xử lý nghiệp vụ liên quan đến phiếu mượn/trả sách.
    /// </summary>
    public class BUSPhieuMuonTra : IBUSPhieuMuonTra // <<< Implement interface
    {
        // --- DEPENDENCIES ---
        private readonly IDALPhieuMuonTra _dalPhieuMuonTra;
        private readonly IDALCuonSach _dalCuonSach; // <<< Cần inject nếu muốn cập nhật trạng thái sách
        private readonly IDALDocGia _dalDocGia;     // <<< Cần inject nếu muốn cập nhật tiền nợ độc giả
        private readonly IDALThamSo _dalThamSo;     // <<< Cần inject nếu muốn lấy quy định tính phạt

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public BUSPhieuMuonTra(
            IDALPhieuMuonTra dalPhieuMuonTra,
            IDALCuonSach dalCuonSach, // Nhận IDALCuonSach
            IDALDocGia dalDocGia,     // Nhận IDALDocGia
            IDALThamSo dalThamSo     // Nhận IDALThamSo
            )
        {
            _dalPhieuMuonTra = dalPhieuMuonTra ?? throw new ArgumentNullException(nameof(dalPhieuMuonTra));
            _dalCuonSach = dalCuonSach ?? throw new ArgumentNullException(nameof(dalCuonSach));
            _dalDocGia = dalDocGia ?? throw new ArgumentNullException(nameof(dalDocGia));
            _dalThamSo = dalThamSo ?? throw new ArgumentNullException(nameof(dalThamSo));
        }

        // --- METHODS ---

        public async Task<List<PhieuMuonTraDTO>> GetAllPhieuMuonTraAsync()
        {
            try
            {
                var entities = await _dalPhieuMuonTra.GetAllAsync();
                // Sử dụng Where để loại bỏ null trước khi Select nếu MapToPhieuMuonTraDTO có thể trả về null
                return entities.Select(e => MapToPhieuMuonTraDTO(e)!).Where(dto => dto != null).ToList();
            }
            catch (Exception ex) { Debug.WriteLine($"Error GetAllPhieuMuonTraAsync: {ex.Message}"); throw new Exception("Lỗi khi lấy danh sách phiếu mượn trả.", ex); }
        }

        public async Task<PhieuMuonTraDTO?> GetPhieuMuonTraByIdAsync(int soPhieuMuonTra)
        {
            try
            {
                var entity = await _dalPhieuMuonTra.GetByIdAsync(soPhieuMuonTra);
                return MapToPhieuMuonTraDTO(entity);
            }
            catch (Exception ex) { Debug.WriteLine($"Error GetPhieuMuonTraByIdAsync: {ex.Message}"); throw new Exception($"Lỗi khi lấy thông tin phiếu mượn trả (Số phiếu: {soPhieuMuonTra}).", ex); }
        }

        public async Task<List<PhieuMuonTraDTO>> GetLoansByDocgiaIdAsync(int idDocGia)
        {
            try
            {
                // Lấy các phiếu chưa trả của độc giả
                var entities = await _dalPhieuMuonTra.GetLoansByDocgiaIdAsync(idDocGia); // Giả định hàm này chỉ lấy phiếu chưa trả
                return entities.Select(e => MapToPhieuMuonTraDTO(e)!).Where(dto => dto != null).ToList();
            }
            catch (Exception ex) { Debug.WriteLine($"Error GetLoansByDocgiaIdAsync: {ex.Message}"); throw new Exception($"Lỗi khi lấy danh sách phiếu mượn trả của độc giả (ID: {idDocGia}).", ex); }
        }

        public async Task<List<PhieuMuonTraDTO>> GetOverdueLoansAsync()
        {
            try
            {
                var entities = await _dalPhieuMuonTra.GetOverdueLoansAsync(DateTime.Today);
                return entities.Select(e => MapToPhieuMuonTraDTO(e)!).Where(dto => dto != null).ToList();
            }
            catch (Exception ex) { Debug.WriteLine($"Error GetOverdueLoansAsync: {ex.Message}"); throw new Exception($"Lỗi khi lấy danh sách phiếu quá hạn.", ex); }
        }


        public async Task<int> GetBorrowedCountAsync()
        {
            try { return await _dalPhieuMuonTra.CountBorrowedLoansAsync(); }
            catch (Exception ex) { Debug.WriteLine($"Error GetBorrowedCountAsync: {ex.Message}"); throw new Exception("Lỗi khi đếm số sách đang mượn.", ex); }
        }

        public async Task<int> GetOverdueCountAsync()
        {
            try { return await _dalPhieuMuonTra.CountOverdueLoansAsync(DateTime.Today); }
            catch (Exception ex) { Debug.WriteLine($"Error GetOverdueCountAsync: {ex.Message}"); throw new Exception("Lỗi khi đếm số sách quá hạn.", ex); }
        }

        public async Task<PhieuMuonTraDTO?> AddPhieuMuonTraAsync(PhieuMuonTraDTO newPhieuMuonTraDto)
        {
            if (newPhieuMuonTraDto == null) throw new ArgumentNullException(nameof(newPhieuMuonTraDto));

            // --- Validation cơ bản ---
            if (newPhieuMuonTraDto.IdDocGia <= 0 || newPhieuMuonTraDto.IdCuonSach <= 0 || newPhieuMuonTraDto.NgayMuon == default || newPhieuMuonTraDto.HanTra == default)
            {
                throw new ArgumentException("Thiếu thông tin bắt buộc (Độc giả, Cuốn sách, Ngày mượn, Hạn trả).");
            }
            if (newPhieuMuonTraDto.NgayMuon.Date > newPhieuMuonTraDto.HanTra.Date)
            {
                throw new ArgumentException("Hạn trả không thể trước ngày mượn.");
            }

            // --- Validation nghiệp vụ ---
            try
            {
                // 1. Kiểm tra Độc giả
                var docGia = await _dalDocGia.GetByIdAsync(newPhieuMuonTraDto.IdDocGia);
                if (docGia == null || docGia.DaAn) // Kiểm tra độc giả tồn tại và không bị xóa mềm
                {
                    throw new InvalidOperationException($"Độc giả ID {newPhieuMuonTraDto.IdDocGia} không hợp lệ hoặc không tồn tại.");
                }
                // Dùng ?? để tránh lỗi nếu NgayHetHan là null (mặc dù không nên)
                // Cần kiểm tra NgayHetHan có giá trị không trước khi so sánh
                if (docGia.NgayHetHan != default(DateTime) && docGia.NgayHetHan < DateTime.Today)
                {
                    throw new InvalidOperationException($"Thẻ độc giả ID {newPhieuMuonTraDto.IdDocGia} đã hết hạn.");
                }

                // 2. Kiểm tra Số sách đang mượn của độc giả so với quy định
                var thamSoEntity = await _dalThamSo.GetThamSoAsync();
                if (thamSoEntity == null) throw new InvalidOperationException("Không đọc được tham số hệ thống.");

                var currentLoans = await _dalPhieuMuonTra.GetLoansByDocgiaIdAsync(newPhieuMuonTraDto.IdDocGia);
                int sachDangMuon = currentLoans.Count;

                // Đảm bảo tên thuộc tính 'SoSachMuonToiDa' trong model 'Thamso' khớp
                if (sachDangMuon >= thamSoEntity.SoSachMuonToiDa)
                {
                    throw new InvalidOperationException($"Độc giả đã mượn tối đa {thamSoEntity.SoSachMuonToiDa} cuốn sách.");
                }

                // 3. Kiểm tra Cuốn sách
                var cuonSach = await _dalCuonSach.GetByIdAsync(newPhieuMuonTraDto.IdCuonSach);
                if (cuonSach == null)
                {
                    throw new InvalidOperationException($"Cuốn sách ID {newPhieuMuonTraDto.IdCuonSach} không tồn tại.");
                }
                if (cuonSach.TinhTrang != 0)
                {
                    string tenSach = cuonSach.IdSachNavigation?.IdTuaSachNavigation?.TenTuaSach ?? $"ID {cuonSach.Id}";
                    string tenTrangThai = GetTenTinhTrang(cuonSach.TinhTrang);
                    throw new InvalidOperationException($"Cuốn sách '{tenSach}' hiện không có sẵn để mượn (Trạng thái: {tenTrangThai}).");
                }

                // 4. Kiểm tra Độc giả có sách mượn quá hạn không (nếu có quy định)
                // Phần này vẫn được comment out vì phụ thuộc vào schema và quy định nghiệp vụ
                /*
                if (thamSoEntity.KiemTraDocGiaQuaHanKhiMuon) // <<< Cần kiểm tra tham số này tồn tại
                {
                    // Cần phương thức CountOverdueLoansByDocGiaIdAsync trong IDALPhieuMuonTra
                    int overdueCount = await _dalPhieuMuonTra.CountOverdueLoansByDocGiaIdAsync(newPhieuMuonTraDto.IdDocGia, DateTime.Today); // <<< Cần hàm này
                    if (overdueCount > 0)
                    {
                        throw new InvalidOperationException("Độc giả đang có sách mượn quá hạn, không thể mượn thêm.");
                    }
                }
                */

                // --- Mapping & Add ---
                var entity = MapToPhieuMuonTraEntity(newPhieuMuonTraDto);
                entity.NgayTra = null; // Luôn null khi mới mượn
                entity.SoTienPhat = null; // Luôn null khi mới mượn

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var addedEntity = await _dalPhieuMuonTra.AddAsync(entity);
                    if (addedEntity == null) throw new Exception("Không thể thêm phiếu mượn trả.");

                    // Cập nhật trạng thái cuốn sách thành "Đã cho mượn" (1)
                    cuonSach.TinhTrang = 1;
                    bool sachUpdated = await _dalCuonSach.UpdateAsync(cuonSach);
                    if (!sachUpdated) throw new InvalidOperationException("Không thể cập nhật trạng thái cuốn sách.");

                    scope.Complete();

                    // Trả về DTO đầy đủ thông tin hơn
                    var finalDto = await GetPhieuMuonTraByIdAsync(addedEntity.SoPhieuMuonTra);
                    // Nếu GetByIdAsync không thành công (hiếm khi xảy ra), trả về DTO ban đầu với ID
                    return finalDto ?? MapToPhieuMuonTraDTO(addedEntity);
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error adding PhieuMuonTra: {ex.Message}"); if (ex is ArgumentException || ex is InvalidOperationException) throw; throw new Exception($"Lỗi khi thêm phiếu mượn trả.", ex); }
        }

        // --- PHƯƠNG THỨC XỬ LÝ TRẢ SÁCH ---
        public async Task<bool> ProcessReturnAsync(int soPhieuMuonTra)
        {
            if (soPhieuMuonTra <= 0) throw new ArgumentException("Số phiếu mượn không hợp lệ.", nameof(soPhieuMuonTra));

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Lấy phiếu mượn đang tồn tại
                    var existingPhieu = await _dalPhieuMuonTra.GetByIdAsync(soPhieuMuonTra);
                    if (existingPhieu == null) throw new KeyNotFoundException($"Không tìm thấy phiếu mượn số {soPhieuMuonTra}.");

                    // KIỂM TRA PHIẾU ĐÃ TRẢ CHƯA (Quan trọng!)
                    // Giả định Entity Phieumuontra CÓ thuộc tính `NgayTra` kiểu `DateTime?`
                    if (existingPhieu.NgayTra != null)
                    {
                        throw new InvalidOperationException($"Phiếu mượn số {soPhieuMuonTra} đã được trả vào ngày {existingPhieu.NgayTra.Value.ToString("dd/MM/yyyy HH:mm")}.");
                    }
                    // Nếu dùng trường DaTra (bool): if (existingPhieu.DaTra) throw new InvalidOperationException("Phiếu này đã được trả trước đó.");

                    // Lấy thông tin tham số và cuốn sách
                    var thamSoEntity = await _dalThamSo.GetThamSoAsync();
                    if (thamSoEntity == null) throw new InvalidOperationException("Không đọc được tham số hệ thống.");
                    var cuonSach = await _dalCuonSach.GetByIdAsync(existingPhieu.IdCuonSach);

                    // --- Tính tiền phạt ---
                    int soNgayQuaHan = 0;
                    int? soTienPhat = null; // Dùng int? để khớp với Model
                    DateTime ngayTraThucTe = DateTime.Now;

                    // Đảm bảo HanTra có giá trị hợp lệ trước khi so sánh
                    if (existingPhieu.HanTra != default(DateTime) && ngayTraThucTe.Date > existingPhieu.HanTra.Date)
                    {
                        soNgayQuaHan = (int)(ngayTraThucTe.Date - existingPhieu.HanTra.Date).TotalDays;
                        // Đảm bảo DonGiaPhat có trong Thamso
                        soTienPhat = soNgayQuaHan * (thamSoEntity.DonGiaPhat); // Tính phạt
                    }

                    // --- Cập nhật Entity Phieumuontra ---
                    existingPhieu.NgayTra = ngayTraThucTe; // Gán ngày trả
                    existingPhieu.SoTienPhat = soTienPhat; // Gán tiền phạt (có thể là null)

                    bool phieuUpdated = await _dalPhieuMuonTra.UpdateAsync(existingPhieu);
                    if (!phieuUpdated) throw new InvalidOperationException("Lỗi khi cập nhật thông tin phiếu mượn.");

                    // --- Cập nhật trạng thái CuonSach ---
                    if (cuonSach != null)
                    {
                        cuonSach.TinhTrang = 0; // Cập nhật trạng thái sách (0 = Sẵn sàng)
                        bool sachUpdated = await _dalCuonSach.UpdateAsync(cuonSach);
                        if (!sachUpdated) throw new InvalidOperationException("Lỗi khi cập nhật trạng thái cuốn sách.");
                    }
                    else { Debug.WriteLine($"Warning: Cuốn sách ID {existingPhieu.IdCuonSach} không tìm thấy để cập nhật trạng thái."); }

                    // --- Cập nhật Tổng nợ Độc giả ---
                    if (soTienPhat.HasValue && soTienPhat.Value > 0)
                    {
                        var docGia = await _dalDocGia.GetByIdAsync(existingPhieu.IdDocGia);
                        if (docGia != null)
                        {
                            // Đảm bảo TongNoHienTai trong model Docgia là int
                            docGia.TongNoHienTai = docGia.TongNoHienTai + soTienPhat.Value; // Cộng dồn tiền phạt
                            bool docGiaUpdated = await _dalDocGia.UpdateAsync(docGia); // Cần UpdateAsync trong IDALDocGia
                            if (!docGiaUpdated) throw new InvalidOperationException("Lỗi khi cập nhật tiền nợ cho độc giả.");
                        }
                        else { Debug.WriteLine($"Warning: Độc giả ID {existingPhieu.IdDocGia} không tìm thấy để cập nhật tiền nợ."); }
                    }

                    scope.Complete(); // Hoàn thành transaction
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing return for PhieuMuonTra (SoPhieu: {soPhieuMuonTra}): {ex}");
                    throw; // Ném lại để GUI có thể bắt và hiển thị lỗi
                }
            }
        }
        // --- KẾT THÚC PHẦN SỬA ---

        public async Task<List<PhieuMuonTraDTO>> GetHistoryByDocGiaIdAsync(int idDocGia)
        {
            if (idDocGia <= 0)
            {
                throw new ArgumentException("ID độc giả không hợp lệ.", nameof(idDocGia));
            }
            try
            {
                // <<< GỌI PHƯƠNG THỨC ĐÚNG CỦA DAL >>>
                var entities = await _dalPhieuMuonTra.GetHistoryByDocGiaIdAsync(idDocGia);
                return entities.Select(e => MapToPhieuMuonTraDTO(e)!).Where(dto => dto != null).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error GetHistoryByDocGiaIdAsync (BUS - ID DocGia: {idDocGia}): {ex.Message}");
                throw new Exception($"Lỗi khi lấy lịch sử mượn trả của độc giả (ID: {idDocGia}). Vui lòng thử lại.", ex);
            }
        }

        // Lấy lịch sử các phiếu đã phát sinh phạt
        public async Task<List<PhieuMuonTraDTO>> GetFineHistoryByDocGiaIdAsync(int idDocGia)
        {
            if (idDocGia <= 0)
            {
                throw new ArgumentException("ID độc giả không hợp lệ.", nameof(idDocGia));
            }
            try
            {
                // <<< GỌI PHƯƠNG THỨC ĐÚNG CỦA DAL (đã thêm ở bước trước) >>>
                var entities = await _dalPhieuMuonTra.GetFineHistoryByDocGiaIdAsync(idDocGia);
                return entities.Select(e => MapToPhieuMuonTraDTO(e)!).Where(dto => dto != null).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error GetFineHistoryByDocGiaIdAsync (BUS - ID DocGia: {idDocGia}): {ex.Message}");
                throw new Exception($"Lỗi khi lấy lịch sử phạt của độc giả (ID: {idDocGia}).", ex);
            }
        }


        public async Task<bool> HardDeletePhieuMuonTraAsync(int soPhieuMuonTra)
        {
            if (soPhieuMuonTra <= 0) throw new ArgumentException("Số phiếu không hợp lệ.", nameof(soPhieuMuonTra));

            try
            {
                // TODO: Kiểm tra nghiệp vụ trước khi xóa (vd: đã thu phạt?)
                var existingPhieu = await _dalPhieuMuonTra.GetByIdAsync(soPhieuMuonTra);
                if (existingPhieu == null) return false;

                // KIỂM TRA ĐÃ TRẢ CHƯA
                if (existingPhieu.NgayTra == null)
                {
                    throw new InvalidOperationException($"Không thể xóa phiếu mượn số {soPhieuMuonTra} vì chưa được trả.");
                }

                return await _dalPhieuMuonTra.HardDeleteAsync(soPhieuMuonTra);
            }
            catch (Exception ex) { Debug.WriteLine($"Error HardDeletePhieuMuonTraAsync: {ex.Message}"); throw new Exception($"Lỗi khi xóa vĩnh viễn phiếu mượn trả (Số phiếu: {soPhieuMuonTra}).", ex); }
        }

        // --- Mapping Methods ---
        private PhieuMuonTraDTO? MapToPhieuMuonTraDTO(Phieumuontra? entity)
        {
            if (entity == null) return null;
            return new PhieuMuonTraDTO
            {
                SoPhieuMuonTra = entity.SoPhieuMuonTra,
                IdDocGia = entity.IdDocGia,
                IdCuonSach = entity.IdCuonSach,
                NgayMuon = entity.NgayMuon,
                NgayTra = entity.NgayTra, // Giả định thuộc tính này tồn tại
                HanTra = entity.HanTra,
                SoTienPhat = entity.SoTienPhat, // Giả định thuộc tính này tồn tại
                MaDocGia = entity.IdDocGiaNavigation?.MaDocGia,
                TenDocGia = entity.IdDocGiaNavigation?.TenDocGia,
                MaCuonSach = entity.IdCuonSachNavigation?.MaCuonSach,
                TenTuaSach = entity.IdCuonSachNavigation?.IdSachNavigation?.IdTuaSachNavigation?.TenTuaSach
            };
        }
        private Phieumuontra MapToPhieuMuonTraEntity(PhieuMuonTraDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new Phieumuontra
            {
                SoPhieuMuonTra = dto.SoPhieuMuonTra,
                IdDocGia = dto.IdDocGia,
                IdCuonSach = dto.IdCuonSach,
                NgayMuon = dto.NgayMuon,
                NgayTra = dto.NgayTra, // Có thể là null
                HanTra = dto.HanTra,
                SoTienPhat = dto.SoTienPhat // Có thể là null
            };
        }

        private string GetTenTinhTrang(int tinhTrangCode)
        {
            switch (tinhTrangCode)
            {
                case 0: return "Sẵn sàng";
                case 1: return "Đã cho mượn";
                // Thêm các case khác nếu có (ví dụ: 2: Đang bảo trì, 3: Mất,...)
                default: return $"Không xác định ({tinhTrangCode})";
            }
        }

    } // End class BUSPhieuMuonTra
} // End namespace BUS