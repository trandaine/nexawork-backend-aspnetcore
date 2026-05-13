Các lệnh chuẩn bị chạy dự án


- Update database để tạo các bảng và cấu trúc cơ sở dữ liệu dựa trên các entities đã được định nghĩa trong DbContext:
``` Bash


dotnet ef migrations add [MigrationName] -s [Startup_Project] -p [Project_Contains_DbContext] -c [DbContext_Name]
dotnet ef database update -s [Startup_Project] -p [Project_Contains_DbContext] -c [DbContext_Name]

```

- Docker command để chạy RabbitMQ server:
``` Bash
docker run --detach --hostname my-rabbit --name nexawork-rabbitmq \
    --env RABBITMQ_DEFAULT_USER=admin \
    --env RABBITMQ_DEFAULT_PASS=Admin@123456 \
    --publish 15672:15672 \
    --publish 5672:5672 \
     rabbitmq:3-management

```