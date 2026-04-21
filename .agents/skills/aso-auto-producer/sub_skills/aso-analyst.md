---
name: aso-analyst
description: Hành động nội bộ - dùng để thu thập thông tin link store và tạo ASO_Design_Plan.md
---

# ASO Analyst

Nhiệm vụ: Cào data, phân tích vả viết bản kế hoạch nghệ thuật.

## Các bước thực hiện:
1. Nhận link, khởi động `browser_subagent`. Chỉ "nhìn" Above-the-fold (Header, Logo, Screenshot đầu). Trích xuất Vibe, Color Palette, Art Style. Trích xuất Text Description nếu thể hiện trên DOM.
2. Search web với từ khoá "app store screenshot trend cho thể loại [genre]".
3. Quét `Assets/GameArtist/StyleLibrary` xem có style profile nào phù hợp không.
4. Hỏi Orchestrator/User: Tên của dự án/thư mục này là gì? Chọn style nào?
5. Ghi kết quả vào `ASO_Design_Plan.md`. ĐÓNG ĐINH các "Prompt tiếng Anh" cùng với kích thước tĩnh phải ghi rõ: Icon (512x512), Feature (1024x500), Screenshot 1-5 (900x1600 cho màn hình dọc, hoặc 1600x900 cho ngang tuỳ tính chất game). Cũng cần ghi lại trường `ProjectFolder` để làm tên thư mục con lưu trữ ảnh con.
> **LƯU Ý QUAN TRỌNG:** Kế hoạch phải chỉ định rõ `generate_image`, không được sinh chữ (typography/text) trong ảnh, ảnh xuất ra phải là minh họa sạch thuần tuý.
> Kế hoạch phải định nghĩa một Anchor Character chung cho cả 5 Screenshots để tạo tính nhất quán.
