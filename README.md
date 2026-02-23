# MiniShopee (ASP.NET Core + Blazor)

Một phiên bản Shopee thu nhỏ chạy local với backend + frontend trong cùng ứng dụng Blazor Server.

## Tính năng chính
- Đăng ký + đăng nhập/đăng xuất theo role: `Admin`, `Staff`, `Customer`, `VipCustomer`.
- Trang Home công khai: xem sản phẩm trước khi đăng nhập.
- Danh sách sản phẩm, thêm giỏ hàng, đặt đơn khi đã đăng nhập.
- Theo dõi trạng thái đơn hàng, chọn phương thức thanh toán (COD/Banking/E-Wallet).
- Voucher thường + voucher VIP độc quyền.
- Rule VIP tự động: tổng chi tiêu >= `1,000,000 VND` sẽ được thêm role `VipCustomer`.
- Quản trị:
  - Admin quản lý user, chỉ định staff.
  - Admin/Staff tạo voucher.
  - Trang báo cáo doanh thu, số đơn, số khách VIP, số tài khoản bị khóa.
- Chấm điểm uy tín user (mặc định 10 điểm). Bị report nhiều sẽ giảm điểm; >= 3 report hoặc 0 điểm thì bị block.
- Trang profile: cập nhật thông tin cá nhân + avatar URL.

## Tài khoản seed local
- `admin@local / Admin123$`
- `staff@local / Staff123$`
- `customer@local / Customer123$`
- `vip@local / Vip123$`

## Chạy local
```bash
cd src/MiniShopee
dotnet restore
dotnet run
```

Ứng dụng dùng SQLite file local `minishopee.db`.
