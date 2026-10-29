namespace OOP_project
{
    public class PremiumPlan : MembershipPlan
    {
        public override string PlanName => "Premium Plan";
        public override decimal MonthlyFee => 400000.00m;
        public override int MaxBookingsPerMonth => int.MaxValue;
    }
}
