# game-dev-unity

> **Mô tả:** Use when starting any Unity game development task, architecture design, optimizing rendering, scripting, multi-platform deployment, or performance profiling.

## 🎯 Mục đích sử dụng
Đây là **Unity Master Skill Router** (Kỹ năng điều phối trung tâm cho Unity). Bất kỳ tác vụ lập trình, thiết kế hệ thống, hay refactor code nào trong Unity đều BẮT BUỘC phải tuân thủ chuẩn mực kiến trúc (`DevArchitechture`) và phải thông qua skill này để điều hướng đến các Sub-Skill chuyên biệt (VD: UI, Physics, Rendering, Networking...).

## ⚙️ Kích hoạt
Luôn luôn kích hoạt (Status: MANDATORY) khi bắt đầu bất kỳ tác vụ phát triển game Unity nào.

## 📥 Đầu vào (Input)
- Yêu cầu công việc liên quan đến Unity (VD: "Tối ưu hóa UI", "Viết script di chuyển").

## 📤 Đầu ra (Output)
- Chuyển hướng người dùng/agent đến đúng Sub-Skill tương ứng (VD: `sub-skill/unity-ui-optimization.md`).
- Kích hoạt chuỗi Workflow bắt buộc.

## ⚠️ Lưu ý quan trọng
- **SUPREME DIRECTIVE (Chỉ thị tối cao):** Cấm tuyệt đối việc tự ý viết code Unity mà không đọc các Sub-Skill liên quan. Code sẽ bị từ chối nếu vi phạm.
- **Workflow Bắt buộc:** 
  1. `/plan` (Lên kế hoạch)
  2. `/execute` (Thực thi)
  3. `/debug` & `/review` (Sửa lỗi & Đánh giá)
  4. `/finish` (Nghiệm thu & Lưu kiến thức)
- Không được biện hộ "Tác vụ quá nhỏ nên không cần đọc Sub-Skill". Mọi tác vụ đều tiềm ẩn rủi ro về Memory/GC hoặc logic Unity. Bắt buộc phải load background trước khi làm.
