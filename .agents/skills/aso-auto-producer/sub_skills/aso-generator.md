---
name: aso-generator
description: Hành động nội bộ - đọc ASO_Design_Plan.md để gọi tool generate_image tốc độ cao
---

# ASO Generator

Nhiệm vụ: Chạy vòng lặp vô tri gen ảnh từ Plan. Không cần lý luận nội dung.

## Các bước thực hiện:
1. Đọc nội dung file `ASO_Design_Plan.md`.
2. Extract danh sách 7 tasks cần vẽ (Icon, Feature, 5 Screenshots) và trích xuất `ProjectFolder` (Tên dự án).
3. Lần lượt gọi tool `generate_image` với đúng `Prompt` đã ghi trong file. **BẮT BUỘC BẮT BUỘC:** Bạn PHẢI truyền đường dẫn tuyệt đối của các ảnh khuôn (Template) có sẵn vào tham số `ImagePaths` của tool dựa theo loại ảnh cần sinh để kích hoạt chế độ ép tỷ lệ và kích thước khung hình:
   - Icon (512x512): Truyền `[absolute_workspace_path]\.agents\skills\aso-auto-producer\templates\icon_512.png`
   - Feature Graphic (1024x500): Truyền `[absolute_workspace_path]\.agents\skills\aso-auto-producer\templates\feature_1024x500.png`
   - Screenshots màn hình dọc (900x1600): Truyền `[absolute_workspace_path]\.agents\skills\aso-auto-producer\templates\screenshot_v_900x1600.png`
   - Screenshots màn hình ngang (1600x900): Truyền `[absolute_workspace_path]\.agents\skills\aso-auto-producer\templates\screenshot_h_1600x900.png`
   *> **Lưu ý:** Tuyệt đối KHÔNG in/nhúng lại các ảnh đã sinh ra màn hình chat (không cần báo cáo hình ảnh cho User ở bước này).*
4. Dùng tool `run_command` (lệnh shell copy) để di chuyển và lưu trữ toàn bộ ảnh sinh ra vào chung một folder chuyên biệt tại `Assets/GameArtist/Generated/[ProjectFolder]/`.
5. **HẬU KỲ KHUNG HÌNH (QUAN TRỌNG):** Tool `generate_image` đôi khi sẽ phớt lờ Template và trả về ảnh vuông. Do đó, sau khi lưu ảnh, bạn phải chạy lệnh PowerShell kiểm tra kích thước thật của ảnh trước.
   - CHỈ KHI kích thước của file ảnh không khớp với chuẩn quy định, bạn mới được gọi lệnh chạy script python rọc ảnh:
   `python [absolute_workspace_path]\.agents\skills\aso-auto-producer\scripts\resize_image.py --image "<đường_dẫn_ảnh>" --width <chiều_rộng_đích> --height <chiều_cao_đích> --out "<đường_dẫn_ảnh>"`
   - Nếu ảnh đã sinh ra đúng chuẩn rồi thì tuyệt đối không được gọi script.
