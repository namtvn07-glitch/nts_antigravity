# game-designer

> **Mô tả:** Master orchestrator skill for creating Game Design Documents (GDD) via an autonomous 6-phase pipeline including document generation and UI wireframing.

## 🎯 Mục đích sử dụng
Là kỹ năng Orchestrator cốt lõi (Super Skill) trong việc thiết kế Game (Game Design Pipeline). Nó đảm nhiệm việc dịch và thiết kế ý tưởng trò chơi ban đầu thành tài liệu hoàn chỉnh qua 6 giai đoạn, từ GDD, chia nhỏ ra từng phòng ban (Art, Dev, Audio, UI), đến việc phác thảo UI/UX (ASCII, Mermaid) và thiết lập tài liệu kỹ thuật tổng hợp.

## ⚙️ Kích hoạt
Kích hoạt tự động hoặc khi người dùng có ý tưởng và muốn tạo Game Design Document (GDD).

## 📥 Đầu vào (Input)
- Ý tưởng game ban đầu của người dùng (Raw Idea).
- Các file subskill (`subskills/*.md`) làm tài liệu hướng dẫn cho mỗi phase.

## 📤 Đầu ra (Output)
Một bộ hồ sơ đầy đủ bao gồm:
- `[ProjectName]_Master_GDD.md`
- 5 file phân tích phòng ban (Art, Dev, AudioVFX, UI, Game Data).
- Tài liệu phác thảo UI/UX (Wireframes & Flowchart).
- `[ProjectName]_Integration_Map.md`.
- `[ProjectName]_Project_Hub.md` và `technical-spec.md`.

## ⚠️ Lưu ý quan trọng
- **PIPELINE TỰ ĐỘNG CHUỖI:**
  - **Phase 1 (Human Gate):** Phải tạo Master GDD và **DỪNG LẠI (STOP AND HALT EXECUTION)** để chờ người dùng phê duyệt ("Do you approve this Master GDD?").
  - **Phase 2 đến Phase 6 (Autonomous):** Sau khi Phase 1 được duyệt, AI bắt buộc phải chạy tự động liên tục một mạch nối tiếp nhau từ Phase 2 đến hết Phase 6, không được dừng lại giữa chừng.
- Phải tạo bản đồ Integration (đầu mối logic) giữa Code, Art, UI và Audio ở Phase 5.
- Hoàn thành bằng cách nhắc người dùng chạy workflow `/commit` và khởi chạy `/finish` để lưu mẫu thiết kế.
