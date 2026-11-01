namespace OOP_project
{
    public class Booking
    {
        private static int _nextId = 1;

        public string BookingId { get; init; }
        public Member Member { get; private set; }
        public FitnessClass FitnessClass { get; private set; }
        public DateTime BookingDate { get; init; }
        public BookingStatus Status { get; private set; }

        public Booking(Member member, FitnessClass fitnessClass, DateTime bookingDate, BookingStatus status, string? bookingId = null)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
            FitnessClass = fitnessClass ?? throw new ArgumentNullException(nameof(fitnessClass));
            BookingDate = bookingDate;
            Status = status;
            if (!string.IsNullOrWhiteSpace(bookingId))
            {
                BookingId = bookingId.Trim();
                SynchronizeStaticId(BookingId);
            }
            else
            {
                BookingId = $"BK{_nextId++:D4}";
            }
        }

        private static void SynchronizeStaticId(string id)
        {
            if (id.StartsWith("BK", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(id[2..], out int num) && num >= _nextId)
            {
                _nextId = num + 1;
            }
        }

        public void UpdateStatus(BookingStatus newStatus)
        {
            Status = newStatus;
        }

        public void MarkAttended()
        {
            Status = BookingStatus.Attended;
        }

        // Giữ lại để tương thích 
        public Member GetMember() => Member;
        public BookingStatus GetStatus() => Status;
    }
}
