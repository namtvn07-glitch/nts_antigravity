# Platform Bridge SDKs

Mạng quảng cáo (Ad Network) khác nhau sẽ có những cú pháp (SDK) khác nhau để báo cáo "Click Tải Game" (CTA - Call-to-action) hoặc báo đếm thời gian chơi game. 

Với tư cách AI Agent làm Playable Ads, bạn phải hỏi User xem họ muốn Build Ads này cho nền tảng nào. Dưới đây là các đoạn code cần chèn vào hàm `openStore()` và cấu trúc `head` tương ứng.

## 1. AppLovin / IronSource (MRAID)
MRAID là chuẩn công nghiệp 3D chung.

**Thêm thư viện SDK vào `head`:**
```html
<script src="mraid.js"></script>
```

**Cập nhật hàm `openStore()`:**
```javascript
function openStore() {
    if (typeof mraid !== 'undefined') {
        mraid.open("https://play.google.com/store/apps/details?id=com.your.bundle");
    } else {
        window.open("https://play.google.com/store/apps/details?id=com.your.bundle", "_blank");
    }
}
```

## 2. Meta (Facebook Playable)
Facebook cấm mở đường link URL cứng, thay vào đó sử dụng biến hệ thống của họ để mở Store.

**Cập nhật cấu hình Head:** (Tuân thủ FB, bắt buộc thêm FbPlayableAd)
```html
<script src="https://connect.facebook.net/en_US/fbplayablead.js"></script>
```

**Cập nhật hàm `openStore()`:**
```javascript
function openStore() {
    if (typeof FbPlayableAd !== 'undefined') {
        FbPlayableAd.onCTAClick();
    } else {
        console.log("FbPlayableAd click call");
    }
}
```

## 3. Unity Ads (Bypass)
Unity Playable Ads yêu cầu gọi hàm MRAID giống AppLovin, tuy nhiên đôi khi hãng khuyến nghị dùng `mraid.open` với redirect URL chung chung và theo dõi qua tham số trong URL. Có thể dùng cấu trúc của AppLovin cho Unity Ads. Cần giới hạn Size file < 5MB và 1 Tệp index duy nhất.

## 4. Google Ads (UAC Playable)
UAC của Google chặn các tag `<script src="..." ` bên ngoài (Ngoại trừ Google Fonts hay một số loại CND sạch, tuy nhiên khuyên dùng là tất cả cho file Local).

**Google Ads BẮT BUỘC bỏ `<script src="mraid.js"></script>`**.
**Hàm `openStore()` thay đổi:**
```javascript
function openStore() {
    // Google Ads sẽ tự chặn window.open và redirect
    window.open("https://play.google.com/store/apps/details?id=com.your.bundle", "_blank");
}
```

---

## Lời khuyên cho Agent:
Nếu user không xác định mạng quảng cáo, hãy sử dụng **cấu trúc mặc định** (Bypass MRAID):
```javascript
function openStore() {
    var isMraid = typeof mraid !== "undefined";
    if (isMraid) {
        mraid.open("https://play.google.com/store/apps/details?id=com.your.bundle");
    } else {
        window.open("https://play.google.com/store/apps/details?id=com.your.bundle");
    }
}
```
Và KHÔNG thêm `<script src="mraid.js">` nếu không chắc chắn (Vì nếu để sai, Google Ads tự động Reject).
