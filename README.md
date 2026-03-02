# MiniShopee (ASP.NET Core + Blazor)

Một phiên bản Shopee thu nhỏ (chỉ bán áo) chạy local với backend + frontend trong cùng ứng dụng Blazor Server.

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
  - Admin/Staff có trang quản lý đơn hàng để cập nhật trạng thái (Confirmed/Packing/Shipping/Delivered/Cancelled).
- Chấm điểm uy tín user (mặc định 10 điểm). Bị report nhiều sẽ giảm điểm; >= 3 report hoặc 0 điểm thì bị block.
- Trang profile: cập nhật thông tin cá nhân + avatar URL.

## Tài khoản seed local
- `admin@local / Admin123$`
- `staff@local / Staff123$`
- `customer@local / Customer123$`
- `vip@local / Vip123$`

## Cài .NET SDK (nếu máy chưa có `dotnet`)
```bash
bash scripts/install-dotnet-sdk.sh
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
dotnet --info
```

Repo có `global.json` để khóa SDK `8.0.100` (net8) cho môi trường local ổn định.

## Cài đầy đủ môi trường để mở/chạy `.sln`
### Cách nhanh (khuyên dùng)
```bash
bash scripts/setup-sln-local.sh
```
Script sẽ tự:
- cài .NET SDK nếu máy chưa có,
- restore package cho `MiniShopee.sln`,
- build toàn bộ solution,
- cài/cập nhật `dotnet-ef` (phục vụ migration local).

### Mở solution
- Visual Studio / Rider: mở file `MiniShopee.sln` ở thư mục gốc.
- VS Code: mở folder repo, cài extension `C# Dev Kit`, sau đó chạy `dotnet restore` và `dotnet run`.

### Nếu bị chặn mạng khi cài SDK
Một số môi trường (proxy/firewall nội bộ) có thể chặn tải từ máy chủ .NET. Khi đó bạn dùng cách Docker bên dưới để chạy app mà không cần SDK trên máy host.

## Chạy bằng Docker (không cần cài .NET SDK trên máy)
```bash
docker build -t minishopee-local .
docker run --rm -p 8080:8080 minishopee-local
```
Sau đó mở: `http://localhost:8080`

## Chạy local
```bash
cd src/MiniShopee
dotnet restore
dotnet run
```

Ứng dụng dùng SQLite file local `minishopee.db`.
