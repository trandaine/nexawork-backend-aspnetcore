Các lệnh chuẩn bị chạy dự án


- Update database để tạo các bảng và cấu trúc cơ sở dữ liệu dựa trên các entities đã được định nghĩa trong DbContext:
``` Bash


dotnet ef migrations add [MigrationName] -s [Startup_Project] -p [Project_Contains_DbContext] -c [DbContext_Name]
dotnet ef database update -s [Startup_Project] -p [Project_Contains_DbContext] -c [DbContext_Name]

```