# game-art-configurator

> **Mô tả:** A knowledge management skill to modify, query, or add rules to the Global_DNA.md and automatically vectorize them for the RAG system.

## 🎯 Mục đích sử dụng
Đóng vai trò là tác nhân Quản lý Kiến thức Chủ động (Active Knowledge Management Agent) đối với file cấu hình gốc về triết lý thiết kế tổng thể của game (`Assets/GameArtist/Global_DNA.md`). Skill này giúp thêm, sửa, truy vấn luật thiết kế mới, đồng thời tự động cập nhật cơ sở dữ liệu Vector (RAG) và kiểm tra xem có xung đột nào với các style cục bộ hiện tại hay không.

## ⚙️ Kích hoạt
Skill được gọi khi người dùng yêu cầu thay đổi, thêm, hoặc truy vấn quy tắc về thiết kế chung (Global rules). Ví dụ: "Thêm luật UI phải chiếm 80% canvas".

## 📥 Đầu vào (Input)
- **Yêu cầu thay đổi luật thiết kế:** Nội dung quy tắc mới cần thêm/sửa vào `Global_DNA.md`.
- File cấu hình gốc hiện tại: `<workspace>/Assets/GameArtist/Global_DNA.md`.

## 📤 Đầu ra (Output)
1. **`Global_DNA.md` (đã cập nhật)**: Chứa quy tắc mới được chèn vào đúng heading.
2. **`global_index.json` (đã cập nhật)**: Sinh ra từ quá trình chạy script `create_global_embeddings.py` để cập nhật dữ liệu RAG.

## ⚠️ Lưu ý quan trọng
- Phải lập tức chạy script vectorization (`python scripts/create_global_embeddings.py`) sau khi cập nhật markdown.
- Cần chú ý đọc log của script ở Terminal. Nếu script báo lỗi `[WARNING]` về **Conflict** (xung đột luật giữa Global và Local), bạn **BẮT BUỘC** phải thông báo lại cho người dùng trong phản hồi.
- Kết thúc bằng workflow `/finish` để lưu các xung đột cấu trúc tìm được vào thư mục `docs/learned/`.
