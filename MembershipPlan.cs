namespace OOP_project
{
    public abstract class MembershipPlan
    {
        public abstract string PlanName { get; }
        public abstract decimal MonthlyFee { get; }
        public abstract int MaxBookingsPerMonth { get; }

        public virtual bool CanBookClass(int currentBookingCount)
        {
            return currentBookingCount < MaxBookingsPerMonth;
        }

        // Các phương thức giữ lại để tương thích với sơ đồ UML
        public string GetPlanName() => PlanName;
        public double GetMonthlyFee() => (double)MonthlyFee;
        public int GetMaxBookingsPerMonth() => MaxBookingsPerMonth;
    }
}
