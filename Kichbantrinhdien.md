Dữ liệu có sẵn:
Member: M001 (Basic), M002 (Basic), M003 (Premium).
Trainer: T001 (Yoga), T002 (Zumba), T003 (Pilates).
Class: C001 (Yoga, 05/11/2026, 18:00), C002 (Zumba, 06/11/2026, 18:00), C003 (Pilates, 07/11/2026, 18:00).
Booking: Chưa có.

I. Quản lý hội viên (Manage Members)
Kịch bản 1: Thêm Member và Input Validation

Các bước:
-> Chọn 1 → 1 (Thêm hội viên).
-> Nhập tên rỗng bằng cách nhấn Enter.
-> Nhập tên hợp lệ: Demo Member.
-> Nhập email sai: demogmail.com.
-> Nhập lại email: demo@gmail.com.
-> Nhập số điện thoại sai: 12345.
-> Nhập lại: 0912345678.
-> Nhập gói tập 3 (không hợp lệ).
-> Chọn 1 (Basic Plan).
-> Chọn 1 → 2 để xem danh sách hội viên.

Kết quả mong đợi:
Tên rỗng, email sai, số điện thoại sai và gói tập không hợp lệ đều bị từ chối.
Tạo thành công hội viên M004 với Basic Plan.

Kịch bản 2: Cập nhật thông tin Member

Các bước:
-> Chọn 1 → 3, nhập M004.
-> Chọn 1, đổi tên thành Demo Member Updated.
-> Chọn 1 → 3, nhập M004, chọn 2 để đổi email thành demo.updated@gmail.com.
-> Chọn 1 → 3, nhập M004, chọn 3 để đổi số điện thoại thành 0987654321.
-> Chọn 1 → 2 để xem lại thông tin.
-> Thử cập nhật Member không tồn tại bằng ID M999.

Kết quả mong đợi:
Cập nhật thành công tên, email và số điện thoại.
ID M004 không thay đổi.
Hệ thống từ chối M999 vì không tồn tại.

Kịch bản 3: Nâng cấp và hạ cấp Membership Plan

Các bước:
-> Chọn 1 → 4, nhập M001.
-> Chọn 2 để nâng cấp từ Basic lên Premium.
-> Kiểm tra gói tập mới của M001.
-> Chọn 1 → 4, nhập M001, chọn 1 để hạ cấp về Basic.
-> Thử chọn Basic thêm một lần nữa.

Kết quả mong đợi:
Nâng cấp và hạ cấp thành công.
Nếu chọn lại gói hiện tại, hệ thống thông báo không cần thay đổi.
Basic giới hạn 5 booking được xác nhận, Premium không giới hạn theo cấu hình.

Kịch bản 4: Xóa Member

Các bước:
-> Chọn 1 → 5.
-> Nhập M004.
-> Chọn 1 → 2 để kiểm tra danh sách.

Kết quả mong đợi:
Xóa M004 thành công vì chưa có lịch sử điểm danh.
Danh sách còn M001, M002, M003.

II. Quản lý huấn luyện viên (Manage Trainers)

Kịch bản 5: Thêm Trainer và Input Validation

Các bước:
-> Chọn 2 → 1.
-> Nhập tên rỗng, sau đó nhập Demo Trainer.
-> Nhập email sai abc, sau đó nhập trainer.demo@gmail.com.
-> Nhập số điện thoại sai 123, sau đó nhập 0911111111.
-> Để chuyên môn rỗng, sau đó nhập Yoga.
-> Chọn 2 → 2 để kiểm tra danh sách.

Kết quả mong đợi:
Các trường không hợp lệ đều bị từ chối.
Trainer T004 được thêm thành công với chuyên môn Yoga.

Kịch bản 6: Cập nhật Trainer

Các bước:
-> Chọn 2 → 3, nhập T004.
-> Chọn 1, đổi tên thành Demo Trainer Updated.
-> Chọn 2 → 3, nhập T004, chọn 2 để cập nhật email.
-> Chọn 2 → 3, nhập T004, chọn 3 để cập nhật số điện thoại.
-> Chọn 2 → 3, nhập T004, chọn 4, đổi chuyên môn thành Pilates.
-> Chọn 2 → 2 để xem lại thông tin.

Kết quả mong đợi:
Cập nhật thành công thông tin Trainer.
ID T004 không thay đổi.

Kịch bản 7: Xóa Trainer

Các bước:
-> Chọn 2 → 5, nhập T001.
-> Quan sát thông báo lỗi.
-> Chọn 2 → 5, nhập T004.
-> Chọn 2 → 2 để xem danh sách.

Kết quả mong đợi:
Không thể xóa T001 vì đang phụ trách C001.
T004 được xóa thành công vì chưa phụ trách lớp nào.

III. Quản lý lớp học và xung đột Trainer

Kịch bản 8: Tạo lớp bị trùng lịch Trainer

Các bước:

-> Chọn 3 → 1.
-> Nhập tên lớp: Zumba Extra.
-> Nhập thời gian sai abc, sau đó nhập 2026-11-05 18:30.
-> Nhập thời lượng 0, sau đó nhập 60.
-> Nhập sức chứa 0, sau đó nhập 2.
-> Thử chọn Trainer 99, sau đó chọn 1 (T001).

Kết quả mong đợi:
Các giá trị không hợp lệ bị từ chối.
Hệ thống từ chối tạo lớp do T001 đã phụ trách C001 từ 18:00–19:00.
Lớp mới dự kiến diễn ra từ 18:30–19:30 nên trùng 30 phút.

Kịch bản 9: Tạo Class thành công

Các bước:
-> Chọn 3 → 1.
-> Nhập tên: Zumba Extra.
-> Nhập thời gian: 2026-11-05 18:30.
-> Nhập thời lượng: 60.
-> Nhập sức chứa: 2.
-> Chọn Trainer số 2 (T002).
-> Chọn 3 → 2 để kiểm tra danh sách Class.

Kết quả mong đợi:
Tạo C004 thành công.
C004 diễn ra 18:30–19:30, do T002 phụ trách.
Kịch bản 10: Phân công Trainer và kiểm tra xung đột

Các bước:
-> Chọn 2 → 4, nhập Trainer T001, Class C004.
-> Quan sát thông báo xung đột với C001.
-> Chọn 3 → 4, nhập Class C004, Trainer T003.
-> Kiểm tra kết quả phân công.
-> Chọn 2 → 4, nhập Trainer T002, Class C004 để khôi phục.

Kết quả mong đợi:
T001 bị từ chối do trùng lịch.
T003 được phân công khi không trùng lịch.
Khôi phục T002 thành công.
Một lớp chỉ có một Trainer phụ trách.

Kịch bản 11: Cập nhật lớp học

Các bước:
-> Chọn 3 → 3, nhập C004.
-> Chọn 1, đổi tên thành Zumba Evening.
-> Chọn 3 → 3, nhập C004, chọn 4.
-> Nhập thời lượng mới 90.
-> Chọn 3 → 2 kiểm tra giờ kết thúc.
-> Đổi thời lượng về 60.

Kết quả mong đợi:
Tên lớp được cập nhật.
Khi DurationMinutes = 90, giờ kết thúc là 20:00.
Khi DurationMinutes = 60, giờ kết thúc là 19:30.

IV. Booking và xung đột Member

Kịch bản 12: Đặt lớp thành công

Các bước:
-> Chọn 4 → 1.
-> Nhập Member M001.
-> Nhập Class C001.
-> Chọn 4 → 4 kiểm tra.

Kết quả mong đợi:
Booking BK0001 được tạo thành công.
Trạng thái Confirmed.
C001 có 1/10 chỗ được sử dụng.

Kịch bản 13: Đặt trùng một lớp

Các bước:
-> Chọn 4 → 1.
-> Nhập Member M001.
-> Nhập Class C001 lần nữa.

Kết quả mong đợi:
Hệ thống từ chối booking trùng.
Không tạo Booking ID mới.

Kịch bản 14: Xung đột lịch Member

Các bước:
-> Chọn 4 → 1.
-> Nhập Member M001.
-> Nhập Class C004.

Kết quả mong đợi:
Hệ thống từ chối vì M001 đã đặt C001 từ 18:00–19:00.
C004 diễn ra từ 18:30–19:30, trùng 30 phút với C001.
Không tạo booking mới.

Kịch bản 15: Đặt nhiều lớp không trùng lịch

Các bước:
-> Chọn 4 → 1, đặt M002 vào C004 → BK0002.
-> Chọn 4 → 1, đặt M002 vào C003 → BK0003.
-> Chọn 4 → 1, đặt M002 vào C002 → BK0004.
-> Chọn 4 → 4 để kiểm tra.

Kết quả mong đợi:
Cả ba booking thành công.
M002 có ba booking Confirmed vì các lớp diễn ra vào những ngày khác nhau.

Kịch bản 16: Xung đột khi cập nhật lịch Class

Các bước:
-> Chọn 3 → 3, nhập C004.
-> Chọn 2, cập nhật thời gian thành 2026-11-07 18:30.
-> Quan sát thông báo xung đột M002 với C003.
-> Chọn 3 → 3, nhập C004, chọn 2.
-> Nhập thời gian 2026-11-06 18:30.
-> Quan sát thông báo xung đột Trainer T002 với C002.

Kết quả mong đợi:
Lần đầu bị từ chối do trùng lịch Member.
Lần thứ hai bị từ chối do trùng lịch Trainer.
C004 giữ nguyên lịch ban đầu.

Kịch bản 17: Hai lớp liền kề không bị trùng lịch

Các bước:
-> Chọn 3 → 3, nhập C004.
-> Chọn 2, cập nhật thời gian thành 2026-11-05 19:00.
-> Chọn 4 → 1, đặt M001 vào C004.
-> Chọn 4 → 4 kiểm tra Booking.

Kết quả mong đợi:
C004 chuyển thành 19:00–20:00.
M001 đặt C004 thành công (BK0005).
C001 kết thúc đúng lúc C004 bắt đầu nên không xảy ra xung đột.

Kịch bản 18: Xung đột khi thay đổi DurationMinutes

Các bước:
-> Chọn 3 → 3, nhập C001.
-> Chọn 4 để cập nhật thời lượng.
-> Nhập DurationMinutes = 90.
-> Quan sát kết quả.

Kết quả mong đợi:
C001 dự kiến chuyển thành 18:00–19:30.
M001 đã đặt C004 lúc 19:00–20:00.
Hệ thống từ chối tăng thời lượng vì trùng lịch Member.
DurationMinutes của C001 vẫn là 60.

Kịch bản 19: Validation sức chứa

Các bước:
-> Chọn 3 → 3, nhập C004.
-> Chọn 3 để cập nhật sức chứa.
-> Nhập sức chứa mới 1.

Kết quả mong đợi:
C004 đang có M001 và M002.
Không thể giảm sức chứa xuống thấp hơn 2.
Sức chứa vẫn là 2.

V. Waitlist và hủy Booking

Kịch bản 20: Thêm Member vào Waitlist

Các bước:
-> Chọn 3 → 3, nhập C001.
-> Chọn 3, giảm sức chứa từ 10 xuống 1.
-> Chọn 4 → 1, đặt M003 vào C001 → BK0006.
-> Chọn 4 → 1, đặt M002 vào C001 → BK0007.
-> Chọn 4 → 4 để xem danh sách.

Kết quả mong đợi:
M001 đang giữ vị trí chính thức.
M003 vào Waitlist vị trí 1.
M002 vào Waitlist vị trí 2.

Kịch bản 21: Hủy Booking và tự động đôn Waitlist

Các bước:
-> Chọn 4 → 2, nhập C001 và BK0007 để hủy lượt chờ của M002.
-> Kiểm tra M002 được loại khỏi Waitlist.
-> Chọn 4 → 3, nhập C001 để thử đôn khi lớp vẫn đầy.
-> Chọn 4 → 2, nhập C001 và BK0001 để hủy booking của M001.
-> Chọn 4 → 4 kiểm tra danh sách.

Kết quả mong đợi:
BK0007 chuyển sang Cancelled.
Không thể đôn thủ công khi lớp đã đầy.
BK0001 chuyển sang Cancelled.
M003 được tự động đôn từ Waitlist lên Confirmed với BK0006.

Kịch bản 22: Tăng sức chứa và kiểm tra Waitlist

Các bước:
-> Chọn 3 → 3, nhập C001.
-> Chọn 3, tăng sức chứa lên 2.
-> Chọn 4 → 3, nhập C001.
-> Kiểm tra danh sách chờ.

Kết quả mong đợi:
Tăng sức chứa thành công.
Waitlist trống, không còn Member để đôn.

VI. Điểm danh và xóa dữ liệu

Kịch bản 23: Điểm danh thành công

Các bước:
-> Chọn 5 → 1, nhập C001.
-> Kiểm tra M003 trong danh sách chính thức.
-> Chọn 5 → 2, nhập C001 và M003.
-> Chọn 5 → 2, nhập C004 và M001.
-> Chọn 5 → 1, nhập C004 để kiểm tra.

Kết quả mong đợi:
M003 được điểm danh tại C001.
M001 được điểm danh tại C004.
Hai booking chuyển sang Attended.
Kịch bản 24: Validation khi điểm danh

Các bước:
-> Chọn 5 → 2, nhập C001 và M003 lần nữa.
-> Chọn 5 → 2, nhập C001 và M001.
-> Chọn 4 → 2, nhập C004 và BK0005.

Kết quả mong đợi:
Không thể điểm danh M003 lần hai.
M001 không thể điểm danh C001 vì booking đã bị hủy.
Không thể hủy BK0005 vì đã được điểm danh.
Kịch bản 25: Kiểm tra xóa dữ liệu đã điểm danh

Các bước:
-> Chọn 1 → 5, thử xóa M003.
-> Chọn 3 → 5, thử xóa C001.
-> Chọn 2 → 5, thử xóa T001.

Kết quả mong đợi:
Không thể xóa M003 vì có lịch sử điểm danh.
Không thể xóa C001 vì có booking Attended.
Không thể xóa T001 vì còn phụ trách C001.

Kịch bản 26: Xóa Class chưa có điểm danh

Các bước:
-> Chọn 3 → 5.
-> Nhập C002.
-> Chọn 3 → 2 kiểm tra danh sách Class.
-> Chọn 6 → 3 kiểm tra báo cáo.

Kết quả mong đợi:
C002 được xóa thành công.
BK0004 của M002 chuyển sang Cancelled.
Danh sách còn C001, C003, C004.

VII. Báo cáo hoạt động (Reports)

Kịch bản 27: Báo cáo từng Member

Các bước:
-> Chọn 6 → 1, nhập M001.
-> Kiểm tra lịch sử BK0001 và BK0005.
-> Chọn 6 → 1, nhập M002.
-> Chọn 6 → 1, nhập M003.
-> Thử nhập Member ID M999.

Kết quả mong đợi:
M001: 1 Cancelled, 1 Attended.
M002: 2 Confirmed, 2 Cancelled.
M003: 1 Attended.
M999 báo không tìm thấy hội viên.

Kịch bản 28: Báo cáo toàn bộ Member

Các bước:
-> Chọn 6 → 2.
-> Quan sát thông tin của M001, M002 và M003.
-> Kiểm tra các gói tập và lịch sử booking.

Kết quả mong đợi:
Hiển thị đầy đủ thông tin các hội viên.
Lịch sử booking có cả trạng thái Confirmed, Cancelled và Attended
Kịch bản 29: Thống kê hoạt động chung

Các bước:
-> Chọn 6 → 3.
-> Kiểm tra tổng số Member, Trainer và Class.
-> Kiểm tra số lượng Booking theo từng trạng thái.

Kết quả mong đợi trong cùng phiên chạy:
Member: 3.
Trainer: 3.
Class: 3.
Tổng Booking lịch sử: 7.
Attended: 2.
Confirmed: 2.
WaitListed: 0.
Cancelled: 3.
