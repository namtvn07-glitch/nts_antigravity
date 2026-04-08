1. THÔNG TIN CHUNG (CONCEPT)

Thể loại: Arcade / Endless Runner (Biến thể Flappy).

Thời lượng quảng cáo: 15 - 20 giây.

Mục tiêu (Core Loop): Điều khiển nhân vật bay qua các chướng ngại vật (Ống/Cột) hoặc bắn vỡ các khối cản đường để sinh tồn và đi đến đích.

2. CƠ CHẾ VẬT LÝ & ĐIỀU KHIỂN (PHYSICS & CONTROLS)

Trọng lực (Gravity): Nhân vật liên tục bị kéo xuống dưới cùng màn hình.

Chuyển động (Movement): * Nhân vật cố định ở trục X (bên trái màn hình).

Bối cảnh (Background) và Chướng ngại vật di chuyển từ phải sang trái để tạo cảm giác tiến lên.

Tương tác (Input):

Hành động 1 (Nhảy): Người chơi Tap (chạm) vào bất kỳ đâu trên màn hình, nhân vật sẽ nhận một lực đẩy (Velocity Y âm) để bay lên.

Hành động 2 (Bắn súng): Nhân vật Tự động bắn (Auto-fire) một viên đạn mỗi 0.5 giây về phía trước (trục X dương). (Ghi chú Kiến trúc sư: Trong Playable Ad, auto-fire tốt hơn việc bắt user tap 2 nút khác nhau, giúp giảm độ khó và tránh người chơi bấm nhầm thoát game sớm).

3. HỆ THỐNG VA CHẠM (COLLISIONS & HITBOX)
Có 3 nhóm đối tượng chính cần AI xử lý va chạm (Overlap/Collide):

Nhân vật (Player) vs Chướng ngại vật (Pipes/Ground): Nếu chạm vào nhau -> Kích hoạt trạng thái Thua (Game Over).

Đạn (Bullet) vs Chướng ngại vật có thể phá hủy (Breakable Blocks):

Mỗi khối cản đường có một lượng "Máu" (HP = 1 hoặc 2).

Khi đạn chạm khối: Hủy viên đạn + Trừ 1 HP của khối + Hiển thị hiệu ứng chớp trắng (Tween flash).

Khi HP khối = 0: Hủy khối (Mở đường cho Player) + Phát hiệu ứng nổ vụn (Particles).

Đạn (Bullet) vs Giới hạn màn hình: Nếu đạn bay ra khỏi màn hình bên phải -> Tự động hủy đạn để giải phóng bộ nhớ (Destroy).

4. KỊCH BẢN QUẢNG CÁO (AD FLOW & LIFECYCLE)

Giai đoạn 1: Tutorial (0-3s): Màn hình tối đi một chút. Hiện bàn tay (Hand pointer) nhấp nháy hướng dẫn "Tap to Fly & Destroy!". Game tạm dừng (Paused) cho đến khi người chơi Tap lần đầu tiên.

Giai đoạn 2: Gameplay (3-15s): * Vượt qua cụm cột thứ 1 (Chỉ bay, không cần bắn).

Cụm cột thứ 2 bị bịt kín bởi một "Khối gạch". Người chơi phải giữ nhân vật bay ngang tầm khối gạch để đạn bắn vỡ nó rồi chui qua.

Giai đoạn 3: Endcard (Kết thúc):

Điều kiện Thắng: Vượt qua cụm cột thứ 2.

Điều kiện Thua: Chạm đất, chạm cột, hoặc chạm khối gạch chưa bị phá.

Hành động: Dừng mọi chuyển động. Hiện Popup chúc mừng/Tiếc nuối với nút CTA to, rõ ràng: "PLAY FULL GAME". Nút này liên kết với hàm ExitApi của nền tảng quảng cáo (Do Skill 4 xử lý).