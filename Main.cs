namespace OOP_project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Hỗ trợ hiển thị tiếng Việt

            Admin sysAdmin = Admin.GetInstance();

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n===========================================");
                Console.WriteLine("        BẢNG ĐIỀU KHIỂN QUẢN TRỊ VIÊN      ");
                Console.WriteLine("===========================================");
                Console.WriteLine("1. Quản lý Hội viên ");
                Console.WriteLine("2. Quản lý Huấn luyện viên ");
                Console.WriteLine("3. Quản lý Lớp học ");
                Console.WriteLine("4. Quản lý Đặt lịch & Hàng đợi ");
                Console.WriteLine("5. Điểm danh lớp học ");
                Console.WriteLine("6. Xuất báo cáo hoạt động ");
                Console.WriteLine("0. Thoát chương trình");
                Console.WriteLine("===========================================");
                Console.Write("Vui lòng chọn chức năng (0-6): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        sysAdmin.ManageMembers();
                        break;
                    case "2":
                        sysAdmin.ManageTrainers();
                        break;
                    case "3":
                        sysAdmin.ManageFitnessClasses();
                        break;
                    case "4":
                        sysAdmin.ManageBookings();
                        break;
                    case "5":
                        sysAdmin.MarkAttendance();
                        break;
                    case "6":
                        sysAdmin.GenerateReports();
                        break;
                    case "0":
                        isRunning = false;
                        Console.WriteLine("\n[Hệ thống] Đang lưu dữ liệu... Tạm biệt!");
                        break;
                    default:
                        Console.WriteLine("\n[Lỗi] Lựa chọn không hợp lệ. Vui lòng nhập số từ 0 đến 6.");
                        break;
                }

                // Tạm dừng màn hình để người dùng đọc thông báo trước khi menu lặp lại
                if (isRunning)
                {

                    Console.WriteLine("\nNhấn phím Enter bất kỳ để tiếp tục...");
                    Console.ReadLine();
                    try
                    {
                        Console.Clear(); // Xóa màn hình console cho gọn gàng (an toàn khi không có TTY)
                    }
                    catch (IOException)
                    {
                        // Bỏ qua lỗi khi console bị chuyển hướng luồng I/O
                    }
                }
            }
        }
    }
}