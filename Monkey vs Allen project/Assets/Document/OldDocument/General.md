Thông tin về game Monkey vs Allen

Đây là tài liệu phục vụ cho các developer

API key: AIzaSyAmwiwud-b6p9ZZ7ROdnZO7081XFLU0OwM

# Tổng quan

Màn hình ngang

Gần giống plant and zombie nhưng sẽ có thêm vài cơ chế mới

Thay vì chiến đấu trên mặt đất, chúng ta sẽ chiến đấu trên không, vì vậy, sẽ không có sẵn các block đất để người chơi có thể đặt Tower(trong PvZ thì là plants)

Mục tiêu: phá nhà của địch và dành chiến thắng, để làm được điều đó, người chơi cần đặt các Block để mở rộng lãnh thỗ, đặt Tower, triệu hồi Monkey để chúng chiến đấu cho mình

Mục tiêu của địch: loại bỏ chướng ngại vật trên đường đi, cụ thể là các Block, Tower, Monkey, phá nhà của người chơi

Điểm khác so với PvZ:

\-Cơ chế Block:

+Khi mới vào ván đấu, người chơi chỉ được cung cấp 2 hàng block ngay trước nhà, chứ không phải 9 hàng như PvZ

\-Chiều dài sân đấu: có thể lên tới 18 hàng chứ không phải 9 hàng như PvZ

\-Và nhiều chế độ(phó bản)

# Các thực thể có trong game

\-Nhà của người chơi. Mọi người có thể gọi nó với tên khác là phi thuyền bay. Thuộc tính: máu

\-Nhà của kẻ địch: đối diện với nhà người chơi. Thuộc tính: máu

\-Một lưới 2D(Grid), là vật vô hình, hình chữ nhật, nằm giữa nhà của người chơi và kẻ địch. Lưới 2D được lấp đầy bởi các ô(Cell, sẽ được đề cập sau), chiều rộng Lưới 2D thường là 5 ô(một số màn là 6), chiều dài tùy biến theo từng ván đấu. Vì chiều dài tương đối dài thế nên người dùng sẽ cần phải vuốt màn hình theo chiều ngang để xem trận địa của ta và địch. Các khái niệm khác:

+Gốc tọa độ: góc trái-trên

+Hàng: số lượng hàng là chiều rộng của grid(có 5 hoặc 6 hàng)

+Cột: số lượng cột là chiều dài của grid(có rất nhiều cột)

\-Ô(Cell): nằm bên trong grid, các thuộc tính: tọa độ, vật thể phía trên, block đang có

## Khối hộp(Block)

Không có công dụng gì quá đặc biệt ngoài việc giúp Monkey và Tower đứng, cũng không thể bị phá hủy

## Trụ(Tower)

Công dụng là tấn công kẻ địch từ phía xa. Thường chiếm diện tích 2x1(2 là chiều dài). Các trụ không thể đứng chồng lên nhau.

Để đặt được Trụ, thứ nhất, bạn cần chỉ định vị trí đặt trên Block. Sau đó sẽ có các Builder Monkey di chuyển từ Nhà chính lên vị trí cần đặt. Khi đến vị trí cần đặt, các Builder sẽ cần thêm thời gian để xây Trụ. Vì vậy, trong quá trình xây dựng bạn phải bảo vệ các Builder khỏi Allen. Vì vậy, việc đặt trụ gần nhà chính sẽ khó bị phá hủy hơn

Một số thuộc tính:

+health(đa phần sẽ có máu giống nhau, tương đối trâu), sát thương, tốc độ bắn, tầm đánh(thường là vô cực/toàn bộ chiều dài Lưới 2D)

+damage: Với lượng sát thương lớn, chúng là vị cứu tinh khi bạn thất thế

\-Lưu ý: tầm đánh luôn là một thuộc tính số nguyên, đơn vị là ô(Cell)

Có nhiều chủng loại

### Barrack

Là loại trụ đặc biệt, chúng triệu hồi Monkey để tấn công kẻ địch chứ không tấn công trực tiếp. Khi Monkey tử trận, chúng sẽ không lăn ra và chết mà chúng sẽ về trụ để hồi sức, sau đó lại ra chiến đấu tiếp. Đây là một Trụ cực kì hiệu quả để chặn đứng kẻ địch

## Khỉ(monkey)

Đội quân Monkey luôn di chuyển lên phía trước

Sở hữu những thuộc tính tương tự Tower và tất nhiên có thêm thuộc tính moveSpeed

So với đối trọng của nó là Allen. Monkey có hỏa lực cao hơn, sở hữu nhiều thẻ bài tấn công tầm xa hơn nhưng ngược lại khả năng phòng thủ tương đối yếu

## Kẻ Địch(Enemy)

Nguyên nhân dẫn đến thua cuộc cho ván đấu của bạn. So với phe Monkey, đội quan Allen sẽ trâu hơn nhiều,

Bọn này biết bay do chúng đều có cánh. Nhưng khi đi trên Block của mình, tốc độ giảm 30%

Nói ngắn gọn, chúng có cơ chế tương đồng với zombie trong PvZ, ngoại trừ việc rất nhiều Enemy trong game này có khả năng tấn công tầm xa

+Có nhiều biến thể(Enemy Variants): Đa phần có khả năng tấn công Tower và Monkey, cận chiến hoặc tầm xa(bắn đạn), một số con đặc biệt đi theo để hỗ trợ đồng đội.

+Thuộc tính: máu, sát thương, tốc đánh, tầm đánh

\-Spawner: là một loại Enemy đặc biệt, nó không tấn công nhà chính, thay vào đó, nó triệu hồi các Enemy. Khi máu dưới 20%, chúng di chuyển về một vị trí(sau đây tạm gọi là destination) gần nhà chính của địch, sau khi đến destination, Spawner hồi lượng máu tương ứng 50% máu tối đa trong 5s(những level đầu không có spawner)

## Roles

Mỗi thẻ bài sẽ đều thuộc về một role. Các thẻ bài trong role sẽ có một đặc điểm chung nào đó. Tính năng này sẽ phát huy tác dụng khi số lượng thẻ bài trở nên tương đối nhiều và cần phải phân loại

Hearty: Có rất nhiều máu, chúng là những người lính dũng cảm lao lên và chịu đòn cho team

Crazy: hỏa lực lớn

Brainy: Chúng là những lá bài có skill mạnh. Những lá bài thuộc nhóm này gồm DonaldTrump với skill tăng giá thẻ bài, ShieldCrusherViking với skill ban phát hiệu ứng phản kích cho toàn đội. Natan với khả năng tạo giáp ảo cho đồng minh lân cận

Sneaky(chỉ xuất hiện ở phe Monkey): Chúng là những người lính giữ vị trí tốt trong giao tranh, tránh nhận sát thương. Hoặc là những sát thủ có khả năng tấn công trực tiếp vào tuyến sau

Swarmy(chỉ xuất hiện ở phe Allen): Hỏa lực không lớn, máu cũng chẳng trâu nhưng chúng mạnh vì số lượng áp đảo trên mặt trận.

Như vậy, mỗi phe sẽ có đúng 4 vai trò

## Kho bài(PlayerHand)

Chứa danh sách các lá bài được chọn trong trận đấu

## Lá bài(MonkeyCard)

Mỗi lá bài sẽ có công dụng khác nhau, đa phần là đặt block, đặt Trụ, triệu hồi Monkey, một số là cường hóa, hay đặc biệt là dịch chuyển vị trí tháp, nhân bản lá bài hiện có trong Inventory

### Thuộc tính

Giá(cost): là một số nguyên, đơn vị là chuối(banana)

Cooldown: sau khi sử dụng thành công một lá bài, bạn cần phải đợi một khoảng thời gian nhất định mới có thể tái sử dụng

Độ hiếm(rarity, đo lường một cách xấp xỉ công sức để kiếm được lá bài đó, không ảnh hưởng đến cơ chế ván đấu, và độ mạnh cũng không hoàn toàn tỉ lệ với độ hiếm của lá bài)

### Card types

\-Lá bài đặt block: Xuất hiện ở màn Sky. Vì diện tích sân đấu tương đối rộng, nên việc đặt từng block là một cực hình, vì vậy, lá bài block sẽ đặt block với diện tích 3x3, tức là đặt 9 block cùng một lúc. Khi đặt, người chơi cần kéo thả lá bài vào đúng vị trí cần đặt. Một vị trí đặt block là hợp lệ nếu nó liên kết với các block khác, hay nói cách khác, block đó không được cô lập

\-Lá bài đặt trụ: bạn chỉ có thể đặt nó trên block. Khi đặt, người chơi cần kéo thả lá bài vào đúng vị trí cần đặt

\-MonkeyCard: lá bài triệu hồi Monkey từ nhà đi ra, để đặt thành công, bạn cần phải chọn làn

## Đạn(bullet)

Được tạo ra bởi Tower, Monkey và Enemy. Đạn có 1 thuộc tính quan trọng: team - đạn sẽ chỉ tương tác với thực thể khác team và thuộc tính damage

## Chuối(Banana)

Đơn vị tiền tệ trong trận đấu, bạn cần chuối để có thể sử dụng các lá bài

# Các cơ chế trong game

## Cơ chế kinh tế

**Ý tưởng cũ:** Ở nhà người chơi có một cây chuối tên là Infinity banana tree. Cây chuối này ban đầu sẽ cho bạn 5 chuối mỗi 5s. Bạn có thể dùng chuối để nâng cấp giúp Infinity banana tree sản sinh chuối nhanh hơn 🡪 Cơ chế kinh tế quá đơn giản

**Ý tưởng PVZ(đang dùng, cơ chế chính)**

Đầu trận sẽ không có BananaTree nào. Để tạo ra chuối, bạn cần phải mang lá bài BananaTree vào trận đấu. Banana Tree là một loại Tower sản sinh chuối sau mỗi khoảng thời gian.

Phía Monkey sở hữu lá bài BananaTree, lá bài này thuộc loại Tower, nó sẽ tạo ra chuối sau mỗi một khoảng thời gian

Phía Allen có "tiếp viện" từ bên ngoài, nên sẽ có nguồn tiền tệ ổn định, không cần phải trồng cây như bên Allen. Tuy tốc độ ban đầu thấp, nhưng bên Allen có thể nâng cấp để tăng tốc độ sản sinh thêm một bậc(Giống Tower Conquest)

🡪 cơ chế đơn giản, dễ hiểu, vừa đảm bảo sự tăng tiến độ khó theo thời gian, vừa tạo cơ hội để Allen lật kèo khi Monkey chiếm ưu thế

## Cơ chế thu thập lá bài

Hiện tại có 2 cơ chế:

### \-Cơ chế 1: Giữ nguyên giống PvZ(hiện tại)

Bạn được chọn trước các lá bài được sử dụng trong trận đấu và khi bạn mua nó, lá bài sẽ cần thời gian để hồi chiêu. Cơ chế hồi chiêu của lá bài sẽ giúp hạn chế chiến thuật spam. Spam là việc người chơi chỉ sử dụng vài lá bài trong suốt trận đấu và bỏ qua những lá bài còn lại

🡪 Đây là một cơ chế được sử dụng cho rất nhiều game

### \-Cơ chế 2: Random(đang được cân nhắc áp dụng cho các chế độ khác)

Trước khi vào trận, người chơi sẽ được chọn trước bộ bài(deck) gồm 15 lá bài. Bạn có thể dùng preset thay vì mỗi trận đấu phải chọn đi chọn lại

Trong trận đấu, bạn có thể "phù phép" một Allen bằng Trụ phù phép(Enchanter). Trụ này không giết kẻ địch, nó "phù phép" một kẻ địch sau mỗi 7s. Nếu trong tay của người chơi đang cầm số lượng lá bài tối đa(10), trụ phù phép sẽ tạm ngưng hoạt động

Khi giết Enemy bị dính "phù phép", chúng sẽ rơi ra một lá bài ngẫu nhiên trong bộ bài ban đầu. Lá bài ngẫu nhiên đó sau đó được đưa vào tay người chơi

Để sử dụng lá bài, bạn cũng cần chuối. Khi sử dụng xong, lá bài lập tức biến mất khỏi tay

- Đây là cơ chế mới mẻ được sử dụng trong PvZ heroes. Cơ chế lá bài này sẽ đảm bảo tính ngẫu nhiên
- Nhưng có một nhược điểm: khi tháp này bị phá hủy, người chơi không còn nguồn nào khác để sản sinh lá bài. Vậy nên người chơi cần phải bảo vệ tháp này ở giai đoạn đầu trận, vì ban đầu người chơi chỉ có duy nhất 1 lá bài có thể sản sinh

Cách khắc phục nhược điểm trên là tạo ra nhiều lá bài có khả năng "Draw"

## Cơ chế Mặt trời - Meteor

Cứ 7 phút ban ngày sẽ có 3 phút ban đêm. Ban ngày, mọi thứ diễn ra bình thường. Ban đêm, toàn bộ Banana Tree sẽ ngủ và không sản sinh thêm chuối, các Allen sẽ nhận hiệu ứng hồi máu I. Ban đêm có thể coi là ác mộng của người chơi vì Banana Tree không hoạt động

# Chiến thuật

Những lá bài rẻ tiền sẽ hữu dụng trong đầu trận đấu nhưng đến cuối sẽ gặp khó khăn vì chúng yếu. Khi về cuối game, những là bài đắt tiền sẽ hỗ trợ bạn rất tốt

Khi phân theo giá trị kinh tế, có 3 kiểu bộ bài mình sẽ đề cập:

\-Rẻ-trung bình: toàn những lá bài có giá từ 1 chuối đến 7 chuối. Bộ bài này sẽ giúp người chơi thắng rất nhanh nhưng đến cuối game sẽ bị tình trạng dư thừa chuối và dễ bị lật kèo

\-Rẻ-trung bình-đắt: giá tiền nào cũng có mặt trong bộ bài, nên bộ bài này phù hợp cho những người mới chơi, từ đầu game đến cuối game đều ổn định

\-Trung bình-đắt: không khuyến khích, vì đa phần ván đầu Enemy sẽ được triệu tập ở mọi lane, và bạn sẽ không có đủ lực lượng về đầu game

## Combination

Một bộ bài tốt sẽ thường có sự phối hợp tốt giữa các lá bài(Combination). Ví dụ:

\-Kết hợp lá bài sinh ra Monkey có tốc đánh cao với lá bài tăng sát thương(cộng thẳng vào chỉ số chứ không phải phần trăm)

\-Kết hợp Mortal(cơ chế giống hệt máy bắn dưa trong PvZ) với lá bài làm chậm để có được một Trụ giống hệt dưa băng trong PvZ, có khả năng làm chậm đám đông

\-Kết hợp Imp(khỉ con với nội tại càng đông càng vui: sát thương tỉ lệ với số lượng khỉ con có trên Lưới 2D) với lá bài Imitator

\-Kết hợp đội hình tanker với SkullCrusher Viking, nội tại phản kích của Viking cực kì hợp với đội hình nhiều máu

## Counter

\- Một lá bài có thể bị coi là yếu nếu so với đội hình Allen này nhưng mạnh nếu so với đội hình Allen khác/ván đấu khác. Ví dụ :

\+ Lá bài có trait Deadly counter cứng Gargantuar và các tanker khác

\+ Khi địch nhiều, cần phải chọn ra những lá bài gây sát thương diện rộng, khi địch ít và khỏe, cần chuyển sang những lá bài tập trung sát thương đơn mục tiêu

Để counter đội hình thiên tốc đánh, ta có thể sử dụng Gargantuar throwing imp: tích nội tại dựa trên lượng đòn đánh nhận phải

# Hệ thống tiền tệ

## Gold

Thu được sau khi thắng ván đấu, ván đấu càng khó thì gold thu được càng nhiều

Được dùng để mua shard cho các lá bài Common, Uncommon, Rare.

Được dùng để mua thẻ thử cho các lá bài Epic, Exotic, Legendary.

Được dùng để thử vận may với Lucky Tree

## Diamond

Thu được từ việc hoàn thành các nhiệm vụ từ bảng tin, và từ các mốc quà trong chế độ Rank

Đây là tài nguyên hiếm

Được dùng để mua shard cho các lá bài Epic, Exotic, Legendary, gói VIP

Được dùng để thử vận may với Casinô

# Hệ thống nâng cấp

Khi người chơi sở hữu đủ mảnh của một lá bài, người chơi có thể quy đổi thành sở hữu lá bài đó VĨNH VIỄN

Người chơi vẫn có thể sưu tập thêm mảnh(shard) của lá bài đó, nếu đạt mốc nhất định thì có thể dùng để nâng cấp lá bài. Lá bài sẽ được nhận một trong số các danh mục sau:

- Máu tối đa
- Giáp và kháng phép
- Sức mạnh/Sức mạnh phép thuật
- Skill mới/Nâng cấp chỉ số của skill

Lá bài có thể được nâng cấp tối đa 4 lần. Lần nâng cấp thứ 2 và thứ 5 sẽ mở khóa cơ chế mới cho lá bài. Ví dụ với thẻ bài Slimz sẽ mở khóa cơ chế kẻ địch bị choáng bởi Poke sẽ nhận thêm 25% sát thương từ mọi nguồn. Các lần nâng cấp còn lại là buff chỉ số đơn thuần

Số lượng mảnh cần để mở khóa một lá bài là 20 hay tăng thêm một level đều là 20.

Level tối đa là 6. Level 6 là level đặc biệt, có tên gọi khác là Mastery level. Level này chỉ tồn tại trong vòng 14 ngày kể từ khi nâng cấp, Entity đạt cấp 6 sẽ có nhiệm vụ đặc biệt

Sau khi lá bài được nâng cấp đến mức tối đa, lá bài sẽ có sức mạnh bằng khoảng 1.5 lần sức mạnh khi chưa nâng cấp. Shar

D

# Assembly definition

"Không có một cách thiết kế hoàn hảo nào, chỉ có cách thiết kế phù hợp với thời điểm hiện tại, cách thiết kế hoạt động tại thời điểm hiện tại có thể phải thay đổi, làm lại trong tương lai"

**Core**: Không phụ thuộc vào module nào khác

**Managers**: Phụ thuộc vào tất cả Module còn lại.

WaveManager cần biết về: kiểu của Entity hiện tại và container chứa Entity đó

**Grid**: Chỉ phụ thuộc vào Core

**Entities**: Phụ thuộc vào Core và Grid. Lý do: vì một số Tower có tương tác với tower cạnh nó, nên nó cần phải được biết thông tin của Grid

Cố gắng thiết kế để Grid không cần biết về thuộc tính của Tower, Monkey, Enemies, nó chỉ nên là chỗ chứa.

Vì các Tower, Monkey có tương tác với nhau nên chúng cần phải biết thông tin, sự kiện của những đồng minh còn lại. Ví dụ:

Vietnam monkey: khi một Tower hoặc Monkey chết thì nó sẽ được tăng sát thương thêm 1. Vì vậy, "Vietnam monkey" cần phải biết khi nào đồng minh của mình chết 🡪 Sử dụng event trong C#

Stuart: khi giết thành công một kẻ địch, sẽ được hồi lại kĩ năng đặc biệt: bắn tạc đạn. Vì vậy, Stuart cần phải biết nếu viên đạn do mình bắn kết liễu được kẻ địch

Các Tower có khả năng buff diện rộng 🡪 cần phải lấy được danh sách đồng minh từ những tọa độ lân cận

Tower cần biết những Enemy nào cùng làn 🡪 Lại một lần nữa cần truy cập Grid

**Cards**: Phụ thuộc vào Core, Grid và Entities Lý do: khi đặt Tower và Monkey sẽ có điều kiện: 2 tower không được chồng lên nhau, vì vậy Cards phải biết thông tin của Grid

**Combat**: Phụ thuộc vào Core và Entities

Hãy nghiên cứu: Entity là đối tượng tạo ra viên đạn, nhưng viên đạn lại là thứ va chạm với Entity 🡪 Không thể tách Bullet và Entity thành 2 assembly khác nhau theo cách thông thường

Cách 2: sử dụng interface(đang nghiên cứu), tức là Core sẽ có một interface tên là IEntity, nhờ đó bullet sẽ sử

**UI**: Phụ thuộc vào Core, Managers và Cards