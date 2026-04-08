1. THÔNG TIN CHUNG (CONCEPT)

Thể loại: Puzzle / Arcade (Top-down Slide Maze).

Góc nhìn: 2D từ trên xuống (Top-down).

Môi trường: Grid-based Tilemap (Ma trận lưới).

Mục tiêu (Core Loop): Tìm đường trong mê cung. Người chơi vuốt để nhân vật trượt đi, thu thập ngôi sao/kẹo và tìm đường đến cửa đích mà không đâm vào bẫy.

2. HỆ THỐNG TILEMAP (LEVEL ARCHITECTURE)
Hệ thống sử dụng một mảng 2D (2D Array) để dựng Tilemap, gồm các loại Tile sau:

[1] Wall (Tường/Đá): Vật cản cứng. Nhân vật không thể đi qua.

[0] Path (Đường đi/Nước): Không gian trống để nhân vật trượt qua.

[2] Collectible (Vật phẩm): Kẹo, Xu, Ngôi sao, hoặc Lá. Nằm trên đường đi.

[3] Hazard (Bẫy gai/Kẻ thù): Các chông gai tĩnh hoặc con ong tuần tra.

[4] Exit (Cửa đích): Điểm đến cuối cùng.

3. CƠ CHẾ VẬT LÝ & ĐIỀU KHIỂN (PHYSICS & CONTROLS)

Trạng thái cơ bản: Không có trọng lực. Nhân vật đứng yên tại vị trí một ô lưới (Grid) cho đến khi nhận được lệnh điều khiển.

Input (Điều khiển): Người chơi Vuốt (Swipe) trên màn hình theo 4 hướng: Lên, Xuống, Trái, Phải.

Hành động (Slide Mechanic): * Khi có lệnh vuốt, nhân vật sẽ trượt thẳng theo hướng đó với tốc độ cao (velocity cao).

Ràng buộc cốt lõi: Nhân vật chỉ dừng lại khi tông vào một Tile Tường (Wall). Khi đang trượt, người chơi KHÔNG thể đổi hướng giữa chừng.

Hiệu ứng (Visuals): Khi nhân vật trượt, tạo ra một vệt cầu vồng (Trail) đuổi theo phía sau.

4. HỆ THỐNG VA CHẠM (COLLISIONS & EVENTS)
Sử dụng hệ thống va chạm Tilemap của Phaser (Arcade Physics):

Player vs Wall: Ngắt vận tốc (Velocity = 0), căn chỉnh (snap) nhân vật vừa khít vào ô lưới sát tường, sẵn sàng cho thao tác vuốt tiếp theo.

Player vs Collectible (Overlap): Hủy (Destroy) vật phẩm, phát hiệu ứng hạt (Particles/Stars) nhấp nháy nhỏ, cộng điểm tiến trình màn chơi.

Player vs Hazard (Overlap): Kích hoạt trạng thái Thua (Game Over) - Nhân vật nổ tung hoặc dừng lại khóc.

Player vs Exit (Overlap): Kích hoạt trạng thái Thắng (Level Clear).

5. KỊCH BẢN QUẢNG CÁO (AD FLOW & LIFECYCLE)

Giai đoạn 1: Tutorial (0-3s): * Màn hình có một mê cung ngắn hình chữ L.

Hiển thị bàn tay (Hand Cursor) nhấp nháy vuốt sang phải, rồi vuốt xuống để hướng dẫn.

Game chờ cho đến khi người dùng vuốt lần đầu.

Giai đoạn 2: Gameplay (3-15s): * Người chơi phải tự tìm đường thu thập đủ 3 vật phẩm quan trọng nằm ở các ngã rẽ, yêu cầu phải vuốt đập tường nhiều lần để bẻ lái.

Cố tình đặt một bẫy gai ở một góc dễ bị vuốt nhầm để tăng tính thử thách.

Giai đoạn 3: Endcard (Kết thúc):

Điều kiện Thắng: Nhân vật đến được ô Exit.

Điều kiện Thua: Tông vào Hazard.

Hành động chung: Hiện popup "STAGE CLEAR" (nếu thắng) hoặc "FAILED" (nếu thua). Nút Call-To-Action (CTA) sáng lên: "PLAY FULL GAME" gắn với hàm ExitApi() của mạng quảng cáo.