# NhemDangFugBixs.VContainer.SourceGenerator

Công cụ Roslyn Source Generator cho VContainer (Unity). Tự động sinh đăng ký DI từ các thuộc tính `[AutoRegister]`/`[AutoRegisterIn]` để giảm boilerplate.

Quick start

- Build tooling:
  dotnet build NhemDangFugBixs.Tooling.sln

- Generate & validate for a project (preflight):
  dotnet di-smoke preflight MyGame.csproj

- Run runtime smoke (optional):
  dotnet di-smoke validate bin/Debug/net10.0/MyGame.dll --format json

Changelog & release
- Xem CHANGELOG.md cho chi tiết phiên bản.
- Tạo release: tag commit, push tag (vX.Y.Z) → workflow sẽ pack CLI và tạo Release.

Troubleshooting — duplicate registrations

Nguyên nhân phổ biến:
- Legacy generated files (các file .g.cs từ phiên bản cũ) vẫn tồn tại trong Generated/ hoặc Assets/Plugins/Analyzers và gây đăng ký trùng lặp.
- Cùng một type (FullName) tồn tại trong nhiều assembly (code duplicated across asmdef).
- Cấu hình assembly (AllowedAssemblies) hoặc Attribute bị nhầm scope.

Cách khắc phục nhanh:
1. Bật phiên bản generator mới (>= v6.0.1) để có dedupe/DeclaringAssembly filtering.
2. Xóa các file generated cũ: tìm `**/Generated/*.g.cs` trong Unity project và xóa (hoặc commit overwrite).
3. Chạy `dotnet di-smoke preflight` để phát hiện duplicate trước khi vào Unity Play Mode.
4. Nếu type trùng tên trong nhiều assembly, refactor để tránh duplicate types hoặc đóng gói một implementation vào shared asmdef.

Contributing
- Xây dựng: dotnet build
- Test: dotnet test
- Docs: cd website && npm ci && npm run build

License
- Một phần của NhemDangFugBixs Tooling collection.
