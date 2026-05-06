# game-art-orchestrator

> **Mô tả:** Automates the process of generating game assets matching a specific art style using RAG, Few-Shot visual prompting (via generate_image), and a Human-In-The-Loop VLM evaluation pipeline.

## 🎯 Mục đích sử dụng
Skill đóng vai trò **Nhạc trưởng (Orchestrator)** trong việc tự động hóa tạo Game Assets tuân thủ chặt chẽ theo style (`Generation_DNA.md`) và luật Global. Đây là một pipeline 2 pha nghiêm ngặt: dùng RAG kết hợp Few-Shot Visual Prompting để sinh ảnh, và áp dụng luồng kiểm duyệt VLM có sự tham gia của con người (Human-in-the-Loop) để đảm bảo chất lượng.

## ⚙️ Kích hoạt
Tự động kích hoạt khi người dùng yêu cầu "draw", "generate", hoặc "create" một asset mới (ví dụ: vũ khí, nhân vật, icon...).

## 📥 Đầu vào (Input)
- **Prompt từ người dùng**: Yêu cầu mô tả tài nguyên cần vẽ.
- Cấu hình style: `Generation_DNA.md` (Local) và các luật lấy từ hệ thống RAG (Global).

## 📤 Đầu ra (Output)
1. Ảnh phác thảo (Sketch) hoặc Ảnh hoàn thiện (Render) được nhúng trực tiếp trong chat.
2. Form đánh giá Evaluator Scoring (Dựa trên `Evaluation_Rules.json`).
3. File ảnh chất lượng cao được lưu (export) đúng theo cấu trúc: `<workspace>/Assets/Projects/<Project_Name>/GameAssets/<Style_Name>/<Category>/[tên_file]_[YYYY-MM-DD].png`.
4. Cập nhật vào danh mục asset: `Generated_Asset_Catalog.md`.

## ⚠️ Lưu ý quan trọng
- **Quy tắc Phase 1 (Sketch):** Không được vẽ màu hoặc làm chi tiết ngay. Bắt buộc tạo **Sketch & Silhouettes** trước và phải chờ người dùng xác nhận (`Human-In-The-Loop`). Luôn sinh **duy nhất MỘT** nhân vật/vật thể ở giữa khung hình, nền Pure Magenta (`#FF00FF`), không đổ bóng nền.
- **Dữ liệu RAG:** Không tự ý phân tích file JSON, bắt buộc phải dùng text rules và ảnh từ kết quả của script `retrieve_orchestrator_context.py`.
- **Nhúng Ảnh:** Phải nhúng ảnh vào phản hồi bằng Markdown Format Tuyệt Đối (`![Image Name](absolute/path)`) và thay thế dấu backslash (`\`) bằng forward slash (`/`).
- **UI Category:** Nếu là UI, cần chạy thêm kịch bản cắt khoảng trong suốt (auto-resize/crop) bằng Python (thư viện PIL) và viết file hướng dẫn `UI_Integration_Guide.md`.
- Ghi chú vào workflow `/finish` và `/commit` khi công việc tạo ảnh đã hoàn tất và được nghiệm thu.
