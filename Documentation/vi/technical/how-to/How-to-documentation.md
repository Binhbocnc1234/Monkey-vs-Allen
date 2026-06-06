---
type: technical
module: how_to_documentation

audience: [developer, agent]
status: active
language: vi
description: Vietnamese guide for project documentation format, tone, completeness rules, and YAML frontmatter requirements.
related:
  - agent_testing_playbook
---

# Giới thiệu

Tài liệu how-to (hướng dẫn thực hành) cung cấp các chỉ dẫn từng bước cụ thể để hoàn thành một tác vụ kỹ thuật trong dự án.

**Mục đích:**
- Hướng dẫn developer thực hiện một công việc cụ thể (chạy test, tạo branch, viết tài liệu, v.v.)
- Giảm thời gian mày mò, loại bỏ phỏng đoán
- Đảm bảo mọi developer làm việc theo cùng một quy trình

**Đối tượng:** 
- Agent - tài liệu cần phải đọc tốt bởi agent
- Developer — người đã có kiến thức nền tảng về Unity và C#.

---

# Tính nhất quán

Tất cả how-to docs phải tuân thủ các quy tắc sau để tránh mâu thuẫn và trùng lặp.

### 1.1 Không trùng lặp nội dung

Không được phép có 2 tài liệu cùng mô tả một logic hoặc quy trình. Khi cần tham chiếu chéo, dùng `See [tên file](đường dẫn)` hoặc `Xem [tên file](đường dẫn)`.

**Ví dụ:**

```
❌ Sai:
File A: "Entity có HP là 100"
File B: "Entity bắt đầu với 100 máu"

✅ Đúng:
Chỉ một file nói "Entity khởi tạo với 100 HP"
File kia tham chiếu: Xem [EntitySystem](../gameplay/EntitySystem.md)
```

### 1.2 Giọng văn trung tính và đồng nhất

Giữ giọng văn trung tính, kỹ thuật, không quảng cáo. Mệnh lệnh thì (imperative) cho các bước hướng dẫn.

**Ví dụ:**

```
❌ Sai:
Bạn sẽ muốn mở Unity Editor trước nhé, rồi bạn vào Test Runner...

✅ Đúng:
Mở Unity Editor. Vào Window > General > Test Runner.
```

### 1.3 Quy ước đặt tên và heading

- Phần heading và tên file được quy định chi tiết ở phần [Chuẩn định dạng & quy ước](#chuẩn-định-dạng--quy-ước).
- Module name vẫn dùng snake_case trong frontmatter: `how_to_documentation`.

**Ví dụ:**

```markdown
# Cách chạy EditMode tests    ← H1
## Yêu cầu                     ← H2
### Công cụ cần có             ← H3
```

---

# Chuẩn định dạng & quy ước

## Ngôn ngữ

Mặc định là viết Tiếng Anh và chỉ viết Tiếng Anh. Chỉ viết tài liệu tiếng Việt khi người dùng yêu cầu

## Markdown Style

- Dùng đúng thứ bậc heading: `#` cho H1, `##` cho H2, `###` cho H3; không nhảy cấp.
- Dùng `**bold**` cho tên nút, nhãn UI, hoặc ý cần nhấn mạnh.
- Dùng `inline code` cho đường dẫn, tên file, module, và identifier.
- Dùng fenced code block cho lệnh, cấu hình, hoặc ví dụ dài.
- Dùng danh sách, bảng, và đường kẻ ngang khi chúng giúp tài liệu dễ đọc hơn.
- Không dùng HTML hoặc emoji.

**Ví dụ heading hierarchy:**

```markdown
# Cách viết tài liệu
## Markdown Style
### Heading hierarchy
```

## Writing Conventions

- Giữ giọng văn trung tính, cung cấp thông tin, không quảng cáo.
  - Ví dụ: viết `Mở Unity Editor.` thay vì `Bạn sẽ muốn mở Unity Editor nhé.`
- Có thể dùng thuật ngữ kỹ thuật khi tài liệu dành cho developer.
  - Ví dụ: `Chạy EditMode test` thay vì diễn giải vòng vo bằng lời thường ngày.
- Viết ngắn gọn, ưu tiên câu ngắn và động từ mệnh lệnh.
  - Ví dụ: `Nhấn Run All.` thay vì `Sau đó bạn hãy nhấn vào Run All để bắt đầu chạy toàn bộ test.`
- Cách tham chiếu chéo phải rõ ràng, dùng link đúng ngữ cảnh.
  - Ví dụ: `Xem [How-to-test](How-to-test.md)` hoặc `See [How-to-test](How-to-test.md)`.
- Tài liệu tiếng Việt là chính; bản tiếng Anh chỉ là bản phản chiếu khi có yêu cầu.
  - Ví dụ: tạo `vi/technical/how-to/How-to-documentation.md` trước, rồi mới tạo bản `en` tương ứng nếu cần.
- Nếu một file được giữ chỗ, nội dung phải để trống hoặc chỉ ghi trạng thái reserved.
  - Ví dụ: `Grid.md (reserved)` không viết nội dung mô tả chưa chốt.

## File Naming

- Dùng PascalCase cho tên file markdown.
  - Ví dụ: `How-to-documentation.md`, `How-to-test.md`.
- Tránh viết tắt nếu không thật sự cần.
  - Ví dụ: dùng `How-to-documentation.md` thay vì `HTDoc.md`.
- Chỉ dùng dấu gạch nối khi cần để giữ tính rõ ràng cho cụm từ nhiều từ.
  - Ví dụ: `How-to-documentation.md` là chấp nhận được vì đây là cách đặt tên chuẩn của nhóm how-to.

---

# Tính đầy đủ

Một how-to doc được coi là **đầy đủ** khi có đủ 5 thành phần sau:

### 2.1 Tiền đề (Preconditions)

Mô tả môi trường, công cụ, trạng thái trước khi bắt đầu. Người đọc phải có đủ điều kiện này mới thực hiện được.

**Ví dụ:**

```markdown
**Tiền đề:**
- Project đã mở trong Unity Editor
- Test Runner đang mở (Window > General > Test Runner)
- Có ít nhất một test file trong thư mục Tests/
```

### 2.2 Các bước thực hiện (Step list)

Đánh số, mỗi bước một hành động cụ thể. Dùng mệnh lệnh thì (imperative).

**Ví dụ:**

```markdown
1. Chọn tab EditMode trong Test Runner
2. Nhấn Run All
3. Quan sát kết quả hiển thị
```

### 2.3 Kết quả mong đợi (Expected results)

Mô tả điều sẽ xảy ra sau mỗi bước hoặc sau tất cả các bước.

**Ví dụ:**

```markdown
**Kết quả mong đợi:**
- Tất cả test đều pass (hiển thị màu xanh)
- Không có test fail hoặc bị bỏ qua
- Thanh progress chạy đến 100%
```

### 2.4 Các tình huống đặc biệt (Edge cases)

Cách xử lý lỗi, ngoại lệ, hoặc trường hợp không mong đợi.

**Ví dụ:**

```markdown
**Tình huống đặc biệt:**
- **Test fail:** Xem log ở Console để biết lỗi. Sửa code và chạy lại.
- **Test Runner không hiển thị:** Vào Window > General > Test Runner để mở.
- **Project không compile:** Sửa lỗi compile trước, sau đó mới chạy test.
```

### 2.5 Xác nhận kiểm tra (Verification)

Cách biết đã làm đúng — có thể tự động hoặc thủ công.

**Ví dụ:**

```markdown
**Xác nhận:**
- Dòng chữ "All tests passed" hiển thị
- Số lượng test passed = tổng số test trong suite
```

---

# Cấu trúc & giọng điệu

### Thứ tự khuyến nghị

```
Giới thiệu → Tiền đề → Các bước → Kết quả → Xử lý lỗi
```

### Quy tắc viết

- Viết **ngắn gọn**, mệnh lệnh thì (imperative).
- Tránh văn kể chuyện dài dòng.
- Dùng **bảng so sánh**, **danh sách đánh số**, **code-block** cho lệnh.
- **Bold** cho UI labels, button names.
- `Inline code` cho file paths, tên module, identifiers.

**Ví dụ hoàn chỉnh:**

```markdown
## Cách chạy EditMode tests

**Tiền đề:**
- Unity Editor đang mở
- Test Runner đã mở (Window > General > Test Runner)

**Các bước:**
1. Chọn tab EditMode
2. Nhấn Run All
3. Quan sát kết quả

**Kết quả mong đợi:**
- Các test chạy trong ~15s
- Tất cả test đều pass (màu xanh)

**Xử lý lỗi:**
- Nếu test fail → xem Console log → sửa lỗi → chạy lại
- Nếu Test Runner không mở được → Restart Unity

**Xác nhận:**
- Dòng "All tests passed" hiển thị
```

### So sánh: Nên và Không nên

| Không nên | Nên |
|-----------|-----|
| "Trước tiên bạn cần mở Unity Editor lên" | "Mở Unity Editor" |
| "Sau đó bạn hãy nhấn vào Run All" | "Nhấn Run All" |
| "Nếu mọi thứ ổn thì bạn sẽ thấy test pass" | "Kết quả: tất cả test pass" |