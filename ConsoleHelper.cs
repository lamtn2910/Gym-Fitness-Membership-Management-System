using System.Globalization;
using System.Text.RegularExpressions;

namespace OOP_project
{
    public static class ConsoleHelper
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PhoneRegex = new(
            @"^0\d{9}$",
            RegexOptions.Compiled);

        public static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    return input;
                }
                Console.WriteLine("[Lỗi] Giá trị không được để trống. Vui lòng nhập lại.");
            }
        }

        public static string ReadEmail(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(input) && EmailRegex.IsMatch(input))
                {
                    return input;
                }
                Console.WriteLine("[Lỗi] Định dạng email không hợp lệ (ví dụ hợp lệ: user@example.com). Vui lòng nhập lại.");
            }
        }

        public static string ReadPhoneNumber(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(input) && PhoneRegex.IsMatch(input))
                {
                    return input;
                }
                Console.WriteLine("[Lỗi] Số điện thoại không hợp lệ (phải gồm 10 chữ số và bắt đầu bằng số 0, ví dụ: 0901234567). Vui lòng nhập lại.");
            }
        }

        public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int result) && result >= min && result <= max)
                {
                    return result;
                }
                if (max == int.MaxValue && min != int.MinValue)
                {
                    Console.WriteLine($"[Lỗi] Vui lòng nhập một số nguyên >= {min}.");
                }
                else if (min != int.MinValue && max != int.MaxValue)
                {
                    Console.WriteLine($"[Lỗi] Vui lòng nhập một số nguyên từ {min} đến {max}.");
                }
                else
                {
                    Console.WriteLine("[Lỗi] Số nguyên không hợp lệ. Vui lòng nhập lại.");
                }
            }
        }

        public static DateTime ReadDateTime(string prompt, bool futureOnly = false)
        {
            string[] formats = ["yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm", "dd/MM/yyyy HH:mm", "dd-MM-yyyy HH:mm"];
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ||
                    DateTime.TryParse(input, out result))
                {
                    if (futureOnly && result <= DateTime.Now)
                    {
                        Console.WriteLine("[Lỗi] Thời gian phải ở trong tương lai. Vui lòng nhập lại.");
                        continue;
                    }
                    return result;
                }
                Console.WriteLine("[Lỗi] Định dạng thời gian không hợp lệ (ví dụ: 2026-11-20 14:30 hoặc 20/11/2026 14:30). Vui lòng nhập lại.");
            }
        }

        public static void Pause()
        {
            Console.WriteLine("\nNhấn Enter để tiếp tục...");
            Console.ReadLine();
        }
    }
}
