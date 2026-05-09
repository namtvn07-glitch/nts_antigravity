# 🏛️ System Architecture & Technical Standards

Tài liệu này quy định các tiêu chuẩn kiến trúc cốt lõi và kỹ thuật vận hành cho toàn bộ hệ sinh thái **Agentic Skills & Workflows** trong **NTS Game Studio**. Tất cả các AI Agents, scripts, và workflows BẮT BUỘC phải tuân thủ nghiêm ngặt các nguyên tắc dưới đây.

## 1. 🌐 Kiến Trúc Model Context Protocol (MCP)
Hệ thống sử dụng MCP làm xương sống để giao tiếp giữa AI Model và môi trường Unity/Local File System.
*   **Context Isolation:** Mỗi tác vụ phải được chạy trong một workspace biệt lập hoặc thư mục được chỉ định. Agent không được can thiệp vào các tệp ngoài phạm vi dự án hiện tại.
*   **Tool-Calling First:** Bắt buộc ưu tiên sử dụng các MCP tools chuyên biệt (`mcp_unityMCP_*`, `view_file`, `write_to_file`, v.v.) thay vì tự viết các lệnh bash/powershell thủ công. Điều này đảm bảo tính ổn định, dễ tracking và an toàn (Safety).

## 2. 🧠 Kiến Trúc GraphRAG & Knowledge Base
Dự án vận hành hoàn toàn dựa trên kiến trúc GraphRAG (Knowledge Graph + Retrieval-Augmented Generation) để thiết lập không chỉ tính nhất quán mà còn là sự liên kết ngữ nghĩa đa chiều giữa các thực thể, loại bỏ triệt để hiện tượng "ảo giác" (Hallucination) do phụ thuộc vào trí nhớ mặc định của LLM.
*   **Mandatory Retrieval:** TRƯỚC KHI sinh ra bất kỳ dòng code, kịch bản (GDD), hay hình ảnh nào, Agent bắt buộc phải truy vấn tri thức từ mạng lưới Knowledge Graph trong `.agents/knowledge/` và `.agents/learned/`.
*   **Node & Edge Mapping:** Mọi dữ liệu đi vào hệ thống (Art reference, Tech rules) không chỉ được băm nhỏ thành vectors mà còn được trích xuất thành các thực thể (Nodes) và mối quan hệ (Edges). Ví dụ: `[Player] -(has_mechanic)-> [DoubleJump] -(requires_physics)-> [Rigidbody2D]`.
*   **Multi-hop Queries:** Hệ thống sử dụng Graph Retrieval kết hợp Semantic Search để có thể suy luận logic qua nhiều lớp (multi-hop). Điều này giúp Agent nhận thức rõ sự phụ thuộc: khi thay đổi một Rule về Ánh sáng toàn cục (Global Lighting), nó sẽ kéo theo các thay đổi liên đới đối với vật liệu (Materials) của các Entity khác trong Scene.
*   **Conflict Resolution Hierarchy:** Trong mạng Graph, các Node thuộc quy tắc cục bộ hoặc đặc tả (Local Style Rules / Specific Contexts) LUÔN mang trọng số ưu tiên cao hơn và ghi đè (override) các Node quy tắc toàn cục (Global Rules).

## 3. 🎯 Kỹ Thuật Few-Shot Prompting Chuyên Sâu
*   **Zero-Shot is Prohibited:** Nghiêm cấm việc Agent tự "sáng tác" (Zero-shot) đối với các tác vụ sinh Art Assets, Audio Prompts, hoặc Code Architecture.
*   **Visual Few-Shot (VLM):** Trong khâu sản xuất Game Art (qua `game-art-orchestrator`), hệ thống phải nạp trực tiếp các hình ảnh tham khảo (top-K retrieved images) vào Vision Language Model, đi kèm với các quy tắc Text DNA tương ứng, để đảm bảo kiểm soát chính xác style nghệ thuật.
*   **Code Few-Shot:** Trong khâu lập trình (`game-dev-unity`), Agent phải khảo sát và tìm kiếm các Design Pattern / Template có sẵn trong source code dự án làm ví dụ (Ví dụ: cách khai báo Singleton, Object Pooling của dự án) trước khi viết module mới.

## 4. 🛡️ Human-In-The-Loop (HITL) & Safety Boundaries
Hệ thống tích hợp cổng chặn HITL xuyên suốt để đảm bảo quyền kiểm soát tuyệt đối thuộc về người dùng (User).
*   **Approval Gates (Cổng kiểm duyệt):** Bất kỳ thao tác nào tiêu tốn tài nguyên nặng (như chạy VLM Render Art) hoặc có nguy cơ phá vỡ hệ thống (Ghi đè/xóa Script C# lõi) ĐỀU PHẢI cung cấp Implementation Plan, Sketch, hoặc Blueprint rõ ràng. Agent phải DỪNG LẠI và chờ User duyệt (`Approve`) trước khi thực thi.
*   **Verification Before Completion:** Không được phép tự tuyên bố "hoàn thành" nhiệm vụ lập trình nếu chưa thực hiện các bước kiểm thử (Compile check qua MCP, hoặc tạo Walkthrough báo cáo). Bằng chứng (Evidence) luôn phải đi trước kết luận.
