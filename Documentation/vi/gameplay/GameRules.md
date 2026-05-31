---
type: gameplay
module: game-rules
version: 1.0
audience: user
related: [entity-system, card-system, grid-system, economy-system, game-modes]
---

# Tổng quan

Monkey vs Alien là một game đối kháng theo lane giữa hai đội: **Monkey** và **Alien**. Mỗi đội sử dụng những lá bài để triệu hồi Entity ra chiến trường. Các Entity tự động di chuyển và chiến đấu, đẩy lực lượng về phía căn cứ đối phương. Người chơi đóng vai trò chỉ huy, lựa chọn thời điểm và vị trí tung ra từng lá bài để tạo lợi thế.

# Mục tiêu và điều kiện thắng thua

Phe nào **tiêu diệt được 3 Target của đối phương trước** sẽ giành chiến thắng. Target là những Entity đặc biệt nằm sâu trong căn cứ mỗi bên, và đây cũng là điều kiện thua duy nhất — nếu bạn mất 3 Target, bạn thua trận.

Tuyến phòng thủ cuối cùng của bạn chính là những Target này, và toàn bộ chiến thuật trong trận đấu đều xoay quanh việc bảo vệ Target của mình trong khi tìm cách đánh sập Target của đối phương.

# Chiến trường

Trận đấu diễn ra trên một chiến trường được chia thành nhiều **lane độc lập** chạy dọc theo chiều ngang màn hình. Số lượng lane tối đa là 5, tùy vào cấp độ của mỗi màn chơi.

Mỗi lane có độ dài từ **9 đến 15 ô (cell)**, và các Entity chỉ có thể tương tác với nhau nếu chúng ở cùng một lane. Nếu hai Entity cùng lane, đồng đội sẽ hỗ trợ nhau và kẻ thù sẽ tấn công nhau.

Khi một lá bài được sử dụng, Entity sẽ xuất hiện ở đầu sân của đội mình trên lane đã chọn — phe Left (Monkey) sinh ra từ bên trái, phe Right (Alien) sinh ra từ bên phải — và tự động tiến về phía đối phương.

# Entity

Entity là đơn vị cơ bản trên chiến trường. Mỗi Entity sở hữu các chỉ số sau:

| Chỉ số | Ý nghĩa |
|--------|---------|
| Lane | Xác định Entity đang thuộc lane nào |
| Health | Máu. Nếu về 0, Entity chết |
| Move Speed | Tốc độ di chuyển |
| Range | Tầm đánh |
| Strength | Sát thương gây ra mỗi đòn đánh |
| Attack Speed | Tốc độ tấn công (số đòn mỗi giây) |

Ngoài ra, một số Entity còn sở hữu những kỹ năng đặc biệt — có thể hồi máu đồng đội, cường hóa bản thân, gây sát thương diện rộng, hoặc làm choáng kẻ địch.

> **Ví dụ:** Basic Monkey là một Entity cận chiến đơn giản với các chỉ số ở mức trung bình và chưa có kỹ năng đặc biệt ngay từ đầu. Trong khi đó, Slimz — một Entity thuộc lớp Sneaky — có thể tung ra một đòn Poke với 15% tỉ lệ làm choáng kẻ địch trong 3 giây.

# Tài nguyên

Để sử dụng lá bài, người chơi cần có tài nguyên. Hai phe có cách vận hành tài nguyên khác nhau:

- **Phe Monkey**: Sở hữu lá bài **Banana Tree** (thuộc loại Tower Entity). Lá bài này tự động sản xuất chuối sau mỗi khoảng thời gian nhất định, tương tự cơ chế Sunflower trong Plants vs. Zombies.

- **Phe Alien**: Nhận "tiếp viện" từ bên ngoài nên có nguồn tài nguyên ổn định, không cần trồng cây. Tuy tốc độ ban đầu thấp, Alien có thể nâng cấp để tăng tốc độ sản sinh thêm một bậc, giống cơ chế trong Tower Conquest.

# Lá bài

Mỗi lá bài có hai ràng buộc: **chi phí tài nguyên (cost)** và **thời gian hồi (cooldown)**. Người chơi chỉ có thể dùng lá khi có đủ tài nguyên và lá bài đã hết thời gian hồi.

Quyết định dùng bài vì thế phụ thuộc vào cả lượng tài nguyên hiện tại lẫn thời điểm có thể dùng được lá tiếp theo. Nếu bạn tiêu hết tài nguyên vào một lá bài đắt tiền, bạn sẽ phải đợi để có thể dùng lá tiếp theo.

Lá bài được chia làm nhiều loại, bao gồm Combat Card (triệu hồi Entity chiến đấu) và Tower Card (triệu hồi một Builder đến xây tháp tại vị trí đã chọn).
