import { useState } from "react";
import { usePasswordRecovery } from "../hook";
import Label from "../../../components/form/Label";
import Input from "../../../components/form/input/InputField";
import Button from "../../../components/ui/button/Button";
import { Link } from "react-router-dom";



export const ForgotPasswordForm = () => {
    const [email, setEmail] = useState('');
    const { requestResetLink, isLoading, message } = usePasswordRecovery();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await requestResetLink(email);
    };

    return (
        <div className="flex flex-col flex-1">
            <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">

                <Link
                    to="/signin"
                    className="text-sm text-brand-500 hover:text-brand-600 dark:text-brand-400"
                >
                    Back to Login
                </Link>
                <form onSubmit={handleSubmit} className="space-y-4">
                    <h1 className="font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">Forgot password?</h1>
                    <p className="text-sm text-gray-500 dark:text-gray-400">Enter you email, then we will send you reset link.</p>

                    {message && (
                        <div className={`p-3 text-sm rounded-md ${message.type === 'error' ? 'bg-red-100 text-red-600' : 'bg-green-100 text-green-700'}`}>
                            {message.text}
                        </div>
                    )}

                    <div>
                        <Label>Email</Label>
                        <Input 
                        type="email" 
                        value={email} 
                        onChange={(e) => setEmail(e.target.value)} 
                        required disabled={isLoading} />
                    </div>

                    <Button type="submit" className="w-full" disabled={isLoading}>
                        {isLoading ? 'Sending...' : 'Send activation link'}
                    </Button>
                </form>
            </div>
        </div>
    );
};