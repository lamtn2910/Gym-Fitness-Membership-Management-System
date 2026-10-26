namespace OOP_project
{
    public enum BookingStatus
    {
        Confirmed,   // Xác nhận tham gia chính thức
        WaitListed,  // Danh sách chờ
        Cancelled,   // Đã hủy
        Attended,    // Đã điểm danh tham gia

        // Hỗ trợ tương thích ngược với mã cũ
        ConFirmed = Confirmed
    }
}
