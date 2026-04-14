# 🎮 AI GAME STUDIO: SYSTEM ARCHITECTURE & WORKFLOW DOCUMENT (V2.0)

## 1. TỔNG QUAN HỆ THỐNG (SYSTEM OVERVIEW)
- **Mục tiêu:** Tự động hóa quy trình sản xuất Playable Ads (HTML5) chất lượng cao, từ phân tích xu hướng đến file thành phẩm hoàn chỉnh.
- **Môi trường vận hành:** `Google Antigravity` (Local Workspace).
- **Cơ chế cốt lõi:** - Sử dụng khung mẫu (Template-based) để đảm bảo độ ổn định của mã nguồn.
  - Quy trình nén ảnh WebP và nhúng Base64 tự động qua Terminal.
  - Kiểm thử trực quan (Visual Testing) thông qua Browser nội bộ.
- **Đầu ra (Output):** 01 file `index.html` duy nhất (Single-file), dung lượng tối ưu < 3MB, tương thích 100% với Facebook, Google, và các Ad Networks.

---

## 2. ĐỘI NGŨ AGENT & VAI TRÒ CHUYÊN BIỆT

### 🤖 AGENT 1: MARKET TREND ANALYST (Chuyên gia Thị trường)
- **Nhiệm vụ:** Phân tích dữ liệu từ `task_input.json`. Xác định:
  - Thể loại (Genre) đang hot.
  - Cơ chế cốt lõi (Core Mechanic) có tỷ lệ giữ chân cao.
  - Chủ đề (Visual Theme) hấp dẫn thị giác.
- **Output:** `Market_Gap.md`.

### 🤖 AGENT 2: LEAD GAME DESIGNER (Giám đốc Thiết kế)
- **Nhiệm vụ:** Chuyển hóa phân tích thị trường thành kịch bản game.
- **Ràng buộc thiết kế:** - Vòng lặp gameplay (Core Loop) không quá 20 giây.
  - Cách điều khiển (Controls) phải cực kỳ đơn giản (Tap/Swipe).
  - Ưu tiên thiết kế nhân vật/vật thể dạng tĩnh hoặc chuyển động trượt để tối ưu tài nguyên AI Art.
- **Output:** `GDD.md` (Game Design Document).

### 🤖 AGENT 3: UA & MONETIZATION STRATEGIST (Chuyên gia UA)
- **Nhiệm vụ:** Thiết kế luồng chuyển đổi người dùng.
  - Thiết kế màn hình kết thúc (End Card).
  - Đặt nút Call-to-Action (CTA) "Tải ngay" kèm logic chuyển hướng Store.
- **Output:** Cập nhật nội dung CTA & End Card vào `GDD.md`.

### 🤖 AGENT 4B: AI ART DIRECTOR & OPTIMIZER (Sản xuất Đồ họa)
- **Nhiệm vụ:**
  1. Gọi API (DALL-E/Midjourney) để tạo Asset dựa trên Theme.
  2. **Tối ưu kỹ thuật (Terminal):** Tự động xóa nền, Crop ảnh sát biên.
  3. **Siêu nén:** Chuyển đổi định dạng sang **WebP** với chất lượng tối ưu (giảm 80% dung lượng so với PNG).
- **Output:** Thư mục `/assets/*.webp`.

### 🤖 AGENT 4A: PLAYABLE ENGINEER (Kỹ sư Lập trình)
- **Nhiệm vụ:**
  1. **Template Selection:** Chọn khung code Phaser 3 phù hợp nhất từ thư viện (Survivor, Merge, Runner, hoặc Puzzle).
  2. **Code Injection:** Bơm các thông số từ GDD (tốc độ, trọng lực, thời gian) và mã hóa Base64 các ảnh WebP để nhúng thẳng vào Template.
  3. **Visual Debug:** Sử dụng Browser để kiểm tra hitbox vật lý. Tự động điều chỉnh `setSize` và `setOffset` dựa trên ảnh chụp màn hình.
- **Output:** File `index.html` hoàn chỉnh.

### 🤖 AGENT 5: EXECUTIVE PRODUCER (Giám đốc Sản xuất)
- **Nhiệm vụ:** Kiểm duyệt cuối cùng.
  - Chạy thử game trên Browser để kiểm tra "Game Feel".
  - Đảm bảo không có lỗi JavaScript Console.
  - Xác nhận nút CTA hoạt động đúng mục tiêu.
- **Output:** `status_log.json` (Trạng thái: Hoàn tất).

---

## 3. QUY TRÌNH THỰC THI CHI TIẾT (PIPELINE)

1. **Giai đoạn Khởi tạo:** User nhập yêu cầu qua Dashboard -> Hệ thống tạo `task_input.json`.
2. **Giai đoạn Thiết kế:** Agent 1 & 2 xây dựng hồ sơ `GDD.md`.
3. **Giai đoạn Sản xuất Art:** Agent 4B sinh ảnh -> Script Terminal nén WebP -> Lưu vào `/assets`.
4. **Giai đoạn Lập trình:** Agent 4A chọn Template -> Script Terminal mã hóa Base64 -> Nhúng vào HTML -> Xuất bản `index.html`.
5. **Giai đoạn QA:** Agent 4A & 5 kiểm thử trên Browser tích hợp -> Tự động sửa lỗi hiển thị/vật lý.
6. **Giai đoạn Bàn giao:** File `index.html` sẵn sàng để tải về hoặc đẩy lên hệ thống chạy Ads.

---

## 4. RÀNG BUỘC KỸ THUẬT (TECHNICAL CONSTRAINTS)
- **Dung lượng:** Tuyệt đối không vượt quá 3MB (đã nén).
- **Kiến trúc file:** Mọi thứ (HTML, CSS, JS, Assets) bắt buộc nằm trong 1 file duy nhất.
- **Hiệu suất:** Thời gian nạp game (Load time) dưới 2 giây trên môi trường mobile web.
- **Cấu hình:** Phải có biến `GameConfig` ở đầu file để người dùng can thiệp nhanh các chỉ số gameplay mà không cần can thiệp logic sâu.