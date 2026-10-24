namespace OOP_project
{
    public class Member : Person
    {
        public MembershipPlan? CurrentPlan { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime PlanExpiryDate { get; set; }
        private int currentMonthlyBookings = 0 ;
        public Member(string? id, string? name, string? email, string? phoneNumber ) : base(id, name, email, phoneNumber) {}
        public void ChangePlan(MembershipPlan newPlan)
        {
            CurrentPlan = newPlan;
        }
        public MembershipPlan GetCurrentPlan()
        {
            return CurrentPlan;
        }
        public int GetMonthlyBookingCount()
        {
            // Hàm này nên trả về số lượng class mà Member NÀY đã book trong tháng, 
            return currentMonthlyBookings;
        }
        public bool CanBookMore()
        {
            if (CurrentPlan == null) return false;

            // Gọi hàm canBookClass từ MembershipPlan
            return CurrentPlan.CanBookClass(currentMonthlyBookings);
        }
    }
}