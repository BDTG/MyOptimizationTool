# MyOptimizationTool

<!-- BADGES -->
![Ngôn ngữ](https://img.shields.io/badge/language-C%23-blueviolet)
![Nền tảng](https://img.shields.io/badge/.NET-9.0-blue)
![Giao diện](https://img.shields.io/badge/UI-WinUI%203-brightgreen)
![Trạng thái](https://img.shields.io/badge/status-đang%20phát%20triển-orange)
![Giấy phép](https://img.shields.io/badge/license-MIT-blue)

Một công cụ mạnh mẽ và hiện đại được xây dựng bằng C# và WinUI 3 để giám sát, dọn dẹp và tối ưu hóa hệ điều hành Windows 11.

![MyOptimizationTool Screenshot](https://i.imgur.com/example.png) 

## 🚀 Giới thiệu

**MyOptimizationTool** là một bộ công cụ "tất cả trong một" được thiết kế với giao diện trực quan, hiện đại, giúp người dùng dễ dàng kiểm soát và tối ưu hóa hiệu năng máy tính Windows 11. Dự án được xây dựng hoàn toàn bằng công nghệ C#/.NET 9 mới nhất, tuân thủ chặt chẽ kiến trúc MVVM để đảm bảo mã nguồn sạch, dễ bảo trì và mở rộng.

## ✨ Các Tính năng Chính

*   **🖥️ Giám sát Hệ thống Toàn diện:**
    *   Theo dõi **CPU, RAM, Ổ đĩa** theo thời gian thực.
    *   **Giám sát GPU chuyên sâu (NVIDIA, AMD, Intel):** Hiển thị chi tiết % Tải (Load), VRAM đã sử dụng, và Nhiệt độ của từng card đồ họa.
    *   Giao diện được trình bày khoa học, trực quan với các đồng hồ tròn và hiệu ứng chuyển động mượt mà.

*   **🛠️ Tinh chỉnh Hệ thống (System Tweaks):**
    *   Một danh sách các tinh chỉnh phổ biến và hữu ích cho Registry và PowerShell, giúp cá nhân hóa và cải thiện hiệu năng hệ thống.
    *   Mỗi tinh chỉnh được bật/tắt dễ dàng thông qua một công tắc (Toggle Switch).

*   **🗑️ Dọn dẹp Hệ thống (System Cleanup):**
    *   Quét và tính toán dung lượng các file rác trong các thư mục tạm của Windows và Người dùng.
    *   Cho phép người dùng chọn và giải phóng dung lượng đĩa cứng một cách an toàn.

*   **🌐 Tối ưu Mạng (Network Tweak):**
    *   Áp dụng các kịch bản tinh chỉnh mạng nâng cao từ file JSON để giảm độ trễ (ping) và cải thiện hiệu suất kết nối, đặc biệt hữu ích cho game thủ.
    *   Tích hợp chức năng khôi phục cài đặt mạng của Windows về mặc định một cách an toàn.

*   **🎮 Trình khởi chạy Game (Game Launcher):**
    *   Thêm và quản lý danh sách các game yêu thích.
    *   Khởi chạy game trực tiếp từ ứng dụng.

*   **⚙️ Tùy chỉnh & Cài đặt:**
    *   Dễ dàng chuyển đổi giữa các giao diện Sáng (Light), Tối (Dark), hoặc theo mặc định của hệ thống.
    *   Trang Dashboard chuyên nghiệp hiển thị thông tin phiên bản và lịch sử cập nhật (changelog) chi tiết.

## 🛠️ Công nghệ & Thư viện

*   **Ngôn ngữ:** C# 12
*   **Nền tảng:** .NET 9 & Windows App SDK 1.5
*   **Giao diện:** WinUI 3
*   **Kiến trúc:** MVVM (sử dụng [CommunityToolkit.Mvvm](https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/))
*   **Giám sát Phần cứng:** [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)

## 🏁 Bắt đầu

Để build và chạy dự án này trên máy của bạn, hãy làm theo các bước sau.

### Yêu cầu
*   [Visual Studio 2022](https://visualstudio.microsoft.com/) (với các workload ".NET Desktop Development" và "Universal Windows Platform development").
*   [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
*   Hệ điều hành Windows 11.

## 📂 Cấu trúc Dự án

Dự án được tổ chức theo kiến trúc MVVM để đảm bảo sự phân tách rõ ràng giữa giao diện (View), logic hiển thị (ViewModel) và logic nghiệp vụ (Core/Services).

MyOptimizationTool/
├── Assets/         # Tài nguyên (icon, JSON, file cấu hình)
├── Converters/     # Value converters cho XAML binding
├── Core/           # Logic nghiệp vụ chính (TweakManager, CleanupService...)
├── Models/         # Các lớp dữ liệu (POCOs, ObservableObjects)
├── Services/       # Dịch vụ hệ thống (SystemInfoService, RegistryService...)
├── ViewModels/     # Logic và trạng thái cho các View
└── Views/          # Giao diện người dùng (XAML Pages & Windows)


## 🗺️ Lộ trình Phát triển (Roadmap)

Đây là những tính năng dự kiến sẽ được phát triển trong các phiên bản tiếp theo:

*   [x] Tích hợp chức năng giám sát GPU chi tiết (Load, VRAM, Temp).
*   [ ] **Quản lý Ứng dụng Khởi động (Startup Apps Manager):** Cho phép người dùng xem và vô hiệu hóa các chương trình khởi động cùng Windows.
*   [ ] **Mở rộng System Tweaks:** Thêm nhiều tinh chỉnh hơn về hiệu năng, bảo mật và giao diện từ các nguồn uy tín.
*   [ ] **Sao lưu Registry:** Tự động tạo bản sao lưu trước khi áp dụng các tinh chỉnh liên quan đến Registry.
*   [ ] **Cải thiện Xử lý Lỗi:** Xây dựng hệ thống ghi log và hiển thị thông báo lỗi thân thiện hơn với người dùng.
*   [ ] **Đa ngôn ngữ:** Hỗ trợ chuyển đổi ngôn ngữ (Tiếng Anh/Tiếng Việt).

## 🙌 Đóng góp

Mọi sự đóng góp đều được chào đón! Nếu bạn có ý tưởng về tính năng mới hoặc muốn vá lỗi, hãy:
1.  Fork dự án này.
2.  Tạo một Branch mới (`git checkout -b feature/AmazingFeature`).
3.  Commit các thay đổi của bạn (`git commit -m 'Add some AmazingFeature'`).
4.  Push lên Branch (`git push origin feature/AmazingFeature`).
5.  Mở một Pull Request.

Bạn cũng có thể mở một **Issue** để báo lỗi hoặc đề xuất tính năng.

## 📄 Giấy phép

Dự án này được cấp phép theo **Giấy phép MIT** - xem file `LICENSE` để biết thêm chi tiết.

## 👤 Tác giả

**BDTG**

*   GitHub: [@BDTG](https://github.com/BDTG)

---