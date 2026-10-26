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
            FitnessClasses = new List<FitnessClass>([
                new FitnessClass("C001", "Yoga Class", DateTime.Now.AddDays(1), 10, Trainers[0]),
                new FitnessClass("C002", "Zumba Class", DateTime.Now.AddDays(2), 15, Trainers[1]),
                new FitnessClass("C003", "Pilates Class", DateTime.Now.AddDays(3), 12, Trainers[2])
            ]);
            
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
                            Console.WriteLine($"ID: {user.Id}, Tên: {user.Name}, Email: {user.Email}, Số điện thoại: {user.PhoneNumber}, Gói tập: {(user.CurrentPlan != null ? user.CurrentPlan.GetPlanName() : "Chưa có gói tập")}");
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
                        Console.Write("Nhập ID hội viên cần cập nhật thông tin: ");
                        string? memberIdToUpdate = Console.ReadLine();
                        var memberToUpdate = Members.FirstOrDefault(m => m.Id == memberIdToUpdate);
                        if (memberToUpdate == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
                            break;
                        }
                        // hỏi thông tin cần cập nhật name, email, phoneNumber
                        Console.WriteLine("\n--- CẬP NHẬT THÔNG TIN HỘI VIÊN ---");
                        Console.WriteLine("1. Cập nhật tên");
                        Console.WriteLine("2. Cập nhật email");
                        Console.WriteLine("3. Cập nhật số điện thoại");
                        string? choiceToUpdate = Console.ReadLine();
                        
                        switch (choiceToUpdate)
                        {
                            case "1":
                                // Logic cập nhật tên
                                Console.Write("Nhập tên mới cho hội viên: ");
                                string? newName = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(newName))
                                {
                                    memberToUpdate.Name = newName;
                                    Console.WriteLine($"[Hệ thống] Thông tin của hội viên {memberToUpdate.Name} đã được cập nhật.");
                                }
                                else
                                {
                                    Console.WriteLine("[Lỗi] Tên không được để trống.");
                                }
                                break;
                            case "2":
                                // Logic cập nhật email
                                Console.Write("Nhập email mới cho hội viên: ");
                                string? newEmail = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(newEmail))
                                {
                                    memberToUpdate.Email = newEmail;
                                    Console.WriteLine($"[Hệ thống] Thông tin của hội viên {memberToUpdate.Name} đã được cập nhật.");
                                }
                                else
                                {
                                    Console.WriteLine("[Lỗi] Email không được để trống.");
                                }
                                break;
                            case "3":
                                // Logic cập nhật số điện thoại
                                Console.Write("Nhập số điện thoại mới cho hội viên: ");
                                string? newPhone = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(newPhone))
                                {
                                    memberToUpdate.PhoneNumber = newPhone;
                                    Console.WriteLine($"[Hệ thống] Thông tin của hội viên {memberToUpdate.Name} đã được cập nhật.");
                                }
                                else
                                {
                                    Console.WriteLine("[Lỗi] Số điện thoại không được để trống.");
                                }
                                break;
                            default:
                                Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                                break;
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
            Console.WriteLine("3. Xóa huấn luyện viên");
            Console.WriteLine("4. Sửa thông tin huấn luyện viên");
            Console.WriteLine("5. Phân công lớp họ");
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
                    // Logic xóa huấn luyện viên
                    Console.Write("Nhập ID huấn luyện viên cần xóa: ");
                    string? trainerIdToDelete = Console.ReadLine();
                    var trainerToDelete = Trainers.FirstOrDefault(t => t.Id == trainerIdToDelete);
                    if (trainerToDelete != null)
                    {
                        Trainers.Remove(trainerToDelete);
                        Console.WriteLine($"[Hệ thống] Huấn luyện viên {trainerToDelete.Name} đã được xóa.");
                    }
                    else
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập.");
                    }
                    break;
                case "4":
                    // Logic sửa thông tin huấn luyện viên
                    Console.Write("Nhập ID huấn luyện viên cần sửa thông tin: ");
                    string? trainerIdToUpdate = Console.ReadLine();
                    var trainerToUpdate = Trainers.FirstOrDefault(t => t.Id == trainerIdToUpdate);
                    if (trainerToUpdate != null)
                    {
                        Console.WriteLine("\n--- CẬP NHẬT THÔNG TIN HUẤN LUYỆN VIÊN ---");
                        Console.WriteLine("1. Cập nhật tên");
                        Console.WriteLine("2. Cập nhật email");
                        Console.WriteLine("3. Cập nhật số điện thoại");
                        Console.WriteLine("4. Cập nhật chuyên môn");
                        Console.Write("Lựa chọn của bạn (1-4): ");
                        string? choiceToUpdate = Console.ReadLine();

                        switch (choiceToUpdate)
                        {
                            case "1":
                                Console.Write("Tên mới: ");
                                string? newName = Console.ReadLine();
                                trainerToUpdate.Name = newName;
                                break;
                            case "2":
                                Console.Write("Email mới: ");
                                string? newEmail = Console.ReadLine();
                                trainerToUpdate.Email = newEmail;
                                break;
                            case "3":
                                Console.Write("Số điện thoại mới: ");
                                string? newPhone = Console.ReadLine();
                                trainerToUpdate.PhoneNumber = newPhone;
                                break;
                            case "4":
                                Console.Write("Chuyên môn mới: ");
                                string? newSpecialization = Console.ReadLine();
                                trainerToUpdate.Specialization = newSpecialization;
                                break;
                            default:
                                Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập.");
                    }
                    break;
                case "5":
                    // Logic phân công lớp học
                    Console.Write("Nhập ID huấn luyện viên cần phân công lớp học: ");
                    string? trainerId = Console.ReadLine();
                    var trainer = Trainers.FirstOrDefault(t => t.Id == trainerId);
                    if (trainer != null)
                    {
                        Console.Write("Nhập ID lớp học cần phân công: ");
                        string? classId = Console.ReadLine();
                        var fitnessClass = FitnessClasses.FirstOrDefault(fc => fc.ClassId == classId);
                        if (fitnessClass != null && trainer.Specialization == fitnessClass.ClassName.Split(' ')[0]) // Kiểm tra chuyên môn
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
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ LỚP HỌC (FITNESS CLASS) ---");
                Console.WriteLine("1. Tạo lớp học mới (set ScheduleTime, Capacity)");
                Console.WriteLine("2. Xem danh sách lớp học");
                Console.WriteLine("3. Xóa lớp học");
                Console.WriteLine("4. Cập nhật thông tin lớp học");
                Console.WriteLine("5. Phân công huấn luyện viên cho lớp học (assignTrainer)");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-5): ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Nhập thông tin lớp học mới:");
                        Console.Write("ID: ");
                        string? classId = Console.ReadLine();
                        string? className;
                        while (true)
                        {
                            Console.Write("Tên lớp học (ví dụ: Yoga Class, Zumba Class): ");
                            className = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(className) && className.EndsWith("Class"))
                            {
                                break;
                            }
                            Console.WriteLine("[Lỗi] Tên lớp học phải kết thúc bằng 'Class'. Vui lòng nhập lại.");
                        }
                        DateTime scheduleTime;
                        Console.Write("Thời gian (yyyy-MM-dd HH:mm): ");
                        string? scheduleTimeText = Console.ReadLine();
                        if (DateTime.TryParse(scheduleTimeText, out scheduleTime))
                        {
                            break;
                        }
                        Console.WriteLine("[Lỗi] Định dạng thời gian không hợp lệ. Vui lòng nhập lại.");
                        Console.Write("Sức chứa: ");
                        string? capacityText = Console.ReadLine();
                        int capacityToAdd = int.TryParse(capacityText, out int capacity) ? capacity : 0;
                        if (capacityToAdd <= 0)
                        {
                            Console.WriteLine("[Lỗi] Sức chứa phải là số nguyên dương. Vui lòng nhập lại.");
                            break;
                        }
                        Console.Write("ID huấn luyện viên để phân công cho lớp học: ");
                        string? trainerId = Console.ReadLine();
                        var trainer = Trainers.FirstOrDefault(t => t.Id == trainerId);
                        if (trainer == null || trainer.Specialization != className.Split(' ')[0]) // Kiểm tra chuyên môn
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập hoặc chuyên môn không phù hợp. Vui lòng tạo huấn luyện viên trước khi phân công.");
                            break;
                        }

                        FitnessClasses.Add(new FitnessClass(classId, className, scheduleTime, capacityToAdd, trainer));
                        Console.WriteLine($"[Hệ thống] Lớp học {className} đã được tạo thành công.");
                        break;
                    case "2":
                        Console.WriteLine("\n--- DANH SÁCH LỚP HỌC ---");
                        foreach (var fc in FitnessClasses)
                        {
                            Console.WriteLine($"ID: {fc.ClassId}, Tên: {fc.ClassName}, Thời gian: {fc.ScheduleTime}, Sức chứa: {fc.Capacity}, Huấn luyện viên: {(fc.Trainer != null ? fc.Trainer.Name : "Chưa phân công")}");
                        }
                        break;
                    case "3":
                        Console.Write("Nhập ID lớp học cần xóa: ");
                        string? classIdToDelete = Console.ReadLine();
                        var classToDelete = FitnessClasses.FirstOrDefault(fc => fc.ClassId == classIdToDelete);
                        if (classToDelete != null)
                        {
                            FitnessClasses.Remove(classToDelete);
                            Console.WriteLine($"[Hệ thống] Lớp học {classToDelete.ClassName} đã được xóa.");
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học với ID đã nhập.");
                        }
                        break;
                    case "4":
                        Console.Write("Nhập ID lớp học cần cập nhật: ");
                        string? classIdToUpdate = Console.ReadLine();
                        var classToUpdate = FitnessClasses.FirstOrDefault(fc => fc.ClassId == classIdToUpdate);
                        if (classToUpdate == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học với ID đã nhập.");
                            break;
                        }

                        Console.WriteLine("\n--- CẬP NHẬT THÔNG TIN LỚP HỌC ---");
                        Console.WriteLine("1. Cập nhật thời gian");
                        Console.WriteLine("2. Cập nhật sức chứa");
                        Console.WriteLine("3. Cập nhật huấn luyện viên");
                        string? classChoiceToUpdate = Console.ReadLine();

                        switch (classChoiceToUpdate)
                        {
                            case "1":
                                while (true)
                                {
                                    Console.Write("Nhập thời gian mới (yyyy-MM-dd HH:mm): ");
                                    string? newScheduleTimeText = Console.ReadLine();
                                    if (DateTime.TryParse(newScheduleTimeText, out DateTime newScheduleTime))
                                    {
                                        classToUpdate.ScheduleTime = newScheduleTime;
                                        Console.WriteLine("[Hệ thống] Thời gian lớp học đã được cập nhật.");
                                        break;
                                    }
                                    Console.WriteLine("[Lỗi] Định dạng thời gian không hợp lệ. Vui lòng nhập lại.");
                                }
                                break;
                            case "2":
                                while (true)
                                {
                                    Console.Write("Nhập sức chứa mới: ");
                                    string? newCapacityText = Console.ReadLine();
                                    if (int.TryParse(newCapacityText, out int newCapacity) && newCapacity > 0)
                                    {
                                        classToUpdate.Capacity = newCapacity;
                                        Console.WriteLine("[Hệ thống] Sức chứa lớp học đã được cập nhật.");
                                        break;
                                    }
                                    Console.Write("[Lỗi] Sức chứa phải là số nguyên dương. Vui lòng nhập lại: ");
                                }
                                break;
                            case "3":
                                Console.Write("Nhập ID huấn luyện viên mới: ");
                                string? newTrainerId = Console.ReadLine();
                                var newTrainer = Trainers.FirstOrDefault(t => t.Id == newTrainerId);
                                if (newTrainer == null)
                                {
                                    Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập.");
                                    break;
                                }
                                classToUpdate.Trainer = newTrainer;
                                Console.WriteLine($"[Hệ thống] Huấn luyện viên của lớp học {classToUpdate.ClassName} đã được cập nhật.");
                                break;
                            default:
                                Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                                break;
                        }
                        break;
                    case "5":
                        Console.Write("Nhập ID huấn luyện viên cần phân công lớp học: ");
                        string? trainerIdForClass = Console.ReadLine();
                        var trainerForClass = Trainers.FirstOrDefault(t => t.Id == trainerIdForClass);
                        if (trainerForClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập.");
                            break;
                        }

                        Console.Write("Nhập ID lớp học cần phân công: ");
                        string? classIdForAssignment = Console.ReadLine();
                        var fitnessClass = FitnessClasses.FirstOrDefault(fc => fc.ClassId == classIdForAssignment);
                        if (fitnessClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học với ID đã nhập.");
                            break;
                        }

                        trainerForClass.AssignClass(fitnessClass);
                        Console.WriteLine($"[Hệ thống] Huấn luyện viên {trainerForClass.Name} đã được phân công lớp học {fitnessClass.ClassName}.");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n[Lỗi] Lựa chọn không hợp lệ. Vui lòng nhập số từ 0 đến 5.");
                        break;
                }
            }
        }
        private readonly List<Booking> BookingHistory = new List<Booking>();

        public void ManageBookings()
        {
            while(true)
            {
                Console.WriteLine("\n--- QUẢN LÝ ĐẶT LỊCH & HÀNG ĐỢI ---");
                Console.WriteLine("1. Đặt lớp cho hội viên (bookClass)");
                Console.WriteLine("2. Hủy đặt lớp (cancelBooking)");
                Console.WriteLine("3. Đôn hội viên từ danh sách chờ (promoteFromWaitlist)");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-3): ");
                string choice = Console.ReadLine();
                switch(choice)
                {
                    case "1":
                    Console.Write("Nhập ID hội viên cần đặt lớp: ");
                    string? memberId = Console.ReadLine();
                    var member = Members.FirstOrDefault(m => m.Id == memberId);
                    if( member == null)
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");  
                        break;
                    } 
                    Console.Write("Nhập ID lớp học: ");
                    string? idclass = Console.ReadLine();
                    var fitnessClass = FitnessClasses.FirstOrDefault(c => c.ClassId == idclass);
                    if (fitnessClass == null)
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                        break;
                    }
                    if (fitnessClass.ScheduleTime <= DateTime.Now)
                    {
                        Console.WriteLine("[Lỗi] Lớp học đã bắt đầu hoặc kết thúc.");
                        break;
                    }
                    if (member.CurrentPlan == null)
                    {
                        Console.WriteLine("[Lỗi] Hội viên chưa có gói tập.");
                        break;
                    }
                    // Kiểm tra hội viên đã đăng ký lớp này chưa
                    bool alreadyBooked = fitnessClass.Roster.Any(b => b.GetMember().Id == memberId) || fitnessClass.WaitList.Any(b => b.GetMember().Id == memberId);
                    if (alreadyBooked)
                    {
                        Console.WriteLine("[Lỗi] Hội viên đã đăng ký lớp này.");
                        break;
                    }
                    // Đếm các lớp đã đăng ký trong cùng tháng
                    int monthlyBookings = FitnessClasses
                    .Where(c =>c.ScheduleTime.Year == fitnessClass.ScheduleTime.Year && c.ScheduleTime.Month == fitnessClass.ScheduleTime.Month)
                    .Sum(c =>c.Roster.Count(b => b.GetMember().Id == memberId) + c.WaitList.Count(b => b.GetMember().Id == memberId));
                    if(!member.CurrentPlan.CanBookClass(monthlyBookings))
                    {
                        Console.WriteLine("[Lỗi] Hội viên đã đạt giới hạn đặt lớp của gói tập.");
                        break;
                    }
                    // Ưu tiên người trong hàng đợi nếu lớp còn chỗ
                    while (fitnessClass.Roster.Count < fitnessClass.Capacity && fitnessClass.WaitList.Count > 0)
                    {
                        Booking next = fitnessClass.WaitList.Dequeue();
                        next.UpdateStatus(BookingStatus.ConFirmed);
                        fitnessClass.Roster.Add(next);
                    }
                    Booking newBooking = fitnessClass.BookClass(member);
                    // Lịch sử Booking
                    BookingHistory.Add(newBooking); 

                    Console.WriteLine($"Booking ID: {newBooking.BookingId}");
                    Console.WriteLine($"Hội viên: {member.Name}");
                    Console.WriteLine($"Lớp học: {fitnessClass.ClassName}");
                    Console.WriteLine($"Trạng thái: {newBooking.GetStatus()}");

                    if (newBooking.GetStatus() == BookingStatus.WaitListed)
                    {
                        Console.WriteLine($"Vị trí hàng đợi: {fitnessClass.WaitList.Count}");
                    }
                    break; 
                    case "2":
                    Console.Write("Nhập ID lớp cần hủy: ");
                    string? cancleclassId = Console.ReadLine();
                    var cancelClass = FitnessClasses.FirstOrDefault(c => c.ClassId == cancleclassId);
                    if( cancelClass  == null)
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy lớp!");
                        break;
                    }
                    Console.Write("Nhập Booking ID cần hủy: ");
                    string? bookingId = Console.ReadLine();
                    var bookingToCancel = cancelClass.Roster.Concat(cancelClass.WaitList).FirstOrDefault(b => b.BookingId == bookingId);
                    if (bookingToCancel == null)
                    {
                        Console.WriteLine("\n[Lỗi] Không tìm thấy booking.");
                        break;
                    }
                    if (bookingToCancel.GetStatus() == BookingStatus.Attended)
                    {
                        Console.WriteLine("[Lỗi] Không thể hủy booking đã điểm danh.");
                        break;
                    }
                    if (cancelClass.ScheduleTime <= DateTime.Now)
                    {
                        Console.WriteLine("[Lỗi] Không thể hủy lớp đã bắt đầu.");
                        break;
                    }
                    // Hàm trong FitnessClass tự động đôn WaitList
                    cancelClass.CancleBooking(bookingId);
                    Console.WriteLine("Hủy đặt lớp thành công.");
                    break;

                    case "3":
                    Console.Write("Nhập ID lớp học: ");
                    string? promoteClassId = Console.ReadLine();

                    var promoteClass = FitnessClasses.FirstOrDefault(c => c.ClassId == promoteClassId);

                    if (promoteClass == null)
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                        break;
                    }

                    if (promoteClass.ScheduleTime <= DateTime.Now)
                    {
                        Console.WriteLine("[Lỗi] Lớp học đã bắt đầu.");
                        break;
                    }

                    if (promoteClass.Roster.Count >= promoteClass.Capacity)
                    {
                        Console.WriteLine("Lớp học đã đủ chỗ.");
                        break;
                    }

                    if (promoteClass.WaitList.Count == 0)
                    {
                        Console.WriteLine("Danh sách chờ đang trống.");
                        break;
                    }
                    // Đôn theo thứ tự FIFO
                    while (promoteClass.WaitList.Count > 0 && promoteClass.Roster.Count < promoteClass.Capacity)
                    {
                        Booking nextBooking = promoteClass.WaitList.Dequeue();

                        nextBooking.UpdateStatus(BookingStatus.ConFirmed);
                        promoteClass.Roster.Add(nextBooking);

                        Console.WriteLine($"Đã xác nhận: {nextBooking.GetMember().Name}");
                    }
                    break;

                    case "4":
                    Console.WriteLine("\n--- DANH SÁCH ĐẶT LỚP ---");
                    foreach (var fc in FitnessClasses)
                    {
                        Console.WriteLine($"\nLớp: {fc.ClassName} ({fc.ClassId})");
                        Console.WriteLine($"Thời gian: {fc.ScheduleTime:dd/MM/yyyy HH:mm}");
                        Console.WriteLine($"Số chỗ: {fc.Roster.Count}/{fc.Capacity}");

                        Console.WriteLine("Danh sách chính thức:");
                        if (fc.Roster.Count == 0)
                            Console.WriteLine("  Chưa có hội viên.");

                        foreach (var booking in fc.Roster)
                        {
                            Console.WriteLine($"  ID: {booking.BookingId} | " +$"Hội viên: {booking.GetMember().Name} | " +$"Trạng thái: {booking.GetStatus()}");
                        }

                        Console.WriteLine("Danh sách chờ:");

                        if (fc.WaitList.Count == 0)
                            Console.WriteLine("  Danh sách chờ trống.");

                        foreach (var booking in fc.WaitList)
                        {
                            Console.WriteLine($"  ID: {booking.BookingId} | " +$"Hội viên: {booking.GetMember().Name} | " +$"Trạng thái: {booking.GetStatus()}");
                        }
                    }
                    break;

                    case "0":
                    return;

                    default:
                    Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                    break;
                }
            }
        }

        public void MarkAttendance()
        {
            Console.WriteLine("\n--- ĐIỂM DANH ---");
            Console.WriteLine("1. Xem danh sách tham gia chính thức (getConfirmedAttendees)");
            Console.WriteLine("2. Đánh dấu có mặt (markAttendance)");
            Console.WriteLine("0. Quay lại menu chính");
            Console.WriteLine("Vui lòng chọn chức năng (0-3): ");
            string choice = Console.ReadLine();
            switch(choice)
            {
                case "1":
                Console.Write("Nhập ID lớp học: ");
                string? classId = Console.ReadLine();

                var fitnessClass = FitnessClasses.FirstOrDefault(c => c.ClassId == classId);

                if (fitnessClass == null)
                {
                    Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                    break;
                }

                // Lấy danh sách hội viên từ Roster
                List<Member> attendees = fitnessClass.GetConfirmedAttendees();

                Console.WriteLine($"\nLớp: {fitnessClass.ClassName}");
                Console.WriteLine("--- DANH SÁCH HỘI VIÊN ---");

                if (attendees.Count == 0)
                {
                    Console.WriteLine("Chưa có hội viên đăng ký.");
                    break;
                }

                foreach (var member in attendees)
                {
                    var booking = fitnessClass.Roster.First(b => b.GetMember().Id == member.Id);
                    Console.WriteLine($"ID: {member.Id} | " +$"Tên: {member.Name} | " +$"Trạng thái: {booking.GetStatus()}");
                }
                break;

                case "2":
                Console.Write("Nhập ID lớp học: ");
                string? attendanceClassId = Console.ReadLine();

                var attendanceClass = FitnessClasses.FirstOrDefault(c => c.ClassId == attendanceClassId);

                if (attendanceClass == null)
                {
                    Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                    break;
                }

                if (DateTime.Now < attendanceClass.ScheduleTime)
                {
                    Console.WriteLine("[Lỗi] Lớp học chưa bắt đầu.");
                    break;
                }

                Console.Write("Nhập ID hội viên cần điểm danh: ");
                string? memberId = Console.ReadLine();
                // Chỉ tìm trong danh sách chính thức
                var bookingToMark = attendanceClass.Roster.FirstOrDefault(b => b.GetMember().Id == memberId);

                if (bookingToMark == null)
                {
                    Console.WriteLine("[Lỗi] Hội viên không có trong danh sách chính thức.");
                    break;
                }

                if (bookingToMark.GetStatus() == BookingStatus.Attended)
                {
                    Console.WriteLine("Hội viên đã được điểm danh.");
                    break;
                }

                if (bookingToMark.GetStatus() != BookingStatus.ConFirmed)
                {
                    Console.WriteLine("[Lỗi] Booking chưa được xác nhận.");
                    break;
                }

                // Gọi phương thức có sẵn trong FitnessClass
                attendanceClass.MarkAttendance(memberId!);

                Console.WriteLine($"Điểm danh thành công: " +$"{bookingToMark.GetMember().Name}");

                Console.WriteLine($"Trạng thái mới: {bookingToMark.GetStatus()}");
                break;

                case "0":
                return;

                default:
                Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                break;
            }
        }

        public void GenerateReports()
        {
            ActivityReportService reportService = new ActivityReportService();
            while (true)
            {
            Console.WriteLine("\n--- BÁO CÁO HOẠT ĐỘNG ---");
            Console.WriteLine("1. Xuất báo cáo theo hội viên");
            Console.WriteLine("2. Xuất báo cáo tất cả hội viên");
            Console.WriteLine("3. Xem thống kê hoạt động lớp học");
            Console.WriteLine("0. Quay lại menu chính");
            Console.WriteLine("Vui lòng chọn chức năng (0-3): ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Nhập ID hội viên: ");
                    string? memberId = Console.ReadLine();

                    var member = Members.FirstOrDefault(m => m.Id == memberId);

                    if (member == null)
                    {
                        Console.WriteLine("[Lỗi] Không tìm thấy hội viên.");
                        break;
                    }

                    // Lấy lịch sử booking của hội viên
                    List<Booking> memberBookings = BookingHistory.Where(b => b.GetMember().Id == memberId).ToList();
                    // Gọi ActivityReportService
                    string report = reportService.generateMemberReport(member, memberBookings);
                    Console.WriteLine(report);
                    break;

                case "2":
                    Console.WriteLine("\n--- BÁO CÁO TẤT CẢ HỘI VIÊN ---");

                    if (Members.Count == 0)
                    {
                        Console.WriteLine("Chưa có hội viên.");
                        break;
                    }

                    foreach (var m in Members)
                    {
                        List<Booking> bookings = BookingHistory.Where(b => b.GetMember().Id == m.Id).ToList();
                        Console.WriteLine(reportService.generateMemberReport(m, bookings));
                    }
                    break;

                case "3":
                    Console.WriteLine("\n--- THỐNG KÊ HOẠT ĐỘNG ---");

                    Console.WriteLine($"Tổng số hội viên: {Members.Count}");
                    Console.WriteLine($"Tổng số huấn luyện viên: {Trainers.Count}");
                    Console.WriteLine($"Tổng số lớp học: {FitnessClasses.Count}");
                    Console.WriteLine($"Tổng lượt đặt lớp: {BookingHistory.Count}");

                    int confirmed = BookingHistory.Count(b => b.GetStatus() == BookingStatus.ConFirmed);
                    int waitlisted = BookingHistory.Count(b => b.GetStatus() == BookingStatus.WaitListed);
                    int cancelled = BookingHistory.Count(b => b.GetStatus() == BookingStatus.Cancelled);
                    int attended = BookingHistory.Count(b => b.GetStatus() == BookingStatus.Attended);

                    Console.WriteLine($"Đã xác nhận: {confirmed}");
                    Console.WriteLine($"Đang chờ: {waitlisted}");
                    Console.WriteLine($"Đã hủy: {cancelled}");
                    Console.WriteLine($"Đã tham gia: {attended}");

                    Console.WriteLine("\n--- THỐNG KÊ TỪNG LỚP ---");

                    foreach (var fc in FitnessClasses)
                    {
                        int totalAttended = fc.Roster.Count(b => b.GetStatus() == BookingStatus.Attended);
                        Console.WriteLine($"\nLớp: {fc.ClassName}");
                        Console.WriteLine($"ID: {fc.ClassId}");
                        Console.WriteLine($"Sức chứa: {fc.Capacity}");
                        Console.WriteLine($"Đã đăng ký: {fc.Roster.Count}");
                        Console.WriteLine($"Đang chờ: {fc.WaitList.Count}");
                        Console.WriteLine($"Đã tham gia: {totalAttended}");
                        Console.WriteLine(
                            $"Còn trống: {Math.Max(0, fc.Capacity - fc.Roster.Count)}");
                    }
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                    break;
                }
            }
        }
    }
}