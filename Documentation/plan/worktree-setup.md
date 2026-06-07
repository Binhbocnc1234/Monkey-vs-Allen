---
type: technical
audience: [developer, agent]
status: deprecated
language: vi
description: Deprecated Git worktree setup plan kept for historical context; the current agent workflow uses shared-directory branches.
---

# Kế hoạch thiết lập Git worktree cho agent

Thiết lập workflow dùng Git worktree để agent làm việc trong thư mục riêng thay vì sửa trực tiếp workspace chính. Mục tiêu là giảm rủi ro agent ghi đè thay đổi người dùng đang chỉnh, đặc biệt với tài liệu và các file đang dirty.

---

# Mục tiêu

- Workspace chính là nơi developer làm việc thủ công.
- Mỗi task agent dùng một worktree riêng và một branch riêng.
- Agent commit kết quả trong branch task.
- Developer review diff hoặc merge/cherry-pick từ branch agent vào workspace chính.
- Agent không sửa workspace chính trừ khi user yêu cầu rõ.

---

# Nguyên tắc

1. Không tạo worktree bên trong repo chính.
2. Không để agent và developer cùng sửa một working tree.
3. Trước khi agent bắt đầu task, tạo branch riêng từ base commit rõ ràng.
4. Nếu workspace chính đang dirty, commit hoặc stash những thay đổi cần agent nhìn thấy trước khi tạo worktree.
5. Agent phải chạy `git status` trong worktree của nó trước khi sửa file.
6. Agent phải commit kết quả trước khi yêu cầu developer review.

---

# So sánh với workflow chỉ dùng branch

Git branch và Git worktree đều tách lịch sử commit, nhưng chỉ worktree tách luôn working directory.

| Tiêu chí | Chỉ dùng branch | Dùng worktree |
|----------|-----------------|---------------|
| Tách commit history | Có | Có |
| Tách working directory | Không | Có |
| Developer và agent làm song song | Dễ đạp lên nhau nếu cùng repo dirty | An toàn hơn vì mỗi bên có thư mục riêng |
| Chuyển task nhanh | Phải stash/commit trước khi checkout branch khác | Mở task branch ở thư mục riêng, không cần rời workspace chính |
| Bảo vệ user edits chưa commit | Yếu, vì agent vẫn thấy và có thể sửa cùng working tree | Tốt hơn, vì agent không chạm workspace chính |
| Review kết quả agent | Dùng diff branch | Dùng diff branch |
| Chi phí setup | Thấp | Cao hơn một chút vì có thêm thư mục worktree |
| Unity/cache artifacts | Ít bản copy hơn | Có thể phát sinh cache riêng nếu mở Unity ở nhiều worktree |

**Kết luận:**
- Chỉ dùng branch là đủ nếu chỉ có một người/agent làm việc và workspace sạch.
- Worktree phù hợp hơn khi developer đang chỉnh file, agent cần làm task riêng, hoặc task có nguy cơ sửa nhiều tài liệu/code.
- Với vấn đề agent khôi phục đoạn user đã xóa, branch đơn thuần không đủ vì cả hai vẫn có thể dùng cùng working tree. Worktree giải quyết tốt hơn bằng cách tách không gian làm việc.

---

# Cấu trúc thư mục đề xuất

Repo chính:

```text
D:\Project\_Unity Projects\Monkey vs Allen
```

Thư mục worktree:

```text
D:\Project\_Unity Projects\Monkey vs Allen.worktrees
```

Ví dụ worktree cho một task:

```text
D:\Project\_Unity Projects\Monkey vs Allen.worktrees\agent-docs-frontmatter
```

Không đặt worktree dưới `Assets/`, `Documentation/`, hoặc bất kỳ thư mục nào nằm trong repo chính.

---

# Tool triển khai

Các tool sau nằm trong `tools/` và dùng Python để chạy được trên Windows, Linux, hoặc macOS:

| Tool | Công dụng |
|------|-----------|
| `create_agent_worktree.py` | Tạo branch `agent/<task-slug>` và worktree tương ứng ở thư mục sibling `.worktrees`. |
| `list_agent_worktrees.py` | Liệt kê worktree hiện tại bằng `git worktree list --porcelain`. |
| `cleanup_agent_worktree.py` | Xóa worktree sau task và tùy chọn xóa branch task. |

Ví dụ:

```powershell
python .\tools\create_agent_worktree.py docs-frontmatter
python .\tools\list_agent_worktrees.py
python .\tools\cleanup_agent_worktree.py "..\Monkey vs Allen.worktrees\docs-frontmatter" --branch agent/docs-frontmatter
```

---

# Workflow tạo worktree cho task agent

**Preconditions:**
- Đang đứng ở repo chính.
- Branch chính là `master`.
- Remote `origin/master` đã được fetch gần đây.

**Steps:**

1. Kiểm tra trạng thái workspace chính.

```powershell
git status --short
```

2. Nếu workspace chính có thay đổi mà agent cần dùng, commit trước.

```powershell
git add <files>
git commit -m "Prepare base for agent task"
```

3. Tạo worktree và branch task bằng tool.

```powershell
python .\tools\create_agent_worktree.py docs-frontmatter
```

4. Mở agent trong worktree mới.

```powershell
cd "../Monkey vs Allen.worktrees/docs-frontmatter"
codex
```

5. Agent thực hiện task trong worktree riêng.

```powershell
git status --short
```

6. Agent commit kết quả.

```powershell
git add <files>
git commit -m "Update documentation frontmatter workflow"
```

7. Developer review từ repo chính.

```powershell
git diff master..agent/docs-frontmatter
```

8. Nếu đồng ý, merge vào branch chính.

```powershell
git merge agent/docs-frontmatter
```

---

# Workflow cleanup sau task

Sau khi merge hoặc quyết định bỏ task, xóa worktree và branch task.

Nếu branch đã merge:

```powershell
python .\tools\cleanup_agent_worktree.py "../Monkey vs Allen.worktrees/docs-frontmatter" --branch agent/docs-frontmatter
```

Nếu muốn bỏ task chưa merge:

```powershell
python .\tools\cleanup_agent_worktree.py "../Monkey vs Allen.worktrees/docs-frontmatter" --branch agent/docs-frontmatter --force
```

Nếu Git báo worktree stale:

```powershell
git worktree prune
git worktree list
```

---

# Quy tắc cho agent

Agent phải tuân thủ các rule sau khi làm việc với worktree:

1. Chỉ sửa file trong worktree được giao.
2. Không sửa workspace chính khi đang chạy task worktree.
3. Không khôi phục nội dung user đã xóa trong workspace khác.
4. Trước khi sửa documentation, chạy tool đọc tài liệu nếu cần:

```powershell
python .\tools\query_docs_frontmatter.py --root .\Documentation --audience agent --status active
python .\tools\get_markdown_headers.py .\Documentation\en\technical\how-to\How-to-documentation.md
```

5. Sau khi sửa documentation, validate frontmatter.

```powershell
python .\tools\validate_docs_frontmatter.py --root .\Documentation
```

6. Trước khi kết thúc task, báo rõ:
- branch task
- worktree path
- files changed
- commands/tests đã chạy
- các conflict hoặc dirty files còn lại

---

# Khi nào không cần worktree

Không bắt buộc tạo worktree cho task nhỏ chỉ đọc file hoặc trả lời câu hỏi.

Worktree nên dùng khi:
- task có chỉnh sửa nhiều file
- task đụng vào `Documentation/`
- task có thể chạy format/codegen
- user đang chỉnh cùng repo
- agent cần thử nghiệm nhiều hướng rồi review sau

---

# Rủi ro và cách xử lý

| Rủi ro | Cách xử lý |
|--------|------------|
| Worktree không thấy thay đổi chưa commit trong workspace chính | Commit hoặc stash/apply thay đổi trước khi tạo worktree. |
| Agent branch bị lệch xa `master` | Rebase hoặc tạo worktree mới từ `master` mới nhất. |
| Merge conflict | Developer resolve conflict trong repo chính sau khi review diff. |
| Worktree stale do tool bên ngoài tạo | Chạy `git worktree list`, kiểm tra path, sau đó `git worktree prune`. |
| Unity tạo cache trong nhiều worktree | Đặt worktree ngoài repo chính và không commit generated cache files. |

---

# Acceptance criteria

Workflow được xem là sẵn sàng khi:

- Có thư mục chuẩn cho agent worktrees ngoài repo chính.
- Agent task mới có thể tạo branch/worktree riêng bằng một lệnh.
- Agent có rule rõ ràng không sửa workspace chính.
- Developer có bước review diff trước khi merge.
- Cleanup worktree/branch được mô tả rõ.
- Documentation validation vẫn chạy được trong worktree.
