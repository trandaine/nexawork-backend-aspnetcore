import axios from 'axios';

const getOidcAccessToken = (): string | null => {
  try {
    // Lặp qua tất cả các key trong sessionStorage
    for (let i = 0; i < sessionStorage.length; i++) {
      const key = sessionStorage.key(i);
      
      // Tìm key mặc định của oidc-client-ts (bắt đầu bằng "oidc.user:")
      if (key && key.startsWith('oidc.user:')) {
        const oidcString = sessionStorage.getItem(key);
        
        if (oidcString) {
          // Parse chuỗi JSON thành object
          const oidcData = JSON.parse(oidcString);
          
          // Trả về trường access_token
          return oidcData.access_token || null; 
        }
      }
    }
  } catch (error) {
    console.error('Lỗi khi phân tích dữ liệu OIDC từ sessionStorage:', error);
  }
  return null;
};

export const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000/api', 
  timeout: 10000, 
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(
  (config) => {
    // Gọi hàm lấy token
    const token = getOidcAccessToken();
    
    // Nếu có token, tự động đính kèm vào header Authorization
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axiosInstance.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response) {
      const status = error.response.status;

      // Xử lý lỗi 401: Token hết hạn hoặc không hợp lệ
      if (status === 401) {
        console.warn('Unauthorized! Yêu cầu xác thực OAuth2 lại.');
        // Lưu ý: Với oidc-client-ts, bạn thường dùng UserManager.signinSilent() để tự động làm mới token, 
        // hoặc gọi UserManager.signinRedirect() để đẩy người dùng về trang Identity Server đăng nhập lại.
      }
      
      if (status === 403) {
        console.warn('Forbidden! Bạn không có quyền truy cập.');
      }
    }

    return Promise.reject(error);
  }
);

// export default axiosInstance;