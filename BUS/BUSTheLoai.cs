// File: BUS/BUSTheLoai.cs
using BUS;
using DAL;
using DAL.Models;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BUS
{
    public class BUSTheLoai : IBUSTheLoai
    {
        private readonly IDALTheLoai _dalTheLoai;
        private readonly IDALTuaSach _dalTuaSach;

        public BUSTheLoai(IDALTheLoai dalTheLoai, IDALTuaSach dalTuaSach)
        {
            _dalTheLoai = dalTheLoai ?? throw new ArgumentNullException(nameof(dalTheLoai));
            _dalTuaSach = dalTuaSach ?? throw new ArgumentNullException(nameof(dalTuaSach));
        }

        private TheLoaiDTO MapToTheLoaiDTO(Theloai entity)
        {
            if (entity == null) return null!;
            return new TheLoaiDTO
            {
                Id = entity.Id,
                MaTheLoai = entity.MaTheLoai,
                TenTheLoai = entity.TenTheLoai,
            };
        }

        private Theloai MapToTheLoaiEntity(TheLoaiDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new Theloai
            {
                Id = dto.Id,
                MaTheLoai = dto.MaTheLoai?.Trim(),
                TenTheLoai = dto.TenTheLoai?.Trim() ?? string.Empty
            };
        }

        public async Task<List<TheLoaiDTO>> GetAllTheLoaiAsync()
        {
            Debug.WriteLine(">>> BUS: Entering GetAllTheLoaiAsync");
            try
            {
                Debug.WriteLine(">>> BUS: Calling DAL GetAllAsync...");
                var entities = await _dalTheLoai.GetAllAsync();
                Debug.WriteLine($">>> BUS: DAL GetAllAsync returned {(entities == null ? "NULL" : $"{entities.Count} entities")}.");

                if (entities == null)
                {
                    Debug.WriteLine(">>> BUS: Entities list from DAL is null, returning empty DTO list.");
                    return new List<TheLoaiDTO>();
                }

                Debug.WriteLine(">>> BUS: Mapping entities to DTOs...");
                var dtos = entities
                            .Select(e => MapToTheLoaiDTO(e))
                            .Where(dto => dto != null) // <<< THÊM BƯỚC LỌC NULL DTO
                            .ToList();
                Debug.WriteLine($">>> BUS: Mapping complete. Returning {dtos.Count} DTOs.");
                return dtos;
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết hơn ở BUS trước khi ném lại
                Debug.WriteLine($"*** ERROR in BUSTheLoai.GetAllTheLoaiAsync: {ex.ToString()}");
                throw new Exception($"Lỗi khi lấy danh sách thể loại từ BUS: {ex.Message}", ex); // Wrap lỗi
            }
        }

        // ... (Các phương thức khác giữ nguyên như trong file bạn đã cung cấp)
        public async Task<TheLoaiDTO?> GetTheLoaiByIdAsync(int id) { if (id <= 0) return null; try { var entity = await _dalTheLoai.GetByIdAsync(id); return MapToTheLoaiDTO(entity); } catch (Exception ex) { Debug.WriteLine($"Error in BUSTheLoai.GetTheLoaiByIdAsync (ID: {id}): {ex.Message}"); throw; } }
        public async Task<TheLoaiDTO?> AddTheLoaiAsync(TheLoaiDTO theLoaiDto) { if (theLoaiDto == null) throw new ArgumentNullException(nameof(theLoaiDto)); if (string.IsNullOrWhiteSpace(theLoaiDto.TenTheLoai)) { throw new ArgumentException("Tên thể loại không được rỗng.", nameof(theLoaiDto.TenTheLoai)); } theLoaiDto.TenTheLoai = theLoaiDto.TenTheLoai.Trim(); bool tenExists = await _dalTheLoai.IsTenTheLoaiExistsAsync(theLoaiDto.TenTheLoai); if (tenExists) { throw new InvalidOperationException($"Tên thể loại '{theLoaiDto.TenTheLoai}' đã tồn tại."); } try { int maxNumber = await _dalTheLoai.GetMaxMaTheLoaiNumberAsync(); int nextNumber = maxNumber + 1; theLoaiDto.MaTheLoai = $"TL{nextNumber:D3}"; bool maGeneratedExists = await _dalTheLoai.MaTheLoaiExistsAsync(theLoaiDto.MaTheLoai); if (maGeneratedExists) { throw new InvalidOperationException($"Mã thể loại '{theLoaiDto.MaTheLoai}' được tạo tự động bị trùng. Vui lòng thử lại."); } Debug.WriteLine($"Generated MaTheLoai: {theLoaiDto.MaTheLoai}"); } catch (Exception ex) { Debug.WriteLine($"Lỗi khi tạo mã thể loại tự động: {ex.Message}"); throw new InvalidOperationException("Không thể tạo mã thể loại tự động.", ex); } try { var entityToAdd = MapToTheLoaiEntity(theLoaiDto); var addedEntity = await _dalTheLoai.AddAsync(entityToAdd); if (addedEntity == null) { throw new Exception("Thêm thể loại vào CSDL thất bại (DAL trả về null)."); } return MapToTheLoaiDTO(addedEntity); } catch (Exception ex) { Debug.WriteLine($"Error in BUSTheLoai.AddTheLoaiAsync during DAL call: {ex.Message}"); throw new Exception($"Lỗi hệ thống khi thêm thể loại: {ex.Message}", ex); } }
        public async Task<bool> UpdateTheLoaiAsync(TheLoaiDTO theLoaiDto) { if (theLoaiDto == null) throw new ArgumentNullException(nameof(theLoaiDto)); if (theLoaiDto.Id <= 0) throw new ArgumentException("ID thể loại không hợp lệ để cập nhật.", nameof(theLoaiDto.Id)); if (string.IsNullOrWhiteSpace(theLoaiDto.TenTheLoai)) { throw new ArgumentException("Tên thể loại không được rỗng.", nameof(theLoaiDto.TenTheLoai)); } theLoaiDto.TenTheLoai = theLoaiDto.TenTheLoai.Trim(); bool tenExists = await _dalTheLoai.IsTenTheLoaiExistsExcludingIdAsync(theLoaiDto.TenTheLoai, theLoaiDto.Id); if (tenExists) { throw new InvalidOperationException($"Tên thể loại '{theLoaiDto.TenTheLoai}' đã được sử dụng bởi một thể loại khác."); } if (string.IsNullOrWhiteSpace(theLoaiDto.MaTheLoai)) { throw new InvalidOperationException("Mã thể loại không được rỗng khi cập nhật."); } try { var entityToUpdate = MapToTheLoaiEntity(theLoaiDto); bool success = await _dalTheLoai.UpdateAsync(entityToUpdate); return success; } catch (Exception ex) { Debug.WriteLine($"Error in BUSTheLoai.UpdateTheLoaiAsync (ID: {theLoaiDto.Id}): {ex.Message}"); throw new Exception($"Lỗi hệ thống khi cập nhật thể loại (ID: {theLoaiDto.Id}): {ex.Message}", ex); } }
        public async Task<bool> DeleteTheLoaiAsync(int id) { if (id <= 0) throw new ArgumentException("ID thể loại không hợp lệ để xóa.", nameof(id)); bool canDelete = await CanDeleteTheLoaiAsync(id); if (!canDelete) { throw new InvalidOperationException($"Không thể xóa thể loại (ID: {id}) vì vẫn còn sách/tựa sách thuộc thể loại này."); } try { bool success = await _dalTheLoai.DeleteAsync(id); return success; } catch (Exception ex) { Debug.WriteLine($"Error in BUSTheLoai.DeleteTheLoaiAsync (ID: {id}): {ex.Message}"); throw new Exception($"Lỗi hệ thống khi xóa thể loại (ID: {id}): {ex.Message}", ex); } }
        public async Task<bool> CanDeleteTheLoaiAsync(int theLoaiId) { if (theLoaiId <= 0) return false; try { bool isUsed = await _dalTuaSach.IsTheLoaiUsedAsync(theLoaiId); return !isUsed; } catch (Exception ex) { Debug.WriteLine($"Error in BUSTheLoai.CanDeleteTheLoaiAsync (ID: {theLoaiId}): {ex.Message}"); return false; } }

    }
}