// Project/Namespace: BUS

// --- USING DIRECTIVES ---
using BUS; // Cần cho interface IBUSSach mà class này implement
using DAL; // Cần cho interface IDALSach (và IDALCuonSach, IDALTuaSach nếu cần validation sâu)
using DAL.Models; // Cần cho Entity Sach, Cuonsach (để mapping)
using DTO; // Cần cho SachDTO, TuaSachDTO, TacGiaDTO, TheloaiDTO (để mapping và nhận/trả dữ liệu)
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException, DateTime
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (Select, ToList, Any, Sum, Where)
using System.Threading.Tasks; // Cần cho async/await Task


namespace BUS // Namespace của Business Logic Layer
{
    /// <summary>
    /// Business Logic Layer triển khai IBUSSach, xử lý nghiệp vụ cho Sách (ấn bản).
    /// Phụ thuộc vào IDALSach và có thể các DAL khác để validation nghiệp vụ.
    /// Thực hiện mapping DTO/Entity và validation nghiệp vụ.
    /// </summary>
    public class BUSSach : IBUSSach // <<< Implement interface IBUSSach
    {
        // --- DEPENDENCIES ---
        // BUS phụ thuộc vào DAL thông qua interface IDALSach
        private readonly IDALSach _dalSach;
        // Cần thêm các DAL khác để thực hiện validation nghiệp vụ
        private readonly IDALCuonSach _dalCuonSach; // Cần để kiểm tra ràng buộc khi xóa Hard Delete
        // Có thể cần IDALTuaSach nếu muốn kiểm tra TuaSach có tồn tại khi thêm/sửa Sach
        // private readonly IDALTuaSach _dalTuaSach; // <<< Nếu cần check TuaSach tồn tại

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        // Constructor nhận các DAL dependencies qua interface
        public BUSSach(IDALSach dalSach, IDALCuonSach dalCuonSach /*, IDALTuaSach dalTuaSach*/) // Nhận IDALs qua DI
        {
            _dalSach = dalSach ?? throw new ArgumentNullException(nameof(dalSach));
            _dalCuonSach = dalCuonSach ?? throw new ArgumentNullException(nameof(dalCuonSach)); // Gán dependency mới
            // _dalTuaSach = dalTuaSach ?? throw new ArgumentNullException(nameof(dalTuaSach)); // Gán dependency nếu cần
        }

        // --- PRIVATE HELPER METHODS: MAPPING BETWEEN ENTITY AND DTO ---

        /// <summary>
        /// Ánh xạ từ Entity Sach (có kèm navigation properties) sang DTO SachDTO.
        /// Đảm bảo trả về các giá trị mặc định (string.Empty, new List) thay vì null cho các thuộc tính có thể null.
        /// </summary>
        /// <param name="entity">Đối tượng Entity Sach (có thể đã Include các navigation property).</param>
        /// <returns>Đối tượng SachDTO tương ứng hoặc null nếu entity là null.</returns>
        private SachDTO? MapToSachDTO(Sach? entity) // Cho phép entity là null
        {
            // Kiểm tra null trước khi mapping
            if (entity == null) return null;

            var dto = new SachDTO
            {
                Id = entity.Id,
                MaSach = entity.MaSach ?? string.Empty, // Đảm bảo không trả về null cho string DTO
                IdTuaSach = entity.IdTuaSach,
                SoLuong = entity.SoLuong,
                SoLuongConLai = entity.SoLuongConLai,
                DonGia = entity.DonGia,
                NamXb = entity.NamXb,
                NhaXb = entity.NhaXb ?? string.Empty, // Đảm bảo không trả về null cho string DTO
                DaAn = entity.DaAn == 1, // Mapping int (0/1) to bool (false/true)

                // Lấy thông tin từ navigation properties nếu đã được Include ở DAL
                // Sử dụng null-conditional operator (?.) và null-coalescing operator (??) để an toàn
                TenTuaSach = entity.IdTuaSachNavigation?.TenTuaSach ?? string.Empty, // Tên Tựa sách
                TenTheLoai = entity.IdTuaSachNavigation?.IdTheLoaiNavigation?.TenTheLoai ?? string.Empty, // Tên Thể loại
                // Lấy danh sách tên tác giả (từ collection IdTacGia trong TuaSachNavigation)
                // Kiểm tra IdTuaSachNavigation trước khi truy cập IdTacGia
                TenTacGia = entity.IdTuaSachNavigation?.IdTacGia?.Select(tg => tg.TenTacGia ?? string.Empty).ToList() ?? new List<string>() // Đảm bảo tên tác giả cũng không null
            };

            // Giả định: Nếu muốn hiển thị 1 string TenTacGia thay vì List<string>
            // dto.TenTacGiaString = string.Join(", ", dto.TenTacGia);

            return dto;
        }

        /// <summary>
        /// Ánh xạ từ DTO SachDTO sang Entity Sach (cho mục đích thêm/cập nhật).
        /// Trim khoảng trắng cho các trường string.
        /// </summary>
        /// <param name="dto">Đối tượng SachDTO.</param>
        /// <returns>Đối tượng Entity Sach tương ứng.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu dto là null.</exception>
        // Sửa trong file: BUS/BUSSach.cs

        // Sửa trong file: BUS/BUSSach.cs

        private Sach MapToSachEntity(SachDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Sach
            {
                Id = dto.Id,
                MaSach = dto.MaSach?.Trim(),
                IdTuaSach = dto.IdTuaSach,
                SoLuong = dto.SoLuong,
                SoLuongConLai = dto.SoLuongConLai,
                // SỬA ĐỔI Ở ĐÂY: Chuyển đổi tường minh từ decimal sang int
                DonGia = Convert.ToInt32(dto.DonGia), // <<< SỬA DÒNG NÀY
                NamXb = dto.NamXb,
                NhaXb = dto.NhaXb?.Trim(),
                // DaAn không map ở đây khi update thông thường
            };
        }


        // --- METHOD IMPLEMENTATIONS (Triển khai các phương thức từ IBUSSach) ---

        /// <summary>
        /// Lấy tất cả sách DTO chưa bị xóa mềm. Bao gồm thông tin Tựa sách, Thể loại, Tác giả.
        /// </summary>
        /// <returns>Task chứa danh sách SachDTO.</returns>
        public async Task<List<SachDTO>> GetAllSachAsync()
        {
            try
            {
                // Gọi DAL để lấy danh sách Entity (DAL đã filter DaAn=0 và Include navigation properties)
                var entities = await _dalSach.GetAllAsync();

                // Kiểm tra kết quả từ DAL có null không trước khi Select
                if (entities == null)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: DAL.GetAllAsync for Sach returned null. Returning empty list.");
                    return new List<SachDTO>(); // Trả về danh sách rỗng thay vì null
                }

                // Mapping danh sách Entity sang danh sách DTO và trả về
                // Sử dụng Select và gọi MapToSachDTO cho từng entity.
                // Thêm .Where(dto => dto != null) để phòng trường hợp MapToSachDTO trả về null (mặc dù code hiện tại không làm vậy)
                return entities.Select(e => MapToSachDTO(e)).Where(dto => dto != null).ToList()!; // Filter out potential null DTOs and ensure non-nullability
            }
            catch (Exception ex) // Bắt lỗi từ tầng DAL hoặc lỗi hệ thống khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.GetAllSachAsync: {ex.ToString()}");
                // Ném lại lỗi để tầng GUI xử lý/hiển thị thông báo lỗi phù hợp
                throw new Exception("Lỗi khi tải danh sách sách.", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin một sách DTO theo ID, chỉ các sách chưa bị xóa mềm.
        /// </summary>
        /// <param name="id">ID của sách.</param>
        /// <returns>Task chứa SachDTO hoặc null nếu không tìm thấy hoặc ID không hợp lệ.</returns>
        public async Task<SachDTO?> GetSachByIdAsync(int id)
        {
            // Validation ID cơ bản
            if (id <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: BUSSach.GetSachByIdAsync received invalid ID: {id}");
                return null; // ID không hợp lệ
            }

            try
            {
                // Gọi DAL để lấy Entity theo ID (DAL đã filter DaAn=0 và Include navigation properties)
                var entity = await _dalSach.GetByIdAsync(id);
                // Mapping Entity sang DTO và trả về (sẽ trả về null nếu entity là null)
                return MapToSachDTO(entity);
            }
            catch (Exception ex) // Bắt lỗi từ tầng DAL hoặc lỗi hệ thống khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.GetSachByIdAsync (ID: {id}): {ex.ToString()}");
                // Ném lại lỗi
                throw new Exception($"Lỗi khi lấy thông tin sách theo ID {id}.", ex);
            }
        }

        /// <summary>
        /// Lấy tổng số sách (ví dụ: số lượng còn lại) hoặc tổng số ấn bản sách.
        /// (Cần làm rõ logic GetTotalCountAsync ở DAL và đây)
        /// </summary>
        /// <returns>Task chứa tổng số lượng.</returns>
        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                // Gọi hàm tương ứng từ DAL
                // Giả định DAL.GetTotalCountAsync() trả về tổng số ấn bản sách chưa bị xóa mềm.
                return await _dalSach.GetTotalCountAsync();
            }
            catch (Exception ex) // Bắt lỗi từ tầng DAL hoặc lỗi hệ thống khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.GetTotalCountAsync: {ex.ToString()}");
                // Ném lại lỗi
                throw new Exception("Lỗi khi lấy tổng số lượng sách.", ex);
            }
        }


        /// <summary>
        /// Thêm mới Sách (ấn bản) vào hệ thống VỚI SỐ LƯỢNG BAN ĐẦU.
        /// Thường được gọi bởi các quy trình nghiệp vụ phức tạp hơn (ví dụ: Nhập sách nếu tạo sách mới).
        /// Hàm này **YÊU CẦU** SoLuong > 0.
        /// </summary>
        /// <param name="sachDto">DTO chứa thông tin sách cần thêm (bao gồm SoLuong > 0).</param>
        /// <returns>Task chứa DTO của sách đã được thêm vào DB (có ID và MaSach tự sinh nếu có) hoặc null nếu thêm thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu sachDto là null.</exception>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu trong sachDto không hợp lệ theo nghiệp vụ (bao gồm SoLuong <= 0).</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu thao tác không hợp lệ (ví dụ: trùng mã sách).</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<SachDTO?> AddSachAsync(SachDTO sachDto) // Trả về DTO của sách đã thêm
        {
            // Kiểm tra null DTO đầu vào
            if (sachDto == null) throw new ArgumentNullException(nameof(sachDto), "Thông tin sách cần thêm không được rỗng.");

            // *** BUSINESS VALIDATION (Validation nghiệp vụ) ***

            // 1. Kiểm tra IdTuaSach hợp lệ và tồn tại
            if (sachDto.IdTuaSach <= 0) throw new ArgumentException("Vui lòng chọn tựa sách hợp lệ.", nameof(sachDto.IdTuaSach));
            // TODO: Có thể gọi DAL.IDALTuaSach để kiểm tra TuaSach có tồn tại và chưa bị xóa mềm không nếu cần validation chặt chẽ hơn

            // 2. Kiểm tra Nhà xuất bản không được trống
            if (string.IsNullOrWhiteSpace(sachDto.NhaXb)) throw new ArgumentException("Nhà xuất bản không được để trống.", nameof(sachDto.NhaXb));

            // 3. Kiểm tra Năm xuất bản hợp lệ
            if (sachDto.NamXb <= 1000 || sachDto.NamXb > DateTime.Now.Year) // Năm phải lớn hơn 1000 và không quá năm hiện tại
            {
                throw new ArgumentException($"Năm xuất bản không hợp lệ. Vui lòng nhập số nguyên từ 1001 đến {DateTime.Now.Year}.", nameof(sachDto.NamXb));
            }

            // 4. Kiểm tra Số lượng nhập ban đầu (Số lượng cuốn sách vật lý khi nhập kho lần đầu)
            // *** ĐÂY LÀ HÀM YÊU CẦU SỐ LƯỢNG > 0 ***
            if (sachDto.SoLuong <= 0) throw new ArgumentException("Số lượng nhập kho ban đầu phải lớn hơn 0.", nameof(sachDto.SoLuong));

            // 5. Kiểm tra Đơn giá
            if (sachDto.DonGia < 0) throw new ArgumentException("Đơn giá không được âm.", nameof(sachDto.DonGia));
            // Nếu đơn giá 0 hợp lệ, có thể bỏ check này hoặc thêm điều kiện.

            // 6. Kiểm tra trùng Mã ấn bản (nếu Mã không tự sinh ở DAL/DB)
            // Nếu MaSach là Computed Column tự sinh ở DB, bỏ qua validation này ở BUS khi thêm.
            // Nếu MaSach là trường do người dùng nhập hoặc tự sinh ở BUS, thì cần validate trùng.
            if (!string.IsNullOrWhiteSpace(sachDto.MaSach)) // Chỉ validate nếu Mã sách được cung cấp từ DTO
            {
                bool maExists = await _dalSach.IsMaSachExistsAsync(sachDto.MaSach.Trim()); // Giả định DAL có hàm kiểm tra tồn tại mã (cho cả sách đã xóa mềm hoặc chưa)
                if (maExists)
                {
                    throw new InvalidOperationException($"Mã sách '{sachDto.MaSach.Trim()}' đã tồn tại.");
                }
            }
            // Nếu MaSach tự sinh ở DB, thì không cần gửi MaSach trong DTO khi thêm, hoặc gửi empty/null.


            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Logic tính toán SoLuongConLai ban đầu khi thêm mới
                // Khi thêm mới, số lượng còn lại bằng tổng số lượng nhập kho ban đầu
                sachDto.SoLuongConLai = sachDto.SoLuong;
                // sachDto.DaAn = false; // Mặc định khi thêm là chưa xóa mềm (DaAn = 0)

                // Mapping DTO sang Entity để truyền xuống DAL
                var entityToAdd = new Sach // Map thủ công để kiểm soát chính xác các trường
                {
                    // ID sẽ được DB tự sinh (Identity Column)
                    MaSach = sachDto.MaSach?.Trim(), // MaSach có thể null/empty nếu tự sinh ở DB
                    IdTuaSach = sachDto.IdTuaSach,
                    SoLuong = sachDto.SoLuong,
                    SoLuongConLai = sachDto.SoLuongConLai, // Sử dụng giá trị đã tính toán ở BUS
                    DonGia = sachDto.DonGia,
                    NamXb = sachDto.NamXb,
                    NhaXb = sachDto.NhaXb?.Trim(),
                    DaAn = 0 // Sử dụng giá trị 0 (false) cho entity khi thêm mới
                };


                // Gọi DAL để thêm vào DB
                var addedEntity = await _dalSach.AddAsync(entityToAdd); // Giả định DAL AddAsync nhận Entity và trả về Entity đã thêm (có ID)

                // Nếu thêm thành công, cần lấy lại entity đầy đủ thông tin từ DB (có ID và MaSach computed column nếu có)
                // trước khi mapping lại sang DTO và trả về.
                if (addedEntity != null)
                {
                    // Giả định DAL có hàm GetByIdIncludingDeletedAsync để lấy Entity theo ID,
                    // bao gồm cả các navigation properties và cột computed (như MaSach)
                    var resultEntity = await _dalSach.GetByIdIncludingDeletedAsync(addedEntity.Id);
                    return MapToSachDTO(resultEntity); // Sẽ trả về DTO đầy đủ thông tin mới
                }
                return null; // Thêm thất bại (DAL trả về null hoặc lỗi không ném exception)

            }
            catch (ArgumentException) { throw; } // Ném lại lỗi validation đã bắt ở trên
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ đã bắt ở trên
            catch (Exception ex) // Bắt các lỗi khác từ DAL (DbUpdateException, etc.)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.AddSachAsync: {ex.ToString()}");
                // Wrap lại exception để cung cấp thông báo chung hơn cho tầng trên
                throw new Exception($"Lỗi hệ thống khi thêm mới sách: {ex.Message}", ex);
            }
        }

        // *** PHƯƠNG THỨC MỚI ĐỂ THÊM CHỈ METADATA (Số lượng = 0) ***
        /// <summary>
        /// Thêm mới thông tin cơ bản (metadata) của Sách (ấn bản) vào hệ thống.
        /// KHÔNG yêu cầu số lượng ban đầu (SoLuong và SoLuongConLai sẽ được đặt là 0).
        /// Dùng cho màn hình Quản lý Sách để tạo bản ghi trước khi nhập kho.
        /// </summary>
        /// <param name="sachDto">DTO chứa thông tin sách cần thêm (không cần SoLuong).</param>
        /// <returns>Task chứa DTO của sách đã được thêm (có ID) hoặc null nếu thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu sachDto là null.</exception>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu metadata (Tựa Sách, NXB, Năm XB,...) không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu trùng mã sách (nếu có kiểm tra).</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<SachDTO?> AddSachMetadataAsync(SachDTO sachDto)
        {
            // Kiểm tra null DTO đầu vào
            if (sachDto == null) throw new ArgumentNullException(nameof(sachDto), "Thông tin sách cần thêm không được rỗng.");

            // *** BUSINESS VALIDATION (Chỉ kiểm tra metadata) ***

            // 1. Kiểm tra IdTuaSach hợp lệ và tồn tại
            if (sachDto.IdTuaSach <= 0) throw new ArgumentException("Vui lòng chọn tựa sách hợp lệ.", nameof(sachDto.IdTuaSach));
            // TODO: Check TuaSach exists in DB if needed

            // 2. Kiểm tra Nhà xuất bản không được trống
            if (string.IsNullOrWhiteSpace(sachDto.NhaXb)) throw new ArgumentException("Nhà xuất bản không được để trống.", nameof(sachDto.NhaXb));

            // 3. Kiểm tra Năm xuất bản hợp lệ
            if (sachDto.NamXb <= 1000 || sachDto.NamXb > DateTime.Now.Year)
            {
                throw new ArgumentException($"Năm xuất bản không hợp lệ. Vui lòng nhập số nguyên từ 1001 đến {DateTime.Now.Year}.", nameof(sachDto.NamXb));
            }

            // 4. Kiểm tra Đơn giá
            if (sachDto.DonGia < 0) throw new ArgumentException("Đơn giá không được âm.", nameof(sachDto.DonGia));

            // 5. Kiểm tra trùng Mã ấn bản (nếu cần)
            if (!string.IsNullOrWhiteSpace(sachDto.MaSach))
            {
                bool maExists = await _dalSach.IsMaSachExistsAsync(sachDto.MaSach.Trim());
                if (maExists)
                {
                    throw new InvalidOperationException($"Mã sách '{sachDto.MaSach.Trim()}' đã tồn tại.");
                }
            }

            // *** KHÔNG KIỂM TRA SoLuong > 0 Ở ĐÂY ***

            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Mapping DTO sang Entity, đặt SoLuong và SoLuongConLai là 0
                var entityToAdd = new Sach
                {
                    MaSach = sachDto.MaSach?.Trim(),
                    IdTuaSach = sachDto.IdTuaSach,
                    SoLuong = 0, // *** Đặt là 0 ***
                    SoLuongConLai = 0, // *** Đặt là 0 ***
                    DonGia = sachDto.DonGia,
                    NamXb = sachDto.NamXb,
                    NhaXb = sachDto.NhaXb?.Trim(),
                    DaAn = 0 // Mặc định khi thêm là chưa xóa mềm
                };

                // Gọi DAL để thêm vào DB
                var addedEntity = await _dalSach.AddAsync(entityToAdd);

                // Trả về DTO nếu thành công
                if (addedEntity != null)
                {
                    var resultEntity = await _dalSach.GetByIdIncludingDeletedAsync(addedEntity.Id);
                    return MapToSachDTO(resultEntity);
                }
                return null;
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.AddSachMetadataAsync: {ex.ToString()}");
                throw new Exception($"Lỗi hệ thống khi thêm thông tin sách: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Cập nhật thông tin Sách (ấn bản).
        /// Thực hiện validation nghiệp vụ và kiểm tra các ràng buộc.
        /// </summary>
        /// <param name="sachDto">DTO chứa thông tin sách cần cập nhật (phải có ID hợp lệ).</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công, false nếu thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu sachDto là null.</exception>
        /// <exception cref="ArgumentException">Ném ra nếu ID sách không hợp lệ hoặc dữ liệu khác không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu thao tác không hợp lệ (ví dụ: trùng mã sách, số lượng không hợp lệ).</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<bool> UpdateSachAsync(SachDTO sachDto)
        {
            // Kiểm tra DTO đầu vào và ID hợp lệ
            if (sachDto == null) throw new ArgumentNullException(nameof(sachDto), "Thông tin sách cần cập nhật không được rỗng.");
            if (sachDto.Id <= 0) throw new ArgumentException("ID sách không hợp lệ để cập nhật.", nameof(sachDto.Id));

            // *** BUSINESS VALIDATION (Validation nghiệp vụ) ***

            // 1. Kiểm tra sách có tồn tại không? (Cần lấy sách gốc từ DB)
            var originalEntity = await _dalSach.GetByIdIncludingDeletedAsync(sachDto.Id); // Lấy cả sách đã xóa mềm để kiểm tra tồn tại
            if (originalEntity == null)
            {
                throw new ArgumentException($"Không tìm thấy sách với ID {sachDto.Id} để cập nhật.");
            }
            // Có thể thêm kiểm tra sách có bị xóa mềm không nếu không cho phép cập nhật sách đã xóa mềm
            // if (originalEntity.DaAn == 1) throw new InvalidOperationException($"Không thể cập nhật sách đã bị xóa mềm (ID: {sachDto.Id}).");


            // 2. Kiểm tra IdTuaSach hợp lệ và tồn tại (nếu thay đổi)
            if (sachDto.IdTuaSach <= 0) throw new ArgumentException("Vui lòng chọn tựa sách hợp lệ.", nameof(sachDto.IdTuaSach));
            // TODO: Có thể gọi DAL.IDALTuaSach để kiểm tra TuaSach có tồn tại và chưa bị xóa mềm không nếu cần validation chặt chẽ hơn


            // 3. Kiểm tra Nhà xuất bản không được trống
            if (string.IsNullOrWhiteSpace(sachDto.NhaXb)) throw new ArgumentException("Nhà xuất bản không được để trống.", nameof(sachDto.NhaXb));

            // 4. Kiểm tra Năm xuất bản hợp lệ
            if (sachDto.NamXb <= 1000 || sachDto.NamXb > DateTime.Now.Year)
            {
                throw new ArgumentException($"Năm xuất bản không hợp lệ. Vui lòng nhập số nguyên từ 1001 đến {DateTime.Now.Year}.", nameof(sachDto.NamXb));
            }

            // 5. Kiểm tra Số lượng và Số lượng còn lại (Nghiệp vụ phức tạp)
            // *** LƯU Ý QUAN TRỌNG KHI UPDATE ***
            // Màn hình ucQuanLySach hiện tại không có ô nhập SoLuong, SoLuongConLai khi sửa.
            // Do đó, khi gọi UpdateSachAsync từ ucQuanLySach, sachDto sẽ không chứa giá trị SoLuong, SoLuongConLai mới.
            // => Việc validation SoLuong, SoLuongConLai ở đây có thể không cần thiết hoặc cần logic khác.
            // => Khi mapping entityToUpdate, NÊN LẤY GIÁ TRỊ SoLuong, SoLuongConLai TỪ originalEntity thay vì từ sachDto.

            // Giả sử bạn *không* cho phép sửa SoLuong, SoLuongConLai từ màn hình này:
            // Bỏ qua validation SoLuong, SoLuongConLai ở đây.

            // Nếu BẮT BUỘC phải validation (ví dụ: hàm Update này có thể được gọi từ nơi khác):
            /*
            if (sachDto.SoLuong < 0) throw new ArgumentException("Tổng số lượng không được âm.", nameof(sachDto.SoLuong));
            if (sachDto.SoLuongConLai < 0) throw new ArgumentException("Số lượng còn lại không được âm.", nameof(sachDto.SoLuongConLai));
            int soLuongDangMuon = await _dalCuonSach.GetSoLuongCuonSachDangMuonBySachIdAsync(sachDto.Id);
            if (sachDto.SoLuong < soLuongDangMuon) throw new InvalidOperationException($"Tổng số lượng ({sachDto.SoLuong}) không thể ít hơn số lượng đang mượn ({soLuongDangMuon}).");
            if (sachDto.SoLuongConLai > sachDto.SoLuong) throw new ArgumentException("Số lượng còn lại không thể lớn hơn tổng số lượng.", nameof(sachDto.SoLuongConLai));
            if (sachDto.SoLuongConLai < soLuongDangMuon) throw new InvalidOperationException($"Số lượng còn lại ({sachDto.SoLuongConLai}) không thể ít hơn số lượng đang mượn ({soLuongDangMuon}).");
            */

            // 6. Kiểm tra Đơn giá
            if (sachDto.DonGia < 0) throw new ArgumentException("Đơn giá không được âm.", nameof(sachDto.DonGia));


            // 7. Kiểm tra trùng Mã ấn bản (loại trừ chính nó)
            // Nếu MaSach là Computed Column, người dùng không sửa, bỏ qua validation này.
            // Nếu MaSach có thể sửa và được gửi từ DTO:
            if (!string.IsNullOrWhiteSpace(sachDto.MaSach) && sachDto.MaSach.Trim() != originalEntity.MaSach) // Chỉ kiểm tra nếu Mã sách được gửi từ DTO và khác mã gốc
            {
                bool maExists = await _dalSach.IsMaSachExistsExcludingIdAsync(sachDto.MaSach.Trim(), sachDto.Id); // Giả định DAL có hàm kiểm tra tồn tại loại trừ ID
                if (maExists)
                {
                    throw new InvalidOperationException($"Mã sách '{sachDto.MaSach.Trim()}' đã tồn tại.");
                }
            }
            // Nếu MaSach không thể sửa từ UI, thì khi mapping entityToUpdate, lấy MaSach từ originalEntity.


            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Mapping DTO sang Entity CẦN CẬP NHẬT.
                // DAL sẽ lấy Entity gốc từ DB, cập nhật các thuộc tính từ entityToUpdate, rồi mới SaveChanges.
                var entityToUpdate = new Sach // Map thủ công để kiểm soát chính xác các trường được phép cập nhật
                {
                    Id = sachDto.Id,
                    // MaSach: Sử dụng giá trị từ DTO nếu được phép sửa, nếu không lấy từ originalEntity
                    MaSach = sachDto.MaSach?.Trim(), // Giả định MaSach có thể sửa và đã validate
                    IdTuaSach = sachDto.IdTuaSach,

                    // *** Lấy SoLuong và SoLuongConLai từ bản ghi gốc trong DB ***
                    // *** vì ucQuanLySach không cung cấp giá trị mới cho các trường này ***
                    SoLuong = originalEntity.SoLuong,
                    SoLuongConLai = originalEntity.SoLuongConLai,

                    DonGia = sachDto.DonGia,
                    NamXb = sachDto.NamXb,
                    NhaXb = sachDto.NhaXb?.Trim(),
                    // DaAn KHÔNG được map từ DTO khi update thông thường.
                    DaAn = originalEntity.DaAn // Giữ nguyên trạng thái xóa mềm từ entity gốc
                };

                // Gọi DAL để cập nhật vào DB
                bool success = await _dalSach.UpdateAsync(entityToUpdate); // Giả định DAL UpdateAsync nhận Entity đã cập nhật các trường

                return success; // Trả về kết quả từ DAL
            }
            catch (ArgumentException) { throw; } // Ném lại lỗi validation
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ
            catch (Exception ex) // Bắt các lỗi khác từ DAL (DbUpdateException, etc.)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.UpdateSachAsync (ID: {sachDto.Id}): {ex.ToString()}");
                // Wrap lại exception để cung cấp thông báo chung hơn cho tầng trên
                throw new Exception($"Lỗi hệ thống khi cập nhật sách (ID: {sachDto.Id}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa mềm Sách (ấn bản) bằng cách đặt cờ DaAn = true.
        /// Thực hiện kiểm tra nghiệp vụ trước khi xóa mềm.
        /// </summary>
        /// <param name="id">ID của sách cần xóa mềm.</param>
        /// <returns>Task chứa bool, true nếu xóa mềm thành công, false nếu thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu ID không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu không thể xóa mềm do ràng buộc nghiệp vụ.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<bool> SoftDeleteSachAsync(int id)
        {
            // Validation ID cơ bản
            if (id <= 0) throw new ArgumentException("ID sách không hợp lệ để xóa mềm.", nameof(id));

            // *** BUSINESS VALIDATION: Kiểm tra ràng buộc nghiệp vụ trước khi xóa mềm ***
            // Ví dụ: Không cho xóa mềm nếu còn cuốn sách vật lý nào thuộc ấn bản này đang "Có sẵn" hoặc "Đang mượn".
            // Đây là ràng buộc quan trọng để đảm bảo tính toàn vẹn dữ liệu khi quản lý mượn trả.
            // Cần gọi DAL để kiểm tra số lượng cuốn sách vật lý có trạng thái mượn/có sẵn.
            bool hasActiveCopies = await _dalCuonSach.HasActiveCopiesAsync(id); // Gọi phương thức từ IDALCuonSach

            if (hasActiveCopies)
            {
                throw new InvalidOperationException($"Không thể xóa mềm ấn bản sách (ID: {id}) vì còn tồn tại các cuốn sách vật lý đang có sẵn hoặc đang được mượn.");
            }

            // Có thể thêm kiểm tra các ràng buộc khác nếu có (ví dụ: sách có trong phiếu mượn chưa kết thúc, vv - tùy mô hình DB)


            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Gọi DAL để thực hiện xóa mềm (set DaAn = true)
                bool success = await _dalSach.SoftDeleteAsync(id); // Giả định DAL có hàm SoftDeleteAsync nhận ID

                // Thêm logic nghiệp vụ sau khi xóa mềm nếu cần (ví dụ: log lịch sử thao tác)

                return success; // Trả về kết quả từ DAL
            }
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ tự ném ra
            catch (Exception ex) // Bắt các lỗi khác từ DAL (ví dụ: DbUpdateException nếu có FK cứng chưa được xử lý)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.SoftDeleteSachAsync (ID: {id}): {ex.ToString()}");
                // Wrap lại exception
                throw new Exception($"Lỗi hệ thống khi xóa mềm sách (ID: {id}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phục hồi Sách (ấn bản) đã xóa mềm bằng cách đặt cờ DaAn = false.
        /// Thực hiện kiểm tra nghiệp vụ trước khi phục hồi.
        /// </summary>
        /// <param name="id">ID của sách cần phục hồi.</param>
        /// <returns>Task chứa bool, true nếu phục hồi thành công, false nếu thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu ID không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu không thể phục hồi do ràng buộc nghiệp vụ.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<bool> RestoreSachAsync(int id)
        {
            // Validation ID cơ bản
            if (id <= 0) throw new ArgumentException("ID sách không hợp lệ để phục hồi.", nameof(id));

            // *** BUSINESS VALIDATION: Kiểm tra ràng buộc nghiệp vụ trước khi phục hồi ***
            // Ví dụ: Có thể cần kiểm tra xem Tựa sách liên quan có đang bị xóa mềm không? (tùy quy định)
            // Nếu TuaSach bị xóa mềm, có thể không cho phục hồi Sach thuộc TuaSach đó, hoặc cảnh báo.
            // ...

            // Kiểm tra xem sách có tồn tại và đang bị xóa mềm không
            var entity = await _dalSach.GetByIdIncludingDeletedAsync(id);
            if (entity == null) throw new ArgumentException($"Không tìm thấy sách với ID {id} để phục hồi.");
            // Kiểm tra DaAn kiểu int với 0
            if (entity.DaAn == 0)
            {
                throw new InvalidOperationException($"Sách (ID: {id}) hiện không ở trạng thái xóa mềm.");
            }


            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Gọi DAL để thực hiện phục hồi (set DaAn = false)
                bool success = await _dalSach.RestoreAsync(id); // Giả định DAL có hàm RestoreAsync nhận ID

                // Thêm logic nghiệp vụ sau khi phục hồi nếu cần

                return success; // Trả về kết quả từ DAL
            }
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ tự ném ra
            catch (Exception ex) // Bắt lỗi từ DAL
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.RestoreSachAsync (ID: {id}): {ex.ToString()}");
                // Wrap lại exception
                throw new Exception($"Lỗi hệ thống khi phục hồi sách (ID: {id}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra xem một Sách (ấn bản) có thể xóa vĩnh viễn được không (kiểm tra các ràng buộc cứng trong CSDL).
        /// Dùng để hỗ trợ UI hoặc validation trước khi Hard Delete THỰC SỰ.
        /// </summary>
        /// <param name="id">ID của Sách (ấn bản) cần kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu có thể xóa, false nếu ngược lại.</returns>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khi kiểm tra.</exception>
        public async Task<bool> CanHardDeleteSachAsync(int id)
        {
            // Validation ID cơ bản
            if (id <= 0) return false; // ID không hợp lệ

            try
            {
                // Logic nghiệp vụ: Chỉ có thể xóa vĩnh viễn ấn bản Sách nếu KHÔNG còn bất kỳ cuốn sách vật lý (CuonSach) nào thuộc ấn bản này,
                // BẤT KỂ trạng thái của cuốn sách vật lý (có sẵn, mượn, mất, đã xóa mềm CuonSach).
                // Cần gọi DAL.IDALCuonSach để kiểm tra sự tồn tại của bất kỳ CuonSach nào có IdSach = id trong bảng CUONSACH.
                bool hasAnyCuonSach = await _dalCuonSach.HasAnyCuonSachBySachIdIncludingDeletedAsync(id); // Gọi phương thức từ IDALCuonSach

                return !hasAnyCuonSach; // Trả về true nếu KHÔNG còn cuốn sách vật lý nào liên quan
            }
            catch (Exception ex) // Bắt lỗi từ tầng DAL hoặc lỗi hệ thống khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.CanHardDeleteSachAsync (ID: {id}): {ex.ToString()}");
                // Nếu có lỗi khi kiểm tra, ném exception để tầng GUI xử lý
                throw new Exception($"Lỗi hệ thống khi kiểm tra khả năng xóa vĩnh viễn sách (ID: {id}): {ex.Message}", ex);
                // Alternative: return false; // Trả về false một cách "im lặng" nếu có lỗi kiểm tra (ít thông tin cho người dùng)
            }
        }


        /// <summary>
        /// Xóa vĩnh viễn Sách (ấn bản) khỏi cơ sở dữ liệu.
        /// CHỈ THỰC HIỆN KHI ĐÃ CHẮC CHẮN KHÔNG CÒN BẤT KỲ CUỐN SÁCH VẬT LÝ NÀO LIÊN QUAN (đã kiểm tra bằng CanHardDeleteSachAsync).
        /// </summary>
        /// <param name="id">ID của sách cần xóa vĩnh viễn.</param>
        /// <returns>Task chứa bool, true nếu xóa thành công, false nếu thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu ID không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu không thể xóa vĩnh viễn do vẫn còn cuốn sách vật lý liên quan.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<bool> HardDeleteSachAsync(int id)
        {
            // Validation ID cơ bản
            if (id <= 0) throw new ArgumentException("ID sách không hợp lệ để xóa vĩnh viễn.", nameof(id));

            // *** BUSINESS VALIDATION: Kiểm tra ràng buộc nghiệp vụ trước khi xóa vĩnh viễn ***
            // Kiểm tra lại xem CÓ THỂ xóa vĩnh viễn không (không còn cuốn sách vật lý nào)
            // Việc kiểm tra này là cần thiết ở BUS để đảm bảo tính toàn vẹn nghiệp vụ. UI có thể gọi CanHardDeleteSachAsync trước
            bool canDelete = await CanHardDeleteSachAsync(id);
            if (!canDelete)
            {
                // Ném lỗi nghiệp vụ rõ ràng
                throw new InvalidOperationException($"Không thể xóa vĩnh viễn ấn bản sách (ID: {id}) vì còn tồn tại các cuốn sách vật lý thuộc ấn bản này.");
            }
            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Gọi DAL để thực hiện xóa vĩnh viễn bản ghi trong bảng Sach
                bool success = await _dalSach.HardDeleteAsync(id); // Giả định DAL có hàm HardDeleteAsync nhận ID

                // Thêm logic nghiệp vụ sau khi xóa vĩnh viễn nếu cần (ví dụ: log)

                return success; // Trả về kết quả từ DAL
            }
            // Bắt lại InvalidOperationException nếu DAL cũng ném lỗi ràng buộc (ví dụ: nếu DAL cũng kiểm tra lại số cuốn sách vật lý HOẶC có ràng buộc FK cứng khác chưa được kiểm tra ở BUS)
            catch (InvalidOperationException) { throw; }
            catch (Exception ex) // Bắt các lỗi khác từ DAL (ví dụ: lỗi DB không mong muốn, lỗi FK cứng CSDL nếu kiểm tra ở BUS chưa đủ)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSSach.HardDeleteSachAsync (ID: {id}): {ex.ToString()}");
                // Wrap lại exception
                throw new Exception($"Lỗi hệ thống khi xóa vĩnh viễn sách (ID: {id}): {ex.Message}", ex);
            }
        }


        // Bạn có thể thêm các phương thức nghiệp vụ khác nếu cần:
        // - SearchSachAsync(string keyword)
        // - GetSachByTuaSachIdAsync(int tuaSachId)
        // - GetSachCountByTuaSachIdAsync(int tuaSachId)
        // - ...
    }
}