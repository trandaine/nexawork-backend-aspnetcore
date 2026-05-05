import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { forgotPasswordApi, loginUserApi, registerUserApi, resetPasswordApi } from './api';
import { LoginCredentials, RegisterCredentials, ResetPasswordCredentials } from './types';
import { useAuth } from 'react-oidc-context';

export const useLogin = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const login = async (credentials: LoginCredentials) => {
    setIsLoading(true);
    setError(null); // Reset lỗi cũ trước khi gọi API

    try {
      const response = await loginUserApi(credentials);

      if (response.success && response.accessToken) {
        // 1. Lưu token
        localStorage.setItem('userToken', response.accessToken);

        // 2. Chuyển hướng an toàn
        navigate('/dashboard');
      } else {
        // Nếu backend trả về success = false
        setError(response.message || 'Username or password is incorrect.');
      }
    } catch (err) {
      console.error('Lỗi khi gọi API Login:', err);
      setError('Cannot connect to server. Please try again later.');
    } finally {
      setIsLoading(false); // Hoàn thành (dù thành công hay thất bại)
    }
  };

  return { login, isLoading, error };
};



export const useRegister = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);
  const navigate = useNavigate();

  const register = async (credentials: RegisterCredentials) => {
    setIsLoading(true);
    setError(null);
    setSuccessMsg(null);

    try {
      const response = await registerUserApi(credentials);

      if (response.success) {
        setSuccessMsg(response.message || 'Register successfully!');
        // Đợi 2 giây rồi tự động chuyển sang trang đăng nhập
        setTimeout(() => {
          navigate('/signin');
        }, 2000);
        // 2. Chuyển hướng an toàn
      } else {
        // Nếu backend trả về success = false
        setError(response.message || 'Register failed.');
      }
    } catch (err) {
      console.error('Error calling Register API:', err);
      setError('Cannot connect to server. Please try again later.');
    } finally {
      setIsLoading(false); // Hoàn thành (dù thành công hay thất bại)
    }
  };

  return { register, isLoading, error, successMsg };
};



export const useLogout = () => {
  // const navigate = useNavigate();

  const auth = useAuth();

  const logout = () => {
    // 1. Xóa token khỏi bộ nhớ trình duyệt
    // localStorage.removeItem('userToken');

    auth.signoutRedirect({
      extraQueryParams: {
        // Explicitly tell OpenIddict who is asking for the redirect
        client_id: "nexawork_react_web"
      }
    }); // Kích hoạt quá trình đăng xuất của OIDC, sẽ tự động chuyển hướng về trang đăng nhập sau khi logout thành công

    // Nếu bạn có lưu thông tin user (như tên, email, role) vào localStorage, 
    // hãy xóa luôn ở đây. VD: localStorage.removeItem('userInfo');

    // 2. Điều hướng người dùng về trang đăng nhập
    // Dùng replace: true để họ không thể bấm nút Back trên trình duyệt quay lại trang cũ
    // navigate('/signin', { replace: true });
  };

  return { logout };
};


export const usePasswordRecovery = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState<{ type: 'error' | 'success', text: string } | null>(null);

  const requestResetLink = async (email: string) => {
    setIsLoading(true); setMessage(null);
    try {
      const res = await forgotPasswordApi({ email });
      setMessage({ type: res.success ? 'success' : 'error', text: res.message || '' });
    } catch (err) {
      setMessage({ type: 'error', text: 'Lỗi kết nối máy chủ.' });
    } finally {
      setIsLoading(false);
    }
  };

  const submitNewPassword = async (data: ResetPasswordCredentials) => {
    setIsLoading(true); setMessage(null);
    try {
      const res = await resetPasswordApi(data);
      setMessage({ type: res.success ? 'success' : 'error', text: res.message || '' });
      return res.success; // Trả về kết quả để UI biết đường chuyển trang
    } catch (err) {
      setMessage({ type: 'error', text: 'Lỗi kết nối máy chủ.' });
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  return { requestResetLink, submitNewPassword, isLoading, message };
};