using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace OOP_project
{
    public class Admin
    {
        // 1. Khởi tạo instance duy nhất (Singleton)
        private static Admin? _instance;

        // Các danh sách dữ liệu giả lập Database
        public List<Member> Members { get; private set; }
        public List<Trainer> Trainers { get; private set; }
        public List<FitnessClass> FitnessClasses { get; private set; }

        // 2. Private constructor ngăn không cho tạo instance từ bên ngoài bằng từ khóa 'new'
        private Admin()
        {
            // Khởi tạo các danh sách khi Admin được tạo lần đầu,khởi tạo vài member mẫu để test
            Members = new List<Member>([
                new Member("M001", "Nguyen Van A", "nguyenvana@example.com", "0901234567"),
                new Member("M002", "Tran Thi B", "tranthib@example.com", "0901234568"),
                new Member("M003", "Le Van C", "levanc@example.com", "0901234569")
            ]);
            Members[0].CurrentPlan = new BasicPlan();
            Members[1].CurrentPlan = new BasicPlan();
            Members[2].CurrentPlan = new BasicPlan();
            Trainers = new List<Trainer>([
                new Trainer("T001", "Pham Van D", "phamvand@example.com", "0901234560", "Yoga"),
                new Trainer("T002", "Hoang Thi E", "hoangthie@example.com", "0901234561", "Zumba"),
                new Trainer("T003", "Nguyen Van F", "nguyenvanf@example.com", "0901234562", "Pilates")
            ]);
            FitnessClasses = new List<FitnessClass>();
            
            Console.WriteLine("[Hệ thống] Tài khoản Admin đã được khởi tạo thành công.");
        }

        // 3. Public static method để lấy instance duy nhất
        public static Admin GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Admin();
            }
            return _instance;
        }

        // --- CÁC PHƯƠNG THỨC QUẢN LÝ CỦA ADMIN ---

        public void ManageMembers()
        {
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ HỘI VIÊN ---");
                Console.WriteLine("1. Thêm hội viên mới");
                Console.WriteLine("2. Xem danh sách hội viên");
                Console.WriteLine("3. Xóa hội viên");
                Console.WriteLine("4. Cập nhật thông tin hội viên");
                Console.WriteLine("5. Nâng cấp/Hạ cấp gói tập (MembershipPlan)");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-5): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Logic thêm hội viên mới
                        Console.WriteLine("Nhập thông tin hội viên mới (tên, email, số điện thoại, gói tập):");
                        Console.Write("ID: ");
                        string? id = Console.ReadLine();
                        Console.Write("Tên: ");
                        string? name = Console.ReadLine();
                        Console.Write("Email: ");
                        string? email = Console.ReadLine();
                        Console.Write("Số điện thoại: ");
                        string? phone = Console.ReadLine();
                        Console.Write("TypePlan (1 cho Basic 2 cho Premium): ");
                        string? typeplan = Console.ReadLine();
                        Members.Add(new Member(id, name, email, phone));
                        if( typeplan == "1")
                        {
                            var lastItem = Members[Members.Count - 1];
                            lastItem.CurrentPlan = new BasicPlan();
                        }
                        else if (typeplan == "2" )
                        {
                            var lastItem = Members[Members.Count - 1];
                            lastItem.CurrentPlan = new PremiumPlan();
                        }
                        Console.WriteLine($"[Hệ thống] Hội viên {name} đã được thêm thành công.");
                        break;
                    case "2":
                        // Logic xem danh sách hội viên
                        Console.WriteLine("\n--- DANH SÁCH HỘI VIÊN ---");
                        foreach (var user in Members)
                        {
                            Console.WriteLine($"ID: {user.Id}, Tên: {user.Name}");
                        }
                        break;
                    case "3":
                        // Logic xóa hội viên
                        Console.Write("Nhập ID hội viên cần xóa: ");
                        string? memberIdToDelete = Console.ReadLine();
                        var memberToDelete = Members.FirstOrDefault(m => m.Id == memberIdToDelete);
                        if (memberToDelete != null)
                        {
                            Members.Remove(memberToDelete);
                            Console.WriteLine($"[Hệ thống] Hội viên {memberToDelete.Name} đã được xóa.");
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
                        }
                        break;
                    case "4":
                        // Logic cập nhật thông tin hội viên
                        Console.Write("Nhập ID hội viên cần cập nhật thông tin: ");
                        string? memberIdToUpdate = Console.ReadLine();
                        var memberToUpdate = Members.FirstOrDefault(m => m.Id == memberIdToUpdate);
                        if (memberToUpdate != null)
                        {
                            Console.Write("Nhập tên mới cho hội viên: ");
                            string? newName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newName))
                            {
                                memberToUpdate.Name = newName;
                                Console.WriteLine($"[Hệ thống] Thông tin của hội viên {memberToUpdate.Name} đã được cập nhật.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
                        }
                        break;
                    case "5":
                        // Logic nâng cấp/hạ cấp gói tập
                        Console.Write("Nhập ID hội viên cần thay đổi gói tập: ");
                        string? memberIdToChangePlan = Console.ReadLine();
                        var memberToChangePlan = Members.FirstOrDefault(m => m.Id == memberIdToChangePlan);
                        if (memberToChangePlan != null)
                        {
                            Console.Write("Nhập gói tập mới chọn 1 cho Basic hoặc 2 cho Premium): ");
                            string? newPlan = Console.ReadLine();
                            // Cập nhật gói tập cho hội viên
                            if (newPlan == "1")
                            {
                                if (memberToChangePlan.CurrentPlan.GetPlanName() != "Basic")
                                {
                                    memberToChangePlan.ChangePlan(new BasicPlan());
                                }
                                else
                                {
                                    Console.WriteLine($"[Hệ thống] Hội viên {memberToChangePlan.Name} đã đang ở gói Basic.");
                                }
                            }
                            else if (newPlan == "2")
                            {
                                if (memberToChangePlan.CurrentPlan.GetPlanName() != "Premium")
                                {
                                    memberToChangePlan.ChangePlan(new PremiumPlan());
                                }
                                else
                                {
                                    Console.WriteLine($"[Hệ thống] Hội viên {memberToChangePlan.Name} đã đang ở gói Premium.");
                                }
                            }
                            Console.WriteLine($"[Hệ thống] Gói tập của hội viên {memberToChangePlan.Name} đã được cập nhật.");
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
                        }
                        break;
                    case "0":
                        return; // Quay lại menu chính
                    default:
                        Console.WriteLine("\n[Lỗi] Lựa chọn không hợp lệ. Vui lòng nhập số từ 0 đến 3.");
                        break;
                }
            }
            
        }

        public void ManageTrainers()
        {
            while(true)
            {
            Console.WriteLine("\n--- QUẢN LÝ HUẤN LUYỆN VIÊN ---");
            Console.WriteLine("1. Thêm huấn luyện viên mới");
            Console.WriteLine("2. Xem danh sách huấn luyện viên");
            Console.WriteLine("3. Phân công lớp học (assignClass)");
            Console.WriteLine("0. Quay lại menu chính");
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    // Logic thêm huấn luyện viên mới
                    Console.WriteLine("Nhập thông tin huấn luyện viên mới :");
                    Console.Write("ID: ");
                    string? id = Console.ReadLine();
                    Console.Write("Tên: ");
                    string? name = Console.ReadLine();
                    Console.Write("Email: ");
                    string? email = Console.ReadLine();
                    Console.Write("Số điện thoại: ");
                    string? phone = Console.ReadLine();
                    Console.Write("Chuyên môn: ");
                    string? specialization = Console.ReadLine();
                    Trainers.Add(new Trainer(id, name, email, phone, specialization));
                    Console.WriteLine($"[Hệ thống] Huấn luyện viên {name} đã được thêm thành công.");
                    break;
                case "2":
                    // Logic xem danh sách huấn luyện viên
                    Console.WriteLine("\n--- DANH SÁCH HUẤN LUYỆN VIÊN ---");
                    foreach (var train in Trainers)
                    {
                        Console.WriteLine($"ID: {train.Id}, Tên: {train.Name}, Email: {train.Email}, Số điện thoại: {train.PhoneNumber}, Chuyên môn: {train.Specialization}");
                    }
                    break;
                case "3":
                    // Logic phân công lớp học
                    Console.Write("Nhập ID huấn luyện viên cần phân công lớp học: ");
                    string? trainerId = Console.ReadLine();
                    var trainer = Trainers.FirstOrDefault(t => t.Id == trainerId);
                    if (trainer != null)
                    {
                        Console.Write("Nhập ID lớp học cần phân công: ");
                        string? classId = Console.ReadLine();
                        var fitnessClass = FitnessClasses.FirstOrDefault(fc => fc.ClassId == classId);
                        if (fitnessClass != null)
                        {
                            trainer.AssignClass(fitnessClass);
                            Console.WriteLine($"[Hệ thống] Huấn luyện viên {trainer.Name} đã được phân công lớp học {fitnessClass.ClassName}.");
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học với ID đã nhập.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập.");
                    }
                    break;
                case "0":
                    return; // Quay lại menu chính
                default:
                    Console.WriteLine("\n[Lỗi] Lựa chọn không hợp lệ. Vui lòng nhập số từ 1 đến 2.");
                    break;
            }
            }
        }

        public void ManageFitnessClasses()
        {
            Console.WriteLine("\n--- QUẢN LÝ LỚP HỌC (FITNESS CLASS) ---");
            Console.WriteLine("1. Tạo lớp học mới (set ScheduleTime, Capacity)");
            Console.WriteLine("2. Xem danh sách lớp học");
            // Thêm logic nhập xuất tương ứng...
            Console.WriteLine("[!] Đang chạy logic tạo/xem lớp học...");
        }

        public void ManageBookings()
        {
            Console.WriteLine("\n--- QUẢN LÝ ĐẶT LỊCH & HÀNG ĐỢI ---");
            Console.WriteLine("1. Đặt lớp cho hội viên (bookClass)");
            Console.WriteLine("2. Hủy đặt lớp (cancelBooking)");
            Console.WriteLine("3. Đôn hội viên từ danh sách chờ (promoteFromWaitlist)");
            // Thêm logic tương tác với BookingStatus và Queue<Booking>
            Console.WriteLine("[!] Đang xử lý các yêu cầu Booking...");
        }

        public void MarkAttendance()
        {
            Console.WriteLine("\n--- ĐIỂM DANH ---");
            Console.WriteLine("1. Xem danh sách tham gia chính thức (getConfirmedAttendees)");
            Console.WriteLine("2. Đánh dấu có mặt (markAttendance)");
            // Cập nhật BookingStatus thành ATTENDED
            Console.WriteLine("[!] Đang thực hiện điểm danh...");
        }

        public void GenerateReports()
        {
            Console.WriteLine("\n--- BÁO CÁO HOẠT ĐỘNG ---");
            Console.WriteLine("Nhập ID hội viên để xuất báo cáo (ActivityReportService):");
            // Gọi ActivityReportService.generateMemberReport(...)
            Console.WriteLine("[!] Đang trích xuất báo cáo...");
        }
    }
}