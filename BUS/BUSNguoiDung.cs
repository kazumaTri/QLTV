// Project/Namespace: BUS
using DAL;
using DTO;
using DAL.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BUS.Utilities;
using System.Diagnostics; // Cần cho Debug.WriteLine

namespace BUS
{
    public class BUSNguoiDung : IBUSNguoiDung
    {
        private readonly IDALNguoiDung _dalNguoiDung;
        private readonly IPasswordHasherService _passwordHasherService;

        public BUSNguoiDung(IDALNguoiDung dalNguoiDung, IPasswordHasherService passwordHasherService)
        {
            _dalNguoiDung = dalNguoiDung ?? throw new ArgumentNullException(nameof(dalNguoiDung));
            _passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
        }

        // --- MAPPERS ---
        // Giữ nguyên các hàm MapToDTO và MapToEntity của bạn
        private NguoiDungDTO? MapToDTO(Nguoidung? entity)
        {
            if (entity == null) return null;
            return new NguoiDungDTO
            {
                Id = entity.Id,
                MaNguoiDung = entity.MaNguoiDung,
                TenDangNhap = entity.TenDangNhap,
                TenHienThi = entity.TenNguoiDung,
                IdNhomNguoiDung = entity.IdNhomNguoiDung, // int -> int? OK
                // Lấy tên nhóm từ đối tượng được include bởi DAL
                TenNhomNguoiDung = entity.IdNhomNguoiDungNavigation?.TenNhomNguoiDung, // 
                NgaySinh = entity.NgaySinh, // DateTime? -> DateTime? OK
                ChucVu = entity.ChucVu
                // Không trả về mật khẩu hash
            };
        }

        private Nguoidung MapToEntity(NguoiDungDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new Nguoidung
            {
                Id = dto.Id, // Id có thể là 0 nếu là thêm mới
                MaNguoiDung = dto.MaNguoiDung,
                TenDangNhap = dto.TenDangNhap.Trim(),
                TenNguoiDung = dto.TenHienThi,
                IdNhomNguoiDung = dto.IdNhomNguoiDung ?? 0, // Cần đảm bảo 0 là giá trị hợp lệ hoặc xử lý khác
                NgaySinh = dto.NgaySinh,
                ChucVu = dto.ChucVu,
                // MatKhau sẽ được gán riêng sau khi hash
                DaAn = false // Mặc định là false khi map từ DTO (trừ khi DTO có trường này)
            };
        }

        // --- BUSINESS LOGIC ---

        // Phương thức Đăng nhập (Giữ nguyên logic của bạn)
        public async Task<NguoiDungDTO?> DangNhapAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return null;
            try
            {
                // *** DÒNG QUAN TRỌNG: Gọi DAL để lấy thông tin người dùng từ DB ***
                var userEntity = await _dalNguoiDung.GetByUsernameAsync(username); // Lấy user chưa bị xóa (DaAn = false) 
                if (userEntity == null)
                {
                    Debug.WriteLine($"Đăng nhập thất bại: Không tìm thấy người dùng '{username}' hoặc đã bị xóa.");
                    return null;
                }

                // Xác thực mật khẩu bằng dịch vụ hash
                // Quan trọng: userEntity.MatKhau phải là HASH từ CSDL
                // *** DÒNG QUAN TRỌNG: Gọi dịch vụ PasswordHasherService để kiểm tra mật khẩu ***
                bool isValid = _passwordHasherService.VerifyPassword(password, userEntity.MatKhau); // 

                if (!isValid)
                {
                    Debug.WriteLine($"Đăng nhập thất bại: Mật khẩu không đúng cho người dùng '{username}'.");
                    return null;
                }

                Debug.WriteLine($"Đăng nhập thành công cho người dùng: '{username}'.");
                // *** DÒNG QUAN TRỌNG: Trả về DTO chứa thông tin người dùng (bao gồm vai trò) nếu hợp lệ ***
                return MapToDTO(userEntity); // MapToDTO sẽ lấy TenNhomNguoiDung từ userEntity.IdNhomNguoiDungNavigation 
            }
            // Bắt lỗi cụ thể nếu có thể (ví dụ: lỗi hash, lỗi DB)
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi nghiêm trọng khi đăng nhập cho '{username}': {ex}");
                // Không nên throw lỗi ra GUI, chỉ log và trả về null
                return null;
            }
        }

        // Lấy tất cả người dùng (Giữ nguyên logic của bạn)
        public async Task<List<NguoiDungDTO>> GetAllNguoiDungAsync()
        {
            // Giả định DAL.GetAllAsync() trả về List<Nguoidung> đã Include NhomNguoiDung và chỉ lấy user chưa xóa
            var userEntities = await _dalNguoiDung.GetAllAsync(includeDeleted: false); // Chỉ lấy user chưa xóa
            return userEntities.Select(u => MapToDTO(u)!).Where(dto => dto != null).ToList();
        }

        // Thêm người dùng mới (Giữ nguyên logic hash của bạn)
        public async Task<NguoiDungDTO?> AddNguoiDungAsync(NguoiDungDTO nguoiDungDto, string rawPassword)
        {
            // --- Kiểm tra đầu vào ---
            if (nguoiDungDto == null) throw new ArgumentNullException(nameof(nguoiDungDto));
            if (string.IsNullOrWhiteSpace(rawPassword)) throw new ArgumentException("Mật khẩu không được để trống.", nameof(rawPassword));
            if (string.IsNullOrWhiteSpace(nguoiDungDto.TenDangNhap)) throw new ArgumentException("Tên đăng nhập không được để trống.", nameof(nguoiDungDto.TenDangNhap));
            if (nguoiDungDto.IdNhomNguoiDung == null || nguoiDungDto.IdNhomNguoiDung <= 0) throw new ArgumentException("Nhóm người dùng không hợp lệ.", nameof(nguoiDungDto.IdNhomNguoiDung));
            // Thêm các kiểm tra khác nếu cần (độ dài tên, ký tự đặc biệt...)

            // --- Xử lý ---
            try
            {
                // 1. Hash mật khẩu
                string hashedPassword = _passwordHasherService.HashPassword(rawPassword);

                // 2. Map DTO sang Entity
                var nguoiDungEntity = MapToEntity(nguoiDungDto);
                nguoiDungEntity.MatKhau = hashedPassword; // Gán mật khẩu đã hash
                nguoiDungEntity.DaAn = false; // Mặc định khi thêm mới là chưa xóa

                // 3. Kiểm tra tên đăng nhập đã tồn tại chưa (kể cả đã xóa mềm)
                var existing = await _dalNguoiDung.GetByUsernameIncludingDeletedAsync(nguoiDungEntity.TenDangNhap);
                if (existing != null)
                {
                    // Có thể trả về null hoặc throw lỗi tùy nghiệp vụ
                    Debug.WriteLine($"Thêm thất bại: Tên đăng nhập '{nguoiDungEntity.TenDangNhap}' đã tồn tại.");
                    throw new InvalidOperationException($"Tên đăng nhập '{nguoiDungEntity.TenDangNhap}' đã tồn tại.");
                }

                // 4. Gọi DAL để thêm vào CSDL
                var addedUser = await _dalNguoiDung.AddAsync(nguoiDungEntity);

                // 5. Map kết quả trả về DTO
                return MapToDTO(addedUser);
            }
            catch (InvalidOperationException) { throw; } // Ném lại lỗi tên đăng nhập trùng
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi thêm người dùng mới '{nguoiDungDto.TenDangNhap}': {ex}");
                // Có thể ném lại lỗi hoặc trả về null tùy xử lý ở GUI
                // throw; // Hoặc
                return null;
            }
        }

        // Cập nhật thông tin người dùng (Không cập nhật mật khẩu) (Giữ nguyên logic của bạn)
        public async Task<bool> UpdateNguoiDungAsync(NguoiDungDTO nguoiDungDto)
        {
            // --- Kiểm tra đầu vào ---
            if (nguoiDungDto == null) throw new ArgumentNullException(nameof(nguoiDungDto));
            if (nguoiDungDto.Id <= 0) throw new ArgumentException("ID người dùng không hợp lệ để cập nhật.", nameof(nguoiDungDto.Id));
            if (string.IsNullOrWhiteSpace(nguoiDungDto.TenDangNhap)) throw new ArgumentException("Tên đăng nhập không được để trống.", nameof(nguoiDungDto.TenDangNhap)); // Dù không sửa nhưng vẫn cần validate
            if (nguoiDungDto.IdNhomNguoiDung == null || nguoiDungDto.IdNhomNguoiDung <= 0) throw new ArgumentException("Nhóm người dùng không hợp lệ.", nameof(nguoiDungDto.IdNhomNguoiDung));
            // Thêm các kiểm tra khác nếu cần

            // --- Xử lý ---
            try
            {
                // 1. Lấy người dùng hiện tại từ CSDL (chỉ lấy user chưa bị xóa)
                var existingUser = await _dalNguoiDung.GetByIdAsync(nguoiDungDto.Id, includeDeleted: false);
                if (existingUser == null)
                {
                    Debug.WriteLine($"Cập nhật thất bại: Không tìm thấy người dùng ID {nguoiDungDto.Id} hoặc đã bị xóa.");
                    return false; // Không tìm thấy user để cập nhật
                }

                // 2. Cập nhật các trường thông tin được phép thay đổi
                //    KHÔNG cập nhật TenDangNhap và MatKhau ở đây.
                existingUser.MaNguoiDung = nguoiDungDto.MaNguoiDung; // Cập nhật mã nếu cần
                existingUser.TenNguoiDung = nguoiDungDto.TenHienThi;
                existingUser.IdNhomNguoiDung = nguoiDungDto.IdNhomNguoiDung.Value; // Đã kiểm tra not null
                existingUser.NgaySinh = nguoiDungDto.NgaySinh;
                existingUser.ChucVu = nguoiDungDto.ChucVu;
                // existingUser.DaAn = nguoiDungDto.DaAn; // Việc xóa/khôi phục nên có hàm riêng

                // 3. Gọi DAL để lưu thay đổi
                return await _dalNguoiDung.UpdateAsync(existingUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi cập nhật thông tin người dùng ID {nguoiDungDto.Id}: {ex}");
                // throw; // Hoặc
                return false;
            }
        }

        // Cập nhật mật khẩu (Giữ nguyên logic hash của bạn)
        public async Task<bool> UpdatePasswordAsync(int idNguoiDung, string rawPassword)
        {
            // --- Kiểm tra đầu vào ---
            if (idNguoiDung <= 0) throw new ArgumentException("ID người dùng không hợp lệ.", nameof(idNguoiDung));
            if (string.IsNullOrWhiteSpace(rawPassword)) throw new ArgumentException("Mật khẩu mới không được để trống.", nameof(rawPassword));
            // Thêm kiểm tra độ mạnh mật khẩu nếu cần

            // --- Xử lý ---
            try
            {
                // 1. Hash mật khẩu mới
                string hashedPassword = _passwordHasherService.HashPassword(rawPassword);

                // 2. Lấy người dùng hiện tại (chỉ user chưa bị xóa)
                var existingUser = await _dalNguoiDung.GetByIdAsync(idNguoiDung, includeDeleted: false);
                if (existingUser == null)
                {
                    Debug.WriteLine($"Cập nhật mật khẩu thất bại: Không tìm thấy người dùng ID {idNguoiDung} hoặc đã bị xóa.");
                    return false;
                }

                // 3. Cập nhật mật khẩu đã hash vào entity
                existingUser.MatKhau = hashedPassword;

                // 4. Gọi phương thức UpdateAsync chung để lưu thay đổi
                bool updateResult = await _dalNguoiDung.UpdateAsync(existingUser);

                return updateResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi cập nhật mật khẩu cho người dùng ID {idNguoiDung}: {ex}");
                // throw; // Hoặc
                return false;
            }
        }

        // === PHƯƠNG THỨC ĐẶT LẠI MẬT KHẨU MỚI THÊM VÀO ===
        public async Task<bool> DatLaiMatKhauAsync(string tenDangNhap, string matKhauMoi)
        {
            // Kiểm tra đầu vào cơ bản
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhauMoi))
            {
                Debug.WriteLine("Đặt lại MK thất bại: Tên đăng nhập hoặc mật khẩu mới không hợp lệ.");
                return false;
            }
            // Thêm kiểm tra độ mạnh mật khẩu nếu cần

            try
            {
                // 1. Tìm người dùng bằng tên đăng nhập.
                //    Quan trọng: Tìm cả user đã bị ẩn để cho phép họ đặt lại MK và kích hoạt lại tài khoản.
                var userEntity = await _dalNguoiDung.GetByUsernameIncludingDeletedAsync(tenDangNhap);

                if (userEntity == null)
                {
                    Debug.WriteLine($"Đặt lại MK thất bại: Không tìm thấy người dùng với tên đăng nhập: {tenDangNhap}.");
                    return false; // Không tìm thấy người dùng
                }

                // 2. Hash mật khẩu mới bằng dịch vụ BCryptPasswordHasherService hiện tại (v4.0.3)
                string hashedNewPassword = _passwordHasherService.HashPassword(matKhauMoi);

                // 3. Cập nhật mật khẩu mới (đã hash) cho đối tượng người dùng
                userEntity.MatKhau = hashedNewPassword;

                // 4. Kích hoạt lại tài khoản nếu đang bị ẩn (quan trọng khi đặt lại mật khẩu)
                userEntity.DaAn = false;

                // 5. Lưu thay đổi vào cơ sở dữ liệu thông qua DAL
                //    Hàm UpdateAsync của DAL cần đảm bảo lưu cả thay đổi của MatKhau và DaAn
                bool updateResult = await _dalNguoiDung.UpdateAsync(userEntity);

                if (!updateResult)
                {
                    Debug.WriteLine($"Đặt lại MK: Không thể cập nhật CSDL cho người dùng: {tenDangNhap}");
                }
                else
                {
                    Debug.WriteLine($"Đặt lại MK thành công cho người dùng: {tenDangNhap}");
                }
                return updateResult;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                Debug.WriteLine($"Lỗi nghiêm trọng khi đặt lại mật khẩu cho {tenDangNhap}: {ex}");
                return false; // Không ném lỗi ra GUI, chỉ trả về false
            }
        }
        // ==================================================

        // Xóa người dùng (Hiện đang gọi SoftDelete, cần xem lại logic nếu muốn xóa cứng)
        public async Task<bool> HardDeleteNguoiDungAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID người dùng không hợp lệ.", nameof(id));

            try
            {
                // TODO: Thêm kiểm tra ràng buộc ở đây nếu cần (ví dụ: không cho xóa admin cuối cùng)

                // Hiện tại đang gọi SoftDeleteAsync từ DAL.
                // Nếu thực sự muốn xóa cứng (Hard Delete), cần gọi phương thức HardDeleteAsync tương ứng trong DAL (nếu có)
                // hoặc đổi tên phương thức này thành SoftDeleteNguoiDungAsync cho rõ ràng.
                // Giữ nguyên theo code cũ:
                bool result = await _dalNguoiDung.SoftDeleteAsync(id);
                if (!result) Debug.WriteLine($"Xóa (mềm) người dùng ID {id} thất bại hoặc không tìm thấy.");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi xóa (mềm) người dùng ID {id}: {ex}");
                // throw; // Hoặc
                return false;
            }
        }

        // Có thể bạn cần thêm các hàm RestoreNguoiDungAsync nếu DAL có hỗ trợ
        // public async Task<bool> RestoreNguoiDungAsync(int id)
        // {
        //     if (id <= 0) throw new ArgumentException("ID người dùng không hợp lệ.", nameof(id));
        //     try
        //     {
        //         return await _dalNguoiDung.RestoreAsync(id);
        //     }
        //     catch (Exception ex)
        //     {
        //          Debug.WriteLine($"Lỗi khi khôi phục người dùng ID {id}: {ex}");
        //          return false;
        //     }
        // }

    } // End class BUSNguoiDung
} // End namespace BUS