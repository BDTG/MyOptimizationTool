﻿// In folder: ViewModels/DashboardViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyOptimizationTool.Shared.Models;
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
                AppVersion = "Phiên bản 1.5.0-beta",
                Author = "BDTG",
                CopyrightNotice = $"Bản quyền © {DateTime.Now.Year} BDTG. Đã đăng ký bản quyền.",

                // THAY ĐỔI 2: Cập nhật thông báo mới
                UpdateMessageTitle = "Nâng cấp Kiến trúc: Giờ đây đã có Dịch vụ Chạy nền!",
                UpdateMessageContent = "Ứng dụng đã được tái cấu trúc với một dịch vụ chạy nền chuyên dụng. Điều này giúp thu thập dữ liệu hiệu quả hơn, giảm tải cho giao diện chính và mở đường cho các tính năng tối ưu hóa tự động trong tương lai.",

                // THAY ĐỔI 3: Thêm mục changelog mới nhất vào đầu danh sách
                Changelog = new List<ChangelogEntry>
                {
                    // === MỤC CHANGELOG MỚI ===
                    new ChangelogEntry
                    {
                        Version = "v1.5.0-beta (Hiện tại)",
                        Changes = new List<string>
                        {
                            "✨ [Mới] Xây dựng và tích hợp `MyOptimizationTool.Service`, một Dịch vụ Windows chạy nền chuyên dụng.",
                            "🚀 [Kiến trúc] Tái cấu trúc toàn bộ luồng giám sát hệ thống theo mô hình Client-Server.",
                            "🚀 [Kiến trúc] Di chuyển toàn bộ logic thu thập dữ liệu nặng (WMI, LibreHardwareMonitor) sang dịch vụ nền, giúp giao diện chính nhẹ nhàng và ổn định hơn.",
                            "🚀 [Cải tiến] Sử dụng Named Pipes để giao tiếp hiệu quả giữa giao diện và dịch vụ nền.",
                            "🚀 [Cải tiến] Đóng gói dữ liệu giám sát vào một đối tượng `SystemInfoSnapshot` để tối ưu hóa việc truyền tải."
                        }
                    },
                    // === CÁC MỤC CŨ ===
                    new ChangelogEntry
                    {
                        Version = "v1.4.0-beta",
                        Changes = new List<string>
                        {
                            "✨ [Mới] Thêm chức năng giám sát GPU toàn diện vào trang Giám sát Hệ thống.",
                            "🚀 [Cải tiến] Hiển thị thông số real-time cho Tải GPU (%), VRAM (Sử dụng/Tổng), và Nhiệt độ (°C).",
                            "🚀 [Cải tiến] Tăng cường logic dò tìm cảm biến, hỗ trợ song song card đồ họa rời (NVIDIA, AMD) và card tích hợp (Intel).",
                            "🚀 [Cải tiến] Thiết kế lại giao diện giám sát GPU theo dạng 'thẻ' (card) hiện đại, trực quan và dễ đọc hơn.",
                            "🚀 [Cải tiến] Tối ưu hóa hiệu năng: Thông tin ổ đĩa chỉ quét một lần khi tải trang và các vòng tròn tiến trình (Progress Ring) cập nhật mượt mà, không bị giật.",
                            "🐛 [Sửa lỗi] Khắc phục các lỗi về bố cục (layout) trên trang Giám sát Hệ thống, loại bỏ hiện tượng các phần tử bị chồng chéo."
                        }
                    },                    
                    new ChangelogEntry
                    {
                        Version = "v1.3.0-beta",
                        Changes = new List<string>
                        {
                            "✨ [Mới] Thêm trang 'Tối ưu Mạng' với chức năng Áp dụng Tinh chỉnh và Khôi phục Mặc định.",
                            "🚀 [Cải tiến] Xây dựng 'Tweak Script Engine' có khả năng đọc và thực thi các tác vụ (registry, batch script) từ file kịch bản .json bên ngoài.",
                            "🚀 [Cải tiến] Cấu trúc hóa các kịch bản tối ưu, giúp dễ dàng cập nhật và mở rộng mà không cần build lại ứng dụng."
                        }
                    },                    
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