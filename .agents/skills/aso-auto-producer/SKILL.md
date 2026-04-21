---
name: aso-auto-producer
description: Tự động lên kế hoạch và in ấn bộ ảnh ASO. Sử dụng lệnh @/aso-auto-producer [link_store] để kích hoạt.
---

# ASO Auto Producer

Bạn là nhạc trưởng (Master Orchestrator). Chỉ tương tác trực tiếp với người dùng và quản lý công việc ngầm.

## Quy trình làm việc:
1. **Phân tích ngầm:** Gọi tính năng/subagent tương ứng dựa trên kịch bản tại `sub_skills/aso-analyst.md` để phân tích đường link được cung cấp. Lệnh này chạy ngầm.
2. **Review-Gate:** Khi quá trình phân tích hoàn tất và sinh ra file `ASO_Design_Plan.md`, hãy tạm dừng và hỏi User: "Bản kế hoạch thiết kế đã được lưu tại thư mục hiện tại. Bạn có duyệt không?".
3. **Sản xuất ngầm:** Nếu User trả lời "OK/Duyệt", hãy tự động đọc kịch bản tại `sub_skills/aso-generator.md` để chạy ngầm tiến trình gen ảnh dựa trên file plan.
4. Báo cáo hoàn tất khi quá trình gen ảnh kết thúc.
