# MyOptimizationTool

![Trạng thái Dự án](https://img.shields.io/badge/status-đang%20phát%20triển-orange)

Một công cụ mạnh mẽ và hiện đại được xây dựng bằng C# và WinUI 3 để tối ưu hóa và tinh chỉnh Windows 11.


## 🚀 Giới thiệu

**MyOptimizationTool** là một dự án cá nhân với mục tiêu tạo ra một bộ công cụ "tất cả trong một" để giúp người dùng giám sát, dọn dẹp và tối ưu hóa hệ điều hành Windows 11. Ứng dụng được xây dựng trên nền tảng .NET 9 mới nhất và tuân thủ theo kiến trúc MVVM hiện đại.

## ✨ Các Tính năng Chính

*   **🖥️ Giám sát Hệ thống:** Theo dõi việc sử dụng CPU, RAM, và dung lượng ổ đĩa theo thời gian thực với giao diện đồng hồ tròn trực quan.
*   **🛠️ Tinh chỉnh Hệ thống (System Tweaks):** Cung cấp một danh sách các tinh chỉnh phổ biến cho Registry và PowerShell.
*   **🗑️ Dọn dẹp Rác (System Cleanup):** Quét và xóa các file tạm thời, file rác từ các thư mục hệ thống.
*   **🎮 Trình khởi chạy Game (Game Launcher):** Cho phép người dùng thêm các game của mình và khởi chạy chúng.
*   **🌐 Tối ưu Mạng (Network Tweak):** Áp dụng các tinh chỉnh mạng nâng cao từ kịch bản JSON, với tùy chọn khôi phục về mặc định một cách an toàn.
*   **⚙️ Cài đặt Giao diện:** Dễ dàng chuyển đổi giữa các giao diện Sáng (Light), Tối (Dark), hoặc theo mặc định của hệ thống.
*   **📊 Dashboard Chuyên nghiệp:** Một trang chào mừng hiển thị thông tin phiên bản và lịch sử cập nhật (changelog) của ứng dụng.

## 🛠️ Công nghệ Sử dụng

*   [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
*   [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
*   [WinUI 3](https://docs.microsoft.com/en-us/windows/apps/winui/winui3/)
*   [Windows App SDK](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/)
*   [CommunityToolkit.Mvvm](https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) (cho kiến trúc MVVM)

## 🏁 Bắt đầu

Để chạy dự án này trên máy của bạn, hãy làm theo các bước sau.

### Yêu cầu
*   [Visual Studio 2022](https://visualstudio.microsoft.com/) (với các workload ".NET Desktop Development" và "Universal Windows Platform development" đã được cài đặt).
*   [.NET 9 SDK Preview](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).

### Cài đặt và Chạy

1.  Sao chép kho chứa này về máy của bạn:
    ```sh
    git clone https://github.com/BDTG/MyOptimizationTool.git
    ```
2.  Mở file `MyOptimizationTool.sln` bằng Visual Studio.
3.  **QUAN TRỌNG:** Vì ứng dụng yêu cầu quyền hệ thống, bạn phải chạy Visual Studio với quyền Administrator. Chuột phải vào biểu tượng Visual Studio và chọn **"Run as administrator"**.
4.  Trong Solution Explorer, chuột phải vào **Solution 'MyOptimizationTool'** và chọn **Restore NuGet Packages**.
5.  Đảm bảo nền tảng build đang được chọn là **x64**.
6.  Nhấn **F5** để build và chạy ứng dụng.

## 📂 Cấu trúc Dự án

Dự án tuân thủ theo kiến trúc MVVM để đảm bảo sự phân tách rõ ràng giữa giao diện và logic:

```
MyOptimizationTool/
├── Assets/        # Chứa các tài nguyên như file kịch bản
├── Converters/    # Các lớp chuyển đổi dữ liệu cho XAML Binding
├── Core/          # Các lớp logic nghiệp vụ cốt lõi (services)
├── Models/        # Các lớp đối tượng (POCOs)
├── Services/      # Các dịch vụ chuyên biệt (SystemInfoService)
├── ViewModels/    # Logic và dữ liệu cho các trang (Pages)
└── Views/         # Các file giao diện (XAML Pages & Windows)
```

## 🗺️ Lộ trình Phát triển (Roadmap)

*   [ ] Hoàn thiện logic thực thi cho chức năng "System Tweaks".
*   [ ] Xây dựng lại "Playbook Engine" một cách ổn định.
*   [ ] Thêm chức năng sao lưu/khôi phục Registry trước khi thực hiện tinh chỉnh.
*   [ ] Thêm lại hệ thống bản quyền và kích hoạt.

## 📄 Giấy phép

Dự án này được cấp phép theo Giấy phép MIT.

## 👤 Tác giả

**BDTG** - [GitHub Profile](https://github.com/BDTG)

---