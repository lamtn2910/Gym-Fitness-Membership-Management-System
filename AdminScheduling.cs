namespace OOP_project
{
    // Các nghiệp vụ cần kiểm tra đồng thời nhiều lớp học được quản lý tại Admin.  
    public partial class Admin
    {
        // Kết nối một lớp học với các quy tắc kiểm tra do Admin quản lý.  
        // Khi hội viên đặt lớp, hệ thống gọi ValidateBooking; khi có chỗ trống,  
        // hệ thống gọi CanPromoteBooking để kiểm tra người đang chờ trước khi đôn lên.  
        private void RegisterClass(FitnessClass fitnessClass)
        {
            fitnessClass.BookingValidation = member => ValidateBooking(member, fitnessClass);
            fitnessClass.PromotionValidation = CanPromoteBooking;
        }

        // Tìm lớp học khác có lịch dạy chồng lấn với khoảng thời gian dự kiến của HLV.  
        // So sánh ID huấn luyện viên và thời điểm bắt đầu/kết thúc của từng lớp.  
        // Tham số except cho phép bỏ qua lớp đang cập nhật; trả về lớp trùng đầu tiên  
        // hoặc null nếu huấn luyện viên còn trống lịch.  
        private FitnessClass? FindTrainerConflict(Trainer trainer, DateTime start,
            int durationMinutes, FitnessClass? except = null)
        {
            return FitnessClasses.FirstOrDefault(fc => fc != except &&
                fc.Trainer.Id.Equals(trainer.Id, StringComparison.OrdinalIgnoreCase) &&
                ScheduleRules.Overlaps(fc.ScheduleTime, fc.DurationMinutes, start, durationMinutes));
        }

        // Tìm lớp khác trùng thời gian với lịch mà hội viên đang muốn đăng ký.  
        // Kiểm tra cả danh sách đã xác nhận và danh sách chờ; chỉ xét booking có  
        // trạng thái Confirmed, WaitListed hoặc Attended. Bỏ qua lớp except nếu có.  
        // Trả về lớp gây xung đột đầu tiên, hoặc null nếu không bị trùng lịch.  
        private FitnessClass? FindMemberConflict(Member member, DateTime start,
            int durationMinutes, FitnessClass? except = null)
        {
            return FitnessClasses.FirstOrDefault(fc => fc != except &&
                ScheduleRules.Overlaps(fc.ScheduleTime, fc.DurationMinutes, start, durationMinutes) &&
                fc.Roster.Concat(fc.WaitList).Any(b =>
                    b.Member.Id.Equals(member.Id, StringComparison.OrdinalIgnoreCase) &&
                    (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.WaitListed ||
                     b.Status == BookingStatus.Attended)));
        }

        // Xác thực mọi điều kiện nghiệp vụ trước khi cho phép hội viên đặt lớp.  
        // Từ chối nếu hội viên/lớp không tồn tại, chưa có gói tập, lớp đã bắt đầu,  
        // đã đặt hoặc đang chờ cùng lớp, bị trùng lịch hay vượt giới hạn của gói.  
        // Trả về thông báo lỗi cụ thể; trả về null khi booking hợp lệ.  
        private string? ValidateBooking(Member member, FitnessClass fitnessClass)
        {
            if (!Members.Contains(member)) return "Hội viên không tồn tại trong hệ thống.";
            if (!FitnessClasses.Contains(fitnessClass)) return "Lớp không tồn tại trong hệ thống.";
            if (member.CurrentPlan == null) return "Hội viên chưa có gói tập.";
            if (fitnessClass.ScheduleTime <= DateTime.Now) return "Lớp đã bắt đầu hoặc kết thúc.";

            bool alreadyBooked = fitnessClass.Roster.Concat(fitnessClass.WaitList).Any(b =>
                b.Member.Id.Equals(member.Id, StringComparison.OrdinalIgnoreCase));
            if (alreadyBooked) return "Hội viên đã đặt hoặc nằm trong danh sách chờ của lớp này.";

            FitnessClass? conflict = FindMemberConflict(member, fitnessClass.ScheduleTime,
                fitnessClass.DurationMinutes, fitnessClass);
            if (conflict != null)
                return $"Hội viên trùng lịch với lớp {conflict.ClassName} ({conflict.ClassId}), " +
                       $"{conflict.ScheduleTime:dd/MM/yyyy HH:mm} - {conflict.EndTime:HH:mm}.";

            if (!member.CanBookMore())
                return $"Hội viên đã đạt giới hạn đặt lớp của gói {member.CurrentPlan.PlanName}.";

            return null;
        }

        // Kiểm tra lại điều kiện của một booking đang ở trạng thái WaitListed.  
        // Chỉ cho phép đôn lên khi hội viên và lớp vẫn tồn tại, lớp chưa bắt đầu,  
        // hội viên chưa vượt giới hạn gói và không trùng lịch với lớp khác.  
        // Trả về true nếu có thể chuyển sang Confirmed; ngược lại trả về false.  
        private bool CanPromoteBooking(Booking booking)
        {
            return booking.Status == BookingStatus.WaitListed &&
                   Members.Contains(booking.Member) &&
                   FitnessClasses.Contains(booking.FitnessClass) &&
                   booking.FitnessClass.ScheduleTime > DateTime.Now &&
                   booking.Member.CanBookMore() &&
                   FindMemberConflict(booking.Member, booking.FitnessClass.ScheduleTime,
                       booking.FitnessClass.DurationMinutes, booking.FitnessClass) == null;
        }


        // Xử lý yêu cầu đặt lớp cho một hội viên cụ thể.  
        // Kiểm tra đầu vào rồi gọi ValidateBooking; nếu không hợp lệ, trả về false  
        // và lý do trong error. Nếu hợp lệ, tạo booking qua FitnessClass.BookClass,  
        // lưu vào lịch sử chung và trả về true cùng booking vừa tạo.  
        // Trạng thái Confirmed hay WaitListed do sức chứa của lớp quyết định.  
        public bool TryBookClass(Member member, FitnessClass fitnessClass,
            out Booking? booking, out string error)
        {
            booking = null;
            if (member == null || fitnessClass == null)
            {
                error = "Hội viên hoặc lớp không hợp lệ.";
                return false;
            }

            string? validationError = ValidateBooking(member, fitnessClass);
            if (validationError != null)
            {
                error = validationError;
                return false;
            }

            booking = fitnessClass.BookClass(member);
            _bookingHistory.Add(booking);
            error = string.Empty;
            return true;
        }

        // Tạo một lớp học mới với tên, lịch bắt đầu, thời lượng, sức chứa và HLV.  
        // Chỉ chấp nhận thông tin hợp lệ, thời gian trong tương lai, thời lượng  
        // 1–1440 phút và HLV đã có trong hệ thống, không bị trùng lịch dạy.  
        // Khi thành công, thêm lớp vào danh sách và gắn quy tắc kiểm tra booking;  
        // nếu thất bại, trả về false và nội dung lỗi, không tạo lớp.  
        public bool TryCreateClass(string name, DateTime start, int durationMinutes, int capacity,
            Trainer trainer, out FitnessClass? fitnessClass, out string error)
        {
            fitnessClass = null;
            if (string.IsNullOrWhiteSpace(name) || start <= DateTime.Now ||
                durationMinutes is <= 0 or > 1440 || capacity <= 0 ||
                trainer == null || !Trainers.Contains(trainer))
            {
                error = "Thông tin lớp không hợp lệ (thời gian phải ở tương lai, thời lượng 1-1440 phút).";
                return false;
            }

            FitnessClass? conflict = FindTrainerConflict(trainer, start, durationMinutes);
            if (conflict != null)
            {
                error = $"HLV {trainer.Name} đã dạy lớp {conflict.ClassName} ({conflict.ClassId}) " +
                        $"lúc {conflict.ScheduleTime:dd/MM/yyyy HH:mm} - {conflict.EndTime:HH:mm}.";
                return false;
            }

            fitnessClass = new FitnessClass(name, start, capacity, trainer, durationMinutes);
            FitnessClasses.Add(fitnessClass);
            RegisterClass(fitnessClass);
            error = string.Empty;
            return true;
        }

        // Đổi thời gian bắt đầu và thời lượng của một lớp học đang tồn tại.  
        // Trước khi cập nhật, kiểm tra thời gian hợp lệ, tránh trùng lịch của HLV  
        // và tránh xung đột với các lớp khác của hội viên đã đặt/đang chờ.  
        // Nếu tất cả điều kiện đạt yêu cầu, cập nhật lớp và trả về true;  
        // nếu không, giữ nguyên lịch cũ và trả về thông báo lỗi.  
        public bool TryUpdateClassSchedule(FitnessClass fitnessClass, DateTime newStart,
            int newDurationMinutes, out string error)
        {
            if (fitnessClass == null || !FitnessClasses.Contains(fitnessClass) ||
                newStart <= DateTime.Now || newDurationMinutes is <= 0 or > 1440)
            {
                error = "Thời gian hoặc thời lượng mới không hợp lệ.";
                return false;
            }

            FitnessClass? trainerConflict = FindTrainerConflict(fitnessClass.Trainer,
                newStart, newDurationMinutes, fitnessClass);
            if (trainerConflict != null)
            {
                error = $"HLV {fitnessClass.Trainer.Name} bị trùng với lớp " +
                        $"{trainerConflict.ClassName} ({trainerConflict.ClassId}).";
                return false;
            }

            foreach (Booking booking in fitnessClass.Roster.Concat(fitnessClass.WaitList))
            {
                FitnessClass? memberConflict = FindMemberConflict(booking.Member,
                    newStart, newDurationMinutes, fitnessClass);
                if (memberConflict != null)
                {
                    error = $"Hội viên {booking.Member.Name} ({booking.Member.Id}) bị trùng " +
                            $"với lớp {memberConflict.ClassName} ({memberConflict.ClassId}).";
                    return false;
                }
            }

            fitnessClass.ScheduleTime = newStart;
            fitnessClass.DurationMinutes = newDurationMinutes;
            error = string.Empty;
            return true;
        }

        // Thay huấn luyện viên phụ trách của một lớp học.  
        // Xác minh lớp và HLV đều thuộc hệ thống, rồi kiểm tra HLV mới có đang  
        // dạy lớp khác trùng thời gian không (không tính chính lớp đang sửa).  
        // Nếu không xung đột, gán HLV mới và trả về true; nếu có, giữ nguyên HLV cũ.  
        public bool TryAssignTrainer(FitnessClass fitnessClass, Trainer trainer, out string error)
        {
            if (fitnessClass == null || !FitnessClasses.Contains(fitnessClass) ||
                trainer == null || !Trainers.Contains(trainer))
            {
                error = "Lớp hoặc HLV không tồn tại trong hệ thống.";
                return false;
            }

            FitnessClass? conflict = FindTrainerConflict(trainer,
                fitnessClass.ScheduleTime, fitnessClass.DurationMinutes, fitnessClass);
            if (conflict != null)
            {
                error = $"HLV {trainer.Name} đã được phân công cho lớp " +
                        $"{conflict.ClassName} ({conflict.ClassId}) cùng thời gian.";
                return false;
            }

            // Thay thế HLV cũ, không thêm HLV thứ hai vào cùng một lớp.  
            fitnessClass.Trainer = trainer;
            error = string.Empty;
            return true;
        }

        // Xóa một lớp học khỏi danh sách quản lý khi lớp chưa ghi nhận điểm danh.  
        // Nếu đã có hội viên trạng thái Attended thì từ chối để bảo vệ lịch sử.  
        // Nếu được phép xóa, hủy toàn bộ booking của lớp, xóa lớp khỏi danh sách  
        // và gỡ các hàm kiểm tra booking/waitlist đã đăng ký cho lớp đó.  
        public bool TryDeleteClass(FitnessClass fitnessClass, out string error)
        {
            if (fitnessClass == null || !FitnessClasses.Contains(fitnessClass))
            {
                error = "Lớp không tồn tại trong hệ thống.";
                return false;
            }
            if (fitnessClass.Roster.Any(b => b.Status == BookingStatus.Attended))
            {
                error = "Lớp đã có điểm danh, không thể xóa để tránh mất dữ liệu lịch sử.";
                return false;
            }

            fitnessClass.CancelAllBookings();
            FitnessClasses.Remove(fitnessClass);
            fitnessClass.BookingValidation = null;
            fitnessClass.PromotionValidation = null;
            error = string.Empty;
            return true;
        }

        // Xóa hội viên khỏi hệ thống khi chưa có booking nào được ghi nhận Attended.  
        // Từ chối xóa nếu hội viên không tồn tại hoặc đã có lịch sử điểm danh.  
        // Khi xóa, loại hội viên khỏi danh sách trước, sau đó hủy các booking  
        // trong tất cả lớp để giải phóng chỗ và xử lý waitlist phù hợp;  
        // tránh việc chính hội viên đang bị xóa lại được đôn từ hàng chờ.  
        public bool TryDeleteMember(Member member, out string error)
        {
            if (member == null || !Members.Contains(member))
            {
                error = "Hội viên không tồn tại trong hệ thống.";
                return false;
            }
            if (_bookingHistory.Any(b => b.Member == member && b.Status == BookingStatus.Attended))
            {
                error = "Hội viên đã có lịch sử điểm danh, không thể xóa.";
                return false;
            }

            // Loại khỏi hệ thống trước để không tự động đôn chính người đang bị xóa.  
            Members.Remove(member);
            foreach (FitnessClass fitnessClass in FitnessClasses)
            {
                foreach (Booking booking in fitnessClass.Roster.Concat(fitnessClass.WaitList)
                    .Where(b => b.Member == member).ToList())
                    fitnessClass.CancelBooking(booking.BookingId);
            }
            error = string.Empty;
            return true;
        }

        // Xóa huấn luyện viên khỏi danh sách quản lý.  
        // Chỉ cho xóa khi HLV tồn tại và không còn được gán làm người phụ trách  
        // bất kỳ lớp học nào. Nếu vẫn đang dạy, yêu cầu đổi HLV hoặc xóa lớp trước;  
        // nếu hợp lệ, xóa HLV và trả về true.  
        public bool TryDeleteTrainer(Trainer trainer, out string error)
        {
            if (trainer == null || !Trainers.Contains(trainer))
            {
                error = "HLV không tồn tại trong hệ thống.";
                return false;
            }
            if (FitnessClasses.Any(fc => fc.Trainer.Id.Equals(trainer.Id, StringComparison.OrdinalIgnoreCase)))
            {
                error = "HLV vẫn đang phụ trách lớp. Hãy chuyển HLV hoặc xóa lớp trước.";
                return false;
            }
            Trainers.Remove(trainer);
            error = string.Empty;
            return true;
        }
    }
}
