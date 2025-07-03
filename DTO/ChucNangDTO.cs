// DTO/ChucNangDTO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    /// <summary>
    /// Data Transfer Object for ChucNang (Function/Permission).
    /// Defines the data structure transferred between layers.
    /// </summary>
    public class ChucNangDTO
    {
        /// <summary>
        /// Primary Key identifier for the function.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Optional code for the function (e.g., "CN001").
        /// Might be null or automatically generated.
        /// </summary>
        public string? MaChucNang { get; set; } // Giữ nguyên kiểu nullable giống Entity

        /// <summary>
        /// Name of the function (e.g., "Quản lý Sách"). Cannot be null or empty.
        /// </summary>
        public string TenChucNang { get; set; } = null!; // Đảm bảo không null giống Entity

        /// <summary>
        /// Name of the screen/form associated with this function. Cannot be null or empty.
        /// </summary>
        public string TenManHinh { get; set; } = null!; // Đảm bảo không null giống Entity

        // --- Constructors (Optional but can be helpful) ---

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ChucNangDTO()
        {
            // Initialize potentially null properties if needed,
            // but string properties are initialized to null by default.
            // TenChucNang and TenManHinh use null-forgiving operator based on Entity constraints.
        }

        /// <summary>
        /// Parameterized constructor for easier creation.
        /// </summary>
        /// <param name="id">Function ID</param>
        /// <param name="maChucNang">Function Code (optional)</param>
        /// <param name="tenChucNang">Function Name (required)</param>
        /// <param name="tenManHinh">Associated Screen Name (required)</param>
        public ChucNangDTO(int id, string? maChucNang, string tenChucNang, string tenManHinh)
        {
            Id = id;
            MaChucNang = maChucNang;
            TenChucNang = tenChucNang ?? throw new ArgumentNullException(nameof(tenChucNang));
            TenManHinh = tenManHinh ?? throw new ArgumentNullException(nameof(tenManHinh));
        }

        // --- Overrides for better usability (Optional) ---

        /// <summary>
        /// Returns the function name for display purposes.
        /// </summary>
        public override string ToString()
        {
            return TenChucNang ?? $"ID: {Id}";
        }

        /// <summary>
        /// Compares DTOs based on their ID.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is ChucNangDTO other)
            {
                return this.Id == other.Id;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code based on the ID.
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}