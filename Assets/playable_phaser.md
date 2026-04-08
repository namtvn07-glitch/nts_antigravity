1. MỤC TIÊU TỔNG QUÁT
Xây dựng một hệ thống AI Agent tự động hóa quy trình sản xuất Playable Ads (Quảng cáo tương tác) cho Game Mobile, từ khâu nhận tài nguyên (Assets) đến khâu xuất file hoàn chỉnh cho đa nền tảng (Google, Meta, AppLovin, Unity).

2. THÔNG SỐ KỸ THUẬT CỐT LÕI
Engine: Phaser 3 (Javascript).

Môi trường: Google Antigravity (Sử dụng Model Context Protocol - MCP).

Giới hạn dung lượng: < 5MB (Ưu tiên nén tối đa).

Cấu trúc File: Single File HTML (Tất cả logic, CSS, và Assets phải được nhúng trực tiếp bằng Base64).

Tính tương thích: Chạy tốt trên Mobile WebView (Responsive 100%).

3. CÁC KỸ NĂNG (SKILLS) CẦN THIẾT CHO AGENT
Hệ thống cần 4 Skill chính được thiết kế chuyên biệt:

Skill 1: The Scaffolder (Kiến trúc khung)
Nhiệm vụ: Khởi tạo Boilerplate Phaser 3 tối ưu cho quảng cáo.

Yêu cầu: Thiết lập hệ thống Game State (Loading -> Tutorial -> Gameplay -> Endcard) và đảm bảo tính năng tự động co dãn màn hình (Responsive Scaling).

Skill 2: Asset Transformer (Chuyển đổi tài nguyên)
Nhiệm vụ: Xử lý hình ảnh/âm thanh đầu vào.

Yêu cầu: Tự động nén (compress), tạo SpriteSheet nếu cần, và mã hóa tất cả sang Base64 để tích hợp vào file Javascript.

Skill 3: Platform Bridge (Cầu nối nền tảng)
Nhiệm vụ: Tích hợp SDK quảng cáo.

Yêu cầu: Tự động nhận diện nền tảng mục tiêu để chèn các hàm Call-To-Action (CTA) chính xác (Ví dụ: mraid.open() cho AppLovin, FbPlayableAd.onCTAClick() cho Facebook).

Skill 4: Logic Orchestrator (Điều phối logic tương tác)
Nhiệm vụ: Viết code cho "Core Loop" của game dựa trên mô tả kịch bản.

Yêu cầu: Tạo các tương tác đơn giản (Drag, Click, Swipe) và kết nối với màn hình Game Over để điều hướng người dùng về Store.

4. QUY TRÌNH ĐẦU VÀO & ĐẦU RA (I/O)
Đầu vào (Input):

Thư mục chứa Asset (ảnh, âm thanh).

Bản mô tả kịch bản (ví dụ: "Game Match-3 đơn giản, thắng sau 3 nước đi").

Đầu ra (Output):

01 File index.html duy nhất chứa toàn bộ dự án.

Bản báo cáo dung lượng (File size report).

5. RÀNG BUỘC KỸ THUẬT CHO PROMPT ENGINEER
No Hallucination: Code Phaser phải tuân thủ đúng phiên bản 3.x (không dùng các hàm đã lỗi thời).

Modular Code: Chia nhỏ code thành các module để Agent dễ dàng debug khi có lỗi.

Performance First: Hạn chế sử dụng các thư viện ngoài không cần thiết để giữ dung lượng nhẹ nhất.