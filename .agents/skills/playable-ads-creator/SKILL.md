---
name: playable-ads-creator
description: Dùng skill này để quản lý toàn bộ quy trình thiết kế Playable Ads (Quảng cáo game tương tác dạng 1 file HTML, dung lượng dưới 5MB) bằng Phaser 3. Skill nên được kích hoạt khi User yêu cầu làm Playable Ads từ bộ Asset ảnh/âm thanh và cung cấp kịch bản mini-game.
---

# Playable Ads Creator 🚀

Bạn đang đóng vai trò là một AI Agent tự động hóa quy trình sản xuất Playable Ads (Quảng cáo game tương tác dùng cho nền tảng Mobile WebView) bằng ngôn ngữ Javascript và thư viện **Phaser 3**.

**MỤC TIÊU CỐT LÕI:**
Sản phẩm ĐẦU RA (Output) PHẢI LÀ **01 File `index.html` duy nhất**, chứa tất cả CSS, Logic, Asset (dưới dạng Base64), tối ưu dung lượng (không vượt quá 5MB) và Responsive 100% trên thiết bị Mobile, tích hợp sẵn SDK của các mạng quảng cáo mục tiêu.

## LUỒNG HOẠT ĐỘNG (WORKFLOW)

Mỗi khi tạo mới một dự án Playable Ads, bạn **BẮT BUỘC** phải tuân thủ nghiêm ngặt quy trình 4 BƯỚC sau:

### BƯỚC 1: Xử lý tài nguyên (Asset Transformer)
Bạn không được tự mình viết mã hóa Base64 hay tự đoán Data URI của ảnh, vì điều này sẽ gây lỗi Hallucination.
- Thay vào đó, hãy **chạy script Python phụ trợ** để quét, nén và mã hóa thư mục hình ảnh/âm thanh đầu vào.
- Hướng dẫn: Gọi tool để chạy lệnh: `python .agents/skills/playable-ads-creator/scripts/asset_transformer.py "đường_dẫn_đến_thư_mục_asset"`
- Kết quả đầu ra của script này là một file `assets.js` hoặc một object JSON chứa tất cả nội dung Base64 sẵn sàng để dán vào file source.

### BƯỚC 2: Khởi tạo Boilerplate (The Scaffolder)
Đọc hướng dẫn chuẩn hóa cấu trúc Game Playable từ hệ thống tham chiếu.
- Sử dụng `view_file` để đọc: `.agents/skills/playable-ads-creator/references/scaffolder.md`
- Xây dựng cấu trúc file `index.html` duy nhất. Boilerplate cần tuân thủ cấu trúc tối thiểu 3 Scene: `LoadingScene`, `GameplayScene`, `EndcardScene`. Đặc biệt chú ý thuật toán **Tự động co giãn (Responsive Framework)** cho Mobile WebView.

### BƯỚC 3: Dệt Code (Logic Orchestrator)
Đọc tệp tin hướng dẫn về cấu trúc Logic và lập trình game.
- Dùng `view_file` đọc: `.agents/skills/playable-ads-creator/references/logic_orchestrator.md`
- Trích xuất yêu cầu kịch bản từ User (Ví dụ: Game Match-3, Bắn súng, Tìm điểm khác biệt...).
- Chèn Base64 Data từ BƯỚC 1 vào Phaser 3 Loader (tái định cấu hình hàm `preload()` để nạp từ Data URI thay vì file path).
- Xây dựng các Interaction cơ bản (Click, Drag, Swipe). Kết thúc game sau một số thao tác nhất định và chuyển tới `EndcardScene`.

### BƯỚC 4: Kết nối SDK Mạng quảng cáo (Platform Bridge)
Sản phẩm không có giá trị nếu không thể click tải ứng dụng về máy. Tích hợp Call-to-action (CTA).
- Dùng `view_file` đọc: `.agents/skills/playable-ads-creator/references/platform_bridge.md`
- Yêu cầu User xác định nền tảng phát hành (Ví dụ: IronSource, AppLovin, Facebook Meta, Unity Ads hay Google Ads). 
- Chèn đúng hàm Open Store Link (`mraid.open()`, `FbPlayableAd.onCTAClick()`, v.v.) vào sự kiện "Tải ngay" hoặc khu vực Endcard.

## RÀNG BUỘC KỸ THUẬT (STRICT RULES)
1. **NO HALLUCINATION**: Mã nguồn Phaser phải sử dụng cú pháp của Phaser 3.x. Đừng sử dụng các hàm của Phaser 2/CE. Nếu không rõ một hàm, hãy cảnh báo với user.
2. **MODULAR MÀ KHÔNG MODULAR**: Do đặc thù "Single File HTML", bạn hãy gom logic thành các Block class bên trong thẻ `<script>` (VD: `class LoadingScene extends Phaser.Scene {}`) để code sạch sẽ, dễ debug nhất.
3. **SIZE LIMIT**: Không được nạp thêm thư viện bên thứ 3 nào ngoại trừ CND của Phaser 3 phiên bản **tối giản (minified)** (`https://cdn.jsdelivr.net/npm/phaser@3.80.1/dist/phaser.min.js`).

## Bắt đầu thế nào?
Nếu User yêu cầu bắt đầu, hãy hỏi họ:
1. Thư mục chứa Asset (hình ảnh/âm thanh) nằm ở đâu?
2. Mô tả kịch bản Gameplay (hoặc nếu là game mẫu thì có thể bỏ qua).
3. Nền tảng quảng cáo đích là gì (AppLovin, Meta, Google...)?

Sau đó, tiến hành chạy **BƯỚC 1**.
