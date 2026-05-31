---
type: gameplay
module: effect
version: 1.0
audience: user
related: [entity-system, combat-system, card-system]
---

# Định nghĩa

Hiệu ứng là những trạng thái tạm thời tác động lên Entity trong trận đấu. Chúng làm thay đổi chỉ số, hành vi, hoặc khả năng của Entity theo một hướng nhất định — có thể là lợi (buff) hoặc hại (debuff).

Khác với **tập tính (Trait)** — những đặc tính vốn có của Entity và tồn tại suốt vòng đời — hiệu ứng chỉ tồn tại trong một khoảng thời gian và biến mất khi hết hiệu lực.

# Nguồn gốc

Hiệu ứng phát sinh từ các nguồn sau:
- **Kỹ năng của Entity khác** — Đây là nguồn phổ biến nhất. Một Entity có thể gây hiệu ứng cho đồng đội (buff) hoặc kẻ địch (debuff) thông qua đòn đánh hoặc kỹ năng.
- **Kỹ năng bị động (Passive)** — Một số Entity tự động kích hoạt hiệu ứng cho bản thân hoặc đồng đội khi đủ điều kiện.
- **Môi trường** — Trong một số màn chơi, chính chiến trường có thể gây ra hiệu ứng cho Entity.

Khi Entity sở hữu hiệu ứng chết, toàn bộ hiệu ứng trên nó cũng biến mất theo.

# Phân loại

## Theo tính chất

- **Buff** — Hiệu ứng có lợi: tăng chỉ số, hồi máu, miễn nhiễm, v.v.
- **Debuff** — Hiệu ứng có hại: giảm chỉ số, gây sát thương theo thời gian, làm tê liệt, v.v.

Việc phân biệt buff và debuff có ý nghĩa quan trọng vì một số Entity có kỹ năng **hóa giải debuff** hoặc **xóa bỏ** hiệu ứng trên đồng đội.

## Theo cơ chế

- **Hiệu ứng thời gian** — Tự động biến mất sau một khoảng thời gian nhất định. Đồng hồ đếm ngược bắt đầu từ khi hiệu ứng được áp dụng.
- **Hiệu ứng vĩnh viễn trong trận** — Tồn tại cho đến khi Entity chết hoặc bị gỡ bỏ bởi một kỹ năng khác.

## Đặc điểm tương tác

Một số hiệu ứng có thể **chồng lấn (stack)** — áp dụng nhiều lần sẽ làm mạnh thêm. Một số hiệu ứng khác thì không, và việc áp dụng lại chỉ làm **mới lại thời gian** (reset duration).

# Các loại hiệu ứng thường gặp

Dưới đây là những hiệu ứng phổ biến mà người chơi sẽ gặp trong trận:

| Hiệu ứng | Tác dụng |
|----------|----------|
| **Stun (Choáng)** | Làm tê liệt hoàn toàn Entity — không thể di chuyển, không thể tấn công. Kết thúc sau một thời gian nhất định. Ví dụ: Slimz có 15% tỉ lệ làm choáng kẻ địch 3 giây mỗi đòn Poke. |
| **Slow (Làm chậm)** | Giảm tốc độ di chuyển, thường kèm hiệu ứng hình ảnh màu xanh lam trên thân Entity. |
| **Freeze (Đóng băng)** | Đóng băng mọi hành động, Entity bị đứng yên hoàn toàn, cơ thể chuyển màu xanh biển. |
| **OnFire (Thiêu đốt)** | Gây 3 sát thương mỗi giây trong khoảng thời gian nhất định. Biểu hiện: ngọn lửa nhỏ trên thân. |
| **Poisoning (Độc)** | Gây 2 sát thương phép mỗi 3 giây. Biểu hiện: cơ thể nhấp nháy màu xanh. |
| **Hypnotized (Thôi miên)** | Đổi phe của Entity — nó sẽ tấn công lại đồng đội cũ. Biểu hiện: cơ thể màu tím, có bong bóng nổi lên. |
| **IronBody (Thể sắt)** | Giảm đáng kể sát thương nhận vào. |
| **Fury (Cuồng bạo)** | Tăng sát thương gây ra thêm một lượng nhất định. |
| **Excited (Phấn khích)** | Tăng tốc độ tấn công lên 70%. |
| **Momentum (Đà)** | Tăng tốc chạy lên 70%. |
| **BullEye (Mắt bò)** | Khắc chế các Entity có giáp — gây thêm sát thương hoặc xuyên giáp. |
| **Strikethrough (Xuyên thủng)** | Đòn đánh xuyên qua nhiều kẻ địch trên đường bay. |

# Liên kết với các hệ thống khác

- **Entity System** — Hiệu ứng tác động lên chỉ số và hành vi của Entity.
- **Combat System** — Nhiều hiệu ứng can thiệp vào quá trình gây và nhận sát thương: tăng sát thương đầu ra, giảm sát thương đầu vào, phản sát thương.
- **Card System** — Một số Entity sinh ra từ lá bài có sẵn khả năng gây hiệu ứng, và các lá bài khác nhau sở hữu bộ hiệu ứng riêng.
