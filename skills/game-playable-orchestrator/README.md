# game-playable-orchestrator

> **Mô tả:** AI Game Studio Orchestrator to generate HTML5 Playable Ads (Phaser 3) from scratch using an autonomous 5-phase pipeline.

## 🎯 Mục đích sử dụng
Đóng vai trò là **Executive Producer** (Giám đốc sản xuất) cho Playable Game Studio. Skill này chịu trách nhiệm biến một dự án game hoàn chỉnh (có GDD, Assets, Audio) thành một bản HTML5 Playable Ad tối ưu hóa cao bằng Phaser 3 (chỉ có MỘT file duy nhất, dung lượng < 5MB, sử dụng Base64 media).

## ⚙️ Kích hoạt
Sử dụng lệnh `@[/game-playable-orchestrator] <Idea>` để tạo project mới. Sử dụng lệnh "Tiếp tục" hoặc "Continue" để chạy phase tiếp theo.

## 📥 Đầu vào (Input)
- Yêu cầu ban đầu (New Request) hoặc lệnh "Tiếp tục".
- Trạng thái hiện tại của project được lưu tại `<workspace>/Assets/PlayableGameStudio/Projects/<Project_Name>/.studio_state`.
- Các file hướng dẫn (`phase1_ingest.md`, `phase2_harvest.md`, `phase3_dev.md`, `phase4_package.md`).

## 📤 Đầu ra (Output)
Một file monolithic HTML duy nhất chứa toàn bộ game logic (viết bằng Phaser 3) và assets (Base64), sẵn sàng để dùng làm Playable Ad.

## ⚠️ Lưu ý quan trọng
- **Quy định Kiến trúc (Architectural Rules):**
  1. Không tự bịa ra ngữ cảnh. Bắt buộc đọc `GDD.md`, `task_input.json` và tracker flag.
  2. **Phase-Gated Execution:** BẮT BUỘC dừng và hỏi ý kiến người dùng sau mỗi Phase. Không được tự ý nhảy sang phase tiếp theo nếu chưa có lệnh "Continue".
  3. Cấm hard-code file HTML từ đầu. Chỉ viết logic loop vào `logic_hook.js`, hệ thống sẽ tự bơm (inject) vào `phaser_base.html` ở Phase 4.
- Kích hoạt workflow `/debug` nếu gặp lỗi HTML5/Phaser ở Phase 3.
- Kết thúc ở Phase 4 bằng workflow `/finish` và `/commit`.
