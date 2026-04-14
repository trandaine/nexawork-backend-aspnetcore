// Define the login credentials structure
export interface LoginCredentials {
  usernameOrEmail: string; 
  password: string;
}

// Định nghĩa dữ liệu Backend trả về
export interface AuthResponse {
  success: boolean;
  accessToken?: string;
  message?: string;
}

// Định nghĩa dữ liệu đăng ký 
export interface RegisterCredentials {
  email: string;
  password: string;
}


export interface ForgotPasswordCredentials {
  email: string;
}

export interface ResetPasswordCredentials {
  email: string;
  token: string;
  newPassword: string;
}