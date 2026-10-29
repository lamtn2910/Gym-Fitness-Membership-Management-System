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

        // Chỉ cập nhật lịch thông qua Admin để tránh bỏ qua kiểm tra xung đột.
        public DateTime ScheduleTime { get; internal set; }

        public int DurationMinutes { get; internal set; } = 60;
        public DateTime EndTime => ScheduleTime.AddMinutes(DurationMinutes);

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Sức chứa phải lớn hơn 0.", nameof(value));
                if (value < _roster.Count)
                    throw new ArgumentException("Sức chứa không thể nhỏ hơn số booking đã xác nhận.", nameof(value));
                _capacity = value;
            }
        }

        // Mỗi lớp chỉ có một HLV. Việc phân công phải qua Admin.
        public Trainer Trainer { get; internal set; }

        // Khi lớp nằm trong hệ thống, nghiệp vụ cũng được kiểm tra khi gọi trực tiếp.
        internal Func<Member, string?>? BookingValidation { get; set; }
        internal Func<Booking, bool>? PromotionValidation { get; set; }

        private readonly List<Booking> _roster = [];
        private readonly Queue<Booking> _waitList = [];

        // Đóng gói danh sách (chỉ đọc từ bên ngoài, thay đổi qua phương thức của FitnessClass)
        public IReadOnlyList<Booking> Roster => _roster.AsReadOnly();
        public IReadOnlyCollection<Booking> WaitList => _waitList;

        // Constructor tự động sinh ID dựa trên biến static
        public FitnessClass(string className, DateTime scheduleTime, int capacity, Trainer trainer, int durationMinutes = 60)
        {
            ClassId = GenerateNextId();
            ClassName = className;
            ScheduleTime = scheduleTime;
            Capacity = capacity;
            if (durationMinutes <= 0 || durationMinutes > 1440)
                throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Thời lượng phải từ 1 đến 1440 phút.");
            DurationMinutes = durationMinutes;
            Trainer = trainer ?? throw new ArgumentNullException(nameof(trainer));
        }

        // Constructor truyền ID tùy chọn (dùng cho nạp dữ liệu cũ)
        public FitnessClass(string classId, string className, DateTime scheduleTime, int capacity, Trainer trainer, int durationMinutes = 60)
        {
            if (string.IsNullOrWhiteSpace(classId))
                throw new ArgumentException("Class ID không được để trống.", nameof(classId));

            ClassId = classId.Trim();
            ClassName = className;
            ScheduleTime = scheduleTime;
            Capacity = capacity;
            if (durationMinutes <= 0 || durationMinutes > 1440)
                throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Thời lượng phải từ 1 đến 1440 phút.");
            DurationMinutes = durationMinutes;
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

            if (ScheduleTime <= DateTime.Now)
                throw new InvalidOperationException("Lớp đã bắt đầu, không thể đặt chỗ.");
            if (_roster.Concat(_waitList).Any(b =>
                b.Member.Id.Equals(member.Id, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Hội viên đã đặt hoặc chờ lớp này.");
            if (!member.CanBookMore())
                throw new InvalidOperationException("Hội viên đã đạt giới hạn của gói tập.");
            string? validationError = BookingValidation?.Invoke(member);
            if (validationError != null)
                throw new InvalidOperationException(validationError);

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
            Booking? bookingToCancel = _roster.FirstOrDefault(b => b.BookingId.Equals(bookingId, StringComparison.OrdinalIgnoreCase));
            if (bookingToCancel != null)
            {
                if (bookingToCancel.Status == BookingStatus.Attended) return false;
                bookingToCancel.UpdateStatus(BookingStatus.Cancelled);
                _roster.Remove(bookingToCancel);
                bookingToCancel.Member.DecrementBookingCount();

                // Tự động đôn người từ hàng đợi nếu còn chỗ
                PromoteFromWaitlist();
                return true;
            }

            // Nếu không có trong Roster, tìm trong WaitList
            var waitListAsList = _waitList.ToList();
            Booking? waitlistBooking = waitListAsList.FirstOrDefault(b => b.BookingId.Equals(bookingId, StringComparison.OrdinalIgnoreCase));
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
            if (ScheduleTime <= DateTime.Now || _roster.Count >= Capacity || _waitList.Count == 0)
                return false;

            // Bỏ qua người không còn đủ điều kiện nhưng vẫn giữ nguyên thứ tự hàng đợi.
            int initialCount = _waitList.Count;
            Booking? selected = null;
            for (int i = 0; i < initialCount; i++)
            {
                Booking candidate = _waitList.Dequeue();
                if (selected == null && candidate.Status == BookingStatus.WaitListed &&
                    candidate.Member.CanBookMore() &&
                    !_roster.Any(b => b.Member.Id.Equals(candidate.Member.Id, StringComparison.OrdinalIgnoreCase)) &&
                    (PromotionValidation == null || PromotionValidation(candidate)))
                {
                    selected = candidate;
                }
                else if (candidate.Status == BookingStatus.WaitListed)
                {
                    _waitList.Enqueue(candidate);
                }
            }

            if (selected == null) return false;

            selected.UpdateStatus(BookingStatus.Confirmed);
            _roster.Add(selected);
            selected.Member.IncrementBookingCount();
            return true;
        }

        // Hủy cả danh sách mà không đôn người chờ (dùng khi xóa lớp).
        public void CancelAllBookings()
        {
            foreach (Booking booking in _roster)
            {
                if (booking.Status == BookingStatus.Attended)
                    throw new InvalidOperationException("Không thể xóa lớp đã được điểm danh.");
            }
            foreach (Booking booking in _roster)
            {
                booking.UpdateStatus(BookingStatus.Cancelled);
                booking.Member.DecrementBookingCount();
            }
            foreach (Booking booking in _waitList)
                booking.UpdateStatus(BookingStatus.Cancelled);
            _roster.Clear();
            _waitList.Clear();
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
