import { LoginCredentials, AuthResponse, RegisterCredentials, ForgotPasswordCredentials, ResetPasswordCredentials } from './types';

// Trong thực tế, chuỗi này nên được lấy từ file .env (VD: import.meta.env.VITE_API_URL)
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;


export const loginUserApi = async (credentials: LoginCredentials): Promise<AuthResponse> => {
    const response = await fetch(`${API_BASE_URL}/Authentication/login`, {

        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
    });

    // Dù lỗi 400 hay 401, backend của chúng ta vẫn trả về JSON { success: false, message: ... }
    // Nên ta parse JSON trực tiếp thay vì ném lỗi (throw error) ngay lập tức
    const data: AuthResponse = await response.json();
    return data;
};


export const registerUserApi = async (credentials: RegisterCredentials): Promise<AuthResponse> => {
    const response = await fetch(`${API_BASE_URL}/Authentication/register`, {

        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
    });

    // Dù lỗi 400 hay 401, backend của chúng ta vẫn trả về JSON { success: false, message: ... }
    // Nên ta parse JSON trực tiếp thay vì ném lỗi (throw error) ngay lập tức
    const data: AuthResponse = await response.json();
    return data;
};


export const forgotPasswordApi = async (data: ForgotPasswordCredentials): Promise<AuthResponse> => {
  const response = await fetch(`${API_BASE_URL}/Authentication/forgot-password`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  return await response.json();
};

export const resetPasswordApi = async (data: ResetPasswordCredentials): Promise<AuthResponse> => {
  const response = await fetch(`${API_BASE_URL}/Authentication/reset-password`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  return await response.json();
};