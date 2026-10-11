namespace OOP_project
{
    public class Booking
    {
        public string BookingId { get;set; }
        private Member Member { get;set; }
        private FitnessClass fitnessClass { get;set; }
        private DateTime BookingDate { get;set; }
        private BookingStatus Status { get;set; }
        
        public Booking (string bookingid, Member member, FitnessClass fitnessclass, DateTime bookingdate, BookingStatus status)
        {
            BookingId = bookingid;
            Member = member;
            fitnessClass = fitnessclass;
            BookingDate = bookingdate;
            Status = status;
        }
        public void UpdateStatus (BookingStatus newStatus)
        {
            Status = newStatus;
        }
        public void MarkAttended()
        {
            Status = BookingStatus.Attended;
        }
        public Member GetMember()
        {
            return Member;
        }
        public BookingStatus GetStatus()
        {
            return Status;
        }
    }
}