---
type: gameplay

audience: [player]
status: active
language: vi
description: Defines Vietnamese gameplay documentation for card types, ownership, battle usage, upgrades, and related systems.
related:
  - game-rules
  - entity-system
  - upgrade-system
---

# Lá bài là gì?

Mỗi lá bài đại diện cho một Entity mà bạn có thể triệu hồi ra chiến trường. Nếu Entity là những chiến binh trên mặt trận, thì lá bài chính là "lệnh điều động" — bạn cần có lá bài trong tay và đủ tài nguyên để sử dụng nó.

Mỗi lá bài chứa toàn bộ thông tin về Entity mà nó triệu hồi, bao gồm chỉ số, kỹ năng, và hình dáng.

# Các loại lá bài

Hiện tại có hai loại lá bài chính:

## Combat Card (Lá bài chiến đấu)
Đây là loại lá bài phổ biến nhất. Khi sử dụng, nó triệu hồi một Entity chiến đấu xuống đầu lane của đội bạn. Entity sẽ ngay lập tức di chuyển và tham gia giao tranh.

- Dành cho phe Monkey
- Dành cho phe Alien [Unfinished]

## Tower Card (Lá bài tháp)
Không giống Combat Card triệu hồi Entity ở đầu lane, Tower Card triệu hồi một **Builder** — một Entity đặc biệt chuyên đi xây dựng. Builder di chuyển đến vị trí đã chọn trên lane và bắt đầu thi công. Sau một khoảng thời gian xây dựng nhất định, Tower Entity sẽ hoàn thành và bắt đầu hoạt động.

Tower Card chỉ có thể đặt ở những ô **không nằm ở biên** (không phải ô đầu hoặc cuối lane) và ô đó **không được có tháp nào khác**.

> **Ví dụ:** **Banana Tree** — một Tower Card của phe Monkey. Khi bạn đặt nó xuống, một Builder xuất hiện, di chuyển đến vị trí đã chọn, xây dựng trong vài giây, và sau đó cây chuối bắt đầu sản xuất tài nguyên.

# Cách sở hữu lá bài

Có nhiều cách để bạn sở hữu một lá bài:

- **Ngay từ đầu** — Ngay từ level đầu tiên, bạn đã được cung cấp lá bài Basic Monkey.
- **Chiến thắng trận đấu** — Hoàn thành các màn chơi để nhận thêm lá bài mới.
- **Thu thập mảnh (shard)** — Mỗi lá bài có những mảnh ghép tương ứng. Khi bạn sở hữu đủ số lượng mảnh yêu cầu, bạn có thể quy đổi thành quyền sở hữu **vĩnh viễn** lá bài đó.

Sau khi sở hữu, bạn vẫn có thể tiếp tục thu thập thêm mảnh của lá bài đó để phục vụ cho việc nâng cấp.

## Độ hiếm (Rarity)

Lá bài có nhiều cấp độ hiếm khác nhau: **Common, Occasional, Rare, Epic, Exotic, Legendary**. Độ hiếm **không** ảnh hưởng đến sức mạnh của lá bài hay số lượng mảnh cần để nâng cấp — thay vào đó, nó ảnh hưởng đến **tỉ lệ tìm thấy mảnh (drop chance)**. Lá bài càng hiếm, mảnh của nó càng khó xuất hiện.

# Cách sử dụng lá bài trong trận đấu

Trong một trận đấu, mỗi lá bài tồn tại dưới dạng **Battle Card** — bản sao hoạt động của lá bài với hai ràng buộc:

1. **Chi phí (Cost)** — Lượng tài nguyên bạn phải trả để sử dụng lá bài. Lá càng mạnh, cost càng cao.
2. **Thời gian hồi (Cooldown)** — Sau khi dùng, bạn phải đợi một khoảng thời gian trước khi có thể dùng lại lá đó.

Quy trình sử dụng lá bài trong trận:

1. **Chọn lá bài** — Bạn chọn một lá bài trong tay.
2. **Kiểm tra điều kiện** — Game kiểm tra ba điều:
   - Bạn có đủ tài nguyên không? (Nếu không → "Insuffient Resource")
   - Lá bài đã hết cooldown chưa? (Nếu chưa → "Recovering")
   - Vị trí đặt có hợp lệ không? (Đặc biệt với Tower Card)
3. **Chọn lane** — Bạn chọn lane để triệu hồi Entity.
4. **Triệu hồi** — Entity xuất hiện, tài nguyên bị trừ, và cooldown bắt đầu đếm ngược.

Ngoài ra, các lá bài Enemy Card có một cơ chế đặc biệt: **max stack**. Thay vì chỉ có thể dùng một lần mỗi cooldown, chúng có thể tích lũy nhiều lượt dùng (stack) theo thời gian, cho phép Alien spam nhiều Entity cùng loại liên tiếp.

# Nâng cấp lá bài

Mỗi lá bài có thể nâng cấp tối đa **5 lần**, với mỗi lần mang đến một cải thiện khác nhau:

1. **Nâng cấp chỉ số** — Tăng một chỉ số cụ thể (MaxHealth, Armor, MagicResistance, MoveSpeed, AttackSpeed, v.v.)
2. **Kỹ năng mới** — Mở khóa một kỹ năng hoặc một Passive mới
3. **Nâng cấp chỉ số** (lần nữa)
4. **Nâng cấp kỹ năng** — Cường hóa kỹ năng hoặc Passive đã có
5. **Nâng cấp tạm thời** — Lần nâng cấp thứ 5 có hiệu lực trong một khoảng thời gian nhất định

Trình tự này có thể thay đổi tùy theo từng lá bài, nhưng đa số các lá bài đều tuân theo mô hình trên.

## Ví dụ: Basic Monkey

| Cấp nâng cấp | Nội dung | Chi phí (mảnh) |
|:---:|---|:---:|
| 1 | **Nâng cấp chỉ số**: Strength tăng thêm 5 | 10 |
| 2 | **Kỹ năng mới**: Bầy đàn — Với mỗi Basic Monkey trên chiến trường, lượng giáp của Monkey được tăng thêm 8% | 15 |
| 3 | **Nâng cấp chỉ số**: MaxHealth tăng thêm 40 | 20 |
| 4 | **Nâng cấp kỹ năng**: [ArmorIncreasedAmount] tăng thêm 4% | 25 |
| 5 | **Nâng cấp tạm thời**: Quân Elite xuất hiện với tỉ lệ 10% | 20 |

## Ví dụ: Basic Alien

| Cấp nâng cấp | Nội dung | Chi phí (mảnh) |
|:---:|---|:---:|
| 1 | **Nâng cấp chỉ số**: Strength tăng thêm 5 | 10 |
| 2 | **Kỹ năng mới**: Tân tiến — 15% cơ hội Basic Alien sẽ đội Cranium Helmet | 15 |
| 3 | **Nâng cấp chỉ số**: MaxHealth tăng thêm 40 | 20 |
| 4 | **Nâng cấp kỹ năng**: [Chance] tăng thêm 8% | 25 |
| 5 | **Nâng cấp tạm thời**: Quân Elite xuất hiện với tỉ lệ 10% | 20 |

# Quan hệ với các hệ thống khác

- **Entity System** — Mỗi lá bài quyết định Entity nào xuất hiện khi dùng bài.
- **Economy System** — Cost của lá bài tiêu hao tài nguyên của đội bạn.
- **Grid System** — Tower Card kiểm tra vị trí đặt trên Grid trước khi Builder bắt đầu xây.
- **Progression System** — Mảnh (shard) dùng để sở hữu và nâng cấp lá bài, gắn liền với tiến trình của người chơi.