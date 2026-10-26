namespace OOP_project
{
    public class BasicPlan : MembershipPlan
    {
        public override string PlanName => "Basic Plan";
        public override decimal MonthlyFee => 250.00m;
        public override int MaxBookingsPerMonth => 5;
    }
}
