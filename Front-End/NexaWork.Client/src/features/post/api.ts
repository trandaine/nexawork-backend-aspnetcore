import { CreatePostRequest, PostDto } from './types';
// Import thư viện gọi API của bạn, ví dụ axios:
import axiosInstance from '../../shared/api/axiosInstance'; 

export const createPostAPI = async (data: CreatePostRequest) => {
  // 1. Khởi tạo đối tượng FormData
  const formData = new FormData();

  // 2. Append các trường dữ liệu vào FormData
  formData.append('content', data.content);
  
  // Nối chuỗi cho visibility vì FormData chỉ nhận string hoặc Blob/File
  formData.append('visibility', data.visibility.toString()); 

  // Kiểm tra xem người dùng có upload file không thì mới append
  if (data.mediaFile) {
    formData.append('mediaFile', data.mediaFile);
  }

  // 3. Gọi API (Ví dụ sử dụng axios)
  // Lưu ý: Đa số các thư viện như Axios khi nhận vào FormData sẽ tự động set header 'Content-Type': 'multipart/form-data'
  /*
  const response = await axiosInstance.post('/api/Posts', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  return response.data;
  */
};