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
  // Unique identifier for the post
  postId: string;
  customerName: string;
  content: string;
  // The relative or absolute path to the image. It can be null if the post has no image.
  mediaUrl?: string | null;
  
  // Engagement metrics
  likesCount: number;
  commentsCount: number;
  sharesCount: number;
  visibility: number;
  
  // Timestamps in ISO string format (e.g., "2026-05-14T18:30:39.606Z")
  createdAt: string; 
  updatedAt: string;
}