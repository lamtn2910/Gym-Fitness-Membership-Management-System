namespace OOP_project
{
    class Program
    {
        static void Main(string[] args)
        {
            // Tạo các gói thành viên
            MembershipPlan basicPlan = new BasicPlan();
            MembershipPlan premiumPlan = new PremiumPlan();

            // Hiển thị thông tin về các gói thành viên
            Console.WriteLine("Basic Plan:");
            Console.WriteLine($"Monthly Fee: {basicPlan.GetMonthlyFee()}");
            Console.WriteLine($"Max Bookings Per Month: {basicPlan.GetMaxBookingsPerMonth()}");
            Console.WriteLine();

            Console.WriteLine("Premium Plan:");
            Console.WriteLine($"Monthly Fee: {premiumPlan.GetMonthlyFee()}");
            Console.WriteLine($"Max Bookings Per Month: {premiumPlan.GetMaxBookingsPerMonth()}");
        }
    }
}