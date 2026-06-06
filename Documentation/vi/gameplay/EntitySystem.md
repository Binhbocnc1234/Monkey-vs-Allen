---
type: gameplay
module: entity

audience: [player]
status: active
language: vi
description: Defines Vietnamese gameplay documentation for entities, stats, combat roles, cards, and effects.
related:
  - game-rules
  - card-system
  - effect
---

# Entity là gì?

Entity là những đơn vị xuất hiện trên chiến trường. Bất kỳ thứ gì hiện diện trong trận đấu — từ một chú khỉ Basic Monkey, một cây Banana Tree, cho đến Target căn cứ của bạn — đều là Entity.

Entity được triệu hồi thông qua lá bài. Khi bạn dùng một lá bài, Entity tương ứng sẽ xuất hiện trên chiến trường và bắt đầu hành động theo bản năng của nó.

# Vòng đời của Entity

Mọi Entity đều trải qua một vòng đời chung:

1. **Spawn** — Entity xuất hiện trên chiến trường, thường ở đầu lane của đội mình.
2. **Hành động** — Entity tự động di chuyển, tìm kẻ địch, tấn công, hoặc thực hiện kỹ năng tùy theo bản chất của nó.
3. **Chết** — Khi Health về 0, Entity chết và biến mất khỏi trận đấu.

Trong suốt vòng đời, Entity liên tục tương tác với môi trường xung quanh — đồng minh hỗ trợ lẫn nhau, kẻ địch gây sát thương, và các hiệu ứng (Effect) có thể tạm thời thay đổi chỉ số hoặc hành vi của chúng.

# Chỉ số cốt lõi

Mỗi Entity có một bộ chỉ số quyết định khả năng chiến đấu và di chuyển. Các chỉ số phổ biến bao gồm:

| Chỉ số | Ý nghĩa |
|--------|---------|
| Health | Máu hiện tại |
| MaxHealth | Máu tối đa |
| Strength | Sát thương gây ra mỗi đòn đánh |
| Range | Tầm đánh (tính bằng số ô) |
| AttackSpeed | Tốc độ tấn công (đòn/giây) |
| MoveSpeed | Tốc độ di chuyển (ô/giây) |
| Armor | Giáp vật lý, giảm sát thương vật lý nhận vào |
| MagicResistance | Kháng phép, giảm sát thương phép nhận vào |
| LifeSteal | Hút máu (phần trăm sát thương gây ra) |

Một số Entity còn có các chỉ số mở rộng khác như CriticalChance (tỉ lệ chí mạng), Penetration (xuyên giáp), MagicPower (sức mạnh phép thuật), và nhiều chỉ số đặc thù khác.

# Lớp (Class) và Bộ tộc (Tribe)

Mỗi Entity thuộc về một **lớp** (class) và có thể mang một hoặc nhiều **bộ tộc** (tribe).

Các lớp hiện có:
- **Hearty** — Chuyên về phòng thủ và chống chịu
- **Crazy** — Sát thương cao nhưng kém bền bỉ
- **Brainy** — Thiên về chiến thuật và hiệu ứng
- **Swarmy** — Đánh theo số lượng, từng đơn vị nhỏ nhưng nhiều
- **Sneaky** — Cơ động, bất ngờ, thường mang hiệu ứng đặc biệt

Bộ tộc là những nhóm nhỏ hơn, giúp tương tác với các hiệu ứng và kỹ năng khác. Ví dụ: Basic, Target, Pet, Mechanic, Gargantuar, Cosmic, Medieval, Plant, Fruit, v.v. Một Entity có thể thuộc nhiều bộ tộc cùng lúc — chẳng hạn một Entity vừa là Mechanic vừa là Medieval.

# Hành vi (Behaviour)

Entity không đứng yên — chúng luôn thực hiện một hành vi tại mỗi thời điểm. Hành vi quyết định Entity đang làm gì: di chuyển, tấn công, thi công, hay đứng yên.

Các kiểu hành vi di chuyển:
- **StraightMove** — Đi thẳng về phía đối phương
- **HoppingMove** — Nhảy cóc về phía trước
- **BehindAlliesMove** — Đi phía sau đồng đội, thích hợp cho Entity đánh xa

Các kiểu hành vi tấn công:
- **Melee** — Tấn công cận chiến, Entity tiếp cận và đánh kẻ địch gần nhất
- **Ranged** — Bắn đạn từ xa về phía kẻ địch
- **LobAttack** — Bắn đạn cong, có thể vượt qua chướng ngại
- **StrikeThrough** — Đòn đánh xuyên qua nhiều kẻ địch
- **AreaDamage** — Sát thương diện rộng trong một vùng

> **Ví dụ:** Basic Monkey sử dụng StraightMove để di chuyển và Melee để cận chiến. Slimz sử dụng Ranged — bắn đạn từ xa, và thỉnh thoảng tung ra một đòn Poke đặc biệt.

# Tương tác giữa các Entity

Entity trong cùng một lane và cùng đội sẽ hỗ trợ nhau — các Entity đánh xa đứng sau, Entity đánh gần lao lên phía trước. Entity khác đội sẽ tự động tìm và tấn công kẻ địch trong tầm.

Khi một Entity tấn công kẻ địch, quy trình sát thương diễn ra như sau:

1. Entity tấn công tạo ra một **đòn đánh** (có thể là đòn cận chiến hoặc đạn)
2. Đòn đánh chạm mục tiêu → tạo ra một **DamageContext** (ngữ cảnh sát thương) chứa thông tin về sát thương, loại sát thương, nguồn gây ra
3. Các hiệu ứng trên cả người gây và người nhận có thể can thiệp vào DamageContext — tăng, giảm, hoặc biến đổi sát thương
4. Damage cuối cùng được áp vào Health của mục tiêu

# Kỹ năng (Skill)

Nhiều Entity sở hữu kỹ năng, có thể là **chủ động** (active — Entity chủ động sử dụng) hoặc **bị động** (passive — tự động kích hoạt khi đủ điều kiện). Mỗi kỹ năng có bộ chỉ số riêng được định nghĩa trong ScriptableObject của nó.

> **Ví dụ:** Slimz có một kỹ năng bị động với 15% tỉ lệ mỗi đòn đánh sẽ trở thành đòn Poke. Khi Poke trúng mục tiêu, nó gây sát thương và áp dụng hiệu ứng **Stun** trong 3 giây (làm tê liệt hoàn toàn mục tiêu). Ở cấp độ 3, Poke còn gây thêm hiệu ứng **SlimzSting** — sát thương phụ theo thời gian.

# Quan hệ với các hệ thống khác

- **Card System** — Lá bài là "vé triệu hồi" Entity. Mỗi CardSO tham chiếu đến một EntitySO, quyết định loại Entity nào sẽ xuất hiện.
- **Effect System** — Các hiệu ứng có thể tác động lên chỉ số và hành vi của Entity.
- **Combat System** — Entity gây và nhận sát thương thông qua các đòn đánh, đạn, và kỹ năng.
- **Grid System** — Mỗi Entity chiếm một ô (cell) trên lưới và di chuyển qua các ô.