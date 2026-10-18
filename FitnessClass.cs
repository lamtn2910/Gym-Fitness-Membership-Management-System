namespace OOP_project
{
    public class FitnessClass
    {
        public string ClassId { get;set; }
        public string ClassName { get;set; }
        public DateTime ScheduleTime { get;set; }
        public int Capacity { get;set; } // số lượng tối đa thành viên có thể tham gia lớp học
        public Trainer Trainer { get;set; }
        public List<Booking> Roster { get;set; } // danh sách các booking đã được xác nhận
        public Queue<Booking> WaitList { get;set; } // danh sách các booking đang chờ 

        public FitnessClass(string classId, string className, DateTime scheduleTime, int capacity, Trainer trainer)
        {
            ClassId = classId;
            ClassName = className;
            ScheduleTime = scheduleTime;
            Capacity = capacity;
            Trainer = trainer;
            // 2. Khởi tạo rỗng cho các danh sách (List và Queue) để không bị Null
            Roster = new List<Booking>();
            WaitList = new Queue<Booking>();
        }
        public Booking BookClass(Member member)
        {
            // Tạo ID ngẫu nhiên cho Booking (hoặc bạn có thể tự thiết kế cách tạo ID riêng)
            string bookingId = Guid.NewGuid().ToString().Substring(0, 8); 

            // Nếu danh sách chính (Roster) chưa đầy -> trạng thái CONFIRMED
            // Nếu đã đầy -> trạng thái WAITLISTED (đưa vào hàng đợi)
            BookingStatus status = Roster.Count < Capacity ? BookingStatus.ConFirmed : BookingStatus.WaitListed;
            
            Booking newBooking = new Booking(bookingId, member, this, DateTime.Now, status);

            if (status == BookingStatus.ConFirmed)
            {
                Roster.Add(newBooking);
            }
            else
            {
                WaitList.Enqueue(newBooking);
            }

            return newBooking;
        }
        public void CancleBooking (string bookingid)
        {
            // Tìm booking trong danh sách chính (Roster)
            Booking bookingToCancel = Roster.FirstOrDefault(b => b.BookingId == bookingid);

            if (bookingToCancel != null)
            {
                bookingToCancel.UpdateStatus(BookingStatus.Cancelled);
                Roster.Remove(bookingToCancel);

                // Vì có người vừa hủy chỗ trống, tự động đưa người từ Waitlist lên
                PromoteFromWaitlist(); 
            }
            else
            {
                // Nếu không có trong Roster, tìm trong Waitlist
                // (Xử lý hàng đợi Queue hơi đặc thù, ta phải chuyển sang List tạm thời để xóa)
                var waitlistAsList = WaitList.ToList();
                Booking waitlistBooking = waitlistAsList.FirstOrDefault(b => b.BookingId == bookingid);
                
                if (waitlistBooking != null)
                {
                    waitlistBooking.UpdateStatus(BookingStatus.Cancelled);
                    waitlistAsList.Remove(waitlistBooking);
                    WaitList = new Queue<Booking>(waitlistAsList); // Cập nhật lại Queue
                }
            }
        }
        private void PromoteFromWaitlist ()
        {
            // Nếu có người trong danh sách chờ VÀ lớp vẫn còn chỗ trống
            if (WaitList.Count > 0 && Roster.Count < Capacity)
            {
                // Lấy người đứng đầu hàng đợi ra (Dequeue)
                Booking promotedBooking = WaitList.Dequeue();
                
                // Cập nhật trạng thái thành CONFIRMED và thêm vào Roster
                promotedBooking.UpdateStatus(BookingStatus.ConFirmed);
                Roster.Add(promotedBooking);
            }
        }
        public void MarkAttendance (string memberid)
        {
            // Duyệt qua từng lượt đặt chỗ (booking) trong danh sách lớp (Roster)
            foreach (Booking booking in Roster)
            {
            // 1. Lấy ra thành viên của lượt đặt chỗ này
            Member currentMember = booking.GetMember();

                // 2. So sánh ID. 
            if (currentMember.Id == memberid) 
            {
                // 3. Nếu khớp ID thì đánh dấu là đã tham gia
                booking.MarkAttended();

                // Tìm thấy rồi thì dừng vòng lặp (không cần tìm người tiếp theo nữa)
                break; 
            }
    }
        }
        public List<Member> GetConfirmedAttendees ()
        {
            // Lấy danh sách Member từ các Booking trong Roster
            return Roster.Select(booking => booking.GetMember()).ToList();
        }
    }
}