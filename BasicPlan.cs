namespace OOP_project
{
    public class BasicPlan : MembershipPlan
    {
        private double MonthlyFee = 250.00;
        private int MaxBookings = 5;
        public override string GetPlanName()
        {
            return "Basic Plan";
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