namespace OOP_project
{
    public class FitnessClass
    {
        private static int _nextId = 1;

        public string ClassId { get; init; }

        private string _className = string.Empty;
        public string ClassName
        {
            get => _className;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên lớp học không được để trống.", nameof(value));
                _className = value.Trim();
            }
        }

        public DateTime ScheduleTime { get; set; }

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Sức chứa phải lớn hơn 0.", nameof(value));
                _capacity = value;
            }
        }

        public Trainer Trainer { get; set; }

        private readonly List<Booking> _roster = [];
        private readonly Queue<Booking> _waitList = [];

        // Đóng gói danh sách (chỉ đọc từ bên ngoài, thay đổi qua phương thức của FitnessClass)
        public IReadOnlyList<Booking> Roster => _roster.AsReadOnly();
        public IReadOnlyCollection<Booking> WaitList => _waitList;

        // Constructor tự động sinh ID dựa trên biến static
        public FitnessClass(string className, DateTime scheduleTime, int capacity, Trainer trainer)
        {
            ClassId = GenerateNextId();
            ClassName = className;
            ScheduleTime = scheduleTime;
            Capacity = capacity;
            Trainer = trainer ?? throw new ArgumentNullException(nameof(trainer));
        }

        // Constructor truyền ID tùy chọn (dùng cho nạp dữ liệu cũ)
        public FitnessClass(string classId, string className, DateTime scheduleTime, int capacity, Trainer trainer)
        {
            if (string.IsNullOrWhiteSpace(classId))
                throw new ArgumentException("Class ID không được để trống.", nameof(classId));

            ClassId = classId.Trim();
            ClassName = className;
            ScheduleTime = scheduleTime;
            Capacity = capacity;
            Trainer = trainer ?? throw new ArgumentNullException(nameof(trainer));
            SynchronizeStaticId(classId);
        }

        private static string GenerateNextId() => $"C{_nextId++:D3}";

        private static void SynchronizeStaticId(string id)
        {
            if (id.StartsWith("C", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(id[1..], out int num) && num >= _nextId)
            {
                _nextId = num + 1;
            }
        }

        public Booking BookClass(Member member)
        {
            ArgumentNullException.ThrowIfNull(member, nameof(member));

            BookingStatus status = _roster.Count < Capacity ? BookingStatus.Confirmed : BookingStatus.WaitListed;
            var booking = new Booking(member, this, DateTime.Now, status);

            if (status == BookingStatus.Confirmed)
            {
                _roster.Add(booking);
                member.IncrementBookingCount();
            }
            else
            {
                _waitList.Enqueue(booking);
            }

            return booking;
        }

        public bool CancelBooking(string bookingId)
        {
            if (string.IsNullOrWhiteSpace(bookingId)) return false;

            // Tìm trong danh sách chính
            Booking? bookingToCancel = _roster.FirstOrDefault(b => b.BookingId == bookingId);
            if (bookingToCancel != null)
            {
                bookingToCancel.UpdateStatus(BookingStatus.Cancelled);
                _roster.Remove(bookingToCancel);
                bookingToCancel.Member.DecrementBookingCount();

                // Tự động đôn người từ hàng đợi nếu còn chỗ
                PromoteFromWaitlist();
                return true;
            }

            // Nếu không có trong Roster, tìm trong WaitList
            var waitListAsList = _waitList.ToList();
            Booking? waitlistBooking = waitListAsList.FirstOrDefault(b => b.BookingId == bookingId);
            if (waitlistBooking != null)
            {
                waitlistBooking.UpdateStatus(BookingStatus.Cancelled);
                waitListAsList.Remove(waitlistBooking);
                _waitList.Clear();
                foreach (var b in waitListAsList)
                {
                    _waitList.Enqueue(b);
                }
                return true;
            }

            return false;
        }

        // Tương thích ngược với tên method cũ bị sai chính tả
        public void CancleBooking(string bookingId) => CancelBooking(bookingId);

        public bool PromoteFromWaitlist()
        {
            if (_waitList.Count > 0 && _roster.Count < Capacity)
            {
                Booking promoted = _waitList.Dequeue();
                promoted.UpdateStatus(BookingStatus.Confirmed);
                _roster.Add(promoted);
                promoted.Member.IncrementBookingCount();
                return true;
            }
            return false;
        }

        public bool MarkAttendance(string memberId)
        {
            var booking = _roster.FirstOrDefault(b => b.Member.Id == memberId);
            if (booking != null)
            {
                booking.MarkAttended();
                return true;
            }
            return false;
        }

        public List<Member> GetConfirmedAttendees()
        {
            return _roster.Select(b => b.Member).ToList();
        }
    }
}
