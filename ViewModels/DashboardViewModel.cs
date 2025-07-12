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
                AppVersion = "Phiên bản 1.2.0-beta",
                Author = "BDTG",
                CopyrightNotice = $"Bản quyền © {DateTime.Now.Year} BDTG. RESMOUSE.",

                // THAY ĐỔI 2: Cập nhật thông báo mới
                UpdateMessageTitle = "Giới thiệu Playbook Engine!",
                UpdateMessageContent = "Nền tảng cho các kịch bản tối ưu hóa tùy chỉnh đã được xây dựng. Giờ đây ứng dụng có thể đọc và phân tích các playbook phức tạp.",

                // THAY ĐỔI 3: Thêm mục changelog mới nhất vào đầu danh sách
                Changelog = new List<ChangelogEntry>
                {
                    // === MỤC CHANGELOG MỚI ===
                    new ChangelogEntry
                    {
                        Version = "v1.2.0-beta (Hiện tại)",
                        Changes = new List<string>
                        {   
                            "TOOL 2 mét",
                            "✨ [Mới] Xây dựng nền tảng cho Playbook Engine, cho phép đọc và phân tích các kịch bản tối ưu hóa.",
                            "🚀 [Cải tiến] Nâng cấp bộ phân tích Playbook để hỗ trợ cấu trúc file cấu hình theo định dạng XML.",
                            "🚀 [Cải tiến] Thêm khả năng đọc các tác vụ từ file .yml.",
                        }
                    },
                    // === CÁC MỤC CŨ ===
                    new ChangelogEntry
                    {
                        Version = "v1.1.0-beta",
                        Changes = new List<string>
                        {
                            "Thêm chức năng Dọn dẹp Hệ thống (System Cleanup).",
                            "Thêm màn hình đăng nhập và hệ thống kích hoạt bản quyền.",
                            "Thêm trang Cài đặt (Settings) với tùy chọn thay đổi giao diện.",
                            "Sửa lỗi treo ứng dụng (deadlock) ở trang Giám sát Hệ thống.",
                            "Nâng cấp dự án lên nền tảng .NET 9.0."
                        }
                    },
                    new ChangelogEntry
                    {
                        Version = "v1.0.0-beta",
                        Changes = new List<string>
                        {
                            "Thêm chức năng Game Launcher.",
                            "Thêm chức năng Registry Tweaker.",
                            "Thêm chức năng giám sát hệ thống (System Info) ban đầu."
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