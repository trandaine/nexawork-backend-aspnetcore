## Kế hoạch cho dự án LinkedIn Clone 

## Giai đoạn 1: Lập kế hoạch và thiết kế Entities

### 1. Profile & Networking Entities

Thiết lập các entities cơ bản cho profile người dùng, kết nối mạng lưới, và các tương tác xã hội như kết bạn, theo dõi, và nhắn tin.

- `Customers`: Chứa thông tin cá nhân, kinh nghiệm làm việc, học vấn, kỹ năng, và sở thích.
    - `CustomerId`: Primary key, định danh duy nhất cho mỗi người dùng.
    - `IdentityUserId`: Liên kết với hệ thống xác thực người dùng.
    - `FirstName`: Tên người dùng.
    - `LastName`: Họ người dùng.
    - `Headline`: Tiêu đề chuyên nghiệp của người dùng.
    - `Summary`: Tóm tắt về bản thân và kinh nghiệm làm việc.
    - `Location`: Vị trí địa lý của người dùng.
    - `ProfilePictureUrl`: URL ảnh đại diện của người dùng.
    - `BackgroundPictureUrl`: URL ảnh nền của người dùng.
    - `ConnectionId`: Danh sách các kết nối mạng lưới của người dùng.
    - `CustomerSkillsId`: Danh sách các kỹ năng của người dùng (Foreign Key).
    - `EducationId`: Danh sách các mục học vấn của người dùng (Foreign Key).
    - `ExperienceId`: Danh sách các mục kinh nghiệm làm việc của người dùng (Foreign Key).



- `Connections`: Quản lý các kết nối giữa người dùng, bao gồm trạng thái kết nối (đang chờ, đã chấp nhận, đã từ chối).
    - `ConnectionId`: Primary key, định danh duy nhất cho mỗi kết nối.
    - `CustomerId`: ID của người dùng chủ sở hữu kết nối.
    - `ConnectedCustomerId`: ID của người dùng được kết nối.
    - `Status`: Trạng thái của kết nối (Pending, Accepted, Rejected).
    - `CreatedAt`: Thời gian tạo kết nối.


### 2. Resume & Portfolio Entities (CV & Hồ sơ năng lực)

Thiết lập các entities để quản lý hồ sơ năng lực và CV của người dùng, cho phép họ tạo và chia sẻ thông tin về kinh nghiệm làm việc, học vấn, kỹ năng, và dự án.

- `Educations`: Quản lý thông tin về học vấn của người dùng, bao gồm trường học, chuyên ngành, và thời gian học.
    - `EducationId`: Primary key, định danh duy nhất cho mỗi mục học vấn.
    - `CustomerId`: ID của người dùng chủ sở hữu thông tin học vấn.
    - `SchoolName`: Tên trường học.
    - `Degree`: Bằng cấp đạt được.
    - `FieldOfStudy`: Chuyên ngành học.
    - `StartDate`: Ngày bắt đầu học.
    - `EndDate`: Ngày kết thúc học (có thể để trống nếu đang học).
    - `Description`: Mô tả thêm về học vấn (tùy chọn).

- `Experiences`: Quản lý thông tin về kinh nghiệm làm việc của người dùng, bao gồm công ty, vị trí, và thời gian làm việc.
    - `ExperienceId`: Primary key, định danh duy nhất cho mỗi mục kinh nghiệm.
    - `CustomerId`: ID của người dùng chủ sở hữu thông tin kinh nghiệm.
    - `OrganizationId`: ID của tổ chức (công ty) mà người dùng đã làm việc có sẵn trong hệ thống.
    - `Position`: Vị trí công việc.
    - `Title`: Chức danh công việc.
    - `EmploymentType`: Loại hình công việc (toàn thời gian, bán thời gian, hợp đồng, thực tập).
    - `StartDate`: Ngày bắt đầu làm việc.
    - `EndDate`: Ngày kết thúc làm việc (có thể để trống nếu đang làm việc).
    - `Description`: Mô tả thêm về kinh nghiệm làm việc
    - `IsCurrent`: Cờ để xác định nếu đây là công việc hiện tại của người dùng.

- `Skills`: Quản lý thông tin về kỹ năng mà các công ty đang cần, bao gồm tên kỹ năng và mức độ thành thạo.
    - `SkillId`: Primary key, định danh duy nhất cho mỗi kỹ năng.
    - `Name`: Tên kỹ năng (ví dụ: Java, Python, Project Management).
    - `Description`: Mô tả thêm về kỹ năng (tùy chọn).
    

- `CustomerSkills`: Quản lý mối quan hệ many-to-many giữa người dùng và kỹ năng của họ, cho phép người dùng liệt kê các kỹ năng mà họ có.
    - `CustomerSkillId`: Primary key, định danh duy nhất cho mỗi mục kỹ năng của người dùng.
    - `CustomerId`: ID của người dùng chủ sở hữu kỹ năng.
    - `SkillId`: ID của kỹ năng được liên kết với người dùng.
    - `ProficiencyLevel`: Mức độ thành thạo của người dùng đối với kỹ năng này (Beginner, Intermediate, Advanced).


- `Organizations`: Quản lý thông tin về các tổ chức (công ty) mà người dùng đã làm việc, bao gồm tên công ty, ngành nghề, và địa điểm.
    - `OrganizationId`: Primary key, định danh duy nhất cho mỗi tổ chức.
    - `Name`: Tên công ty.
    - `Industry`: Ngành nghề của công ty.
    - `Location`: Vị trí địa lý của công ty.
    - `Description`: Mô tả thêm về công ty (tùy chọn).
    - `WebsiteUrl`: URL trang web của công ty (tùy chọn).
    - `OrganizationLogoUrl`: URL logo của công ty (tùy chọn).
    - `FoundedDate`: Ngày thành lập của công ty (tùy chọn).

### 3. Content & Feed Entities

Thiết lập các entities để quản lý nội dung và feed của người dùng, cho phép họ tạo và chia sẻ bài viết, hình ảnh, video, và tương tác với nội dung của người khác.

- `Posts`: Quản lý thông tin về bài viết của người dùng, bao gồm nội dung, hình ảnh, video, và thời gian đăng.
    - `PostId`: Primary key, định danh duy nhất cho mỗi bài viết.
    - `CustomerId`: ID của người dùng chủ sở hữu bài viết.
    - `Content`: Nội dung văn bản của bài viết.
    - `MediaUrl`: URL của hình ảnh hoặc video đính kèm (tùy chọn).
    - `CreatedAt`: Thời gian tạo bài viết.
    - `UpdatedAt`: Thời gian cập nhật bài viết (nếu có).
    - `LikesCount`: Số lượng lượt thích bài viết.
    - `CommentsCount`: Số lượng bình luận trên bài viết.
    - `SharesCount`: Số lượng lượt chia sẻ bài viết.
    - `Visibility`: Mức độ hiển thị của bài viết (Public, Connections, Private).
- `Comments`: Quản lý thông tin về bình luận của người dùng trên bài viết, bao gồm nội dung, thời gian đăng, và mối quan hệ với bài viết.
    - `CommentId`: Primary key, định danh duy nhất cho mỗi bình luận.
    - `PostId`: ID của bài viết mà bình luận thuộc về.
    - `CustomerId`: ID của người dùng chủ sở hữu bình luận.
    - `Content`: Nội dung văn bản của bình luận.
    - `CreatedAt`: Thời gian tạo bình luận.
    - `UpdatedAt`: Thời gian cập nhật bình luận (nếu có).
    - `LikesCount`: Số lượng lượt thích bình luận.
- `Reactions`: Quản lý thông tin về các phản ứng của người dùng đối với bài viết và bình luận, bao gồm loại phản ứng (like, love, insightful, etc.)
    - `ReactionId`: Primary key, định danh duy nhất cho mỗi phản ứng.
    - `CustomerId`: ID của người dùng chủ sở hữu phản ứng.
    - `PostId`: ID của bài viết mà phản ứng thuộc về (có thể để trống nếu phản ứng thuộc về bình luận).
    - `CommentId`: ID của bình luận mà phản ứng thuộc về (có thể để trống nếu phản ứng thuộc về bài viết).
    - `ReactionType`: Loại phản ứng (Like, Love, Insightful, etc.).
    - `CreatedAt`: Thời gian tạo phản ứng.

### 4. Job Board Entities

- `JobListings`: Quản lý thông tin về các công việc được đăng bởi các công ty, bao gồm tiêu đề công việc, mô tả, yêu cầu, và thông tin liên hệ.
    - `JobListingId`: Primary key, định danh duy nhất cho mỗi công việc.
    - `OrganizationId`: ID của tổ chức (công ty) đăng công việc.
    - `Title`: Tiêu đề công việc.
    - `Description`: Mô tả chi tiết về công việc.
    - `Requirements`: Yêu cầu đối với ứng viên (kinh nghiệm, kỹ năng, học vấn).
    - `Location`: Vị trí địa lý của công việc.
    - `EmploymentType`: Loại hình công việc (Enum: Part-time, Internship, Contract).
    - `SalaryRange`: Khoảng lương (tùy chọn).
    - `ContactEmail`: Email liên hệ để ứng tuyển.
    - `CreatedAt`: Thời gian tạo công việc.
    - `UpdatedAt`: Thời gian cập nhật công việc (nếu có).
    - `IsActive`: Cờ để xác định nếu công việc đang hoạt động hay đã hết hạn.
- `JobApplications`: Quản lý thông tin về các ứng dụng công việc của người dùng, bao gồm trạng thái ứng dụng và thông tin liên hệ.
    - `JobApplicationId`: Primary key, định danh duy nhất cho mỗi ứng dụng công việc.
    - `JobListingId`: ID của công việc mà người dùng đã ứng tuyển.
    - `CustomerId`: ID của người dùng chủ sở hữu ứng dụng.
    - `ResumeUrl`: URL của CV hoặc hồ sơ năng lực được đính kèm với ứng dụng.
    - `CoverLetter`: Thư xin việc (tùy chọn).
    - `Status`: Trạng thái của ứng dụng (Pending, Reviewed, Accepted, Rejected).
    - `AppliedAt`: Thời gian nộp đơn ứng tuyển.

---
## Giai đoạn 2: Phát triển các entities và thiết lập cơ sở dữ liệu

- Sử dụng Entity Framework Core để tạo các lớp entity tương ứng với các bảng trong cơ sở dữ liệu.
- Thiết lập các mối quan hệ giữa các entities (one-to-many, many-to-many) dựa trên thiết kế đã xác định.
- Tạo các configuration cho các entities để xác định các ràng buộc, khóa ngoại, và các thuộc tính khác (ví dụ: độ dài tối đa của chuỗi, kiểu dữ liệu, v.v.).
- Sử dụng migrations để tạo và cập nhật cơ sở dữ liệu dựa trên các entities đã được định nghĩa.






<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />

# Notes những điểm kỹ thuật:

### 1. Ý nghĩa của `ConnectedCustomer` trong thiết kế mạng xã hội 

Đây được gọi là mối quan hệ Self-Referencing Many-to-Many (Nhiều-Nhiều tự tham chiếu). Dưới đây là lý do tại sao hệ thống của bạn bắt buộc phải có nó:

#### Bản chất của một "Kết nối" (Connection)
Trên các nền tảng như LinkedIn, một "kết nối" luôn diễn ra giữa hai người dùng. Cả hai người này đều là thực thể nằm trong bảng `Customers`.

Để lưu lại việc "Người A kết nối với Người B", bảng `Connections` (bảng trung gian) buộc phải lưu ID của cả hai người. Do đó, chúng ta cần chia ra hai vai trò rõ ràng:

- `CustomerId`: ID của người gửi lời mời kết nối (Người A).

- `ConnectedCustomerId`: ID của người nhận lời mời kết nối (Người B).

#### Mô phỏng dữ liệu thực tế
Hãy xem ví dụ bảng Connections trong database sẽ trông như thế nào:

ConnectionId|CustomerId (Người gửi)|ConnectedCustomerId (Người nhận)|Status
---|---|---|---
Conn_01|ID_Của_An|ID_Của_Bình|Accepted
Conn_02|ID_Của_Châu|ID_Của_An|Pending

Nếu không có cột `ConnectedCustomerId`, cơ sở dữ liệu sẽ chỉ biết "An có một kết nối" nhưng không thể biết "An kết nối với ai".


