using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OOP_project
{
    public partial class Admin
    {
        private static Admin? _instance;

        public List<Member> Members { get; }
        public List<Trainer> Trainers { get; }
        public List<FitnessClass> FitnessClasses { get; }
        private readonly List<Booking> _bookingHistory = [];

        // Các record DTO phẳng dùng để lưu/đọc 4 file JSON (ngăn lỗi vòng lặp tham chiếu & abstract class)
        private record MemberDto(string Id, string Name, string Email, string PhoneNumber, string? PlanType, DateTime JoinDate, DateTime PlanExpiryDate);
        private record TrainerDto(string Id, string Name, string Email, string PhoneNumber, string Specialization);
        private record FitnessClassDto(string ClassId, string ClassName, DateTime ScheduleTime, int DurationMinutes, int Capacity, string TrainerId);
        private record BookingDto(string BookingId, string MemberId, string ClassId, DateTime BookingDate, BookingStatus Status);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        // Private constructor cho Singleton Pattern: Tự động load dữ liệu cũ từ folder "data"
        private Admin()
        {
            Members = [];
            Trainers = [];
            FitnessClasses = [];

            LoadData();
        }

        public static Admin GetInstance()
        {
            return _instance ??= new Admin();
        }

        // Xác định đường dẫn thư mục "data" trong thư mục gốc của project
        private static string GetDataDirectory()
        {
            string current = Directory.GetCurrentDirectory();
            if (File.Exists(Path.Combine(current, "OOP_project.csproj")))
                return Path.Combine(current, "data");

            DirectoryInfo? dir = new(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "OOP_project.csproj")))
                    return Path.Combine(dir.FullName, "data");
                dir = dir.Parent;
            }
            return Path.Combine(current, "data");
        }

        // Đọc dữ liệu từ 4 file JSON trong folder "data"
        public void LoadData()
        {
            string dataDir = GetDataDirectory();
            Directory.CreateDirectory(dataDir);

            string trainersPath = Path.Combine(dataDir, "trainers.json");
            string membersPath = Path.Combine(dataDir, "members.json");
            string classesPath = Path.Combine(dataDir, "fitnessClasses.json");
            string bookingsPath = Path.Combine(dataDir, "bookings.json");

            Members.Clear();
            Trainers.Clear();
            FitnessClasses.Clear();
            _bookingHistory.Clear();

            // 1. Nạp danh sách Huấn luyện viên (Trainers)
            if (File.Exists(trainersPath))
            {
                var trainerDtos = JsonSerializer.Deserialize<List<TrainerDto>>(File.ReadAllText(trainersPath), JsonOptions) ?? [];
                foreach (var dto in trainerDtos)
                {
                    Trainers.Add(new Trainer(dto.Id, dto.Name, dto.Email, dto.PhoneNumber, dto.Specialization));
                }
            }

            // 2. Nạp danh sách Hội viên (Members)
            if (File.Exists(membersPath))
            {
                var memberDtos = JsonSerializer.Deserialize<List<MemberDto>>(File.ReadAllText(membersPath), JsonOptions) ?? [];
                foreach (var dto in memberDtos)
                {
                    MembershipPlan? plan = dto.PlanType switch
                    {
                        "Basic" or "Basic Plan" => new BasicPlan(),
                        "Premium" or "Premium Plan" => new PremiumPlan(),
                        _ => null
                    };

                    var member = new Member(dto.Id, dto.Name, dto.Email, dto.PhoneNumber, plan)
                    {
                        JoinDate = dto.JoinDate,
                        PlanExpiryDate = dto.PlanExpiryDate
                    };
                    Members.Add(member);
                }
            }

            // 3. Nạp danh sách Lớp học (FitnessClasses)
            if (File.Exists(classesPath))
            {
                var classDtos = JsonSerializer.Deserialize<List<FitnessClassDto>>(File.ReadAllText(classesPath), JsonOptions) ?? [];
                foreach (var dto in classDtos)
                {
                    var trainer = Trainers.FirstOrDefault(t => t.Id.Equals(dto.TrainerId, StringComparison.OrdinalIgnoreCase));
                    if (trainer != null)
                    {
                        int duration = dto.DurationMinutes is > 0 and <= 1440 ? dto.DurationMinutes : 60;
                        var fitnessClass = new FitnessClass(dto.ClassId, dto.ClassName, dto.ScheduleTime, dto.Capacity, trainer, duration);
                        FitnessClasses.Add(fitnessClass);
                        RegisterClass(fitnessClass);
                    }
                }
            }

            // 4. Nạp lịch sử Đặt chỗ (Bookings) và khôi phục Roster / WaitList
            if (File.Exists(bookingsPath))
            {
                var bookingDtos = JsonSerializer.Deserialize<List<BookingDto>>(File.ReadAllText(bookingsPath), JsonOptions) ?? [];
                foreach (var dto in bookingDtos)
                {
                    var member = Members.FirstOrDefault(m => m.Id.Equals(dto.MemberId, StringComparison.OrdinalIgnoreCase));
                    var fitnessClass = FitnessClasses.FirstOrDefault(c => c.ClassId.Equals(dto.ClassId, StringComparison.OrdinalIgnoreCase));
                    if (member != null && fitnessClass != null)
                    {
                        var booking = new Booking(member, fitnessClass, dto.BookingDate, dto.Status, dto.BookingId);
                        _bookingHistory.Add(booking);
                        fitnessClass.RestoreBooking(booking);
                    }
                }
            }

            Console.WriteLine($"[Hệ thống] Đã tải dữ liệu từ thư mục 'data' ({Members.Count} hội viên, {Trainers.Count} HLV, {FitnessClasses.Count} lớp học, {_bookingHistory.Count} lượt đặt chỗ).");
        }

        // Lưu toàn bộ 4 danh sách xuống 4 file JSON trong folder "data"
        public void SaveData()
        {
            string dataDir = GetDataDirectory();
            Directory.CreateDirectory(dataDir);

            var trainerDtos = Trainers.Select(t => new TrainerDto(
                t.Id, t.Name, t.Email, t.PhoneNumber, t.Specialization)).ToList();

            var memberDtos = Members.Select(m => new MemberDto(
                m.Id, m.Name, m.Email, m.PhoneNumber, m.CurrentPlan?.PlanName, m.JoinDate, m.PlanExpiryDate)).ToList();

            var classDtos = FitnessClasses.Select(c => new FitnessClassDto(
                c.ClassId, c.ClassName, c.ScheduleTime, c.DurationMinutes, c.Capacity, c.Trainer.Id)).ToList();

            var bookingDtos = _bookingHistory.Select(b => new BookingDto(
                b.BookingId, b.Member.Id, b.FitnessClass.ClassId, b.BookingDate, b.Status)).ToList();

            File.WriteAllText(Path.Combine(dataDir, "trainers.json"), JsonSerializer.Serialize(trainerDtos, JsonOptions));
            File.WriteAllText(Path.Combine(dataDir, "members.json"), JsonSerializer.Serialize(memberDtos, JsonOptions));
            File.WriteAllText(Path.Combine(dataDir, "fitnessClasses.json"), JsonSerializer.Serialize(classDtos, JsonOptions));
            File.WriteAllText(Path.Combine(dataDir, "bookings.json"), JsonSerializer.Serialize(bookingDtos, JsonOptions));
        }

        // ==========================================
        // 1. QUẢN LÝ HỘI VIÊN (MEMBERS)
        // ==========================================
        public void ManageMembers()
        {
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ HỘI VIÊN ---");
                Console.WriteLine("1. Thêm hội viên mới ");
                Console.WriteLine("2. Xem danh sách hội viên");
                Console.WriteLine("3. Cập nhật thông tin hội viên");
                Console.WriteLine("4. Thay đổi gói tập ");
                Console.WriteLine("5. Xóa hội viên");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-5): ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.WriteLine("\nNhập thông tin hội viên mới:");
                        string name = ConsoleHelper.ReadNonEmptyString("Tên: ");
                        string email = ConsoleHelper.ReadEmail("Email: ");
                        string phone = ConsoleHelper.ReadPhoneNumber("Số điện thoại: ");
                        int planChoice = ConsoleHelper.ReadInt("Chọn gói tập (1: Basic Plan, 2: Premium Plan): ", 1, 2);

                        MembershipPlan plan = planChoice == 1 ? new BasicPlan() : new PremiumPlan();

                        // Constructor tự sinh ID từ biến static
                        var newMember = new Member(name, email, phone, plan);
                        Members.Add(newMember);
                        SaveData();

                        Console.WriteLine($"\n[Hệ thống] Tạo thành công hội viên: {newMember.Name} | Mã số tự sinh: {newMember.Id} | Gói: {newMember.CurrentPlan?.PlanName}");
                        break;

                    case "2":
                        Console.WriteLine("\n--- DANH SÁCH HỘI VIÊN ---");
                        if (Members.Count == 0)
                        {
                            Console.WriteLine("Chưa có hội viên nào trong hệ thống.");
                            break;
                        }
                        foreach (var m in Members)
                        {
                            Console.WriteLine($"ID: {m.Id,-5} | Tên: {m.Name,-18} | Email: {m.Email,-24} | SĐT: {m.PhoneNumber,-11} | Gói: {m.CurrentPlan?.PlanName ?? "Chưa có"} | Đã book tháng: {m.CurrentMonthlyBookings}");
                        }
                        break;

                    case "3":
                        Console.Write("Nhập ID hội viên cần cập nhật: ");
                        string? updateId = Console.ReadLine()?.Trim();
                        var memberToUpdate = Members.FirstOrDefault(m => m.Id.Equals(updateId, StringComparison.OrdinalIgnoreCase));
                        if (memberToUpdate == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
                            break;
                        }

                        Console.WriteLine("\n--- CẬP NHẬT THÔNG TIN HỘI VIÊN ---");
                        Console.WriteLine("1. Cập nhật tên");
                        Console.WriteLine("2. Cập nhật email");
                        Console.WriteLine("3. Cập nhật số điện thoại");
                        Console.Write("Chọn (1-3): ");
                        string? updateChoice = Console.ReadLine()?.Trim();

                        switch (updateChoice)
                        {
                            case "1":
                                memberToUpdate.Name = ConsoleHelper.ReadNonEmptyString("Tên mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Cập nhật tên thành công.");
                                break;
                            case "2":
                                memberToUpdate.Email = ConsoleHelper.ReadEmail("Email mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Cập nhật email thành công.");
                                break;
                            case "3":
                                memberToUpdate.PhoneNumber = ConsoleHelper.ReadPhoneNumber("Số điện thoại mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Cập nhật số điện thoại thành công.");
                                break;
                            default:
                                Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                                break;
                        }
                        break;

                    case "4":
                        Console.Write("Nhập ID hội viên cần đổi gói tập: ");
                        string? planMemberId = Console.ReadLine()?.Trim();
                        var memberPlan = Members.FirstOrDefault(m => m.Id.Equals(planMemberId, StringComparison.OrdinalIgnoreCase));
                        if (memberPlan == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
                            break;
                        }

                        Console.WriteLine($"Gói tập hiện tại: {memberPlan.CurrentPlan?.PlanName ?? "Chưa có"}");
                        int newPlanChoice = ConsoleHelper.ReadInt("Chọn gói tập mới (1: Basic, 2: Premium): ", 1, 2);

                        if (newPlanChoice == 1)
                        {
                            if (memberPlan.CurrentPlan is BasicPlan)
                            {
                                Console.WriteLine($"[Thông báo] Hội viên {memberPlan.Name} hiện đã đang ở gói Basic Plan.");
                            }
                            else
                            {
                                memberPlan.ChangePlan(new BasicPlan());
                                SaveData();
                                Console.WriteLine($"[Hệ thống] Đã chuyển đổi sang gói Basic Plan cho {memberPlan.Name}.");
                            }
                        }
                        else
                        {
                            if (memberPlan.CurrentPlan is PremiumPlan)
                            {
                                Console.WriteLine($"[Thông báo] Hội viên {memberPlan.Name} hiện đã đang ở gói Premium Plan.");
                            }
                            else
                            {
                                memberPlan.ChangePlan(new PremiumPlan());
                                SaveData();
                                Console.WriteLine($"[Hệ thống] Đã nâng cấp lên gói Premium Plan cho {memberPlan.Name}.");
                            }
                        }
                        break;

                    case "5":
                        Console.Write("Nhập ID hội viên cần xóa: ");
                        string? deleteId = Console.ReadLine()?.Trim();
                        var memberToDelete = Members.FirstOrDefault(m => m.Id.Equals(deleteId, StringComparison.OrdinalIgnoreCase));
                        if (memberToDelete != null)
                        {
                            if (TryDeleteMember(memberToDelete, out string deleteMemberError))
                            {
                                SaveData();
                                Console.WriteLine($"[Hệ thống] Đã xóa hội viên {memberToDelete.Name} (ID: {memberToDelete.Id}) và hủy các booking liên quan.");
                            }
                            else
                            {
                                Console.WriteLine($"[Lỗi] {deleteMemberError}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
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

        // ==========================================
        // 2. QUẢN LÝ HUẤN LUYỆN VIÊN (TRAINERS)
        // ==========================================
        public void ManageTrainers()
        {
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ HUẤN LUYỆN VIÊN ---");
                Console.WriteLine("1. Thêm huấn luyện viên mới ");
                Console.WriteLine("2. Xem danh sách huấn luyện viên");
                Console.WriteLine("3. Cập nhật thông tin huấn luyện viên");
                Console.WriteLine("4. Phân công huấn luyện viên cho lớp");
                Console.WriteLine("5. Xóa huấn luyện viên");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-5): ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.WriteLine("\nNhập thông tin huấn luyện viên mới:");
                        string name = ConsoleHelper.ReadNonEmptyString("Tên: ");
                        string email = ConsoleHelper.ReadEmail("Email: ");
                        string phone = ConsoleHelper.ReadPhoneNumber("Số điện thoại: ");
                        string spec = ConsoleHelper.ReadNonEmptyString("Chuyên môn (vd: Yoga, Zumba, Pilates): ");

                        // Constructor tự sinh ID dựa vào biến static
                        var newTrainer = new Trainer(name, email, phone, spec);
                        Trainers.Add(newTrainer);
                        SaveData();

                        Console.WriteLine($"\n[Hệ thống] Thêm thành công HLV: {newTrainer.Name} | Mã số tự sinh: {newTrainer.Id} | Chuyên môn: {newTrainer.Specialization}");
                        break;

                    case "2":
                        Console.WriteLine("\n--- DANH SÁCH HUẤN LUYỆN VIÊN ---");
                        if (Trainers.Count == 0)
                        {
                            Console.WriteLine("Chưa có huấn luyện viên nào trong hệ thống.");
                            break;
                        }
                        foreach (var t in Trainers)
                        {
                            Console.WriteLine($"ID: {t.Id,-5} | Tên: {t.Name,-18} | Email: {t.Email,-24} | SĐT: {t.PhoneNumber,-11} | Chuyên môn: {t.Specialization}");
                        }
                        break;

                    case "3":
                        Console.Write("Nhập ID huấn luyện viên cần sửa: ");
                        string? trainerId = Console.ReadLine()?.Trim();
                        var trainer = Trainers.FirstOrDefault(t => t.Id.Equals(trainerId, StringComparison.OrdinalIgnoreCase));
                        if (trainer == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập.");
                            break;
                        }

                        Console.WriteLine("\n--- CẬP NHẬT THÔNG TIN HUẤN LUYỆN VIÊN ---");
                        Console.WriteLine("1. Cập nhật tên");
                        Console.WriteLine("2. Cập nhật email");
                        Console.WriteLine("3. Cập nhật số điện thoại");
                        Console.WriteLine("4. Cập nhật chuyên môn");
                        Console.Write("Lựa chọn (1-4): ");
                        string? choice = Console.ReadLine()?.Trim();

                        switch (choice)
                        {
                            case "1":
                                trainer.Name = ConsoleHelper.ReadNonEmptyString("Tên mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Cập nhật tên thành công.");
                                break;
                            case "2":
                                trainer.Email = ConsoleHelper.ReadEmail("Email mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Cập nhật email thành công.");
                                break;
                            case "3":
                                trainer.PhoneNumber = ConsoleHelper.ReadPhoneNumber("Số điện thoại mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Cập nhật số điện thoại thành công.");
                                break;
                            case "4":
                                trainer.Specialization = ConsoleHelper.ReadNonEmptyString("Chuyên môn mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Cập nhật chuyên môn thành công.");
                                break;
                            default:
                                Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                                break;
                        }
                        break;

                    case "4":
                        Console.Write("Nhập ID huấn luyện viên: ");
                        string? tId = Console.ReadLine()?.Trim();
                        var assignTrainer = Trainers.FirstOrDefault(t => t.Id.Equals(tId, StringComparison.OrdinalIgnoreCase));
                        if (assignTrainer == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên.");
                            break;
                        }

                        Console.Write("Nhập ID lớp học cần phân công: ");
                        string? cId = Console.ReadLine()?.Trim();
                        var assignClass = FitnessClasses.FirstOrDefault(fc => fc.ClassId.Equals(cId, StringComparison.OrdinalIgnoreCase));
                        if (assignClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                            break;
                        }

                        if (TryAssignTrainer(assignClass, assignTrainer, out string assignError))
                        {
                            SaveData();
                            Console.WriteLine($"[Hệ thống] Đã phân công HLV {assignTrainer.Name} cho lớp {assignClass.ClassName}.");
                        }
                        else
                        {
                            Console.WriteLine($"[Lỗi] {assignError}");
                        }
                        break;

                    case "5":
                        Console.Write("Nhập ID huấn luyện viên cần xóa: ");
                        string? delId = Console.ReadLine()?.Trim();
                        var toDel = Trainers.FirstOrDefault(t => t.Id.Equals(delId, StringComparison.OrdinalIgnoreCase));
                        if (toDel != null)
                        {
                            if (TryDeleteTrainer(toDel, out string deleteTrainerError))
                            {
                                SaveData();
                                Console.WriteLine($"[Hệ thống] Huấn luyện viên {toDel.Name} đã được xóa.");
                            }
                            else
                            {
                                Console.WriteLine($"[Lỗi] {deleteTrainerError}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên với ID đã nhập.");
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

        // ==========================================
        // 3. QUẢN LÝ LỚP HỌC (FITNESS CLASSES)
        // ==========================================
        public void ManageFitnessClasses()
        {
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ LỚP HỌC (FITNESS CLASS) ---");
                Console.WriteLine("1. Tạo lớp học mới ");
                Console.WriteLine("2. Xem danh sách lớp học");
                Console.WriteLine("3. Cập nhật thông tin lớp học");
                Console.WriteLine("4. Phân công HLV cho lớp");
                Console.WriteLine("5. Xóa lớp học");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-5): ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        if (Trainers.Count == 0)
                        {
                            Console.WriteLine("[Lỗi] Chưa có huấn luyện viên trong hệ thống. Vui lòng thêm HLV trước.");
                            break;
                        }

                        Console.WriteLine("\nNhập thông tin lớp học mới:");
                        string className = ConsoleHelper.ReadNonEmptyString("Tên lớp học (ví dụ: Yoga Class, Zumba Class): ");
                        DateTime schedule = ConsoleHelper.ReadDateTime("Thời gian diễn ra (yyyy-MM-dd HH:mm): ", futureOnly: true);
                        int durationMinutes = ConsoleHelper.ReadInt("Thời lượng (phút, 1-1440; thường 60): ", 1, 1440);
                        int capacity = ConsoleHelper.ReadInt("Sức chứa tối đa của lớp: ", min: 1);

                        Console.WriteLine("\nChọn huấn luyện viên phụ trách:");
                        for (int i = 0; i < Trainers.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {Trainers[i].Name} ({Trainers[i].Specialization}) [ID: {Trainers[i].Id}]");
                        }
                        int trainerIndex = ConsoleHelper.ReadInt("Chọn số thứ tự HLV: ", 1, Trainers.Count) - 1;
                        Trainer selectedTrainer = Trainers[trainerIndex];

                        if (TryCreateClass(className, schedule, durationMinutes, capacity,
                            selectedTrainer, out FitnessClass? newClass, out string createError))
                        {
                            SaveData();
                            Console.WriteLine($"\n[Hệ thống] Tạo lớp thành công: {newClass!.ClassName} | Mã: {newClass.ClassId} | Thời lượng: {newClass.DurationMinutes} phút | HLV: {newClass.Trainer.Name}");
                        }
                        else
                        {
                            Console.WriteLine($"[Lỗi] Không thể tạo lớp: {createError}");
                        }
                        break;

                    case "2":
                        Console.WriteLine("\n--- DANH SÁCH LỚP HỌC ---");
                        if (FitnessClasses.Count == 0)
                        {
                            Console.WriteLine("Chưa có lớp học nào.");
                            break;
                        }
                        foreach (var fc in FitnessClasses)
                        {
                            Console.WriteLine($"ID: {fc.ClassId,-5} | Lớp: {fc.ClassName,-16} | Bắt đầu: {fc.ScheduleTime:dd/MM/yyyy HH:mm} | Kết thúc: {fc.EndTime:dd/MM/yyyy HH:mm} | Chỗ: {fc.Roster.Count}/{fc.Capacity} | Chờ: {fc.WaitList.Count} | HLV: {fc.Trainer.Name}");
                        }
                        break;

                    case "3":
                        Console.Write("Nhập ID lớp học cần cập nhật: ");
                        string? classId = Console.ReadLine()?.Trim();
                        var classToUpdate = FitnessClasses.FirstOrDefault(fc => fc.ClassId.Equals(classId, StringComparison.OrdinalIgnoreCase));
                        if (classToUpdate == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học với ID đã nhập.");
                            break;
                        }

                        Console.WriteLine("\n--- CẬP NHẬT LỚP HỌC ---");
                        Console.WriteLine("1. Cập nhật tên lớp");
                        Console.WriteLine("2. Cập nhật thời gian");
                        Console.WriteLine("3. Cập nhật sức chứa");
                        Console.WriteLine("4. Cập nhật thời lượng");
                        Console.Write("Chọn (1-4): ");
                        string? updateOption = Console.ReadLine()?.Trim();

                        switch (updateOption)
                        {
                            case "1":
                                classToUpdate.ClassName = ConsoleHelper.ReadNonEmptyString("Tên lớp mới: ");
                                SaveData();
                                Console.WriteLine("[Hệ thống] Đã cập nhật tên lớp.");
                                break;
                            case "2":
                                DateTime newStart = ConsoleHelper.ReadDateTime("Thời gian mới (yyyy-MM-dd HH:mm): ", futureOnly: true);
                                if (TryUpdateClassSchedule(classToUpdate, newStart, classToUpdate.DurationMinutes, out string scheduleError))
                                {
                                    SaveData();
                                    Console.WriteLine("[Hệ thống] Đã cập nhật thời gian lớp học.");
                                }
                                else
                                {
                                    Console.WriteLine($"[Lỗi] {scheduleError}");
                                }
                                break;
                            case "3":
                                int newCap = ConsoleHelper.ReadInt("Sức chứa mới: ", min: 1);
                                if (newCap < classToUpdate.Roster.Count)
                                {
                                    Console.WriteLine($"[Lỗi] Sức chứa mới ({newCap}) nhỏ hơn số hội viên đang có ({classToUpdate.Roster.Count}).");
                                    break;
                                }
                                classToUpdate.Capacity = newCap;
                                while (classToUpdate.Roster.Count < classToUpdate.Capacity && classToUpdate.PromoteFromWaitlist()) { }
                                SaveData();
                                Console.WriteLine("[Hệ thống] Đã cập nhật sức chứa lớp học.");
                                break;
                            case "4":
                                int newDuration = ConsoleHelper.ReadInt("Thời lượng mới (phút): ", 1, 1440);
                                if (TryUpdateClassSchedule(classToUpdate, classToUpdate.ScheduleTime, newDuration, out string durationError))
                                {
                                    SaveData();
                                    Console.WriteLine("[Hệ thống] Đã cập nhật thời lượng lớp học.");
                                }
                                else
                                {
                                    Console.WriteLine($"[Lỗi] {durationError}");
                                }
                                break;
                            default:
                                Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                                break;
                        }
                        break;

                    case "4":
                        Console.Write("Nhập ID lớp học: ");
                        string? cIdToAssign = Console.ReadLine()?.Trim();
                        var fcAssign = FitnessClasses.FirstOrDefault(fc => fc.ClassId.Equals(cIdToAssign, StringComparison.OrdinalIgnoreCase));
                        if (fcAssign == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                            break;
                        }

                        Console.Write("Nhập ID HLV mới: ");
                        string? trId = Console.ReadLine()?.Trim();
                        var newTr = Trainers.FirstOrDefault(t => t.Id.Equals(trId, StringComparison.OrdinalIgnoreCase));
                        if (newTr == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy huấn luyện viên.");
                            break;
                        }

                        if (TryAssignTrainer(fcAssign, newTr, out string reassignError))
                        {
                            SaveData();
                            Console.WriteLine($"[Hệ thống] Huấn luyện viên của lớp {fcAssign.ClassName} đã được cập nhật thành {newTr.Name}.");
                        }
                        else
                        {
                            Console.WriteLine($"[Lỗi] {reassignError}");
                        }
                        break;

                    case "5":
                        Console.Write("Nhập ID lớp học cần xóa: ");
                        string? delClassId = Console.ReadLine()?.Trim();
                        var toDelClass = FitnessClasses.FirstOrDefault(fc => fc.ClassId.Equals(delClassId, StringComparison.OrdinalIgnoreCase));
                        if (toDelClass != null)
                        {
                            if (TryDeleteClass(toDelClass, out string deleteClassError))
                            {
                                SaveData();
                                Console.WriteLine($"[Hệ thống] Lớp học {toDelClass.ClassName} đã được xóa và các booking chưa điểm danh đã hủy.");
                            }
                            else
                            {
                                Console.WriteLine($"[Lỗi] {deleteClassError}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học với ID đã nhập.");
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

        // ==========================================
        // 4. QUẢN LÝ ĐẶT LỊCH & HÀNG ĐỢI (BOOKINGS)
        // ==========================================
        public void ManageBookings()
        {
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ ĐẶT LỊCH & HÀNG ĐỢI ---");
                Console.WriteLine("1. Đặt lớp cho hội viên ");
                Console.WriteLine("2. Hủy đặt lớp ");
                Console.WriteLine("3. Đôn hội viên từ danh sách chờ ");
                Console.WriteLine("4. Xem danh sách đặt lớp theo từng lớp");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-4): ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.Write("Nhập ID hội viên cần đặt lớp: ");
                        string? memberId = Console.ReadLine()?.Trim();
                        var member = Members.FirstOrDefault(m => m.Id.Equals(memberId, StringComparison.OrdinalIgnoreCase));
                        if (member == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên với ID đã nhập.");
                            break;
                        }

                        if (member.CurrentPlan == null)
                        {
                            Console.WriteLine("[Lỗi] Hội viên chưa đăng ký gói tập nào.");
                            break;
                        }

                        Console.Write("Nhập ID lớp học: ");
                        string? classId = Console.ReadLine()?.Trim();
                        var fitnessClass = FitnessClasses.FirstOrDefault(c => c.ClassId.Equals(classId, StringComparison.OrdinalIgnoreCase));
                        if (fitnessClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                            break;
                        }

                        if (!TryBookClass(member, fitnessClass, out Booking? newBooking, out string bookingError))
                        {
                            Console.WriteLine($"[Lỗi] {bookingError}");
                            break;
                        }

                        SaveData();

                        Console.WriteLine("\n[Hệ thống] Đặt lớp thành công!");
                        Console.WriteLine($"Mã đặt chỗ : {newBooking!.BookingId}");
                        Console.WriteLine($"Hội viên   : {member.Name}");
                        Console.WriteLine($"Lớp học    : {fitnessClass.ClassName}");
                        Console.WriteLine($"Trạng thái : {newBooking.Status}");

                        if (newBooking.Status == BookingStatus.WaitListed)
                        {
                            Console.WriteLine($"-> Lớp đã đầy, bạn ở vị trí thứ {fitnessClass.WaitList.Count} trong danh sách chờ.");
                        }
                        break;

                    case "2":
                        Console.Write("Nhập ID lớp học: ");
                        string? cancelClassId = Console.ReadLine()?.Trim();
                        var cancelClass = FitnessClasses.FirstOrDefault(c => c.ClassId.Equals(cancelClassId, StringComparison.OrdinalIgnoreCase));
                        if (cancelClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                            break;
                        }

                        Console.Write("Nhập Booking ID cần hủy: ");
                        string? bookingId = Console.ReadLine()?.Trim();

                        var booking = cancelClass.Roster.Concat(cancelClass.WaitList)
                            .FirstOrDefault(b => b.BookingId.Equals(bookingId, StringComparison.OrdinalIgnoreCase));

                        if (booking == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy booking ID trong lớp học này.");
                            break;
                        }

                        if (booking.Status == BookingStatus.Attended)
                        {
                            Console.WriteLine("[Lỗi] Không thể hủy booking đã điểm danh tham gia.");
                            break;
                        }

                        if (cancelClass.CancelBooking(booking.BookingId))
                        {
                            SaveData();
                            Console.WriteLine($"[Hệ thống] Đã hủy thành công booking {booking.BookingId}. Nếu có hội viên trong hàng đợi, hệ thống đã tự động đôn lên!");
                        }
                        else
                        {
                            Console.WriteLine("[Lỗi] Không thể hủy lượt đặt chỗ này.");
                        }
                        break;

                    case "3":
                        Console.Write("Nhập ID lớp học cần đôn hàng đợi: ");
                        string? promoteClassId = Console.ReadLine()?.Trim();
                        var promoteClass = FitnessClasses.FirstOrDefault(c => c.ClassId.Equals(promoteClassId, StringComparison.OrdinalIgnoreCase));
                        if (promoteClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                            break;
                        }

                        if (promoteClass.Roster.Count >= promoteClass.Capacity)
                        {
                            Console.WriteLine("[Thông báo] Lớp học hiện tại đã đầy đủ chỗ.");
                            break;
                        }

                        if (promoteClass.WaitList.Count == 0)
                        {
                            Console.WriteLine("[Thông báo] Danh sách chờ hiện đang trống.");
                            break;
                        }

                        if (promoteClass.PromoteFromWaitlist())
                        {
                            SaveData();
                            var promoted = promoteClass.Roster.Last();
                            Console.WriteLine($"[Hệ thống] Đã đôn thành công hội viên: {promoted.Member.Name} lên danh sách chính thức.");
                        }
                        break;

                    case "4":
                        Console.WriteLine("\n--- DANH SÁCH CHI TIẾT TỪNG LỚP HỌC ---");
                        foreach (var fc in FitnessClasses)
                        {
                            Console.WriteLine($"\n==================================================");
                            Console.WriteLine($"Lớp: {fc.ClassName} (ID: {fc.ClassId}) | Thời gian: {fc.ScheduleTime:dd/MM/yyyy HH:mm} - {fc.EndTime:dd/MM/yyyy HH:mm}");
                            Console.WriteLine($"Sức chứa: {fc.Roster.Count}/{fc.Capacity} | Chờ: {fc.WaitList.Count}");
                            Console.WriteLine("--------------------------------------------------");
                            Console.WriteLine("Danh sách chính thức:");
                            if (fc.Roster.Count == 0)
                            {
                                Console.WriteLine("  (Chưa có hội viên)");
                            }
                            else
                            {
                                foreach (var b in fc.Roster)
                                {
                                    Console.WriteLine($"  - [Booking: {b.BookingId}] Hội viên: {b.Member.Name,-18} | Trạng thái: {b.Status}");
                                }
                            }

                            Console.WriteLine("Danh sách chờ (Waitlist):");
                            if (fc.WaitList.Count == 0)
                            {
                                Console.WriteLine("  (Hàng đợi trống)");
                            }
                            else
                            {
                                int queuePos = 1;
                                foreach (var b in fc.WaitList)
                                {
                                    Console.WriteLine($"  - #{queuePos++} [Booking: {b.BookingId}] Hội viên: {b.Member.Name,-18} | Trạng thái: {b.Status}");
                                }
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

        // ==========================================
        // 5. ĐIỂM DANH LỚP HỌC (ATTENDANCE)
        // ==========================================
        public void MarkAttendance()
        {
            while (true)
            {
                Console.WriteLine("\n--- ĐIỂM DANH LỚP HỌC ---");
                Console.WriteLine("1. Xem danh sách tham gia chính thức ");
                Console.WriteLine("2. Đánh dấu có mặt ");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-2): ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.Write("Nhập ID lớp học: ");
                        string? classId = Console.ReadLine()?.Trim();
                        var fitnessClass = FitnessClasses.FirstOrDefault(c => c.ClassId.Equals(classId, StringComparison.OrdinalIgnoreCase));
                        if (fitnessClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                            break;
                        }

                        var attendees = fitnessClass.GetConfirmedAttendees();
                        Console.WriteLine($"\n--- DANH SÁCH CHÍNH THỨC LỚP {fitnessClass.ClassName} ---");
                        if (attendees.Count == 0)
                        {
                            Console.WriteLine("Chưa có hội viên nào trong danh sách chính thức.");
                            break;
                        }

                        foreach (var m in attendees)
                        {
                            var b = fitnessClass.Roster.First(x => x.Member.Id == m.Id);
                            Console.WriteLine($"Mã HV: {m.Id,-5} | Tên: {m.Name,-18} | Trạng thái điểm danh: {b.Status}");
                        }
                        break;

                    case "2":
                        Console.Write("Nhập ID lớp học: ");
                        string? attClassId = Console.ReadLine()?.Trim();
                        var attClass = FitnessClasses.FirstOrDefault(c => c.ClassId.Equals(attClassId, StringComparison.OrdinalIgnoreCase));
                        if (attClass == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy lớp học.");
                            break;
                        }

                        Console.Write("Nhập ID hội viên cần điểm danh: ");
                        string? memId = Console.ReadLine()?.Trim();

                        var bookingToMark = attClass.Roster.FirstOrDefault(b => b.Member.Id.Equals(memId, StringComparison.OrdinalIgnoreCase));
                        if (bookingToMark == null)
                        {
                            Console.WriteLine("[Lỗi] Hội viên này không nằm trong danh sách chính thức của lớp.");
                            break;
                        }

                        if (bookingToMark.Status == BookingStatus.Attended)
                        {
                            Console.WriteLine($"[Thông báo] Hội viên {bookingToMark.Member.Name} đã được điểm danh trước đó rồi.");
                            break;
                        }

                        attClass.MarkAttendance(memId!);
                        SaveData();
                        Console.WriteLine($"[Hệ thống] Điểm danh thành công cho hội viên: {bookingToMark.Member.Name}!");
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("[Lỗi] Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }

        // ==========================================
        // 6. XUẤT BÁO CÁO HOẠT ĐỘNG (REPORTS)
        // ==========================================
        public void GenerateReports()
        {
            var reportService = new ActivityReportService();

            while (true)
            {
                Console.WriteLine("\n--- BÁO CÁO HOẠT ĐỘNG ---");
                Console.WriteLine("1. Xuất báo cáo theo từng hội viên");
                Console.WriteLine("2. Xuất báo cáo toàn bộ hội viên");
                Console.WriteLine("3. Thống kê hoạt động chung");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Vui lòng chọn chức năng (0-3): ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.Write("Nhập ID hội viên: ");
                        string? memberId = Console.ReadLine()?.Trim();
                        var member = Members.FirstOrDefault(m => m.Id.Equals(memberId, StringComparison.OrdinalIgnoreCase));
                        if (member == null)
                        {
                            Console.WriteLine("[Lỗi] Không tìm thấy hội viên.");
                            break;
                        }

                        var memberBookings = _bookingHistory.Where(b => b.Member.Id == member.Id).ToList();
                        Console.WriteLine(reportService.GenerateMemberReport(member, memberBookings));
                        break;

                    case "2":
                        Console.WriteLine("\n--- BÁO CÁO TẤT CẢ HỘI VIÊN ---");
                        if (Members.Count == 0)
                        {
                            Console.WriteLine("Hệ thống chưa có hội viên nào.");
                            break;
                        }

                        foreach (var m in Members)
                        {
                            var bookings = _bookingHistory.Where(b => b.Member.Id == m.Id).ToList();
                            Console.WriteLine(reportService.GenerateMemberReport(m, bookings));
                        }
                        break;

                    case "3":
                        Console.WriteLine("\n--- THỐNG KÊ HOẠT ĐỘNG TỔNG QUAN ---");
                        Console.WriteLine($"Tổng số hội viên        : {Members.Count}");
                        Console.WriteLine($"Tổng số huấn luyện viên : {Trainers.Count}");
                        Console.WriteLine($"Tổng số lớp học         : {FitnessClasses.Count}");
                        Console.WriteLine($"Tổng số lượt đặt chỗ    : {_bookingHistory.Count}");
                        Console.WriteLine($" - Đã tham gia (Attended): {_bookingHistory.Count(b => b.Status == BookingStatus.Attended)}");
                        Console.WriteLine($" - Đã xác nhận (Confirmed): {_bookingHistory.Count(b => b.Status == BookingStatus.Confirmed)}");
                        Console.WriteLine($" - Đang chờ (Waitlist)   : {_bookingHistory.Count(b => b.Status == BookingStatus.WaitListed)}");
                        Console.WriteLine($" - Đã hủy (Cancelled)    : {_bookingHistory.Count(b => b.Status == BookingStatus.Cancelled)}");
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
