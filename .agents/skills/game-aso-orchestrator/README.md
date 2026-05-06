# game-aso-orchestrator

> **Mô tả:** Automates the planning and production of ASO (App Store Optimization) assets. Use the command @/game-aso-orchestrator [store_link] to trigger.

## 🎯 Mục đích sử dụng
Skill đóng vai trò **Master Orchestrator** tự động hóa việc lên kế hoạch và sản xuất các tài nguyên (assets) tối ưu hóa cửa hàng ứng dụng (ASO). Nó tự động phân tích Vibe, vẽ phác thảo (sketch), và dịch thuật ngữ nghĩa (semantic translation) thành hình ảnh hoàn thiện (render) chất lượng cao.

## ⚙️ Kích hoạt
Gọi lệnh `@[/game-aso-orchestrator] [store_link]` để kích hoạt quy trình sản xuất.

## 📥 Đầu vào (Input)
- Đường link hoặc mô tả sơ bộ về dự án/ASO mong muốn.

## 📤 Đầu ra (Output)
Quy trình 3 bước (3 Phases Workflow):
1. **ASO_Design_Plan.md**: Bản kế hoạch phân tích và lựa chọn Style dựa trên RAG script `retrieve_aso_style.py`.
2. **Sketches**: 1 ảnh chính (Key Art), 1 Icon, và 5 ảnh chụp màn hình (Screenshots) phác thảo dưới dạng Bounding Box.
3. **Render hoàn thiện**: Các ảnh ASO chất lượng cao cuối cùng được xuất ra sau quá trình VLM Semantic Translation.

## ⚠️ Lưu ý quan trọng
- **HUMAN-IN-THE-LOOP**: Ở bước Sketch, bắt buộc phải vẽ Key Art trước, sau khi được người dùng duyệt mới vẽ tiếp Icon và 5 Screenshots. Phải hỏi ý kiến người dùng trước khi chuyển từ phác thảo sang render.
- **Generate Image tool constraints**: Hình ảnh render cuối cùng qua tool phải được resize và crop (thêm viền trong suốt - padding) sao cho giữ đúng tỷ lệ ban đầu bên trong một khung hình vuông 1:1.
- Gắn hình ảnh bằng đường dẫn Markdown tuyệt đối (`![Tên](absolute/path/file.png)`) và bắt buộc dùng dấu slash (`/`).
- Sau khi hoàn tất, cần dùng `/commit` và workflow `/finish` để kết thúc quá trình.
