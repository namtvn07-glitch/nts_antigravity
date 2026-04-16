# SpaceFlapper - Tài liệu Thiết kế Game (GDD v2.1: Hành động Xả Súng Tự Động)

## 1. Tổng quan
- **Vòng lặp cốt lõi**: Trải nghiệm kéo dài < 20 giây. Người chơi chạm để giữ tàu lơ lửng, đồng thời tàu tự động xả đạn (Auto-Fire). Máy bay phải phá hủy kẻ thù hoặc đục thủng tường thiên thạch để vượt qua. Game kết thúc chính xác sau 15 giây.
- **Điều khiển**: Chạm một tay (One Tap).
- **Chủ đề**: Khoa học viễn tưởng hoạt hình dễ thương (Cute Sci-fi Cartoon Action). Màu sắc rực rỡ, vui nhộn và liên tục cháy nổ cực xịn xò.

## 2. Cơ chế Hoạt động (Mechanics)
- **Vật lý bay**: Tàu bị chịu lực kéo của trọng lực rơi tự do xuống dưới. Một cú chạm (Tap) của người chơi sẽ cung cấp lực đẩy hất tàu nảy lên chéo góc một đoạn.
- **Hệ thống vũ khí (Auto-Fire)**: Tàu vũ trụ liên tục bắn ra các tia Laser theo phương ngang sang phải mỗi `300ms`. Tia laser sẽ bám sát đường đạn ngang và bào mòn máu của mục tiêu ngáng đường.
- **Hệ thống Chướng ngại vật**:
  - **Tường Thiên thạch (Asteroids)**: Xuất hiện từ ngoài mép màn hình bên phải và cuộn dần sang trái theo từng cặp (chóp trên/chóp dưới). Chúng rất trâu! Phân bổ `5 Máu` (HP = 5). Cần 5 viên Laser bắn liên tục để đục vỡ vụn thiên thạch này, tạo khoảng trống khổng lồ để bay qua. Nếu không phá vỡ được, người chơi phải tìm cách luồn lách qua khe rất hẹp ở giữa.
  - **Phi thuyền Đi tuần (Enemy Drones)**: Lơ lửng cản đường, thường đứng ngáng ngay giữa khe hở an toàn. Có độ cứng thấp hơn, chỉ `2 Máu` (HP = 2). Cần dính 2 viên đạn để phá hủy gỡ nguy hiểm.
- **Vật phẩm (Bonus Item)**: Các tinh thể năng lượng lấp lánh đôi khi lẩn quất bay trong không trung. Nhặt được để lấy điểm hoặc buff hiệu ứng.
- **Va chạm / Thất bại**: Chạm mép vực trên/dưới của màn hình, đâm vào Tường Thiên thạch (khi nó chưa bị vỡ), hoặc va phải Phi thuyền Địch thì màn chơi lập tức dừng lại (Game Over), hiển thị Bảng End Card quảng cáo.
- **Chiến thắng**: Sống sót và trụ vững thành công đủ thời lượng 15 giây thì Game cũng dừng và thưởng cho người chơi Bảng End Card Tải Game.

## 3. Hiệu ứng Hình ảnh và Âm Thanh (FX)
- **Nảy đạn Laser**: Khi laser ghim vào đá hay địch sẽ lóe mảng bụi lửa nhỏ (Tiny spark flash). Mục tiêu trúng đạn sẽ bị nhuộm nháy màu trắng sáng (Tint White) trong vùng sáng 50ms rùng mình.
- **Kẻ địch Bị Tiêu diệt**: Khi HP của Phi thuyền bị bắn về 0, nó bị xóa khỏi đường bay, hóa thành đám mây vụ nổ nhỏ (`explosion`), kéo theo màn hình rung lắc nhẹ (`screen shake: 50`) để tạo cảm giác lực.
- **Tường Thiên thạch Vỡ Nát**: Nếu thiên thạch chịu đủ 5 đạn và nổ tung, tạo ra một **siêu vụ nổ khổng lồ** (`scale tăng 250%`), kích hoạt rung lắc camera cực mạnh (`screen shake: 100`) nhằm làm phần thưởng thị giác mãn nhãn nhất cho người chơi vì đã phá được cấu trúc bản đồ siêu bền.

## 4. Màn hình Kết thúc / Kêu gọi Hành động (CTA End Card)
- **Điều kiện kích hoạt**: Khi đồng hồ đếm lùi tới 15s HOẶC lúc người chơi thua (game_over).
- **Hiển thị**: Logic physics của game bị tạm khóa. Một lớp overlay màn mờ tối mịt phủ diện rộng lên toàn cảnh gameplay cũ. Từ trung tâm, Nút bấm CTA rực rỡ có hiệu ứng phóng to thu nhỏ nhịp nhàng và dòng chữ "TẢI NGAY / CHƠI NGAY" sẽ chình ình hiện ra quyến rũ người dùng bấm vào tải cài đặt.

## 5. Danh sách Yêu cầu Nguồn Lực Tài Sản (Game Assets)
Mọi tài sản hình ảnh đều là dạng sprite nét 2D chuẩn, phông nền bắt buộc bị tách bỏ hoàn toàn thành lớp alpha channel trong suốt tuyệt đối (riêng cảnh thiên hà thì không cắt). Tất cả đã được chạy qua màng lọc trí tuệ nhân tạo nhận diện vật thể `rembg GPU` cắt góc cực mượt nhằm loại bỏ răng cưa và hạ kích thước nén xuống `.webp`:
- `bg.webp`: Cảnh vũ trụ hoạt hình tươi sáng, đầy bí ẩn.
- `player.webp`: Tàu không gian siêu cấp đáng yêu, mũm mĩm.
- `asteroid.webp`: Cột thiên thạch sặc sỡ chia khối gồ ghề cứng cáp.
- `item.webp`: Đá linh thạch tinh thể năng lượng không gian đa giác tỏa sáng glow rực rỡ.
- `cta_btn.webp`: Nút bấm thiết kế mang cảm hứng viễn tưởng UI, màu gradient lấp lánh như viên kẹo mời gọi "PLAY NOW".
- `laser.webp` [Mới]: Một vạch tia laze siêu tốc độ, màu neon kéo đuôi dọc.
- `enemy.webp` [Mới]: Cỗ máy robot cảnh vệ chiến đấu có mắt đỏ phát sáng ngắm bắn, mặt lơ lửng cau có nhưng vẫn buồn cười cute.
- `explosion.webp` [Mới]: Quả cầu hỗn hợp khí lửa bụi bặm vụ nổ văng tung tóe kiểu hoạt hình comic 2D.
