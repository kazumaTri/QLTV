using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DALNhaXuatBan : IDALNhaXuatBan
    {
        private readonly QLTVContext _context;

        // Constructor nhận QLTVContext qua DI
        public DALNhaXuatBan(QLTVContext context)
        {
            _context = context;
        }

        public List<NhaXuatBan> GetAll()
        {
            try
            {
                return _context.NhaXuatBans.ToList();
            }
            catch (Exception ex)
            {
                // Xử lý log lỗi
                Console.WriteLine($"Error getting all NhaXuatBan: {ex.Message}");
                return new List<NhaXuatBan>();
            }
        }

        public NhaXuatBan GetById(int id)
        {
            try
            {
                return _context.NhaXuatBans.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting NhaXuatBan by ID {id}: {ex.Message}");
                return null;
            }
        }

        public bool Add(NhaXuatBan nxb)
        {
            try
            {
                _context.NhaXuatBans.Add(nxb);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding NhaXuatBan: {ex.Message}");
                _context.Entry(nxb).State = EntityState.Detached; // Quan trọng: Detach entity nếu có lỗi
                return false;
            }
        }

        public bool Update(NhaXuatBan nxb)
        {
            try
            {
                var existingNxb = _context.NhaXuatBans.Find(nxb.Id);
                if (existingNxb == null) return false;

                _context.Entry(existingNxb).CurrentValues.SetValues(nxb);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating NhaXuatBan {nxb.Id}: {ex.Message}");
                _context.Entry(nxb).State = EntityState.Unchanged; // Hoặc Detached tùy ngữ cảnh
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var nxbToDelete = _context.NhaXuatBans.Find(id);
                if (nxbToDelete == null) return false;

                _context.NhaXuatBans.Remove(nxbToDelete);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting NhaXuatBan {id}: {ex.Message}");
                // Cân nhắc không detach ở đây vì entity có thể đã bị xóa một phần
                return false;
            }
        }

        public List<NhaXuatBan> SearchByName(string name)
        {
            try
            {
                return _context.NhaXuatBans
                               .Where(n => n.TenNXB.Contains(name))
                               .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching NhaXuatBan by name '{name}': {ex.Message}");
                return new List<NhaXuatBan>();
            }
        }

        // Implement các phương thức khác nếu có trong Interface
    }
}