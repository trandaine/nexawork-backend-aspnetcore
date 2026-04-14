
const BASE_URL = import.meta.env.VITE_API_BASE_URL;

/**
 * A generic API client for making HTTP requests with automatic token handling.
 * @param endpoint url link to access. Ex: "/user/profile"
 * @param options  RequestInit options (method, headers, body, etc.)
 * @returns Response object (Ok, NotFound, Unauthorized, etc...)
 */
export const apiClient = async (endpoint: string, options: RequestInit = {}) => {
    // 1. Lấy token từ localStorage
    const token = localStorage.getItem('userToken');

    // 2. Thiết lập Header mặc định
    //   const headers: HeadersInit = {
    //     'Content-Type': 'application/json',
    //     ...options.headers, // Giữ lại các header được truyền vào từ bên ngoài (nếu có)
    //   };

    // const headers: Record<string, string> = {
    //     'Content-Type': 'application/json',
    //     ...(options.headers as Record<string, string>),
    // };

    const headers = new Headers(options.headers);

    if (!headers.has('Content-Type')) {
        headers.set('Content-Type', 'application/json');
    }

    // 3. Nếu có token, nhúng nó vào Header dưới dạng Bearer
    if (token) {
        // headers['Authorization'] = `Bearer ${token}`;
        headers.set('Authorization', `Bearer ${token}`);
    }

    // 4. Đóng gói lại cấu hình request
    const config: RequestInit = {
        ...options,
        headers,
    };

    try {
        // 5. Thực hiện gọi API
        const response = await fetch(`${BASE_URL}${endpoint}`, config);

        // 6. Xử lý trường hợp Token hết hạn hoặc không hợp lệ (Lỗi 401 Unauthorized)
        if (response.status === 401) {
            console.warn("User session has expired. Logging out...");
            localStorage.removeItem('userToken');

            // Vì đây là file TypeScript thuần (không phải React Component), 
            // ta không dùng useNavigate() được, mà dùng window.location để ép trang web reload về trang login.
            window.location.href = '/signin';

            // Ném ra lỗi để ngắt luồng thực thi của hàm gọi API ban đầu
            throw new Error('Unauthorized');
        }

        return response;
    } catch (error) {
        console.error(`Lỗi khi gọi API [${endpoint}]:`, error);
        throw error;
    }
};