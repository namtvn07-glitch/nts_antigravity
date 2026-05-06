# game-art-compiler

> **Mô tả:** Use this skill specifically when the user requests to "compile", "analyze", or "update" an art style based on a directory of asset images. The skill acts as a Data Ingestion pipeline that vectorizes style images using `sentence-transformers` and outputs specialized rules (`Generation_DNA.md` and `Evaluation_Rules.json`) explicitly designed to connect with the `game-art-orchestrator`.

## 🎯 Mục đích sử dụng
Skill này đóng vai trò là một **Data Ingestion Pipeline** (Luồng thu thập và xử lý dữ liệu) cho quy trình làm việc của Game Artist. Nó chuyển đổi một thư mục chứa các ảnh tham khảo (Reference Images) thành cấu trúc dữ liệu Vector toán học (`style_index.json`) và các bộ quy tắc (rulesets) cụ thể. Mục đích là để phân tách cơ chế tạo ảnh (Generation) khỏi các quy tắc đánh giá khắt khe (Evaluation heuristics).

## ⚙️ Kích hoạt
Được kích hoạt tự động hoặc thủ công khi người dùng yêu cầu "compile", "analyze", hoặc "update" một art style từ một thư mục ảnh.

## 📥 Đầu vào (Input)
- **Thư mục ảnh tham khảo:** Chứa các file ảnh (`.png`, `.jpg`, `.jpeg`) đại diện cho style cần phân tích (đường dẫn: `<workspace>/Assets/GameArtist/StyleLibrary/<style_name>/`).

## 📤 Đầu ra (Output)
1. **`Generation_DNA.md`**: File chứa các quy tắc thiết kế (Visual DNA, Product Logic, Technical Excellence, Gameplay Affordance) được cấu trúc chặt chẽ để phục vụ RAG (Retrieval-Augmented Generation).
2. **`Evaluation_Rules.json`**: File JSON chứa tiêu chí đánh giá nghiêm ngặt để nghiệm thu asset.
3. **`style_index.json`**: File chứa dữ liệu Vector embeddings của style (được tạo thông qua script `create_embeddings.py`).

## ⚠️ Lưu ý quan trọng
- **Legacy Cleanup:** Bắt buộc xóa file `DNA_Profile.md` (nếu có) vì đây là định dạng V1 đã lỗi thời để tránh làm ô nhiễm hệ thống.
- Sau khi chạy xong, phải sử dụng workflow `/finish` để ghi lại các vấn đề (gotchas) hoặc xung đột trong quá trình ingestion dữ liệu.
