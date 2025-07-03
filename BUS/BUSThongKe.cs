// BUS/BUSThongKe.cs
using DAL; // Cần để sử dụng các interface IDAL...
using DTO; // Cần để sử dụng các lớp DTO
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq; // Cần cho LINQ để xử lý thống kê
using System.Threading.Tasks;
using DAL.Models; // Cần cho các lớp Entity
using System.Globalization; // Cần cho CultureInfo nếu xử lý tên tháng

namespace BUS
{
    /// <summary>
    /// Lớp cài đặt logic nghiệp vụ cho chức năng Thống kê.
    /// Đã thêm các phương thức lấy Top sách/thể loại mượn nhiều nhất.
    /// Đã thêm phương thức lấy số lượt mượn theo tháng.
    /// </summary>
    public class BUSThongKe : IBUSThongKe // Implement interface
    {
        // Inject các DAL cần thiết để lấy dữ liệu thống kê
        private readonly IDALPhieuMuonTra _dalPhieuMuonTra;
        private readonly IDALTheLoai _dalTheLoai;
        private readonly IDALTuaSach _dalTuaSach;
        // Không cần IDALSach, IDALCuonSach trực tiếp ở đây vì thông tin liên kết qua PhieuMuonTra

        // Constructor nhận các dependencies qua DI
        public BUSThongKe(
            IDALPhieuMuonTra dalPhieuMuonTra,
            IDALTheLoai dalTheLoai, // Cần để lấy tên nếu cần (mặc dù join đã có)
            IDALTuaSach dalTuaSach   // Cần để lấy tên nếu cần (mặc dù join đã có)
            )
        {
            _dalPhieuMuonTra = dalPhieuMuonTra ?? throw new ArgumentNullException(nameof(dalPhieuMuonTra));
            _dalTheLoai = dalTheLoai ?? throw new ArgumentNullException(nameof(dalTheLoai));
            _dalTuaSach = dalTuaSach ?? throw new ArgumentNullException(nameof(dalTuaSach));
        }

        // --- Cài đặt các phương thức từ Interface IBUSThongKe ---

        /// <summary>
        /// Thống kê số lượt mượn sách theo từng thể loại trong một khoảng thời gian.
        /// </summary>
        public async Task<List<CtBaoCaoLuotMuonTheoTheLoaiDTO>> GetThongKeLuotMuonTheoTheLoai(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.Date.AddDays(1).AddTicks(-1);
            startDate = startDate.Date;
            Debug.WriteLine($"BUSThongKe: Getting loan stats by genre from {startDate} to {endDate}");
            var resultList = new List<CtBaoCaoLuotMuonTheoTheLoaiDTO>();

            try
            {
                // 1. Lấy phiếu mượn trong khoảng thời gian với Include cần thiết
                var phieuMuonList = await _dalPhieuMuonTra.GetAllInRangeAsync(startDate, endDate);
                if (phieuMuonList == null) return resultList; // Trả về rỗng nếu DAL trả null

                // 2. Lấy tất cả thể loại
                var allTheLoai = await _dalTheLoai.GetAllAsync();
                if (allTheLoai == null) return resultList; // Không thể thống kê nếu không có danh sách thể loại

                // 3. Thống kê bằng LINQ
                var stats = phieuMuonList
                   .Where(p => p.IdCuonSachNavigation?.IdSachNavigation?.IdTuaSachNavigation?.IdTheLoaiNavigation != null) // Lọc phiếu hợp lệ
                   .GroupBy(p => p.IdCuonSachNavigation.IdSachNavigation.IdTuaSachNavigation.IdTheLoaiNavigation) // Nhóm theo Entity Thể Loại
                   .Select(g => new { TheLoai = g.Key, SoLuotMuon = g.Count() })
                   .ToList();

                // 4. Tạo DTO kết quả
                // Tính lại tổng từ stats để tỉ lệ chỉ dựa trên các phiếu có thể loại hợp lệ
                decimal totalLuotMuonTrongStats = stats.Sum(s => s.SoLuotMuon);

                foreach (var tl in allTheLoai)
                {
                    var statData = stats.FirstOrDefault(s => s.TheLoai.Id == tl.Id);
                    int luotMuon = statData?.SoLuotMuon ?? 0;
                    // Tính tỉ lệ dựa trên tổng số lượt mượn có thể loại hợp lệ
                    decimal tiLe = (totalLuotMuonTrongStats > 0 && luotMuon > 0) ? Math.Round(((decimal)luotMuon / totalLuotMuonTrongStats) * 100, 2) : 0;

                    resultList.Add(new CtBaoCaoLuotMuonTheoTheLoaiDTO
                    {
                        MaTheLoai = tl.MaTheLoai,
                        TenTheLoai = tl.TenTheLoai,
                        SoLuotMuon = luotMuon,
                        TiLe = tiLe,
                        // Thêm Id nếu DTO có (đảm bảo DTO khớp với dữ liệu)
                        // IdTheLoai = tl.Id
                    });
                }

                resultList = resultList.OrderByDescending(r => r.SoLuotMuon).ToList();
                Debug.WriteLine($"BUSThongKe: Found {resultList.Count} categories with loan stats.");
                return resultList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in BUSThongKe.GetThongKeLuotMuonTheoTheLoai: {ex}");
                throw new Exception("Lỗi trong quá trình xử lý thống kê lượt mượn theo thể loại.", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách Top N Tựa Sách được mượn nhiều nhất.
        /// </summary>
        public async Task<List<ThongKeItemDTO>> GetTopBorrowedTuaSachAsync(int topN)
        {
            Debug.WriteLine($"BUSThongKe: Getting top {topN} borrowed TuaSach...");
            try
            {
                // *** THAY ĐỔI DÒNG NÀY ***
                // var allPhieuMuon = await _dalPhieuMuonTra.GetAllInRangeAsync(DateTime.MinValue, DateTime.MaxValue);
                var allPhieuMuon = await _dalPhieuMuonTra.GetAllAsync(); // <<< SỬA THÀNH GetAllAsync()

                if (allPhieuMuon == null || !allPhieuMuon.Any())
                {
                    Debug.WriteLine("BUSThongKe: No borrowing records found for top TuaSach.");
                    return new List<ThongKeItemDTO>();
                }
                Debug.WriteLine($"BUSThongKe: Found {allPhieuMuon.Count} total borrowing records for top TuaSach.");

                // Group by Tựa Sách và đếm (giữ nguyên phần còn lại)
                var topTuaSach = allPhieuMuon
                    .Where(p => p.IdCuonSachNavigation?.IdSachNavigation?.IdTuaSachNavigation != null)
                    .GroupBy(p => p.IdCuonSachNavigation.IdSachNavigation.IdTuaSachNavigation)
                    .Select(g => new ThongKeItemDTO
                    {
                        Ten = g.Key.TenTuaSach,
                        SoLuotMuon = g.Count()
                    })
                    .OrderByDescending(item => item.SoLuotMuon)
                    .Take(topN)
                    .ToList();

                Debug.WriteLine($"BUSThongKe: Calculated top {topTuaSach.Count} TuaSach.");
                return topTuaSach;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in BUSThongKe.GetTopBorrowedTuaSachAsync: {ex}");
                throw new Exception("Lỗi khi thống kê tựa sách mượn nhiều nhất.", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách Top N Thể Loại được mượn nhiều nhất.
        /// </summary>
        public async Task<List<ThongKeItemDTO>> GetTopBorrowedTheLoaiAsync(int topN)
        {
            Debug.WriteLine($"BUSThongKe: Getting top {topN} borrowed TheLoai...");
            try
            {
                // *** THAY ĐỔI DÒNG NÀY ***
                // var allPhieuMuon = await _dalPhieuMuonTra.GetAllInRangeAsync(DateTime.MinValue, DateTime.MaxValue);
                var allPhieuMuon = await _dalPhieuMuonTra.GetAllAsync(); // <<< SỬA THÀNH GetAllAsync()

                if (allPhieuMuon == null || !allPhieuMuon.Any())
                {
                    Debug.WriteLine("BUSThongKe: No borrowing records found for top TheLoai.");
                    return new List<ThongKeItemDTO>();
                }
                Debug.WriteLine($"BUSThongKe: Found {allPhieuMuon.Count} total borrowing records for top TheLoai.");

                // Group by Thể Loại và đếm (giữ nguyên phần còn lại)
                var topTheLoai = allPhieuMuon
                    .Where(p => p.IdCuonSachNavigation?.IdSachNavigation?.IdTuaSachNavigation?.IdTheLoaiNavigation != null)
                    .GroupBy(p => p.IdCuonSachNavigation.IdSachNavigation.IdTuaSachNavigation.IdTheLoaiNavigation)
                    .Select(g => new ThongKeItemDTO
                    {
                        Ten = g.Key.TenTheLoai,
                        SoLuotMuon = g.Count()
                    })
                    .OrderByDescending(item => item.SoLuotMuon)
                    .Take(topN)
                    .ToList();

                Debug.WriteLine($"BUSThongKe: Calculated top {topTheLoai.Count} TheLoai.");
                return topTheLoai;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in BUSThongKe.GetTopBorrowedTheLoaiAsync: {ex}");
                throw new Exception("Lỗi khi thống kê thể loại mượn nhiều nhất.", ex);
            }
        }

        // *** TRIỂN KHAI PHƯƠNG THỨC MỚI: LƯỢT MƯỢN THEO THÁNG ***
        /// <summary>
        /// Lấy số lượt mượn sách theo từng tháng trong một năm cụ thể.
        /// </summary>
        /// <param name="year">Năm cần thống kê.</param>
        /// <returns>Danh sách MonthlyBorrowCountsDTO chứa thông tin tháng và số lượt mượn.</returns>
        public async Task<List<MonthlyBorrowCountsDTO>> GetMonthlyBorrowCountsAsync(int year)
        {
            Debug.WriteLine($"BUSThongKe: Getting monthly borrow counts for year {year}...");
            try
            {
                // Xác định khoảng thời gian cho năm được yêu cầu
                DateTime startDate = new DateTime(year, 1, 1);
                // Lấy đến *cuối ngày* 31/12 của năm đó
                DateTime endDate = new DateTime(year, 12, 31, 23, 59, 59, 999);

                // Lấy tất cả phiếu mượn trong năm đó từ DAL
                // Giả sử GetAllInRangeAsync bao gồm cả ngày bắt đầu và kết thúc
                var phieuMuonTrongNam = await _dalPhieuMuonTra.GetAllInRangeAsync(startDate, endDate);

                if (phieuMuonTrongNam == null)
                {
                    Debug.WriteLine($"BUSThongKe: No borrowing records found for year {year}.");
                    // Trả về danh sách rỗng đủ 12 tháng với count = 0
                    return Enumerable.Range(1, 12)
                       .Select(month => new MonthlyBorrowCountsDTO { Month = month, BorrowCount = 0 })
                       .ToList();
                }
                Debug.WriteLine($"BUSThongKe: Found {phieuMuonTrongNam.Count} records for year {year}.");

                // Nhóm các phiếu mượn theo tháng của ngày mượn (NgayMuon.Month)
                var monthlyCounts = phieuMuonTrongNam
                    .GroupBy(p => p.NgayMuon.Month) // Nhóm theo tháng (số từ 1-12)
                    .Select(g => new MonthlyBorrowCountsDTO
                    {
                        Month = g.Key,            // Lấy số tháng (key của group)
                        BorrowCount = g.Count()   // Đếm số lượng phiếu trong nhóm (số lượt mượn)
                    })
                    .OrderBy(dto => dto.Month)    // Sắp xếp kết quả theo tháng tăng dần
                    .ToList();

                // Đảm bảo kết quả trả về luôn đủ 12 tháng, kể cả những tháng không có lượt mượn nào
                var fullYearCounts = Enumerable.Range(1, 12) // Tạo dãy số từ 1 đến 12
                    .Select(month => new MonthlyBorrowCountsDTO // Với mỗi tháng trong dãy số
                    {
                        Month = month, // Gán số tháng
                        // Tìm trong kết quả 'monthlyCounts' xem có dữ liệu cho tháng này không
                        // Nếu có, lấy BorrowCount, nếu không, gán BorrowCount = 0
                        BorrowCount = monthlyCounts.FirstOrDefault(mc => mc.Month == month)?.BorrowCount ?? 0
                    })
                    .ToList(); // Chuyển kết quả thành List

                Debug.WriteLine($"BUSThongKe: Calculated monthly counts for {fullYearCounts.Count} months.");
                return fullYearCounts; // Trả về danh sách đầy đủ 12 tháng
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in BUSThongKe.GetMonthlyBorrowCountsAsync for year {year}: {ex}");
                // Ném lại lỗi để tầng trên xử lý, hoặc có thể trả về list rỗng/null tùy yêu cầu
                throw new Exception($"Lỗi khi thống kê lượt mượn theo tháng cho năm {year}.", ex);
            }
        }
        // --- KẾT THÚC TRIỂN KHAI ---

    } // End class BUSThongKe
} // End namespace BUS