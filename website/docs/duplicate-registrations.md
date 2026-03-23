---
id: duplicate-registrations
title: Duplicate registrations FAQ
---

# Duplicate registrations (tại sao xảy ra và cách sửa)

Nguyên nhân:
- Generator cũ (v3) đã từng tạo file .g.cs trong nhiều chỗ; nếu không xóa, sẽ tồn tại nhiều registrations.
- Types với cùng FullName xuất hiện trong nhiều assembly.
- Build/Copy của Unity giữ lại các Generated/*.g.cs cũ trong Assets.

Kiểm tra:
- Chạy tìm kiếm trong Unity project: `git ls-files | grep "Generated/.*\.g\.cs"` hoặc tìm trong Explorer.
- Chạy `dotnet di-smoke preflight MyProject.csproj` để báo lỗi duplicate.

Sửa:
1. Xóa file generated cũ (Generated/*.g.cs) trong repo và trong Unity Assets (Assets/Plugins/Analyzers) nếu cần.
2. Cập nhật dependency: nâng generator lên v6.x (hoặc phiên bản đã fix) và rebuild.
3. Nếu vẫn duplicate do multiple assemblies, refactor code (di chuyển type chung vào shared asmdef).
4. Commit thay đổi, rebuild và chạy `dotnet di-smoke validate`.

Các bước triển khai docs:
- README đã cập nhật; site docs có trang này (website/docs/duplicate-registrations.md).
- Đã thêm workflow docs-preview để test deploy preview.
