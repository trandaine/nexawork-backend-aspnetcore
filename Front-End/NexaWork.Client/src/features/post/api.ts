import { CreatePostRequest } from './types';
import { axiosInstance } from '../../shared/api/axiosInstance';


/**
 * ham de tao bai post
 * @param data 
 * @returns 
 */
export const createPostAPI = async (data: CreatePostRequest) => {
  const formData = new FormData();
  
  // Nạp dữ liệu văn bản
  formData.append('Content', data.content);
  formData.append('Visibility', data.visibility.toString());

  // Nạp file nếu người dùng có chọn ảnh
  if (data.mediaFile) {
    formData.append('MediaFile', data.mediaFile);
  }

  // Gọi API thông qua axiosInstance. 
  // Lưu ý: Không cần set 'Content-Type' thủ công, Axios sẽ tự động cấu hình chuẩn xác cho FormData.
  // const response = await axiosInstance.post('/Posts', formData);
  const response = await axiosInstance.post('/Posts', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  
  return response.data;
};