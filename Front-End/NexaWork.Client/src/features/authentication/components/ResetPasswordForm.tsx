import { Navigate, useNavigate, useSearchParams } from "react-router-dom";
import Input from "../../../components/form/input/InputField";
import Label from "../../../components/form/Label";
import Button from "../../../components/ui/button/Button";
import { usePasswordRecovery } from "../hook";
import { useState } from "react";



export const ResetPasswordForm = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [password, setPassword] = useState('');

    // Lấy data từ URL (ví dụ: /reset-password?email=abc@gmail.com&token=12345)
    const email = searchParams.get('email');
    const token = searchParams.get('token');

    const { submitNewPassword, isLoading, message } = usePasswordRecovery();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!email || !token) return;

        const isSuccess = await submitNewPassword({ email, token, newPassword: password });

        if (isSuccess) {
            setTimeout(() => navigate('/signin'), 3000); // Đổi pass thành công -> Về trang Login sau 3s
        }
    };

    // Nếu người dùng truy cập thẳng vào /reset-password mà không có email/token trên URL
    if (!email || !token) {
        return <Navigate to="*" replace />;
    }

    return (
        <div className="flex flex-col flex-1">
            <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
                <form onSubmit={handleSubmit} className="space-y-4">
                    <h2 className="font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">Create new password</h2>
                    <p className="text-gray-500 text-sm">Account: {email}</p>

                    {message && (
                        <div className={`p-3 text-sm rounded-md ${message.type === 'error' ? 'bg-red-100 text-red-600' : 'bg-green-100 text-green-700'}`}>
                            {message.text}
                        </div>
                    )}

                    <div>
                        <Label>New password</Label>
                        <Input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required disabled={isLoading} />
                    </div>

                    <Button type="submit" className="w-full" disabled={isLoading}>
                        {isLoading ? 'Processing...' : 'Confirm password'}
                    </Button>
                </form>
            </div>
        </div>
    );
};