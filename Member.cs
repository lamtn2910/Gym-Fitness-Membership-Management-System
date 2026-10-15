namespace OOP_project
{
    public class Member : Person
    {
        private MembershipPlan currentPlan { get; set; }
        private DateTime joinDate { get; set; }
        private DateTime planExpiryDate { get; set; }
        private int currentMonthlyBookings = 0 ;
        public Member(string id, string name, string email, string phoneNumber) : base(id, name, email, phoneNumber) {}
        public void ChangePlan(MembershipPlan newPlan)
        {
            currentPlan = newPlan;
        }
        public MembershipPlan GetCurrentPlan()
        {
            return currentPlan;
        }
        public int GetMonthlyBookingCount()
        {
            // Hàm này nên trả về số lượng class mà Member NÀY đã book trong tháng, 
            return currentMonthlyBookings;
        }
        public bool CanBookMore()
        {
            if (currentPlan == null) return false;

            // Gọi hàm canBookClass từ MembershipPlan
            // và truyền vào số lượng class đã book hiện tại.
            return currentPlan.CanBookClass(this.GetMonthlyBookingCount());
        }
    }
}