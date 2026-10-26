namespace OOP_project
{
    public class Member : Person
    {
        private static int _nextId = 1;

        public MembershipPlan? CurrentPlan { get; set; }
        public DateTime JoinDate { get; init; } = DateTime.Now;
        public DateTime PlanExpiryDate { get; set; }
        public int CurrentMonthlyBookings { get; private set; } = 0;

        // Constructor tự sinh ID dựa trên biến static (theo yêu cầu đề bài)
        public Member(string name, string email, string phoneNumber, MembershipPlan? initialPlan = null)
            : base(GenerateNextId(), name, email, phoneNumber)
        {
            CurrentPlan = initialPlan;
            PlanExpiryDate = DateTime.Now.AddMonths(1);
        }

        // Constructor cho phép truyền ID tùy chọn (dùng khi nạp dữ liệu cũ)
        public Member(string id, string name, string email, string phoneNumber, MembershipPlan? initialPlan = null)
            : base(id, name, email, phoneNumber)
        {
            CurrentPlan = initialPlan;
            PlanExpiryDate = DateTime.Now.AddMonths(1);
            SynchronizeStaticId(id);
        }

        private static string GenerateNextId()
        {
            return $"M{_nextId++:D3}";
        }

        private static void SynchronizeStaticId(string id)
        {
            if (id.StartsWith("M", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(id[1..], out int num) && num >= _nextId)
            {
                _nextId = num + 1;
            }
        }

        public void ChangePlan(MembershipPlan newPlan)
        {
            ArgumentNullException.ThrowIfNull(newPlan, nameof(newPlan));
            CurrentPlan = newPlan;
            PlanExpiryDate = DateTime.Now.AddMonths(1);
        }

        public void IncrementBookingCount() => CurrentMonthlyBookings++;
        public void DecrementBookingCount()
        {
            if (CurrentMonthlyBookings > 0) CurrentMonthlyBookings--;
        }
        public void ResetMonthlyBookings() => CurrentMonthlyBookings = 0;

        public MembershipPlan? GetCurrentPlan() => CurrentPlan;
        public int GetMonthlyBookingCount() => CurrentMonthlyBookings;

        public bool CanBookMore()
        {
            return CurrentPlan != null && CurrentPlan.CanBookClass(CurrentMonthlyBookings);
        }
    }
}
