# Hướng dẫn Release & Quản lý Version bằng Git Tag

Để chia sẻ package này một cách chuyên nghiệp, bạn nên sử dụng hệ thống Git Tag. Unity Package Manager (UPM) hỗ trợ rất tốt việc import package qua đường link Git kèm theo version cụ thể.

## 1. Quy trình chuẩn bị Release
Trước khi tạo Tag, hãy đảm bảo file `package.json` ở root folder đã được cập nhật version đúng ý bạn.

```json
"version": "1.0.0"
```

## 2. Các bước thực hiện lệnh Git
Mở terminal tại thư mục gốc của dự án và chạy các lệnh sau:

### Bước 1: Commit toàn bộ thay đổi
```bash
git add .
git commit -m "release: chuẩn bị phiên bản v1.0.0"
```

### Bước 2: Tạo Git Tag
Lưu ý: Version trong Tag nên khớp với version trong `package.json`.
```bash
git tag v1.0.0
```

### Bước 3: Đẩy lên Server (GitHub/GitLab)
```bash
git push origin master --tags
```

---

## 3. Cách chia sẻ và Import vào dự án khác
Khi đã push tag lên GitHub, bạn có thể chia sẻ link cho người khác hoặc dùng cho dự án của chính mình theo các cách sau:

### Cách A: Import qua Package Manager UI
1. Mở Unity -> **Window** -> **Package Manager**.
2. Click nút **[+]** ở góc trên bên trái.
3. Chọn **Add package from git URL...**
4. Nhập link theo định dạng: `https://github.com/user/project-repo.git#v1.0.0`
   * *(Lưu ý dấu `#v1.0.0` ở cuối chính là để chỉ định Tag cụ thể)*.

### Cách B: Chỉnh sửa trực tiếp `manifest.json`
Thêm dòng sau vào danh sách `dependencies` trong file `Packages/manifest.json`:
```json
"com.nhemdangfugbixs.tooling": "https://github.com/user/project-repo.git#v1.0.0"
```

---

## 4. Tại sao nên dùng Tag?
- **Tính ổn định**: Khi bạn làm việc tiếp ở nhánh master, các dự án đang dùng bản `#v1.0.0` sẽ không bị ảnh hưởng bởi những bug mới phát sinh.
- **Dễ dàng Rollback**: Nếu bản `v1.1.0` có lỗi, bạn chỉ cần sửa URL về `#v1.0.0` là mọi thứ quay lại trạng thái ổn định ngay lập tức.
- **Chuyên nghiệp**: Giúp team của bạn biết chính xác họ đang sử dụng phiên bản nào của công cụ.
