// Định nghĩa Enum cho Visibility để code rõ ràng hơn thay vì dùng số magic number (Tùy chọn)
// Giả sử backend của bạn quy định: 0 = Public, 1 = Connections Only, 2 = Private
export enum PostVisibility {
  Public = 0,
  Connections = 1,
  Private = 2,
}

// 1. Interface đại diện cho dữ liệu gửi đi khi TẠO bài viết (Khớp với Swagger)
export interface CreatePostRequest {
  content: string;
  // Dùng kiểu 'File' của trình duyệt thay vì string, vì React sẽ xử lý file upload qua input type="file"
  mediaFile?: File | null; 
  visibility: PostVisibility | number;
}

// 2. Interface đại diện cho dữ liệu nhận về khi LẤY bài viết (Dự trù cho API GET)
export interface PostDto {
  postId: string;
  customerName: string;
  content: string;
  mediaUrl?: string | null;
  // mediaUrl?: string; // Tên sẽ phụ thuộc vào backend trả về
  likesCount: number;
  commentsCount: number;
  sharesCount: number;
  visibility: number;
  createdAt: string; 
  updatedAt: string;// ISO string, bạn có thể chuyển đổi sang Date khi sử dụng

  // ... (bạn sẽ bổ sung thêm các trường như author, createdAt khi có Swagger của hàm GET)
}