// Project/Namespace: GUI

// --- USING DIRECTIVES ---
using System;
using System.Configuration; // Cho ConfigurationManager (Nếu bạn vẫn dùng App.config)
using System.Diagnostics; // Cho Debug
using System.Linq; // Cho LINQ (ví dụ: Application.OpenForms.Cast<Form>())
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Cho IConfiguration, AddJsonFile
using DAL; // Chứa Interfaces và Implementations của DAL
using DAL.Models; // Chứa DbContext (QLTVContext)
using BUS; // Chứa Interfaces và Implementations của BUS
using BUS.Utilities; // Chứa IPasswordHasherService
// using GUI; // Không cần using chính namespace của nó

namespace GUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Cấu hình cơ bản cho ứng dụng WinForms
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Tạo và build Host để cấu hình DI và các dịch vụ khác
            var host = CreateHostBuilder().Build();

            // Sử dụng scope để quản lý vòng đời DbContext và các dịch vụ Scoped khác
            // Điều này đảm bảo mỗi yêu cầu (trong trường hợp này là mỗi lần chạy form)
            // có thể có instance DbContext riêng nếu cần (khi dùng Scoped)
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    // Lấy Form đăng nhập từ DI container
                    // DI container sẽ tự động inject các dependencies đã đăng ký (như IBUSNguoiDung, IServiceProvider) vào constructor của frmLogin
                    var loginForm = services.GetRequiredService<frmLogin>();

                    // Chạy ứng dụng với form đăng nhập là form khởi đầu
                    Application.Run(loginForm);
                }
                catch (Exception ex)
                {
                    // Bắt lỗi nghiêm trọng không xử lý được trong quá trình khởi tạo hoặc chạy ứng dụng
                    Debug.WriteLine($"Application Startup/Unhandled Error: {ex.ToString()}");
                    // Hiển thị lỗi cho người dùng cuối
                    MessageBox.Show( // Không cần parent window ở đây
                        $"Đã xảy ra lỗi nghiêm trọng khi khởi động hoặc vận hành ứng dụng:\n{ex.Message}\n\n" +
                        (ex.InnerException != null ? $"Chi tiết: {ex.InnerException.Message}\n\n" : "") + // Hiển thị InnerException nếu có
                        "Vui lòng khởi động lại ứng dụng hoặc liên hệ bộ phận hỗ trợ.",
                        "Lỗi Hệ Thống Nghiêm Trọng",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    // Có thể cân nhắc việc ghi lỗi vào file log ở đây
                }
            }
        }

        /// <summary>
        /// Cấu hình Host Builder, đăng ký các dịch vụ vào DI container.
        /// </summary>
        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                // Cấu hình đọc file appsettings.json
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                // Cấu hình các dịch vụ sẽ được inject
                .ConfigureServices((hostContext, services) =>
                {
                    // --- Đăng ký DbContext ---
                    // Sử dụng AddDbContextPool để tối ưu hiệu năng nếu có nhiều yêu cầu DB đồng thời
                    services.AddDbContext<QLTVContext>(options => // Chỉ định rõ namespace DAL.Models.QLTVContext
                    {
                        var connectionString = hostContext.Configuration.GetConnectionString("QLTVConnectionString");
                        if (string.IsNullOrEmpty(connectionString))
                        {
                            throw new InvalidOperationException("Chuỗi kết nối 'QLTVConnectionString' không được tìm thấy trong appsettings.json.");
                        }
                        options.UseSqlServer(connectionString);
                        // Có thể thêm các cấu hình khác cho DbContext ở đây (ví dụ: logging)
                        // options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())); // Log EF Core queries to console
                        // options.EnableSensitiveDataLogging(); // Chỉ dùng khi debug
                    }, ServiceLifetime.Scoped); // Scoped là lựa chọn phổ biến cho DbContext trong web/API, cũng phù hợp cho WinForms nếu dùng scope như trên

                    // --- Đăng ký DALs (Data Access Layer) ---
                    // Scoped: Mỗi scope (như mỗi lần chạy form được resolve từ scope) sẽ dùng chung 1 instance DAL (và DbContext)
                    services.AddScoped<IDALDocGia, DALDocGia>();
                    services.AddScoped<IDALLoaiDocGia, DALLoaiDocGia>();
                    services.AddScoped<IDALNguoiDung, DALNguoiDung>();
                    services.AddScoped<IDALSach, DALSach>();
                    services.AddScoped<IDALCuonSach, DALCuonSach>();
                    services.AddScoped<IDALTuaSach, DALTuaSach>();
                    services.AddScoped<IDALTacGia, DALTacGia>();
                    services.AddScoped<IDALTheLoai, DALTheLoai>();
                    services.AddScoped<IDALPhieuMuonTra, DALPhieuMuonTra>();
                    services.AddScoped<IDALThamSo, DALThamso>();
                    services.AddScoped<IDALNhomNguoiDung, DALNhomNguoiDung>();
                    services.AddScoped<IDALNhaXuatBan, DALNhaXuatBan>();
                    services.AddScoped<IDALPhieuNhapSach, DALPhieuNhapSach>();
                    services.AddScoped<IDALCtPhieunhap, DALCtPhieunhap>();
                    services.AddScoped<IDALChucNang, DALChucNang>();
                    services.AddScoped<IDALThongBao, DALThongBao>();
                    services.AddScoped<IDALPhieuThu, DALPhieuThu>();// Scoped nếu dùng DbContext

                    // --- Đăng ký Utilities ---
                    // Singleton thường phù hợp cho các service không có trạng thái (stateless) như Password Hasher
                    services.AddSingleton<IPasswordHasherService, BCryptPasswordHasherService>();

                    // --- Đăng ký BUSs (Business Logic Layer) ---
                    // Transient: Mỗi khi yêu cầu một IBUS, một instance mới của BUS sẽ được tạo. Phù hợp nếu BUS không có trạng thái.
                    // Scoped cũng là lựa chọn tốt nếu BUS cần dùng chung DbContext hoặc trạng thái trong một scope.
                    services.AddTransient<IBUSDocGia, BUSDocGia>();
                    services.AddTransient<IBUSLoaiDocGia, BUSLoaiDocGia>();
                    services.AddTransient<IBUSNguoiDung, BUSNguoiDung>();
                    services.AddTransient<IBUSSach, BUSSach>();
                    services.AddTransient<IBUSCuonSach, BUSCuonSach>();
                    services.AddTransient<IBUSTuaSach, BUSTuaSach>();
                    services.AddTransient<IBUSTacGia, BUSTacGia>();
                    services.AddTransient<IBUSTheLoai, BUSTheLoai>();
                    services.AddTransient<IBUSPhieuMuonTra, BUSPhieuMuonTra>();
                    services.AddTransient<IBUSThamSo, BUSThamso>();
                    services.AddTransient<IBUSNhomNguoiDung, BUSNhomNguoiDung>();
                    services.AddTransient<IBUSThongKe, BUSThongKe>(); // Có thể đổi thành Scoped nếu cần chia sẻ state
                    services.AddTransient<IBUSNhaXuatBan, BUSNhaXuatBan>();
                    services.AddTransient<IBUSPhieuNhapSach, BUSPhieuNhapSach>();
                    services.AddTransient<IBUSCtPhieunhap, BUSCtPhieunhap>();
                    services.AddTransient<IBUSChucNang, BUSChucNang>();
                    services.AddTransient<IBUSThongBao, BUSThongBao>(); // Đăng ký BUSThongBao

                    // --- Đăng ký Forms và UserControls (UI Layer) ---
                    // Transient là lựa chọn phổ biến nhất cho Forms và UserControls
                    // để đảm bảo mỗi lần mở là một instance mới, độc lập.
                    services.AddTransient<frmLogin>(); // Form đăng nhập
                    services.AddTransient<frmMain>(); // Form chính
                    services.AddTransient<frmResetPassword>(); // <<<=== ĐĂNG KÝ FORM ĐẶT LẠI MẬT KHẨU

                    // Đăng ký các UserControls nếu chúng cần được inject dependencies
                    services.AddTransient<ucTrangChu>();
                    services.AddTransient<ucQuanLyDocGia>();
                    services.AddTransient<ucQuanLyLoaiDocGia>();
                    services.AddTransient<ucQuanLyNguoiDung>();
                    services.AddTransient<ucQuanLySach>();
                    services.AddTransient<ucQuanLyTuaSach>();
                    services.AddTransient<ucQuanLyTacGia>();
                    services.AddTransient<ucQuanLyTheLoai>();
                    services.AddTransient<ucQuanLyPhieuMuonTra>();
                    services.AddTransient<ucCaiDat>();
                    services.AddTransient<ucThongKe>();
                    services.AddTransient<ucQuanLyNhomNguoiDung>();
                    services.AddTransient<ucQuanLyCuonSach>();
                    services.AddTransient<ucQuanLyNhapSach>();
                    services.AddTransient<ucQuanLyNhaXuatBan>();
                    services.AddTransient<ucQuanLyThongBao>(); // <<<=== ĐÃ THÊM DÒNG NÀY
                    // Thêm các UserControls khác ở đây nếu cần...

                });
    }
}