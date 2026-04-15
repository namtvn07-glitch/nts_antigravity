# AI Game Artist Workflow - Design Specification

**Ngày cập nhật:** 2026-04-15
**Dự án:** Workflow AI Game Artist trên nền tảng Antigravity

## 1. Mục tiêu (Goal)
Tự động hóa quy trình sản xuất Game Asset thông qua công nghệ Multi-Agent trên nền tảng Antigravity. Hệ thống tiếp nhận phác thảo (draft), ảnh phong cách (style reference), ghép nối dựa trên các Template ID dựng sẵn và gọi thực thi thông qua Nano Banana Engine nhằm đảm bảo tính nhất quán cao, giảm rủi ro lỗi logic hình ảnh cho Artist.

## 2. Kiến trúc Hệ thống Multi-Agent (Orchestration)
Hệ thống sử dụng mô hình chuyên biệt hóa:
### 2.1 Art Director Agent
- **Vai trò:** Xử lý và Tối ưu hóa Text, Prompt Engineering.
- **Quy trình:**
  - Lấy User Intent & Text Prompt ban đầu.
  - Phân tích cấu trúc Template ID để lấy các Modifier bắt buộc (như lighting, camera angle, textures).
  - Xuất ra `enhanced_prompt` theo chuẩn JSON cho Nano Banana.

### 2.2 Vision Analyzer Agent
- **Vai trò:** Phân tích, tách lớp cho Inputs dạng Hình ảnh.
- **Quy trình:**
  - Nhận Draft Image và Style Reference.
  - Ánh xạ cấu trúc ControlNet tương ứng (Ví dụ: `draft -> controlnet_canny/depth`, `reference -> ip-adapter`). Khớp trọng số phù hợp.
  - Xuất ra cấu hình `vision_config`.

### 2.3 QA Reviewer Agent
- **Vai trò:** Rà soát tự động trước khi giao cho Artist duyệt.
- **Quy trình:**
  - Sử dụng Visual Language Model (VLM) nội bộ để kiểm tra chéo ảnh do Nano Banana trả về với User Intent gốc.
  - Đánh giá Pass/Fail kèm list lỗi. Nếu Fail, yêu cầu Director và Vision Agent chạy vòng lặp mới tự tinh chỉnh lại tham số.

## 3. Tích hợp Nano Banana Tool (Engine Integration)
- **Phương án tích hợp:** Xây dựng một native **Antigravity Tool/Skill**.
- **Cách thức hoạt động:** Gói gọn `enhanced_prompt` và `vision_config` lại gửi request HTTP trực tiếp sang API của Nano Banana. 
- **Bảo mật:** API Key của Nano Banana được giữ trong biến môi trường `.env` hoặc quản lý Secret của Workspace, tránh hardcode trên code logic Agent.

## 4. Quản lý trạng thái & Hiện vật (State & Artifact Management)
- **Artifact Tracking:** Mọi output (Draft ban đầu, Ref, các lần render ảnh v1, v2) được lưu giữ dưới hạng mục Artifact của dự án. 
- **Context Awareness:** Các phiên làm việc (Chat sessions) có thể đọc ngược Artifacts để tránh yêu cầu user upload lại file n lần. Artist trực tiếp gắn Feedback vào version ảnh lỗi, Agent tự đọc context đó để trigger QA và render phiên bản mới.
