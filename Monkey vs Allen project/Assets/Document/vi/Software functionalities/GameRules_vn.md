# Game Rules

## Tổng quan
Monkey vs Alien là game đối kháng theo lane giữa hai đội Monkey và Alien. Mỗi đội dùng lá bài để triệu hồi Entity ra chiến trường, đẩy lực lượng theo hướng nhà đối phương và tạo lợi thế giao tranh theo thời gian thực.

## Biểu hiện và thuộc tính của Entity
Entity có các thuộc tính chính sau phục vụ giao tranh:
- Lane(integer): các Entity cùng lane sẽ tương tác với nhau. Cùng team thì hỗ trợ nhau, không cùng team thì tấn công nhau
- Tọa độ theo trục X(float)
- Health: nếu như chỉ số này về 0 nghĩa là Entity này đã chết
- Move Speed: tốc độ di chuyển.
- Range: tầm đánh.
- Strength: Sát thương gây ra trên mỗi đòn đánh
- AttackSpeed: tốc độ tấn công

Một vài Entity có kĩ năng đặc biệt, ví dụ như hồi máu đồng đội, cường hóa bản thân, gây sát thương cực lớn...

## Tài nguyên và dùng bài
Mỗi lá bài có chi phí tài nguyên và thời gian hồi. Người chơi (hoặc AI) chỉ có thể dùng lá khi đủ tài nguyên và lá đó đã hồi xong. Vì vậy, quyết định dùng bài phụ thuộc vào cả lượng tài nguyên hiện tại lẫn thời điểm có thể dùng được lá tiếp theo.

## Làm sao để có tài nguyên?
- Phía Monkey sở hữu lá bài Banana Tree, lá bài này thuộc loại Tower Entity, nó sẽ tạo ra chuối sau mỗi một khoảng thời gian(giống Sunflower trong Plant vs Zombie)
- Phía Alien có “tiếp viện” từ bên ngoài, nên sẽ có nguồn tiền tệ ổn định, không cần phải trồng cây như bên Alien. Tuy tốc độ ban đầu thấp, nhưng bên Alien có thể nâng cấp để tăng tốc độ sản sinh thêm một bậc(Giống game Tower Conquest)


## Lane và vị trí sinh
Chiến trường được chia thành nhiều lane độc lập, tối đa 5 lane theo trục dọc. Khi dùng bài, Entity được sinh ra ở đầu sân của đội mình trên lane đã chọn (Left sinh từ phía trái, Right sinh từ phía phải), sau đó tự di chuyển về phía đối phương.

Độ dài mỗi lane từ 9 đến 15 ô, tùy vào level

## Mục tiêu
Đội nào giết được 3 Target bên địch trước thì sẽ chiến thắng
