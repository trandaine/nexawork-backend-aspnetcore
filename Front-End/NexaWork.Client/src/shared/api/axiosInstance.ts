import axios from 'axios';

// 1. Khởi tạo instance với các cấu hình mặc định
const axiosInstance = axios.create({
  // Sử dụng biến môi trường cho linh hoạt, hoặc gõ cứng URL backend .NET tạm thời
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000/api', 
  timeout: 10000, // Hủy request nếu server không phản hồi sau 10 giây
  headers: {
    'Content-Type': 'application/json',
  },
});

// 2. REQUEST INTERCEPTOR: Can thiệp trước khi gửi request xuống Backend
axiosInstance.interceptors.request.use(
  (config) => {
    // Lấy token từ LocalStorage (hoặc nơi bạn đang lưu trữ token sau khi Login)
    const token = localStorage.getItem('accessToken');
    
    // Nếu có token, tự động đính kèm vào header Authorization
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    return config;
  },
  (error) => {
    // Xử lý lỗi trước khi request được gửi đi (ít gặp)
    return Promise.reject(error);
  }
);

// 3. RESPONSE INTERCEPTOR: Can thiệp sau khi nhận kết quả từ Backend trả về
axiosInstance.interceptors.response.use(
  (response) => {
    // Bất kỳ mã trạng thái nào nằm trong dải 2xx sẽ kích hoạt hàm này
    // Bạn có thể format lại data ở đây nếu cần, ví dụ: return response.data;
    return response;
  },
  (error) => {
    // Bất kỳ mã trạng thái nào lọt ra ngoài dải 2xx sẽ kích hoạt hàm này
    if (error.response) {
      const status = error.response.status;

      // Xử lý lỗi 401: Token hết hạn hoặc chưa đăng nhập
      if (status === 401) {
        console.warn('Unauthorized! Yêu cầu đăng nhập lại.');
        // Thực hiện logic xóa token và điều hướng về trang Login ở đây
        // localStorage.removeItem('accessToken');
        // window.location.href = '/auth/signin'; 
      }
      
      // Xử lý lỗi 403: Không có quyền truy cập
      if (status === 403) {
        console.warn('Forbidden! Bạn không có quyền thực hiện hành động này.');
      }
    }

    return Promise.reject(error);
  }
);

export default axiosInstance;