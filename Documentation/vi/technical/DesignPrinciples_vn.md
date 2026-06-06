---
type: technical
module: design-principles

audience: [developer]
status: active
language: vi
description: Defines Vietnamese design principles for modularity, readability, compile-time reduction, and assembly dependencies.
related:
  - architecture
---

# Design purposes

- Reduce compile time. A assembly (.asmdef) that other assembly depend on should be stable.
- Readability: architecture should shout out what project contains.
- Minimal references between assemblies -> easier bug fixing.

# Modularity

Các file nào có nội dung gần nhau hoặc liên kết 2 chiều với nhau thì phải đặt vào một folder/assembly

`Liên kết 2 chiều` có nghĩa là chúng tham chiếu đến nhau, gọi hàm, sử dụng biến của nhau. Ngược lại với `liên kết 2 chiều` là `liên kết một chiều` - chỉ có class A tham chiếu đến class B và không có chiều ngược lại 

# SOLID
