# game-audio-prompter

> **Mô tả:** An AI prompt engineering skill that reads game design documents and audio asset lists to automatically construct specialized BGM and SFX prompts for external Audio LLMs.

## 🎯 Mục đích sử dụng
Đóng vai trò là **Game Audio Designer Agent**. Skill này lấy các mô tả kỹ thuật (Technical mechanics) và phong cách nghệ thuật (Visual Art Styles) từ GDD (Game Design Document) để chuyển đổi/dịch thành các "Audio Prompts" chuyên nghiệp, có thể copy/paste dùng cho các AI tạo âm thanh như Suno, Udio, ElevenLabs, hoặc ChatGPT Advanced Voice.

## ⚙️ Kích hoạt
Tự động hoặc thủ công khi người dùng yêu cầu làm âm thanh cho dự án (invoke `game-audio-prompter`).

## 📥 Đầu vào (Input)
- Bắt buộc quét thư mục dự án và đọc 2 file:
  1. `*_GDD.md`: Để lấy Game Mechanics, Visual Art Style, Overall Vibe.
  2. `*_Audio_Assets.json`: Lấy danh sách ID tài nguyên âm thanh cần tạo, loại layer (BGM/SFX) và cờ loop.

## 📤 Đầu ra (Output)
Tạo ra một Markdown file có tên `<ProjectName>_Audio_Prompt_Book.md` chứa toàn bộ prompt đã được định dạng và chia thành BGM và SFX theo cấu trúc chuẩn.

## ⚠️ Lưu ý quan trọng
- **Quy tắc dịch thuật (Audio Translation Logic):** Không được bê nguyên mô tả hình ảnh sang âm thanh. Cần phải dịch thành *Audio Envelopes/Textures* (vd: Hình ảnh "Line-art" -> Âm thanh "Crisp, zero reverb, bright high-end").
- **Fatigue Rule:** Nếu là âm thanh xuất hiện lặp lại liên tục (ví dụ: tiếng súng bắn), phải thêm constraint: "Fast attack, instant decay, strictly non-fatiguing to the ear".
- **Loops & Duration:** Quản lý chặt chẽ giới hạn thời lượng (vd: SFX max 0.5s) và thêm ràng buộc "seamless looping track" nếu `loop_flag` = true.
- Cuối cùng, kết thúc với workflow `/finish` để ghi nhận các kỹ thuật viết prompt hiệu quả.
