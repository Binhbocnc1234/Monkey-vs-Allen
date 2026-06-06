---
type: gameplay
module: progression

audience: [player]
status: active
language: vi
description: Defines Vietnamese gameplay documentation for upgrades, progression, card improvement, and game-mode relationships.
related:
  - card-system
  - game-modes
---

# Tổng quan

Trong Monkey vs Alien, sức mạnh của bạn không chỉ đến từ kỹ năng điều binh khiển tướng, mà còn đến từ việc **phát triển bộ sưu tập lá bài** của mình. Bạn càng chơi, bạn càng thu thập được nhiều lá bài — và những lá bài bạn đã có có thể được cường hóa qua nhiều cấp độ.

Hệ thống nâng cấp xoay quanh một loại tài nguyên duy nhất: **Mảnh (Shard)**.

# Sở hữu lá bài

Mỗi lá bài trong game đều có những mảnh ghép riêng. Khi bạn sở hữu đủ số lượng mảnh yêu cầu, bạn có thể quy đổi chúng để sở hữu **vĩnh viễn** lá bài đó. Sau khi sở hữu, lá bài sẽ nằm trong bộ sưu tập của bạn và có thể được sử dụng trong trận đấu.

Bạn cũng có thể sở hữu lá bài thông qua những cách khác:
- Nhận lá bài Basic Monkey ngay từ level đầu tiên.
- Chiến thắng trận đấu để nhận thưởng là lá bài mới.
- Đánh bại Alien trong Campaign để nhận lá bài có khả năng khắc chế Alien đó.

# Thu thập mảnh (Shard)

Sau khi sở hữu một lá bài, bạn vẫn tiếp tục thu thập mảnh của nó. Mỗi lá bài có một cấp độ hiếm (Rarity) nhất định, và cấp độ hiếm này **ảnh hưởng đến tỉ lệ tìm thấy mảnh của lá bài đó** — lá bài càng hiếm, mảnh của nó càng khó xuất hiện.

Mảnh có thể nhận được từ:
- Hoàn thành các màn chơi trong Campaign.
- Phần thưởng từ Weekly Event.
- Các sự kiện đặc biệt.

# Nâng cấp lá bài

Khi bạn đã sở hữu một lá bài và có đủ mảnh, bạn có thể nâng cấp nó. Mỗi lá bài có thể nâng cấp tối đa **5 lần**, mỗi lần tốn một lượng mảnh nhất định.

## Các loại nâng cấp

Mỗi lần nâng cấp, lá bài nhận được một trong ba loại cải thiện sau:

1. **Nâng cấp chỉ số** — Các chỉ số như Máu tối đa (MaxHealth), Giáp (Armor), Kháng phép (MagicResistance), Tốc chạy (MoveSpeed), Tốc đánh (AttackSpeed), Sát thương (Strength), v.v. được tăng lên một lượng cố định.
2. **Kỹ năng mới** — Lá bài mở khóa một kỹ năng hoặc nội tại (Passive) hoàn toàn mới, mang đến chiều sâu chiến thuật mới.
3. **Nâng cấp kỹ năng** — Một kỹ năng hoặc nội tại đã có được tăng cường — thời gian hiệu lực dài hơn, sát thương cao hơn, hoặc thêm hiệu ứng phụ.

## Trình tự nâng cấp thông thường

Đa số các lá bài tuân theo trình tự sau:

| Lần nâng cấp | Nội dung | Chi phí (mảnh) |
|:---:|:---|---:|
| 1 | Nâng cấp chỉ số | 10 |
| 2 | Kỹ năng mới | 15 |
| 3 | Nâng cấp chỉ số | 20 |
| 4 | Nâng cấp kỹ năng | 25 |
| 5 | Nâng cấp tạm thời | 20 |

Lần nâng cấp thứ 5 có cơ chế đặc biệt: hiệu ứng của nó chỉ có hiệu lực **trong một khoảng thời gian nhất định**, không phải vĩnh viễn như 4 lần đầu.

## Ví dụ

**Basic Monkey — Trình tự nâng cấp:**

| Lần | Nội dung |
|:---:|---|
| 1 | Sát thương (Strength) tăng thêm 5 |
| 2 | Kỹ năng mới — **Bầy đàn**: Với mỗi Basic Monkey trên chiến trường, lượng giáp của Monkey được tăng thêm 8% |
| 3 | Máu tối đa (MaxHealth) tăng thêm 40 |
| 4 | Kỹ năng **Bầy đàn**: lượng giáp tăng thêm 4% |
| 5 | Kỹ năng tạm thời — **Quân Elite**: Quân Elite xuất hiện với tỉ lệ 10% |

**Basic Alien — Trình tự nâng cấp:**

| Lần | Nội dung |
|:---:|---|
| 1 | Sát thương (Strength) tăng thêm 5 |
| 2 | Kỹ năng mới — **Tân tiến**: 15% cơ hội Basic Alien sẽ đội Cranium Helmet |
| 3 | Máu tối đa (MaxHealth) tăng thêm 40 |
| 4 | Kỹ năng **Tân tiến**: tỉ lệ đội Cranium Helmet tăng thêm 8% |
| 5 | Kỹ năng tạm thời — **Quân Elite**: Quân Elite xuất hiện với tỉ lệ 10% |

# Liên kết với các hệ thống khác

- **Card System** — Mỗi lá bài đều có thể được nâng cấp, và các chỉ số của lá bài trong trận phụ thuộc vào cấp nâng cấp hiện tại.
- **Economy System** — Chi phí tài nguyên trong trận không đổi qua các cấp, nhưng sức mạnh của Entity tăng lên nhờ nâng cấp.
- **Game Modes** — Các chế độ chơi khác nhau cung cấp lượng mảnh và cơ hội nhận thẻ khác nhau.