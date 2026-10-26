using System.Text;

namespace OOP_project
{
    public class ActivityReportService
    {
        public string GenerateMemberReport(Member member, List<Booking> bookings)
        {
            if (member == null)
            {
                return "Lỗi: Không tìm thấy thông tin hội viên.";
            }

            StringBuilder report = new();
            report.AppendLine("========================================");
            report.AppendLine("        BÁO CÁO HOẠT ĐỘNG HỘI VIÊN      ");
            report.AppendLine("========================================");
            report.AppendLine($"Mã hội viên  : {member.Id}");
            report.AppendLine($"Tên hội viên : {member.Name}");
            report.AppendLine($"Email        : {member.Email}");
            report.AppendLine($"Số ĐT        : {member.PhoneNumber}");

            var plan = member.CurrentPlan;
            if (plan != null)
            {
                report.AppendLine($"Gói tập      : {plan.PlanName} ({plan.MonthlyFee:N0} VNĐ/tháng)");
                report.AppendLine($"Hạn gói tập  : {member.PlanExpiryDate:dd/MM/yyyy}");
            }
            else
            {
                report.AppendLine("Gói tập      : Chưa đăng ký gói tập");
            }

            report.AppendLine("----------------------------------------");

            if (bookings == null || bookings.Count == 0)
            {
                report.AppendLine("Hội viên chưa có lịch sử đặt chỗ.");
            }
            else
            {
                int attended = bookings.Count(b => b.Status == BookingStatus.Attended);
                int confirmed = bookings.Count(b => b.Status == BookingStatus.Confirmed);
                int waitlisted = bookings.Count(b => b.Status == BookingStatus.WaitListed);
                int cancelled = bookings.Count(b => b.Status == BookingStatus.Cancelled);

                report.AppendLine($"Tổng số lượt : {bookings.Count}");
                report.AppendLine($" - Đã tham gia  : {attended}");
                report.AppendLine($" - Đã xác nhận  : {confirmed}");
                report.AppendLine($" - Đang chờ     : {waitlisted}");
                report.AppendLine($" - Đã hủy       : {cancelled}");
                report.AppendLine("----------------------------------------");
                report.AppendLine("Chi tiết các lượt đặt chỗ:");

                foreach (var booking in bookings)
                {
                    report.AppendLine($"- Lớp: {booking.FitnessClass.ClassName} | Thời gian: {booking.FitnessClass.ScheduleTime:dd/MM/yyyy HH:mm} | Trạng thái: {booking.Status}");
                }
            }
            report.AppendLine("========================================");
            return report.ToString();
        }

        // Tương thích ngược với tên method cũ
        public string generateMemberReport(Member member, List<Booking> bookings) => GenerateMemberReport(member, bookings);
    }
}
