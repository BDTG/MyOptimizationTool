// In folder: ViewModels/DashboardViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyOptimizationTool.Models;
using System;
using System.Collections.Generic;

namespace MyOptimizationTool.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private DashboardInfo info;

        public DashboardViewModel()
        {
            Info = new DashboardInfo
            {
                // THAY ĐỔI 1: Cập nhật phiên bản ứng dụng
                AppVersion = "Phiên bản 1.3.0-beta",
                Author = "Tên của bạn",
                CopyrightNotice = $"Bản quyền © {DateTime.Now.Year} Tên của bạn. Đã đăng ký bản quyền.",

                // THAY ĐỔI 2: Cập nhật thông báo mới
                UpdateMessageTitle = "Giới thiệu Chức năng Tối ưu Mạng!",
                UpdateMessageContent = "Một module mới cho phép áp dụng các tinh chỉnh mạng phức tạp thông qua kịch bản, đồng thời cung cấp tùy chọn khôi phục về mặc định một cách an toàn.",

                // THAY ĐỔI 3: Thêm mục changelog mới nhất vào đầu danh sách
                Changelog = new List<ChangelogEntry>
                {
                    // === MỤC CHANGELOG MỚI ===
                    new ChangelogEntry
                    {
                        Version = "v1.3.0-beta (Hiện tại)",
                        Changes = new List<string>
                        {
                            "✨ [Mới] Thêm trang 'Tối ưu Mạng' với chức năng Áp dụng Tinh chỉnh và Khôi phục Mặc định.",
                            "🚀 [Cải tiến] Xây dựng 'Tweak Script Engine' có khả năng đọc và thực thi các tác vụ (registry, batch script) từ file kịch bản .json bên ngoài.",
                            "🚀 [Cải tiến] Cấu trúc hóa các kịch bản tối ưu, giúp dễ dàng cập nhật và mở rộng mà không cần build lại ứng dụng."
                        }
                    },
                    // === CÁC MỤC CŨ ===
                    new ChangelogEntry
                    {
                        Version = "v1.2.0-beta",
                        Changes = new List<string>
                        {
                            "Xây dựng nền tảng ban đầu cho Playbook Engine.",
                            "Nâng cấp bộ phân tích để hỗ trợ cấu trúc file XML/YAML."
                        }
                    },
                    new ChangelogEntry
                    {
                        Version = "v1.1.0-beta",
                        Changes = new List<string>
                        {
                            "Thêm chức năng Dọn dẹp Hệ thống, Đăng nhập và Cài đặt Giao diện.",
                            "Sửa lỗi treo ứng dụng (deadlock) ở trang Giám sát Hệ thống.",
                            "Nâng cấp dự án lên nền tảng .NET 9.0."
                        }
                    },
                    new ChangelogEntry
                    {
                        Version = "v1.0.0-beta",
                        Changes = new List<string>
                        {
                            "Thêm chức năng Game Launcher, Registry Tweaker, System Info."
                        }
                    },
                    new ChangelogEntry
                    {
                        Version = "v0.1.0-alpha",
                        Changes = new List<string>
                        {
                            "Khởi tạo dự án và cấu trúc MVVM."
                        }
                    }
                }
            };
        }
    }
}