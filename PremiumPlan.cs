namespace OOP_project
{
    public class PremiumPlan : MembershipPlan
    {
        private double MonthlyFee = 400.00;
        private int MaxBookings = int.MaxValue; //không giới hạn
        public override string GetPlanName()
        {
            return "Premium Plan";
        }

        public override double GetMonthlyFee()
        {
            return MonthlyFee;
        }

        public override int GetMaxBookingsPerMonth()
        {
            return MaxBookings; 
        }

        public override bool CanBookClass(int currentBookingCount)
        {
            return currentBookingCount < GetMaxBookingsPerMonth();
        }
    }
}