using DAL;
// Không cần using DAL.Models; nếu dùng tên đầy đủ ở mọi nơi
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class BUSNhaXuatBan : IBUSNhaXuatBan
    {
        private readonly IDALNhaXuatBan _dalNhaXuatBan;

        public BUSNhaXuatBan(IDALNhaXuatBan dalNhaXuatBan)
        {
            _dalNhaXuatBan = dalNhaXuatBan;
        }

        // *** LƯU Ý ***
        // Đoạn mã dưới đây sử dụng tên đầy đủ "DAL.Models.NhaXuatBan"
        // để tránh lỗi CS0433 do tồn tại tệp DTO/NhaXuatBan.cs.
        // Cách tốt nhất vẫn là xóa tệp DTO/NhaXuatBan.cs đi.

        // Hàm ánh xạ từ Entity (DAL.Models.NhaXuatBan) sang DTO (DTO.NhaXuatBanDTO)
        private NhaXuatBanDTO MapToDTO(DAL.Models.NhaXuatBan nxb) // <-- Sửa: Dùng tên đầy đủ
        {
            if (nxb == null)
            {
                return null;
            }
            return new NhaXuatBanDTO
            {
                Id = nxb.Id,
                TenNXB = nxb.TenNXB,
                DiaChi = nxb.DiaChi,
                DienThoai = nxb.DienThoai,
                Email = nxb.Email // <-- Thêm ánh xạ Email
            };
        }

        // Hàm ánh xạ từ DTO (DTO.NhaXuatBanDTO) sang Entity (DAL.Models.NhaXuatBan)
        private DAL.Models.NhaXuatBan MapToEntity(NhaXuatBanDTO dto) // <-- Sửa: Dùng tên đầy đủ
        {
            if (dto == null)
            {
                return null;
            }
            // Trả về kiểu DAL.Models.NhaXuatBan
            return new DAL.Models.NhaXuatBan // <-- Sửa: Dùng tên đầy đủ
            {
                Id = dto.Id,
                TenNXB = dto.TenNXB,
                DiaChi = dto.DiaChi,
                DienThoai = dto.DienThoai,
                Email = dto.Email // <-- Thêm ánh xạ Email
            };
        }

        public List<NhaXuatBanDTO> GetAll()
        {
            // _dalNhaXuatBan.GetAll() trả về List<DAL.Models.NhaXuatBan>
            var nxbList = _dalNhaXuatBan.GetAll();
            // Sử dụng hàm MapToDTO đã cập nhật
            return nxbList.Select(nxb => MapToDTO(nxb)).Where(dto => dto != null).ToList();
        }

        public NhaXuatBanDTO GetById(int id)
        {
            // _dalNhaXuatBan.GetById(id) trả về DAL.Models.NhaXuatBan
            var nxb = _dalNhaXuatBan.GetById(id);
            // Sử dụng hàm MapToDTO đã cập nhật
            return MapToDTO(nxb);
        }

        public string AddNhaXuatBan(NhaXuatBanDTO nxbDto)
        {
            if (nxbDto == null)
            {
                return "Dữ liệu nhà xuất bản không hợp lệ.";
            }
            if (string.IsNullOrWhiteSpace(nxbDto.TenNXB))
            {
                return "Tên nhà xuất bản không được để trống.";
            }

            // Thêm validation cho Email và Điện thoại ở đây nếu muốn (ngoài UI)
            // Ví dụ: Kiểm tra định dạng cơ bản
            // if (!string.IsNullOrEmpty(nxbDto.Email) && !IsValidEmail(nxbDto.Email)) // Cần hàm IsValidEmail
            // {
            //     return "Định dạng email không hợp lệ.";
            // }
            // if (!string.IsNullOrEmpty(nxbDto.DienThoai) && !IsValidPhoneNumber(nxbDto.DienThoai)) // Cần hàm IsValidPhoneNumber
            // {
            //     return "Định dạng số điện thoại không hợp lệ.";
            // }


            // MapToEntity trả về DAL.Models.NhaXuatBan (đã bao gồm Email)
            var nxbEntity = MapToEntity(nxbDto);
            if (nxbEntity == null)
            {
                return "Không thể chuyển đổi dữ liệu nhà xuất bản.";
            }

            // _dalNhaXuatBan.Add chấp nhận tham số kiểu DAL.Models.NhaXuatBan
            if (_dalNhaXuatBan.Add(nxbEntity))
            {
                return "Thêm nhà xuất bản thành công.";
            }
            else
            {
                return "Thêm nhà xuất bản thất bại. Có lỗi xảy ra trong quá trình lưu.";
            }
        }

        public string UpdateNhaXuatBan(NhaXuatBanDTO nxbDto)
        {
            if (nxbDto == null)
            {
                return "Dữ liệu nhà xuất bản không hợp lệ.";
            }
            if (string.IsNullOrWhiteSpace(nxbDto.TenNXB))
            {
                return "Tên nhà xuất bản không được để trống.";
            }

            // Thêm validation cho Email và Điện thoại ở đây nếu muốn (ngoài UI)
            // Ví dụ:
            // if (!string.IsNullOrEmpty(nxbDto.Email) && !IsValidEmail(nxbDto.Email))
            // {
            //     return "Định dạng email không hợp lệ.";
            // }
            // if (!string.IsNullOrEmpty(nxbDto.DienThoai) && !IsValidPhoneNumber(nxbDto.DienThoai))
            // {
            //     return "Định dạng số điện thoại không hợp lệ.";
            // }

            // _dalNhaXuatBan.GetById trả về DAL.Models.NhaXuatBan
            var existingNxb = _dalNhaXuatBan.GetById(nxbDto.Id);
            if (existingNxb == null)
            {
                return "Không tìm thấy nhà xuất bản để cập nhật.";
            }

            // MapToEntity trả về DAL.Models.NhaXuatBan (đã bao gồm Email)
            var nxbEntityToUpdate = MapToEntity(nxbDto);
            if (nxbEntityToUpdate == null)
            {
                return "Không thể chuyển đổi dữ liệu nhà xuất bản.";
            }
            nxbEntityToUpdate.Id = nxbDto.Id; // Đảm bảo Id đúng

            // _dalNhaXuatBan.Update chấp nhận tham số kiểu DAL.Models.NhaXuatBan
            if (_dalNhaXuatBan.Update(nxbEntityToUpdate))
            {
                return "Cập nhật nhà xuất bản thành công.";
            }
            else
            {
                return "Cập nhật nhà xuất bản thất bại. Có lỗi xảy ra trong quá trình lưu.";
            }
        }

        public string DeleteNhaXuatBan(int id)
        {
            // _dalNhaXuatBan.GetById trả về DAL.Models.NhaXuatBan
            var existingNxb = _dalNhaXuatBan.GetById(id);
            if (existingNxb == null)
            {
                return "Không tìm thấy nhà xuất bản để xóa.";
            }

            // Kiểm tra ràng buộc khóa ngoại trước khi xóa (nếu cần)
            // Ví dụ: Kiểm tra xem có Sách nào thuộc NXB này không
            // if (_dalSach.Any(s => s.IdNxb == id)) // Giả sử có _dalSach
            // {
            //     return "Không thể xóa nhà xuất bản vì đang có sách thuộc nhà xuất bản này.";
            // }

            if (_dalNhaXuatBan.Delete(id))
            {
                return "Xóa nhà xuất bản thành công.";
            }
            else
            {
                // Có thể thêm log lỗi chi tiết hơn ở đây
                return "Xóa nhà xuất bản thất bại. Có lỗi xảy ra.";
            }
        }

        public List<NhaXuatBanDTO> SearchByName(string name)
        {
            // _dalNhaXuatBan.SearchByName trả về List<DAL.Models.NhaXuatBan>
            var nxbList = _dalNhaXuatBan.SearchByName(name);
            // Sử dụng hàm MapToDTO đã cập nhật
            return nxbList.Select(nxb => MapToDTO(nxb)).Where(dto => dto != null).ToList();
        }

        // (Optional) Helper methods for validation if needed in BUS layer
        // private bool IsValidEmail(string email)
        // {
        //     try
        //     {
        //         var addr = new System.Net.Mail.MailAddress(email);
        //         return addr.Address == email;
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        // private bool IsValidPhoneNumber(string number)
        // {
        //     // Add specific phone number validation logic here if needed
        //     // For example, using Regex similar to the UI layer
        //     string phonePattern = @"^(|(\+84|0)\d{9,10})$";
        //     return System.Text.RegularExpressions.Regex.IsMatch(number, phonePattern);
        // }
    }
}
