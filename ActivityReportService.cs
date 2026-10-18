using System.Text; // cho StringBuilder
namespace OOP_project
{
    public class ActivityReportService
    {
        public string generateMemberReport(Member member, List<Booking> bookings)
        {
            if (member == null)
            {
                return "Error: Member information not found.";
            }

            StringBuilder report = new StringBuilder();
            report.AppendLine("========================================");
            report.AppendLine("        MEMBER ACTIVITY REPORT          ");
            report.AppendLine("========================================");
            report.AppendLine($"Member ID   : {member.GetId()}"); 
            report.AppendLine($"Member Name : {member.GetName()}");
            
            var plan = member.GetCurrentPlan(); 
            if (plan != null)
            {
                report.AppendLine($"Plan        : {plan.GetPlanName()}");
            }

            report.AppendLine("----------------------------------------");
            
            if (bookings == null || !bookings.Any())
            {
                report.AppendLine("The member has no booking history.");
            }
            else
            {
                
                int attended = bookings.Count(b => b.GetStatus() == BookingStatus.Attended);
                int confirmed = bookings.Count(b => b.GetStatus() == BookingStatus.ConFirmed);
                int waitlisted = bookings.Count(b => b.GetStatus() == BookingStatus.WaitListed);
                int cancelled = bookings.Count(b => b.GetStatus() == BookingStatus.Cancelled);

                report.AppendLine($"Total Bookings : {bookings.Count}");
                report.AppendLine($" - Attended    : {attended}");
                report.AppendLine($" - Confirmed   : {confirmed}");
                report.AppendLine($" - Waitlisted  : {waitlisted}");
                report.AppendLine($" - Cancelled   : {cancelled}");
                report.AppendLine("----------------------------------------");
                report.AppendLine("Booking Details:");
                
                foreach (var booking in bookings)
                {
                    report.AppendLine($"- Booking ID: {booking.BookingId} | Status: {booking.GetStatus()}");
                }
            }
            report.AppendLine("========================================");
            return report.ToString();
        }
    }
}
