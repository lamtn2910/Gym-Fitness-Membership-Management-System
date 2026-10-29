namespace OOP_project
{
    // Hai khoảng [bắt đầu, kết thúc) xung đột khi có thời gian giao nhau.
    // Lớp kết thúc lúc 10:00 và lớp tiếp theo bắt đầu lúc 10:00 là hợp lệ.
    public static class ScheduleRules
    {
        public static bool Overlaps(DateTime startA, int minutesA, DateTime startB, int minutesB)
        {
            if (minutesA <= 0 || minutesB <= 0)
                throw new ArgumentOutOfRangeException(nameof(minutesA), "Thời lượng phải dương.");
            return startA < startB.AddMinutes(minutesB) && startB < startA.AddMinutes(minutesA);
        }
    }
}
