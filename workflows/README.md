# 🔄 Project Workflows

> **Tổng quan:** Thư mục này chứa các quy trình làm việc (workflows) chuẩn hóa của dự án. Workflows giúp AI định tuyến công việc, làm việc có hệ thống, theo từng bước rõ ràng để giảm thiểu sai sót. 

## 📋 Danh sách Workflows

### 1. `/plan` (Lên kế hoạch)
- **File:** `plan.md`
- **Mục đích:** Xây dựng bản thiết kế kỹ thuật (`implementation_plan.md`) trước khi code.
- **Trigger:** `@[/plan]` hoặc gọi `/plan`
- **Hoạt động chính:**
  - Thu thập SMART POLE context.
  - Khám phá thiết kế (gọi skill `brainstorming` nếu cần).
  - Yêu cầu người dùng duyệt kế hoạch.

### 2. `/execute` (Thực thi)
- **File:** `execute.md`
- **Mục đích:** Thực thi kế hoạch đã duyệt theo từng Layer (Data -> Logic -> UI).
- **Trigger:** `@[/execute]` hoặc gọi `/execute`
- **Hoạt động chính:** 
  - Đọc `task.md` và `implementation_plan.md`.
  - Thực thi tuần tự các bước.
  - Dừng lại cập nhật `task.md` sau mỗi layer.

### 3. `/debug` (Sửa lỗi)
- **File:** `debug.md`
- **Mục đích:** Khắc phục lỗi bằng phương pháp luận có kỷ luật.
- **Trigger:** `@[/debug] [description]`
- **Hoạt động chính:**
  - Phân loại lỗi (Hard bug vs Simple bug).
  - Triển khai `phase-gated-debugging` cho lỗi khó, hoặc `bug-hunter` cho lỗi dễ.
  - Lưu lại nguyên nhân vào thư mục học hỏi (Learnings).

### 4. `/review` (Đánh giá code)
- **File:** `review.md`
- **Mục đích:** Code review trước khi commit.
- **Trigger:** `@[/review]`
- **Hoạt động chính:**
  - Kiểm tra diff git theo `GEMINI.md`.
  - Phân tích hiệu năng, C# Conventions, và Event Memory Leaks.
  - Trình bày lỗi và đề xuất tự động sửa.

### 5. `/commit` (Lưu lịch sử)
- **File:** `commit.md`
- **Mục đích:** Tạo commit message theo chuẩn Conventional Commits.
- **Trigger:** `@[/commit]`
- **Hoạt động chính:**
  - `git add` tự động các file trong hội thoại.
  - Phân tích diff và tạo commit message.
  - Chờ lệnh người dùng (nhấn 1, 2, 3) để thực thi.

### 6. `/finish` (Nghiệm thu & Học hỏi)
- **File:** `finish.md`
- **Mục đích:** Đóng task, trích xuất bài học và tạo `walkthrough.md`.
- **Trigger:** `@[/finish]`
- **Hoạt động chính:**
  - Trích xuất Patterns, Gotchas, Best Practices.
  - Cập nhật vào `GEMINI.md` hoặc các file domain tương ứng.
  - Đề xuất `/teach` nếu task phức tạp.

### 7. `/teach` (Giảng dạy)
- **File:** `teach.md`
- **Mục đích:** Viết tài liệu "Debrief" (Rút kinh nghiệm) chi tiết như một giáo viên giảng bài.
- **Trigger:** Được gọi từ `/finish` hoặc gọi trực tiếp `@[/teach]`.
- **Hoạt động chính:**
  - Viết tài liệu dạng "trò chuyện/cafe" vào `docs/teach/yyyy-mm-dd_feature-name.md`.
  - Bao gồm 9 phần: Reasoning, Roads Not Taken, How pieces connect, Tools, Tradeoffs, Mistakes, Future Pitfalls, Expert vs Beginner, Transferable Lessons.

## 🔄 Vòng đời tiêu chuẩn của một Task
1. Bắt đầu với **`/plan`** để thiết kế.
2. Duyệt kế hoạch, gọi **`/execute`** để code.
3. Nếu gặp lỗi, chạy **`/debug`**.
4. Xong logic, gọi **`/review`** để tối ưu và dọn dẹp.
5. Kiểm duyệt xong, chạy **`/finish`** để lưu kinh nghiệm.
6. Kết thúc với **`/commit`** để lưu vào Git.
*(Nếu task rất đặc biệt và cần truyền đạt kinh nghiệm sâu sắc, chạy thêm **`/teach`**)*
