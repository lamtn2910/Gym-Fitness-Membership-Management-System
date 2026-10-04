namespace OOP_project
{
    public abstract class MembershipPlan
    {
        public abstract string GetPlanName();// lấy tên gói
        public abstract double GetMonthlyFee();// lấy phí hàng tháng
        public abstract int GetMaxBookingsPerMonth();// lấy số lượng đặt chỗ tối đa mỗi tháng
        public abstract bool CanBookClass(int currentBookingCount);// có thể đặt lớp học ?

    }
}